using UnityEngine;

public class HammerDragWorld : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private GameObject targetSprite;
    [SerializeField] private GameObject crackPrefab;

    [Header("Target Offset")]
    [SerializeField] private Vector3 targetOffset = new Vector3(-2.5f, -1f, 0f);

    [Header("Hit Settings")]
    [SerializeField] private LayerMask breakableLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float hitRadius = 0.5f;

    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 targetPosition;

    private Collider2D hammerCollider;
    private Rigidbody2D rb;

    private void Awake()
    {
        mainCamera = Camera.main;

        hammerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (targetSprite != null)
            targetSprite.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickUpHammer();
        }

        if (isDragging)
        {
            DragHammer();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            DropHammer();
        }
    }

    private void TryPickUpHammer()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        if (hit != null && hit.gameObject == gameObject)
        {
            isDragging = true;

            // Stop hammer from pushing player while dragging
            if (hammerCollider != null)
                hammerCollider.enabled = false;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (targetSprite != null)
                targetSprite.SetActive(true);

            Debug.Log("Picked up hammer");
        }
    }

    private void DragHammer()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        // Hammer follows the mouse
        transform.position = mouseWorldPos;

        // Target is separate from the hammer
        targetPosition = mouseWorldPos + targetOffset;

        if (targetSprite != null)
        {
            targetSprite.transform.position = targetPosition;
        }
    }

    private void DropHammer()
    {
        isDragging = false;

        if (targetSprite != null)
            targetSprite.SetActive(false);

        Strike(targetPosition);

        
        if (hammerCollider != null)
            hammerCollider.enabled = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        Debug.Log("Dropped hammer");
    }

    private void Strike(Vector3 position)
    {
        Debug.Log("Hammer hit at: " + position);

        if (crackPrefab != null)
        {
            Instantiate(crackPrefab, position, Quaternion.identity);
        }

        
        Collider2D playerHit = Physics2D.OverlapCircle(position, hitRadius, playerLayer);

        if (playerHit != null)
        {
            Debug.Log("Player was hit by hammer. Game Over.");
            EndGame();
            return;
        }

       
        Collider2D breakableHit = Physics2D.OverlapCircle(position, hitRadius, breakableLayer);

        if (breakableHit != null)
        {
            BreakableObject breakable = breakableHit.GetComponent<BreakableObject>();

            if (breakable != null)
            {
                breakable.TakeHit();
            }
        }
    }

    private void EndGame()
    {
        
        Debug.Log("GAME OVER");

       
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        return worldPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, hitRadius);
    }
}