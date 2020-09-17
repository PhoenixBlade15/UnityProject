using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{

    // Variable Initialization
    bool inTrigger = false;
    bool doorOpen = false;
    public GameObject door;

    // Sprite for lever up
    // SPrite for lever down
    public Sprite leverUp;
    public Sprite leverDown;


    void Update()
	{
        if (inTrigger)
		{
			if (Input.GetKeyDown("e"))
			{

                if (doorOpen)
                {
                    // Show barrier and turn on collision
                    doorOpen = false;
                    door.GetComponent<Collider2D>().enabled = true;
                    door.GetComponent<Renderer>().enabled = true;

                    // Set lever to down
                    GetComponent<SpriteRenderer>().sprite = leverDown;

                } else
                {
                    // Hide barrier and turn off collision
                    doorOpen = true;
                    door.GetComponent<Collider2D>().enabled = false;
                    door.GetComponent<Renderer>().enabled = false;

                    // Set lever to up
                    GetComponent<SpriteRenderer>().sprite = leverUp;

                }

            }
		}
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Change interact text maybe
        inTrigger = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        // Change interact text maybe
        inTrigger = false;
    }
}
