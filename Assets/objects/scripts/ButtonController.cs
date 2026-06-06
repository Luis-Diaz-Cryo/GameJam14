using UnityEngine;

public class ButtonController : MonoBehaviour, insideScreen
{
    [Header("Config")]
    [SerializeField] private string stoneTag = "presser";
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private Sprite pressedbutton;

    private SpriteRenderer spriteRenderer;
    private bool activated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        if (activated) return;

        activated = true;

        // Change button sprite
        if (pressedbutton != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = pressedbutton;
        }

        // Optional: move button down visually
        transform.position += new Vector3(0f, -0.5f, 0f);

        // Activate connected object
        if (objectToActivate != null)
        {
            canHammer hammerObject = objectToActivate.GetComponent<canHammer>();

            if (hammerObject != null)
            {
                hammerObject.Interact();
            }
            else
            {
                Debug.LogWarning(objectToActivate.name + " does not have a canHammer script.");
            }
        }

        Debug.Log("Button pressed");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag(stoneTag))
        {
            Interact();
        }
    }
}