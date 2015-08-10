using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// This MB class will work with the advanced UI system.
/// Will handle selection, unit information buttons / commands.
/// And building menu icons.
/// Will also handle misc menus. 
/// </summary>

public class core_interface : MonoBehaviour {

	#region Parameters

	//Utility
	public core_collect ccol;
	public core_info    cinfo;
	public core_control cctrl;
	public Texture2D    cursor;
	public Texture2D    defaultWeapon;
	public core_ref     infoRef;

	//CANVAS
	public GameObject vetBarRifle;
	public GameObject vetBarMelee;
	public GameObject vetBarBuild;
	public GameObject vetBarMecha;
	public GameObject vetBarElect;
	public GameObject smallMenu;
	public Button opt1;
	public Button opt2;
	public Button opt3;
	public Button leftHand;
	public Button backHand;
	public Button rightHand;
	public Button buildWS;
	public Button buildRadio;
	public Button buildGarage;
	public Button weaponCage;
	public Text   moneyText;

	//Basic
	public List<GameObject> selectedUnits = new List<GameObject>();
	private GameObject lastSelected;

	void Start()
	{
		ccol  = GetComponent<core_collect>();
		cinfo = GetComponent<core_info>();
		cctrl = GetComponent<core_control>();
		infoRef = new core_ref();


	}

	#endregion

	#region Perframe

	void Update()
	{
		#region Selected Units

		if(selectedUnits.Count>0)
		{
			if(selectedUnits[0]!=lastSelected)
			{
				lastSelected = selectedUnits[0];

				actor_info ainfo = selectedUnits[0].GetComponent<actor_info>();

				uLevelBar(vetBarRifle, ainfo.rifleVet);
				uLevelBar(vetBarMelee, ainfo.meleeVet);
				uLevelBar(vetBarBuild, ainfo.buildVet);
				uLevelBar(vetBarMecha, ainfo.mechaVet);
				uLevelBar(vetBarElect, ainfo.electVet);

			}
			else
			{
				//Update as long as selected:
				uCarryButtons();

				if(selectedUnits[0].GetComponent<actor_info>().weaponItem != "")
					weaponCage.transform.GetChild(0).GetComponent<RawImage>().texture = ccol.t2dcol[infoRef.item[selectedUnits[0].GetComponent<actor_info>().weaponItem].iBigIcon];
				else
					weaponCage.transform.GetChild(0).GetComponent<RawImage>().texture = defaultWeapon;
			}

		}
		else
		{
			if(lastSelected!=null)
			{
				lastSelected = null;
				
				uLevelBar(vetBarRifle, 0);
				uLevelBar(vetBarMelee, 0);
				uLevelBar(vetBarBuild, 0);
				uLevelBar(vetBarMecha, 0);
				uLevelBar(vetBarElect, 0);

				leftHand.transform.GetChild(0).GetComponent<Text>().text   = "Hand";
				backHand.transform.GetChild(0).GetComponent<Text>().text   = "Back";
				rightHand.transform.GetChild(0).GetComponent<Text>().text  = "Hand";
				weaponCage.transform.GetChild(0).GetComponent<RawImage>().texture = null;
			}
		}

		#endregion

		#region Command States

		if(cctrl.commandState==0)
		{
			opt1.gameObject.GetComponent<Image>().color = Color.white;
			opt2.gameObject.GetComponent<Image>().color = Color.green;
			opt3.gameObject.GetComponent<Image>().color = Color.white;
		}
		else if(cctrl.commandState==1)
		{
			opt1.gameObject.GetComponent<Image>().color = Color.white;
			opt2.gameObject.GetComponent<Image>().color = Color.white;
			opt3.gameObject.GetComponent<Image>().color = Color.green;
		}

		opt2.onClick.AddListener(delegate  { cctrl.commandState = 0; });
		opt3.onClick.AddListener(delegate  { cctrl.commandState = 1; });

		#endregion

		#region Constant Update

		moneyText.text = cinfo.currentCash+"$";

		#endregion
	}
	
	#endregion

	#region Commands

	//Will get a canvas level bar and update its textures
	//by the given number (Vet bar for example).
	private void uLevelBar(GameObject gBar, int gLevel)
	{
		for(int i=0;i<gBar.transform.childCount;i++)
		{
			RawImage ri = gBar.transform.GetChild(i).GetComponent<RawImage>();

			if(i<gLevel)
				ri.texture = (Texture) ccol.t2dcol["UI_button_1"];
			else
				ri.texture =  ccol.t2dcol["UI_button_0"];
		}
	}

	private void uCarryButtons()
	{
		if(selectedUnits.Count>0)
		{
			if(selectedUnits[0]!=null)
			{
				actor_info ainfo = selectedUnits[0].GetComponent<actor_info>();

				if(ainfo.leftCarry!=null)
					leftHand.transform.GetChild(0).GetComponent<Text>().text = infoRef.item[ainfo.leftCarry.name].iName;
				else
					leftHand.transform.GetChild(0).GetComponent<Text>().text = "Nothing";
				
				if(ainfo.backCarry!=null)
					backHand.transform.GetChild(0).GetComponent<Text>().text = infoRef.item[ainfo.backCarry.name].iName;
				else
					backHand.transform.GetChild(0).GetComponent<Text>().text = "Nothing";
				
				if(ainfo.rightCarry!=null)
					rightHand.transform.GetChild(0).GetComponent<Text>().text = infoRef.item[ainfo.rightCarry.name].iName;
				else
					rightHand.transform.GetChild(0).GetComponent<Text>().text = "Nothing";
			}
		}
	}

	#endregion

}
