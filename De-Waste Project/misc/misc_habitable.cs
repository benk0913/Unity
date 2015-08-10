using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This components porpuse is to activate clones by adding 
//members to the collection. This tries to behave as an "habitable"
//object which carries member and shows their clones inside.

public class misc_habitable : MonoBehaviour {

	public GameObject[] clones;
	public List<GameObject> members = new List<GameObject>();



	//Will insert a new member and activate a clone.
	public bool insertMember(GameObject gActor)
	{
		if(members.Count<clones.Length)
		{
			members.Add((GameObject)gActor);
			clones[members.Count-1].SetActive(true);
			gActor.SetActive(false);
			return true;
		}

		return false;
	}

	//Will rid of the last member joined and disable a clone.
	public void dropMember()
	{
		if(members.Count>0)
		{
			members[members.Count-1].gameObject.SetActive(true);
			clones[members.Count-1].SetActive(false);
			members.RemoveAt(members.Count-1);
		}
	}


}
