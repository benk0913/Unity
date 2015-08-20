using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

public class core_saveScreen : core_saveHandler {

	#region Parameters

	public string saveFileTemplate;

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

	//Will create a new json save file.
	public void createNewSaveFile()
	{
		currentN =  JSON.Parse(saveFileTemplate);

		currentN["saveDate"] = System.DateTime.Now.ToString();

		currentN["data"]["scene"] = Application.loadedLevel.ToString();
		currentN["data"]["whiteRespect"] = cstats.whiteRespect.ToString();
		currentN["data"]["blackRespect"] = cstats.blackRespect.ToString();
		currentN["data"]["redRespect"]   = cstats.redRespect.ToString();
		currentN["data"]["brownRespect"] = cstats.brownRespect.ToString();

		for(int i=0;i<cinv.items.Count;i++)
		{
			currentN["data"]["inventory"]["item"+i] = cinv.items[i];
		}

		loadCurrentSaves();

		string tempContent = "";

		if(loadedSaves.Count==0)
		{
			currentN["saveName"] = "Save 1";
			tempContent = currentN.ToString();
			System.IO.File.WriteAllText(Application.dataPath+"/s_1.json",  tempContent );
		}
		else
		{
			currentN["saveName"] = "Save "+(loadedSaves.Count+1);
			tempContent = currentN.ToString();
			System.IO.File.WriteAllText(Application.dataPath+"/s_"+(loadedSaves.Count+1)+".json",  tempContent );
		}

		currentN = "";

		refreshUI();

		this.gameObject.SetActive(false);
	}

	//Will overwrite existsing save.
	public void overwriteSave(int saveIndex)
	{
		currentN =  JSON.Parse(saveFileTemplate);
		
		currentN["saveDate"] = System.DateTime.Now.ToString();
		
		currentN["data"]["scene"] = Application.loadedLevel.ToString();
		currentN["data"]["whiteRespect"] = cstats.whiteRespect.ToString();
		currentN["data"]["blackRespect"] = cstats.blackRespect.ToString();
		currentN["data"]["redRespect"]   = cstats.redRespect.ToString();
		currentN["data"]["brownRespect"] = cstats.brownRespect.ToString();
		
		for(int i=0;i<cinv.items.Count;i++)
		{
			currentN["data"]["inventory"]["item"+i] = cinv.items[i];
		}
		
		loadCurrentSaves();
		
		string tempContent = "";

		currentN["saveName"] = "Save "+(saveIndex+1);
		tempContent = currentN.ToString();
		System.IO.File.WriteAllText(Application.dataPath+"/s_"+(saveIndex+1)+".json",  tempContent );

		currentN = "";
		
		refreshUI();
		
		this.gameObject.SetActive(false);

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
				overwriteSave(captured);
			});
		}
	}


	#endregion

}
