using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Perish", 20f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Perish();
            return;
        } else if (collision.gameObject.tag == "Player")
        {
            Destroy(collision.gameObject);
            Perish();
        }
    }

    void Perish()
    {
        Destroy(gameObject);
    }
}
