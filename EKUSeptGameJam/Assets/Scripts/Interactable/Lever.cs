using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{

    // Variable Initialization
    bool inTrigger = false;
    bool isDoorOpen = false;
    public bool oneTimeUse = true;
    bool alreadyUsed = false;
    public GameObject door;
    public Mesh doorOpen;
    public Mesh doorShut;

    void Start()
	{
        door.GetComponent<MeshFilter>().mesh = doorShut;
    }

    void Update()
	{
        if (inTrigger)
		{
            if (!alreadyUsed)
            {

                if (Input.GetKeyDown("e"))
                {
                    if (oneTimeUse)
                    {
                        alreadyUsed = true;
                    }

                    if (isDoorOpen)
                    {
                        // Show barrier and turn on collision
                        isDoorOpen = false;
                        door.GetComponent<Collider>().enabled = true;
                        door.GetComponent<MeshFilter>().mesh = doorShut;


                        // Set lever to down
                        transform.Rotate(0, 180, 0);

                    }
                    else
                    {
                        // Hide barrier and turn off collision
                        isDoorOpen = true;
                        door.GetComponent<Collider>().enabled = false;
                        door.GetComponent<MeshFilter>().mesh = doorOpen;

                        // Set lever to up
                        transform.Rotate(0, 180, 0);

                    }

                }
            }
		}
    }

    void OnTriggerEnter(Collider collider)
    {
        GameObject collidedObject = collider.gameObject;
        if (collidedObject.gameObject.tag == "Player")
        {
            if (!alreadyUsed)
            {
                inTrigger = true;
            }
        }
        
    }

    void OnTriggerExit(Collider collider)
    {
        GameObject collidedObject = collider.gameObject;
        if (collidedObject.gameObject.tag == "Player")
        {
            inTrigger = false;
        }
    }
}
