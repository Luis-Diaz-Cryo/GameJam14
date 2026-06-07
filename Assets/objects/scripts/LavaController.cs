using UnityEngine;

public class LavaController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthController health = other.GetComponent<HealthController>();
            if (health != null)
            {
                health.SetBurned(true);
                health.SetAlive(false);
            }
        }
    }
}