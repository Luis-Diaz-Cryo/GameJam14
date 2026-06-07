using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private int health = 1;
    [SerializeField] private GameObject brokenVersion;

    private canHammer hammerInteractable;

    private void Awake()
    {
        hammerInteractable = GetComponent<canHammer>();
    }

    public void TakeHit()
    {
        health--;

        Debug.Log(gameObject.name + " was hit. Health: " + health);

        if (health <= 0)
        {
            if (hammerInteractable != null && hammerInteractable.CanInteract())
            {
                hammerInteractable.Interact();
            }

            Break();
        }
    }

    private void Break()
    {
        if (brokenVersion != null)
        {
            Instantiate(brokenVersion, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}