using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public Mesh doorOpen;
    public string level;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(level) == 1)
        {
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<MeshFilter>().mesh = doorOpen;
        }        
    }
}
