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

        transform.position += new Vector3(0f, -0.5f, 0f);

        if (objectToActivate != null)
            objectToActivate.GetComponent<canHammer>().Interact();

        Debug.Log("presionado");
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