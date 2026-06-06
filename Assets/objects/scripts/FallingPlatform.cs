using UnityEngine;

public class FallingPlatform : MonoBehaviour, canHammer
{
    [SerializeField] private Rigidbody2D rb;

    private bool hasFallen = false;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // Rock starts not falling
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void Interact()
    {
        if (hasFallen) return;

        hasFallen = true;

        // This makes the rock fall
        rb.bodyType = RigidbodyType2D.Dynamic;

        Debug.Log("Rock started falling");
    }

    public bool CanInteract()
    {
        return !hasFallen;
    }
}