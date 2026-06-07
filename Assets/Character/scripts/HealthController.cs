using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthController : MonoBehaviour
{
    [Header("Health Config")]
    [SerializeField] private bool aliveCheck = true;
    [SerializeField] private GameObject deathScreen;

    private bool wasBurned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (aliveCheck == false)
        {
            aliveCheck = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!aliveCheck)
        {
            if (gameObject.GetComponent<Movement>() != null)
            {
                if (deathScreen != null)
                {
                    deathScreen.SetActive(true);
                }
                gameObject.GetComponent<Movement>().enabled = false;
                gameObject.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            deathScreen.SetActive(true);
            deathScreen.GetComponent<UIFadeController>().FadeIn();
            if (wasBurned)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f); // Change color to indicate burning
            }

        }
    }

    public void ResetScene()
    {
        aliveCheck = true;
        if (deathScreen != null)
        {
            deathScreen.GetComponent<UIFadeController>().FadeOut();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetAlive(bool status)
    {
        aliveCheck = status;
    }

    public void SetBurned(bool status)
    {
        wasBurned = status;
    }
}
