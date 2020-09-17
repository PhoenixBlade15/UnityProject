using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            for (int i = 0; i < 10; i++)
            {
                PlayerPrefs.SetInt(i + "", 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
