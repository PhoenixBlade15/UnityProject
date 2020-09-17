using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetJeremyPosition : MonoBehaviour
{
    public GameObject jeremy;

    private void Awake()
    {
        Vector3 newPosition = new Vector3(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"), PlayerPrefs.GetFloat("z"));
        jeremy.transform.position = newPosition;
    }
}
