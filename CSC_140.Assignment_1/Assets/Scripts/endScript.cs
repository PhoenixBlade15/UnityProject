using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endScript : MonoBehaviour
{

	// Check if player wants to quit the game
    void Update()
    {
		if (Input.anyKeyDown)
		{
			Application.Quit();
		}
    }
}
