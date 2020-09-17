using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Vector3[] positions;
    public Transform ObjectToMove;
    public float MoveSpeed = 8;
    public int currentGoal = 0;
    Coroutine MoveIE;

    void Start()
    {
        StartCoroutine(moveObject());
    }

    IEnumerator moveObject()
    {
        while (true)
        {
            MoveIE = StartCoroutine(Moving(currentGoal));
            currentGoal++;
            if (currentGoal == 4)
            {
                currentGoal = 0;
            }
            yield return MoveIE;
        }    
    }

    IEnumerator Moving(int currentPosition)
    {
        while (ObjectToMove.transform.position != positions[currentPosition])
        {
            ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, positions[currentPosition], MoveSpeed * Time.fixedDeltaTime);
            yield return null;
        }

    }

}
