using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Core_collect.
/// The class will make the scene more efficient resourcewisely.
/// It'll load all of the resources from the project and define them with keys.
/// The class will also store resources which are meant to be shown once but in different locations.
/// so instead of destroying and reinstantiating it'll just relocate and enable the object.
/// </summary>
public class core_collect : MonoBehaviour {

	#region Parameters

	//Will store all of the resources loaded in the beggining of the scene.
	public Dictionary<string , GameObject> ocol = new Dictionary<string, GameObject>();

	//Will store all of the texture2d resources loaded in the beggining of the scene.
	public Dictionary<string , Texture2D> t2dcol = new Dictionary<string, Texture2D>();

	//Will store all of the 'repositionable' resources at the beggining of the scene.
	//for example an object that uses one instance and relocates itself will not have to 
	//re instantiate itself but relocate itself to the given position.
	public Dictionary<string , GameObject> rcol = new Dictionary<string, GameObject>();

	void Awake()
	{

		//Load GameObjects
		object[] loaded = Resources.LoadAll("GameObjects"); 
		string cwName = "";

		for(int i=0;i<loaded.Length;i++)
		{
			cwName = loaded[i].ToString().Substring(0,loaded[i].ToString().Length-25);
			print (cwName);
			ocol.Add(cwName,(GameObject)loaded[i]);
		}

		//Load Textures
		loaded = Resources.LoadAll("Textures"); 

		for(int i=0;i<loaded.Length;i++)
		{
			cwName = loaded[i].ToString().Substring(0,loaded[i].ToString().Length-24);
			print (cwName);
			t2dcol.Add(cwName,(Texture2D)loaded[i]);
		}

	}

	#endregion

	#region Commands

	//Will enable and set the object in a given pos.
	public void relocate(string gKey, Vector3 pos)
	{
		rcol[gKey].SetActive(true);
		rcol[gKey].transform.position = pos;
	}

	//Will disable and set the object in 0,0,0
	public void hide(string gKey)
	{
		rcol[gKey].SetActive(false);
		rcol[gKey].transform.position = new Vector3(0,0,0);
	}
	

	#endregion

}
