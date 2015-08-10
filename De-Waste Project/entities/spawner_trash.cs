using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Will spawn trash items from a collection of pre-sets
/// </summary>

public class spawner_trash : MonoBehaviour {

	private core_collect col;
	public List<string> trash = new List<string>();
	public int givenAmount;
	public int givenRadius;

	void Start()
	{
		col = GameObject.Find("CORE").GetComponent<core_collect>();

		for(int i=0;i<givenAmount;i++)
		{
			int rnd = Random.Range(0,4);

			int cx = Random.Range(Mathf.FloorToInt(transform.position.x)-givenRadius,Mathf.FloorToInt(transform.position.x)+givenRadius);
			int cz = Random.Range(Mathf.FloorToInt(transform.position.z)-givenRadius,Mathf.FloorToInt(transform.position.z)+givenRadius);

			Vector3 rndPos = new Vector3(cx,transform.position.y,cz);

			Instantiate(col.ocol[trash[rnd]],rndPos,transform.rotation).name = trash[rnd];

		}

		Destroy(this.gameObject);
	}

}
