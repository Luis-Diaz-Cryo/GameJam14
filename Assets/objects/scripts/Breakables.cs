using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private int health = 1;
    [SerializeField] private GameObject brokenVersion;
    private canHammer canHammer;

    void Start()
    {
        if(gameObject.GetComponent<canHammer>() != null)
            canHammer = gameObject.GetComponent<canHammer>();
    }

    public void TakeHit()
    {
        health--;

        Debug.Log(gameObject.name + " was hit");

        
        if (health <= 0)
        {
            Break();
        }
        if(canHammer!=null && canHammer.CanInteract())
        {
            canHammer.Interact();
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