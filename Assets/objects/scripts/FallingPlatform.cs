using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, canHammer
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float timeBeforeStatic = 1.5f;

    private bool hasFallen = false;
    private Animator anim;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        // Rock starts not falling
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void Interact()
    {
        if (hasFallen) return;

        hasFallen = true;

        Debug.Log("Rock started breaking");

        int groundLayer = LayerMask.NameToLayer("groundLayer");

        if (groundLayer != -1)
        {
            gameObject.layer = groundLayer;
        }
        else
        {
            Debug.LogWarning("Layer 'groundLayer' does not exist.");
        }

        if (anim != null)
        {
            anim.SetTrigger("Break");
        }

        // Make rock fall
        rb.bodyType = RigidbodyType2D.Dynamic;

        // After delay, freeze it again
        StartCoroutine(BecomeStaticAfterDelay());
    }

    private IEnumerator BecomeStaticAfterDelay()
    {
        yield return new WaitForSeconds(timeBeforeStatic);

        // Stop movement before making it static
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Static;

        Debug.Log("Rock became static again");
    }

    public bool CanInteract()
    {
        return !hasFallen;
    }
}