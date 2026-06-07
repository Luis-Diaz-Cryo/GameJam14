using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("tutorial");
    }

    public void OnCreditsClick()
    {
        GameObject credits = transform.GetChild(4).gameObject;
        credits.SetActive(true);
    }

    public void OnEndClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
        
    }
    public void OnBackClick()
    {
        GameObject credits = transform.GetChild(4).gameObject;
        credits.SetActive(false);
    }
}
