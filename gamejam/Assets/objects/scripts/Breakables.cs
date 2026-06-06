using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private int health = 1;
    [SerializeField] private GameObject brokenVersion;

    public void TakeHit()
    {
        health--;

        Debug.Log(gameObject.name + " was hit");

        if (health <= 0)
        {
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