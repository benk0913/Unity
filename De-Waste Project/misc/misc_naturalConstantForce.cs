using UnityEngine;
using System.Collections;

/// <summary>
/// A very basic constant force which 
/// replaces unity's constant force for specific purposes.
/// </summary>

public class misc_naturalConstantForce : MonoBehaviour {
	
	public int gSpeed;

	void Update()
	{
		transform.position+=transform.forward*gSpeed*Time.deltaTime;
	}


}
