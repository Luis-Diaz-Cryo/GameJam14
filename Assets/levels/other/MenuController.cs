using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("tutorial");
    }

    public void OnEndClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
        
    }
}
