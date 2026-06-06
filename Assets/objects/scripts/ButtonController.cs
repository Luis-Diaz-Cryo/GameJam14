using UnityEngine;

public class ButtonController : MonoBehaviour, insideScreen
{
    [Header("Config")]
    [SerializeField] private string stoneTag = "presser";
    [SerializeField] private GameObject objectToActivate;

    private bool activated = false;

    public void Interact()
    {
        if (activated) return;

        activated = true;

        // Move button down visually
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
        Debug.Log("presionado");

        if (activated) return;

        if (other.CompareTag(stoneTag))
        {
            Interact();
        }
    }
}