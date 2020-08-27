using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

	// Checks when player is ready to play the game
    void Update()
    {
		if (Input.anyKeyDown)
		{
			SceneManager.LoadScene("mainScene");
		}
    }
}
