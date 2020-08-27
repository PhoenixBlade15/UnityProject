using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goToWinScene : MonoBehaviour
{

	// If the dog is touched by the player then go to win screen
	void OnCollisionEnter(Collision collision)
	{
		SceneManager.LoadScene("endScene");
	}
}
