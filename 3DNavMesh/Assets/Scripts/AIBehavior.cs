using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehavior : MonoBehaviour
{
	public Transform[] possibleAreas;
	public float chanceToMoveAreas = 0.05f;
	private NavMeshAgent aiBody;
	private float timeSinceMove = 0.0f;
	private int currentArea = 0;


	void Start()
	{
		aiBody = GetComponent<NavMeshAgent>();
	}


	private void Update()
	{
		if ( !(possibleAreas.Length == 0))
		{
			float RNG = Random.Range(0.0f, 100.0f);

			if (RNG < chanceToMoveAreas || timeSinceMove > 30)
			{
				MoveToArea();
				timeSinceMove = 0.0f;
			}
			else
			{
				Wander();
			}

			timeSinceMove += Time.deltaTime;
		} else
		{
			Debug.LogError("Need one or more possible areas for " + this.transform.parent.name);
		}

	}

	private void Wander()
	{

	}

	private void MoveToArea()
	{
		int area;
		do
		{
			area = Random.Range(0, possibleAreas.Length);
		} while (area == currentArea);
		aiBody.destination = possibleAreas[area].position;
		currentArea = area;
	}
}