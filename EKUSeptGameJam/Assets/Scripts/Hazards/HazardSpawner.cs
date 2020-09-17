using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public float cooldown = 5f;

    public GameObject objectToSpawn = null;

    public GameObject[] spawnPositions;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 0f, cooldown);
    }

    IEnumerator Spawn()
    {
        int random = Random.Range(0, spawnPositions.Length);
        Instantiate(objectToSpawn, spawnPositions[random].transform.position, Quaternion.identity);
        return null;
    }
}
