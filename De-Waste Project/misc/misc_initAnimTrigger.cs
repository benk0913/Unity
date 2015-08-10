using UnityEngine;
using System.Collections;

/// <summary>
/// Used for animator parameters initialization.
/// </summary>
/// 
public class misc_initAnimTrigger : MonoBehaviour {

	public string gVar;
	
	void Start()
	{
		GetComponent<Animator>().SetTrigger(gVar);
		Destroy(this);
	}
}
