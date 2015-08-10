using UnityEngine;
using System.Collections;

/// <summary>
/// Used for animator parameters initialization.
/// </summary>

public class misc_initAnimBool : MonoBehaviour {

	public string gVar;
	public bool gBool;

	void Start()
	{
		GetComponent<Animator>().SetBool(gVar,gBool);
		Destroy(this);
	}
}
