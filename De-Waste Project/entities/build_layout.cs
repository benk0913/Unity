using UnityEngine;
using System.Collections;

/// <summary>
/// Basic information of build layouts.
/// </summary>
public class build_layout : MonoBehaviour {

	public string targetStructure;
	public int scrapsAmount;
	public bool hasSpace = true;
	public int vetLevel;

	void OnTriggerEnter(Collider victim)
	{
		hasSpace = false;
		GetComponent<MeshRenderer>().material.color = Color.red;
	}

	void OnTriggerStay(Collider victim)
	{
		hasSpace = false;
		GetComponent<MeshRenderer>().material.color = Color.red;
	}

	void OnTriggerExit(Collider victim)
	{
		hasSpace = true;
		GetComponent<MeshRenderer>().material.color = Color.green;
	}
}
