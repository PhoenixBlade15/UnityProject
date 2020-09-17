using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        GameObject collidedObject = collider.gameObject;
        if (collidedObject.gameObject.tag == "Player")
        {
            gameObject.GetComponent<NavMeshAgent>().speed = 10;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.collider.gameObject;
        if (collidedObject.gameObject.tag == "Player")
        {
            Destroy(collidedObject);
        }
    }
}
