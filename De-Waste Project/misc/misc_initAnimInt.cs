using UnityEngine;
using System.Collections;

public class misc_initAnimInt : MonoBehaviour {

	public string gVar;
	public int gInteger;
	
	void Start()
	{
		GetComponent<Animator>().SetInteger(gVar,gInteger);
		Destroy(this);
	}
}
