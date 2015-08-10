using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class made for a specific structure.
/// </summary>

public class stru_workshop : MonoBehaviour {

	public GameObject canvasHolder;
	public GameObject membersPanel;
	public GameObject menuHolder;
	public GameObject queHolder;
	public Slider     progressBar;

	public List<string> craftMenu = new List<string>(); 
	public List<string> craftQue  = new List<string>();

	private core_collect   ccol;
	private core_info      cinfo;
	private misc_habitable mhab;
	private core_ref 	   cref;
	private stru_storage   sstr;

	public int faction;

	public int   totalWork;
	public float currentWork;

	void Start()
	{
		mhab  = GetComponent<misc_habitable>();
		ccol  = GameObject.Find("CORE").GetComponent<core_collect>();
		cinfo = ccol.gameObject.GetComponent<core_info>();
		cref  = new core_ref();

		//Find storage
		for(int i=0;i<cinfo.storageList.Length;i++)
		{
			if(cinfo.storageList[i].GetComponent<stru_storage>().faction == faction)
			{
				sstr = cinfo.storageList[i].GetComponent<stru_storage>();
				break;
			}
		}

		//Fill workshop items menu;
		for(int i=0;i<craftMenu.Count;i++)
		{
			GameObject tempItem = (GameObject) Instantiate(ccol.ocol["ws_item"]);
			tempItem.transform.SetParent(menuHolder.transform,false);
			tempItem.transform.GetChild(0).gameObject.GetComponent<Text>().text = cref.item[craftMenu[i]].iName;
			tempItem.transform.GetChild(1).gameObject.GetComponent<RawImage>().texture = ccol.t2dcol[cref.item[craftMenu[i]].iIcon];
		}

		//Button listeners.
		for(int i=0;i<membersPanel.transform.childCount;i++)
		{
			membersPanel.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate 
			                                                                              {
				mhab.dropMember();
			});
		}

		//--Button listeners for workshop items
		for(int i=0;i<menuHolder.transform.childCount;i++)
		{
			int capturedI = i;
			menuHolder.transform.GetChild(capturedI).GetComponent<Button>().onClick.AddListener(delegate 
			{
				if(craftAble(capturedI))
				{
					craftQue.Add(craftMenu[capturedI]);

					if(craftQue.Count<=1)
					{
						currentWork = (cref.item[craftQue[craftQue.Count-1]].iParts.Count)*10;
						progressBar.maxValue = currentWork;
					}

					sstr.removeItemsByDick(cref.item[craftMenu[capturedI]].iParts);
					updateQueHolder();
				}
			});
		}


	}

	void Update()
	{

		//Update members icons
		for(int i=0;i<membersPanel.transform.childCount;i++)
		{
			if(i < mhab.members.Count)
			{
				GameObject tempButt = membersPanel.transform.GetChild(i).gameObject;
				if(tempButt.GetComponent<Image>().sprite.texture.name != "icon_manspot_1")
				{
					Texture2D tIcon = ccol.t2dcol["icon_manspot_1"];
					tempButt.GetComponent<Image>().sprite = Sprite.Create(tIcon,new Rect(0,0,tIcon.width,tIcon.height),new Vector2(0.5f,0.5f));
				}
		    }
			else
			{
				GameObject tempButt = membersPanel.transform.GetChild(i).gameObject;
				if(tempButt.GetComponent<Image>().sprite.texture.name != "icon_manspot_0")
				{
					Texture2D tIcon = ccol.t2dcol["icon_manspot_0"];
					tempButt.GetComponent<Image>().sprite = Sprite.Create(tIcon,new Rect(0,0,tIcon.width,tIcon.height),new Vector2(0.5f,0.5f));
				}
			}
		}

		//Work progress
		if(craftQue.Count>0)
		{
			if(currentWork>0)
			{
				currentWork-=mhab.members.Count*Time.deltaTime;
				progressBar.value = currentWork;
			}
			else
			{
				sstr.storage.Add(craftQue[craftQue.Count-1]);
				craftQue.RemoveAt(craftQue.Count-1);

				if(craftQue.Count>0)
				{
					currentWork = (cref.item[craftQue[craftQue.Count-1]].iParts.Count)*10;
					progressBar.maxValue = currentWork;
				}
				updateQueHolder();
			}
		}
	}

	//Activate the GUI menu
	void OnMouseOver()
	{
		if(Input.GetMouseButtonUp(0))
		{
			canvasHolder.SetActive(true);
		}
	}
	

	//Will check if the storage contains 
	//the required parts in the items info.
	private bool craftAble(int menuIndex)
	{
		bool flag = true;
		itemInfo tempItem = cref.item[craftMenu[menuIndex]];
		Dictionary<int,int> providedDick = sstr.getStorageContType();

		for(int i=0;i<tempItem.iParts.Count;i++)
		{
			if(providedDick[tempItem.iParts.Keys.ElementAt(i)] < tempItem.iParts[tempItem.iParts.Keys.ElementAt(i)])
			{
				flag = false;
				break;
			}
		}

		return flag;
	}

	//Will update the que list.
	private void updateQueHolder()
	{
		for(int i=0;i<queHolder.transform.childCount;i++)
		{
			Destroy(queHolder.transform.GetChild(i).gameObject);
		}

		for(int i=0;i<craftQue.Count;i++)
		{
			GameObject tempItem = (GameObject) Instantiate(ccol.ocol["ws_queItem"]);
			tempItem.transform.SetParent(queHolder.transform,false);
			tempItem.GetComponent<Text>().text = cref.item[craftQue[i]].iName;
		}
	}
}
