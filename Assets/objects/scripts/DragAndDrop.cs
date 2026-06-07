using System.Collections;
using UnityEngine;

public class HammerDragWorld : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalHammerSprite;
    [SerializeField] private Sprite heldHammerSprite;

    [Header("Highlight")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Target")]
    [SerializeField] private GameObject targetSprite;
    [SerializeField] private GameObject crackPrefab;

    [Header("Target Offset")]
    [SerializeField] private Vector3 targetOffset = new Vector3(-2.5f, -1f, 0f);

    [Header("Hit Settings")]
    [SerializeField] private LayerMask breakableLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private float hitsMade = 0f;

    [Header("Allowed Strike Area")]
    [SerializeField] private Collider2D allowedStrikeArea;

    [Header("Strike Animation")]
    [SerializeField] private float strikeAngle = -70f;
    [SerializeField] private float strikeTime = 0.15f;
    [SerializeField] private float returnTime = 0.25f;
    [SerializeField] private float pauseAfterStrike = 0.15f;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Collider2D hammerCollider;
    private Rigidbody2D rb;

    private bool isDragging = false;
    private bool isAnimating = false;
    private bool isHovering = false;

    private Vector3 targetPosition;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        mainCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();
        hammerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (spriteRenderer != null && normalHammerSprite != null)
            spriteRenderer.sprite = normalHammerSprite;

        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;

        if (targetSprite != null)
            targetSprite.SetActive(false);
    }

    private void Update()
    {
        if (isAnimating)
            return;

        CheckHover();

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

    private void CheckHover()
    {
        if (isDragging)
            return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        bool mouseIsOverHammer = hit != null && hit.gameObject == gameObject;

        if (mouseIsOverHammer && !isHovering)
        {
            isHovering = true;

            if (spriteRenderer != null)
                spriteRenderer.color = highlightColor;
        }
        else if (!mouseIsOverHammer && isHovering)
        {
            isHovering = false;

            if (spriteRenderer != null)
                spriteRenderer.color = normalColor;
        }
    }

    private void TryPickUpHammer()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        if (hit != null && hit.gameObject == gameObject)
        {
            isDragging = true;
            isHovering = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;

                if (heldHammerSprite != null)
                    spriteRenderer.sprite = heldHammerSprite;
            }

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

        transform.position = mouseWorldPos;

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

        if (!CanStrikeAtTarget())
        {
            Debug.Log("Cannot strike here. Target is outside allowed area.");
            StartCoroutine(ReturnHammerWithoutStrike());
            return;
        }

        StartCoroutine(StrikeAnimation());
    }
    private bool CanStrikeAtTarget()
    {
        if (allowedStrikeArea == null)
        {
            Debug.LogWarning("No allowed strike area assigned.");
            return true;
        }

        return allowedStrikeArea.OverlapPoint(targetPosition);
    }

    private IEnumerator StrikeAnimation()
    {
        isAnimating = true;

        Vector3 strikePosition = transform.position;
        Quaternion beforeStrikeRotation = transform.rotation;
        Quaternion strikeRotation = Quaternion.Euler(0f, 0f, strikeAngle);

        float timer = 0f;

        while (timer < strikeTime)
        {
            timer += Time.deltaTime;
            float t = timer / strikeTime;

            transform.rotation = Quaternion.Lerp(beforeStrikeRotation, strikeRotation, t);

            yield return null;
        }

        transform.rotation = strikeRotation;

        Strike(targetPosition);

        yield return new WaitForSeconds(pauseAfterStrike);

        timer = 0f;

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            float t = timer / returnTime;

            transform.position = Vector3.Lerp(currentPosition, startPosition, t);
            transform.rotation = Quaternion.Lerp(currentRotation, startRotation, t);

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (spriteRenderer != null)
        {
            if (normalHammerSprite != null)
                spriteRenderer.sprite = normalHammerSprite;

            spriteRenderer.color = normalColor;
        }

        if (hammerCollider != null)
            hammerCollider.enabled = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isAnimating = false;

        Debug.Log("Hammer returned to table");
    }

    private IEnumerator ReturnHammerWithoutStrike()
    {
        isAnimating = true;

        float timer = 0f;

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            float t = timer / returnTime;

            transform.position = Vector3.Lerp(currentPosition, startPosition, t);
            transform.rotation = Quaternion.Lerp(currentRotation, startRotation, t);

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (spriteRenderer != null)
        {
            if (normalHammerSprite != null)
                spriteRenderer.sprite = normalHammerSprite;

            spriteRenderer.color = normalColor;
        }

        if (hammerCollider != null)
            hammerCollider.enabled = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isAnimating = false;

        Debug.Log("Hammer returned without striking");
    }

    private void Strike(Vector3 position)
    {
        Debug.Log("Hammer hit at: " + position);

        hitsMade += 1f;

        if (hitsMade >= 4f)
        {
            Debug.Log("Hammer has been used 3 times. Ending game.");
            EndGame();
            return;
        }

        if (crackPrefab != null)
        {
            Instantiate(crackPrefab, position, Quaternion.identity);
        }

        // Check if player was hit
        Collider2D playerHit = Physics2D.OverlapCircle(position, hitRadius, playerLayer);

        if (playerHit != null)
        {
            Debug.Log("Player was hit by hammer. Game Over.");
            EndGame();
            return;
        }

        // Check if hammer hit an interactable object like the falling rock
        Collider2D hitObject = Physics2D.OverlapCircle(position, hitRadius, breakableLayer);

        if (hitObject != null)
        {
            Debug.Log("object hit: " + hitObject.name);

            BreakableObject breakable = hitObject.GetComponent<BreakableObject>();

            if (breakable != null)
            {
                breakable.TakeHit();
            }
            else
            {
                canHammer hammerObject = hitObject.GetComponent<canHammer>();

                if (hammerObject != null && hammerObject.CanInteract())
                {
                    hammerObject.Interact();
                }
            }
        }
    }

    private void EndGame()
    {
        Debug.Log("GAME OVER");
        GameObject player;
        if (hitsMade < 4f)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("Player found: " + (player != null));
        }
        else
        {
            GameObject parentObj = GameObject.Find("UI");
            Transform gameOverScreen = parentObj.transform.Find("mainGameOver");
            player = gameOverScreen != null ? gameOverScreen.gameObject : null;
            Debug.Log("Game Over object found: " + (player != null));
        }
        if (player != null)
        {
            if (!player.activeSelf)
            {
                player.SetActive(true);
            }
            HealthController health = player.GetComponent<HealthController>();
            health.SetAlive(false);
        }
        // Later connect this to your real GameManager:
        // GameManager.Instance.GameOver();
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