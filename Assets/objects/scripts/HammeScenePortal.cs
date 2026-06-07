using UnityEngine;

public class HammerScenePortal : MonoBehaviour, canHammer
{
    [Header("Hammer Settings")]
    [SerializeField] private bool canBeHammered = true;

    [Header("Sprite Change")]
    [SerializeField] private Sprite activatedSprite;

    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    [Header("SFX")]
    [SerializeField] private AudioClip hammerSound;
    [SerializeField] private AudioSource hammerSource;

    private SpriteRenderer spriteRenderer;
    private bool activated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Collider2D portalCollider = GetComponent<Collider2D>();

        if (portalCollider != null)
        {
            portalCollider.isTrigger = true;
        }
    }

    public bool CanInteract()
    {
        return canBeHammered && !activated;
    }

    public void Interact()
    {
        if (activated) return;

        activated = true;

        if (activatedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = activatedSprite;
        }
    
        hammerSource.PlayOneShot(hammerSound);
        Debug.Log("Portal activated by hammer");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated) return;

        if (collision.CompareTag("Player"))
        {
            FadeTransition.Instance.FadeToScene(sceneToLoad);
        }
    }
}