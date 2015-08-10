using UnityEngine;
using System.Collections;

/// <summary>
/// Will fade away upon a given amount of time.
/// </summary>

//*Will work only with skinned mesh renderer.
public class self_fadeAway : MonoBehaviour {

	public float gTime;

	private Color ic;
	private float iTime;

	void Start()
	{
		ic = GetComponent<SkinnedMeshRenderer>().material.color;
		iTime = gTime;
	}

	void Update()
	{
		if(gTime>0)
		{
			gTime-=1*Time.deltaTime;

			GetComponent<SkinnedMeshRenderer>().material.color = new Color(ic.r,ic.g,ic.b,(gTime*(1/iTime)));
		}
		else
		{
			Destroy(this.transform.parent.gameObject);
		}
	}
}
