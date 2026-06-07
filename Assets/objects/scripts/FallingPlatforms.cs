using UnityEngine;

public class FallingPlatforms : MonoBehaviour, canHammer
{
    [SerializeField] private Rigidbody2D rb;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if(rb.linearVelocityY<-5) rb.linearVelocityY=-5;
    }
    public void Interact()
    {
        if(rb.bodyType == RigidbodyType2D.Dynamic)
        {
        rb.bodyType = RigidbodyType2D.Static;
        }
        else
        rb.bodyType = RigidbodyType2D.Dynamic;

    }

    public bool CanInteract()
    {
        return true;
    }
}