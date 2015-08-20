using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class core_loadScreen : core_saveHandler {

	#region Parameters
	
	//components gather from.
	private JSONNode currentN;
	private core_inventory cinv;
	private core_stats     cstats;
	private core_collect   ccol;
	
	//UI
	public GameObject slLayout;
	
	void Awake()
	{
		GameObject.DontDestroyOnLoad(this.gameObject);
	}
	
	void Start()
	{
		//Init vars
		cinv     =  GameObject.Find("CORE").GetComponent<core_inventory>();
		cstats   =  cinv.gameObject.GetComponent<core_stats>();
		ccol     =  cinv.gameObject.GetComponent<core_collect>();
		
		//Load saves:
		loadCurrentSaves();
		refreshUI();
		
		
	}


	
	#endregion

	#region perFrame
	
	void Update()
	{
		//Close window input.
		if(Input.GetKey(core_input.close))
		{
			this.gameObject.SetActive(false);
		}
	}
	
	#endregion


	#region Commands

	//Will load a given save.
	public void loadSave(int saveIndex)
	{
		currentN = JSON.Parse(System.IO.File.ReadAllText(loadedSaves[saveIndex]));
		Application.LoadLevel(currentN["data"]["scene"].AsInt);
		cstats.whiteRespect = currentN["data"]["whiteRespect"].AsInt;
		cstats.blackRespect = currentN["data"]["blackRespect"].AsInt;
		cstats.redRespect   = currentN["data"]["redRespect"].AsInt;
		cstats.brownRespect = currentN["data"]["brownRespect"].AsInt;

		for(int i=0;i<currentN["data"]["inventory"].Count;i++)
		{
			cinv.items.Clear();
			cinv.items.Add(currentN["data"]["inventory"][i]);
		}

		currentN = "";
	}

	//Will refresh the savelist UI
	public void refreshUI()
	{
		loadCurrentSaves();
		//Remove existing save items.
		
		if(slLayout.transform.childCount>1)
		{
			for(int a=1;a<slLayout.transform.childCount;a++)
			{
				Destroy(slLayout.transform.GetChild(a).gameObject);
			}
		}
		
		//Load new save items.
		for(int i=0;i<loadedSaves.Count;i++)
		{
			GameObject tempSave = Instantiate(ccol.ocol["save_chunk"]);
			tempSave.transform.SetParent(slLayout.transform,false);
			
			tempSave.transform.GetChild(0).GetComponent<Text>().text = getSaveName(i);
			tempSave.transform.GetChild(1).GetComponent<Text>().text = getSaveDate(i);
			
			int captured = i;
			tempSave.GetComponent<Button>().onClick.AddListener( delegate {
				loadSave(captured);
			});
		}
	}

	#endregion
}
