using UnityEngine;
using System.Collections;

/// <summary>
/// Contains the reinforcements system, collection, scheme / scene based
/// spawning and entering and so on.
/// (Spawn unit is temp.)
/// </summary>
 
public class core_reinforcements : MonoBehaviour {

	#region Parameters

	public GameObject spawnSpot;
	public GameObject entranceSpot;
	public core_ref cref;
	public core_collect ccol;

	void Start()
	{
		cref = new core_ref();
		ccol = GetComponent<core_collect>();

		spawnSpot 	 = GameObject.FindGameObjectWithTag("spawnObj");
		entranceSpot = GameObject.FindGameObjectWithTag("entranceObj");
	}

	#endregion

	#region Commands

	//Will spawn a unit with a random look and basic veterancy.
	public IEnumerator spawnUnit()
	{
		GameObject tempUnit = (GameObject) Instantiate(ccol.ocol["actor"],spawnSpot.transform.position,spawnSpot.transform.rotation);

		//Apply random clothing texture:
		SkinnedMeshRenderer smr = tempUnit.transform.FindChild("mdl/Box01").GetComponent<SkinnedMeshRenderer>();
		int rnd = Random.Range(0,cref.clothing.Count);
		smr.materials[0].mainTexture = ccol.t2dcol[cref.clothing[rnd]];

		//Apply random face texture:
		MeshRenderer mr = tempUnit.transform.FindChild("mdl/Bone01/Bone03/Bone05/Bone06/Bone07/head").GetComponent<MeshRenderer>();
		rnd = Random.Range(0,cref.faces.Count);
		mr.materials[0].mainTexture = ccol.t2dcol[cref.faces[rnd]];

		//Apply random veterancy:
		tempUnit.GetComponent<actor_info>().rifleVet = Random.Range(0,3);
		tempUnit.GetComponent<actor_info>().meleeVet = Random.Range(0,3);
		tempUnit.GetComponent<actor_info>().mechaVet = Random.Range(0,3);
		tempUnit.GetComponent<actor_info>().electVet = Random.Range(0,3);
		tempUnit.GetComponent<actor_info>().buildVet = Random.Range(0,3);

		yield return new WaitForSeconds(1);

		//Command walk to entrance:
		StartCoroutine(tempUnit.GetComponent<actor_control>().walkToObject(entranceSpot));

	}

	#endregion



}
