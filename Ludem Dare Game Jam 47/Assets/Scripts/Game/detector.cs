using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detector : MonoBehaviour
{
	public string insideOfTag = "Nothing";

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "BlueDetector" || other.tag == "RedDetector" || other.tag == "RedWeapon" || other.tag == "BlueWeapon")
		{
			insideOfTag = other.tag;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		insideOfTag = "Nothing";
	}
}