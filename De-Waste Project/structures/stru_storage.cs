using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Class made for a specific structure.
/// </summary>

public class stru_storage : MonoBehaviour {

	#region Parameters

	public int          faction;
	public GameObject   entrancePoint;
	public List<string> storage = new List<string>();

	//STATUS CANVAS
	public float canvasFadeSpeed = 0.01f;

	public  bool toggleCanvas = true;
	public  bool canvasFadeStatus = true;

	public RawImage bgImage;
	public Image    bgPanel;
	public Text     titleText;
	public Image    exitButton;
	public GameObject itemLister;

	private int itemCount;

	private core_ref 	   cref;
	private core_collect   ccol;
	private core_info      cinfo;
	private core_control   cctrl;

	void Start()
	{
		cref  = new core_ref();
		ccol  = GameObject.Find("CORE").GetComponent<core_collect>(); 
		cinfo = ccol.gameObject.GetComponent<core_info>();
		cctrl = ccol.gameObject.GetComponent<core_control>();
	}

	#endregion

	#region perFrame

	void FixedUpdate()
	{
		#region Canvas Toggle

		if(toggleCanvas)
		{
			if(canvasFadeStatus)
			{
				canvasFadeStatus = false;
				StopAllCoroutines();
				StartCoroutine(canvasFadeOut());

				for(int i=0;i<itemLister.transform.childCount;i++)
				{
					Destroy(itemLister.transform.GetChild(i).gameObject);
				}
			}
			else
			{
				canvasFadeStatus = true;
				StopAllCoroutines();
				StartCoroutine(canvasFadeIn());

				if(!cctrl.specialMode)
				{
					updateTableGUI();
				}
				else
				{
					updateWeaponGUI();
				}
			}

			toggleCanvas = false;


		}

		if(canvasFadeStatus)
		{
			exitButton.gameObject.GetComponent<Button>().onClick.AddListener(delegate 
            { 
				if(canvasFadeStatus)
				{
					if(cctrl.specialMode)
					{
						cctrl.specialMode=false;
						toggleCanvas=true;
					}
					else
					{
						toggleCanvas=true;
					}
				}
			});


		}


		#endregion


	}

	void OnMouseOver()
	{
		if(!cctrl.specialMode)
		{
			if(Input.GetMouseButtonUp(0))
			{
				toggleCanvas=true;
			}
		}
	}

	#endregion

	#region Commands

	//Will fade the canvas colors in.
	public IEnumerator canvasFadeIn()
	{
		Color oc = bgImage.color;
		float currentAlpha = bgImage.color.a;

		while(currentAlpha<0.8f)
		{
			currentAlpha+=0.1f;
			yield return new WaitForSeconds(canvasFadeSpeed);
			bgImage.color    = new Color(oc.r,oc.g,oc.b,currentAlpha);
			bgPanel.color    = new Color(oc.r,oc.g,oc.b,currentAlpha);
			titleText.color  = new Color(oc.r,oc.g,oc.b,currentAlpha);
			exitButton.color = new Color(oc.r,oc.g,oc.b,currentAlpha);

			yield return 0;
		}
	}

	//Will fade the canvas colors out.
	public IEnumerator canvasFadeOut()
	{
		Color oc = bgImage.color;
		float currentAlpha = bgImage.color.a;
		
		while(currentAlpha>0)
		{
			currentAlpha-=0.1f;
			yield return new WaitForSeconds(canvasFadeSpeed);
			bgImage.color    = new Color(oc.r,oc.g,oc.b,currentAlpha);
			bgPanel.color    = new Color(oc.r,oc.g,oc.b,currentAlpha);
			titleText.color  = new Color(oc.r,oc.g,oc.b,currentAlpha);
			exitButton.color = new Color(oc.r,oc.g,oc.b,currentAlpha);
			
			yield return 0;
		}
	}

	//Will simply provide the amount of items.
	public int getIC(string gItem)
	{
		int iCounter = 0;

		for(int i=0;i<storage.Count;i++)
		{
			if(storage[i] == gItem)
				iCounter++;
		}

		return iCounter;
	}

	public Dictionary<int,int> getStorageContType()
	{
		Dictionary<int , int> container = new Dictionary<int, int>();
		
		for(int i=0;i<storage.Count;i++)
		{
			if(container.ContainsKey(cref.item[storage[i]].iType))
				container[cref.item[storage[i]].iType]++;
			else
				container.Add(cref.item[storage[i]].iType,1);
		}
		
		return container;
	}


	//Will return a dick which contains the storage content.
	public Dictionary<string,int> getStorageCont()
	{
		Dictionary<string , int> container = new Dictionary<string, int>();

		for(int i=0;i<storage.Count;i++)
		{
			if(container.ContainsKey(storage[i]))
				container[storage[i]]++;
			else
				container.Add(storage[i],1);
		}

		return container;
	}

