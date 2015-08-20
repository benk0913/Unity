using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class core_saveHandler : MonoBehaviour {

	public List<string> loadedSaves = new List<string>();

	//Will load save files.
	public void loadCurrentSaves()
	{
		loadedSaves.Clear();
		string[] saveFiles = System.IO.Directory.GetFiles(Application.dataPath);
		for(int i=0;i<saveFiles.Length;i++)
		{
			if(saveFiles[i].Contains("s_") && !saveFiles[i].Contains(".meta"))
				loadedSaves.Add(saveFiles[i]);
		}
	}

	//Will return the save displayable name.
	public string getSaveName(int saveIndex)
	{
		if(System.IO.File.Exists(loadedSaves[saveIndex]))
		{
			JSONNode currentN = JSON.Parse(System.IO.File.ReadAllText(loadedSaves[saveIndex]));
			return currentN["saveName"].ToString();
		}

		return "";
	}

	//Will return the save displayable date.
	public string getSaveDate(int saveIndex)
	{
		if(System.IO.File.Exists(loadedSaves[saveIndex]))
		{
			JSONNode currentN = JSON.Parse(System.IO.File.ReadAllText(loadedSaves[saveIndex]));
			return currentN["saveDate"].ToString();
		}
		
		return "";
	}


	
}
