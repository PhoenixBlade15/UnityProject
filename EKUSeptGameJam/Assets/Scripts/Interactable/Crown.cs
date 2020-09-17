using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Crown : MonoBehaviour
{
    public GameObject crown;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            crown.SetActive(true);
            PlayerPrefs.SetFloat("x", collision.gameObject.transform.position.x);
            PlayerPrefs.SetFloat("y", collision.gameObject.transform.position.y);
            PlayerPrefs.SetFloat("z", collision.gameObject.transform.position.z);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Destroy(gameObject);
        }
    }
}
