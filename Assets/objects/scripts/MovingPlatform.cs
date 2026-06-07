using UnityEngine;

public class MovingPlatform : MonoBehaviour, canHammer
{
    [Header("Movement")]
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 startPosition;
    private Vector2 topPosition;
    private bool isActivated = false;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    private void Start()
    {
        startPosition = rb.position;
        topPosition = startPosition + Vector2.up * moveDistance;
    }

    private void FixedUpdate()
    {
        if (!isActivated) return;

        float movement = Mathf.PingPong(Time.fixedTime * moveSpeed, 1f);

        Vector2 newPosition = Vector2.Lerp(startPosition, topPosition, movement);

        rb.MovePosition(newPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }

    public void Interact()
    {
        isActivated = true;
        Debug.Log("Platform activated");
    }

    public bool CanInteract()
    {
        return !isActivated;
    }
}