using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class made for a specific structure.
/// </summary>

public class stru_radio : MonoBehaviour {

	public GameObject canvasHolder;
	public GameObject companiesGrid;
	public GameObject transmittionText;
	public GameObject habButt;

	private core_reinforcements crein;
	private core_info           cinfo;
	private core_collect       ccol;
	private misc_habitable     mhab;


	private float cooldown = 0;

	void Start()
	{
		cinfo 			  = GameObject.Find("CORE").GetComponent<core_info>();
		ccol 			  = cinfo.gameObject.GetComponent<core_collect>();
		crein             = cinfo.gameObject.GetComponent<core_reinforcements>();
		mhab			  = GetComponent<misc_habitable>();

		//Habitable spot
		habButt.GetComponent<Button>().onClick.AddListener(delegate 
		{
			mhab.dropMember();
		});

		//Fill grid with companies and products.

		for(int i=0;i<cinfo.companies.Count;i++)
		{
			info_company cominfo = cinfo.companies[i];

			GameObject tempComp = (GameObject) Instantiate(ccol.ocol["co_win"]);
			tempComp.transform.SetParent(companiesGrid.transform,false);
			tempComp.transform.GetChild(0).GetComponent<Text>().text = cominfo.cName;

			for(int a=0;a<cominfo.products.Length;a++)
			{
				//Set Button:
				GameObject tempProd = (GameObject) Instantiate(ccol.ocol["co_item"]);
				tempProd.transform.SetParent(tempComp.transform.GetChild(1).transform,false);

				//Set Icon:
				Texture2D tempT = ccol.t2dcol[cominfo.products[a].productIcon];
				tempProd.GetComponent<Image>().sprite = Sprite.Create(tempT,new Rect(0,0,tempT.width,tempT.height),new Vector2(0,0));

				//Set Listener:
				Button currentB = tempProd.GetComponent<Button>();

				int capturedI = i;
				int capturedA = a;
				currentB.onClick.AddListener(delegate 
                {
					if(cinfo.currentCash >= cinfo.companies[capturedI].products[capturedA].productValue)
					{
						cooldown = cinfo.companies[capturedI].products[capturedA].productCooldown;
						purchaseProduct(cinfo.companies[capturedI].products[capturedA]);
					}
				});
			}
		}


	}

	void Update()
	{
		if(mhab.members.Count>0)
		{
			//Set habitated icon.
			if(habButt.GetComponent<Image>().sprite.texture.name != "icon_manspot_1")
			{
				Texture2D tIcon = ccol.t2dcol["icon_manspot_1"];
				habButt.GetComponent<Image>().sprite = Sprite.Create(tIcon,new Rect(0,0,tIcon.width,tIcon.height),new Vector2(0.5f,0.5f));
			}

			//Transmittion cooldown.
			if(cooldown>0)
			{
				cooldown-=1*Time.deltaTime;
				transmittionText.SetActive(true);
				companiesGrid.SetActive(false);

			}
			else
			{
				transmittionText.SetActive(false);
				companiesGrid.SetActive(true);
			}
		}
		else
		{
			companiesGrid.SetActive(false);
			transmittionText.SetActive(false);

			//Set inhabitated icon
			if(habButt.GetComponent<Image>().sprite.texture.name != "icon_manspot_0")
			{
				Texture2D tIcon = ccol.t2dcol["icon_manspot_0"];
				habButt.GetComponent<Image>().sprite = Sprite.Create(tIcon,new Rect(0,0,tIcon.width,tIcon.height),new Vector2(0.5f,0.5f));
			}
		}
	}

	//Will attempt to purchase a reinforcement. 
	public void purchaseProduct(info_product gProduct)
	{
		cinfo.currentCash -= gProduct.productValue;

		if(gProduct.productUnit == "actor")
		{
			StartCoroutine(crein.spawnUnit());
		}
	}





	//Toggle UI window on.
	void OnMouseOver()
	{
		if(Input.GetMouseButtonUp(0))
		{
			canvasHolder.SetActive(true);
		}
	}
}
