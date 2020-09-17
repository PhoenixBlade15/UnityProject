using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomTeleporter : MonoBehaviour
{
    public int buildIndex;

    public bool toLevelSelect;

    public string levelSelectScene;

    private void OnTriggerEnter(Collider other)
    {
        if (toLevelSelect)
        {
            SceneManager.LoadScene(levelSelectScene);
            return;
        }
        SceneManager.LoadScene(buildIndex);
    }
}
