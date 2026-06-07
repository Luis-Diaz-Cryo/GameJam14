using UnityEngine;

public class FallingPlatforms : MonoBehaviour, canHammer
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float maxFallSpeed = -5f;

    private bool frozen = false;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb == null || frozen) return;

        // Limit falling speed
        if (rb.linearVelocity.y < maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
        }
    }

    public void Interact()
    {
        if (frozen) return;

        frozen = true;

        // Stop all movement
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Freeze in place
        rb.bodyType = RigidbodyType2D.Static;

        Debug.Log(gameObject.name + " frozen by hammer");
    }

    public bool CanInteract()
    {
        return !frozen;
    }
}