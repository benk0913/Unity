using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains general and map/scheme based information / functions.
/// </summary>
/// 
public class core_info : MonoBehaviour {

	public int playerFaction;
	public int currentCash;
	public GameObject[] storageList;
	public List<info_company> companies = new List<info_company>();
	public factionInfo[] factions;

	void Awake()
	{
		storageList = GameObject.FindGameObjectsWithTag("storage");
	}


}

[System.Serializable]
public class factionInfo
{
	public string fName;
	public bool[] fRelations;

}
