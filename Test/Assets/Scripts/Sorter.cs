using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorter : MonoBehaviour
{

	int[] numArray = new int[] { 4, 8, 5, 7, 9 };
	List<int> numList = new List<int>();

	void Start()
	{
		int numArrayLength = numArray.Length;
		for (int i = 0; i < numArrayLength; i++)
		{
			int largestNum = -292929999;
			for (int j = 0; j < numArrayLength; j++)
			{
				if (numArray[j] > largestNum && !numList.Contains(numArray[j]))
				{
					largestNum = numArray[j];
				}
			}
			numList.Add(largestNum);
		}

		print("Sorted Array: ");
		foreach ( int num in numList)
		{
			print(num + ", ");
		}
    }


}
