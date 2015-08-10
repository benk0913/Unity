using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Will record and contain objects by tag.
/// (Proven to be very useful.)
/// </summary>

public class misc_recordObjects : MonoBehaviour {

	public string gTag;

	public List<GameObject> within = new List<GameObject>(); 

	void OnTriggerEnter(Collider victim)
	{
		if(!within.Contains(victim.gameObject))
		{
			if(victim.gameObject.tag == gTag)
			{
				within.Add(victim.gameObject);
			}
		}
	}

	void Update()
	{
		for(int i=0;i<within.Count;i++)
		{
			if(within[i]!=null)
			{
				if(within[i].tag!=gTag)
					within.RemoveAt(i);
			}
		}
	}

	void OnTriggerExit(Collider victim)
	{
		if(within.Contains(victim.gameObject))
		{
			within.Remove(victim.gameObject);
		}
	}
}
