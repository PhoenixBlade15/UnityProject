using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collider)
    {
        if (Input.GetMouseButtonDown(0))
		{
            Destroy(gameObject);
		}
    }
}
