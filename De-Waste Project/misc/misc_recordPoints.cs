using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Will record and collect positions by tag (If colliders and rigidbodies are 
/// a waste of resources.
/// </summary>
/// 
public class misc_recordPoints : MonoBehaviour {

	public string givenTag;
	public List<GameObject> within = new List<GameObject>();
	public int interval;

	private float gTimer;

	void Start()
	{
		gTimer = interval;
	}

	void Update()
	{
		if(gTimer>0)
		{
			gTimer-=1*Time.deltaTime;
		}
		else
		{
			within.Clear();
			GameObject[] allPoints = GameObject.FindGameObjectsWithTag(givenTag);

			for(int i=0;i<allPoints.Length;i++)
			{
				if(GetComponent<BoxCollider>().bounds.Contains(allPoints[i].transform.position))
					within.Add(allPoints[i]);
			}

			gTimer = interval;
		}
	}

}
