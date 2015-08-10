using UnityEngine;
using System.Collections;

/// <summary>
/// Will disable itself upon a given amount of time.
/// </summary>

public class self_disabler : MonoBehaviour {

	float dTimer;

	void Update()
	{
		if(dTimer>0)
		{
			dTimer-=1*Time.deltaTime;
		}
		else
		{
			this.gameObject.SetActive(false);
		}
	}
}