	//Will return a dick which contains the weapon content
	public Dictionary<string,int> getWeaponCont()
	{
		Dictionary<string , int> container = new Dictionary<string, int>();
		
		for(int i=0;i<storage.Count;i++)
		{
			if(cref.item[storage[i]].iType == 3 || cref.item[storage[i]].iType == 4 || cref.item[storage[i]].iType == 5)
			{
				if(container.ContainsKey(storage[i]))
					container[storage[i]]++;
				else
					container.Add(storage[i],1);
			}
		}
		return container;
	}


	//Will remove items from storage by Dick
	public void removeItemsByDick(Dictionary<int ,int> givenDick)
	{
		for(int a=0;a<givenDick.Count;a++)
		{
			removeItemsByType(givenDick.Keys.ElementAt(a),givenDick[givenDick.Keys.ElementAt(a)]);
		}
	}

	//Will remove an amount of items from the storage by type.
	public void removeItemsByType(int gType,int gAmount)
	{
		int tCounter = 0;

		for(int i=0;i<storage.Count;i++)
		{
			if(tCounter>=gAmount)
			{
				break;
			}
			else
			{
				if(cref.item[storage[i]].iType == gType)
				{
					storage.RemoveAt(i);
					tCounter++;
				}
			}
		}
	}

	//Will update the storage list's GUI.
	private void updateTableGUI()
	{
		Text currIT;
		
		Dictionary<string, int> tempDick = getStorageCont();
		
		for(int i=0;i<tempDick.Keys.Count;i++)
		{
			GameObject tempObj = (GameObject) Instantiate(ccol.ocol["listObj"]);
			tempObj.transform.SetParent(itemLister.transform,false);

			//Text
			currIT = itemLister.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>();
			
			string contentText = "";
			contentText += "X";
			contentText += tempDick[tempDick.Keys.ElementAt(i)];
			contentText += " - ";
			contentText += cref.item[tempDick.Keys.ElementAt(i)].iName;
			contentText += " - ";
			contentText += cref.item[tempDick.Keys.ElementAt(i)].iValue;
			contentText += "$";
			currIT.text = contentText;

			//Icon
			RawImage ri = itemLister.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>();
			ri.texture = ccol.t2dcol[cref.item[tempDick.Keys.ElementAt(i)].iIcon];

			//Button
			string captured = tempDick.Keys.ElementAt(i);

			Button tb = itemLister.transform.GetChild(i).GetComponent<Button>();
			tb.onClick.AddListener( delegate 
            {
				toggleCanvas = true;
				sellItem(captured);


			});

		}
	}

	//Will update the storage list's GUI.
	public void updateWeaponGUI()
	{

		Text currIT;
		Dictionary<string, int> tempDick = getWeaponCont();
		
		for(int i=0;i<tempDick.Keys.Count;i++)
		{
			GameObject tempObj = (GameObject) Instantiate(ccol.ocol["listObj"]);
			tempObj.transform.SetParent(itemLister.transform,false);
			
			//Text
			currIT = itemLister.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>();
			
			string contentText = "";
			contentText += "X";
			contentText += tempDick[tempDick.Keys.ElementAt(i)];
			contentText += " - ";
			contentText += cref.item[tempDick.Keys.ElementAt(i)].iName;
			currIT.text = contentText;
			
			//Icon
			RawImage ri = itemLister.transform.GetChild(i).transform.GetChild(1).GetComponent<RawImage>();
			ri.texture = ccol.t2dcol[cref.item[tempDick.Keys.ElementAt(i)].iIcon];
			
			//Button
			string captured = tempDick.Keys.ElementAt(i);
			int capturedAmount = tempDick[tempDick.Keys.ElementAt(i)];
			Button tb = itemLister.transform.GetChild(i).GetComponent<Button>();

			tb.onClick.AddListener( delegate 
			                       {
				toggleCanvas = true;
				cctrl.specialMode=false;
				cctrl.cmdPickWep(captured,capturedAmount);
				
			});
			
		}
	}

	//Will sell an item from the storage.
	public void sellItem(string itemName)
	{
		for(int i=0;i<storage.Count;i++)
		{
			if(storage[i] == itemName)
			{
				cinfo.currentCash += cref.item[storage[i]].iValue;
				toggleCanvas=true;
				ccol.relocate("cash_marker",transform.position+transform.TransformDirection(0,10,0));
				pointer_effect tp = ccol.rcol["cash_marker"].GetComponent<pointer_effect>();
				print (i);
				tp.givenText = "+"+cref.item[storage[i]].iValue+"$";
				tp.StartCoroutine(tp.triggerSelf());
				storage.RemoveAt(i);
				break;
			}
		}
	}
	#endregion

}
