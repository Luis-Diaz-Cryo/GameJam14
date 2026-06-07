using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class ChangeScnene : MonoBehaviour
{
    [SerializeField]public float changeTime;
    [SerializeField]public string sceneName;

    private void Update()
    {
        changeTime -= Time.deltaTime;

        if (changeTime <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
