using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This MB class handles player commands sent to selected units.
/// -Contains the selection system.
/// -Basic building system.
/// -Unit group commands, like relocate, build, attack, etc...
/// -Basic cover engagement.
/// -Command types ( Like running / walking the current command and so on. )
/// </summary>

public class core_control : MonoBehaviour {

	#region Parameters 

	//Utility
	public core_collect   ccol;
	public core_interface cinter;
	public core_info      cinfo;
	public Camera mainCam;

	//Basic
	private Vector3 startPos;
	private Vector3 endPos;

	private GameObject selectionObj;

	public LayerMask lm;
	
	public int commandState;

	public actor_control actrl;

	private stru_storage tempStorage;

	//Build
	public bool buildMode;
	public GameObject buildLayout;

	//Special
	public bool specialMode;
	public bool doubleClicked;

	//Cover
	public GameObject tcf;
	public misc_recordPoints tcfRO;
	
	
	void Start()
	{

		ccol   = GetComponent<core_collect>();
		cinter = GetComponent<core_interface>();
		cinfo  = GetComponent<core_info>();

		selectionObj = ccol.rcol["selection"];

		#region interButts

		//Build Workshop
		cinter.buildWS.onClick.AddListener( delegate 
	 	{ 
			buildMode = true; 
			if(buildLayout==null)
			{
				//WORKSHOP
				buildLayout = (GameObject) Instantiate(ccol.ocol["building_layout"]);
				buildLayout.GetComponent<build_layout>().targetStructure = "structure_workshop";
				buildLayout.GetComponent<build_layout>().scrapsAmount    = 20;
				buildLayout.GetComponent<build_layout>().vetLevel        = 0;
			}
		});

		//Build Radio
		cinter.buildRadio.onClick.AddListener( delegate 
       	{ 
			buildMode = true; 
			if(buildLayout==null)
			{
				//WORKSHOP
				buildLayout = (GameObject) Instantiate(ccol.ocol["building_layout"]);
				buildLayout.GetComponent<build_layout>().targetStructure = "structure_radiotower";
				buildLayout.GetComponent<build_layout>().scrapsAmount    = 10;
				buildLayout.GetComponent<build_layout>().vetLevel        = 0;
			}
		});



		cinter.leftHand.onClick.AddListener(delegate  { actrl.dropItem(); });
		cinter.rightHand.onClick.AddListener(delegate { actrl.dropItem(); });
		cinter.backHand.onClick.AddListener(delegate  { actrl.dropItem(); });

		//Drop weapon
		cinter.weaponCage.onClick.AddListener(delegate 
  		{ 
			if(actrl.GetComponent<actor_info>().weaponItem != "")
			{
				actrl.dropWeapon();
			} 
		});

		//Small Menu
		//STORE CARRIAGE
		Button cb = cinter.smallMenu.transform.GetChild(0).gameObject.GetComponent<Button>();
		cb.onClick.AddListener(delegate 
        { 
			cmdStore();
			specialMode=false;
		});

		//STORE WEAPON
		cb = cinter.smallMenu.transform.GetChild(1).gameObject.GetComponent<Button>();
		cb.onClick.AddListener(delegate 
		{ 
			cmdStoreWep();
			specialMode=false;
		});

		//PICK WEAPON
		cb = cinter.smallMenu.transform.GetChild(2).gameObject.GetComponent<Button>();
		cb.onClick.AddListener(delegate 
		{ 
			tempStorage.toggleCanvas=true;
		});

		#endregion
	}
	
	#endregion
	
	#region Per Frame
	
	void Update()
	{
		if(tcf==null)
		{
			tcf = ccol.rcol["coverFinder"];
			tcfRO = tcf.GetComponent<misc_recordPoints>();
		}

		//Mouse ray
		Ray mr = mainCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit rchit;

		#region Selection
		
		if(Physics.Raycast(mr,out rchit, 10000,lm))
		{
			if(Input.GetMouseButtonDown(0))
			{
				#region Started Selection

				startPos = rchit.point;

				if(Input.mousePosition.y>Screen.height/5)
				{
					if(!specialMode)
					{
						for(int i=0;i<cinter.selectedUnits.Count;i++)
						{
							cinter.selectedUnits[i].GetComponent<actor_info>().projObj.SetActive(false);
						}

						cinter.selectedUnits.Clear();
					}
				}

				#endregion
			}
			else if(Input.GetMouseButton(0))
			{
				#region Selecting
				endPos = rchit.point;
				
				ccol.relocate("selection",Vector3.Lerp(startPos,endPos,0.5f));
				
				float distX = Mathf.Abs(endPos.x-startPos.x);
				float distZ = Mathf.Abs(endPos.z-startPos.z);
				
				selectionObj.transform.localScale = new Vector3(distX,1,distZ);
				#endregion
			}
			else if(Input.GetMouseButtonUp(0))
			{
				#region Finished Selection
				misc_recordObjects unitCol = selectionObj.GetComponent<misc_recordObjects>();

				if(unitCol.within!=null)
				{
					if(!specialMode)
					{
						for(int i=0;i<unitCol.within.Count;i++)
						{
							if(unitCol.within[i].GetComponent<actor_info>().faction==cinfo.playerFaction)
							{
								unitCol.within[i].GetComponent<actor_info>().projObj.SetActive(true);
								cinter.selectedUnits.Add(unitCol.within[i].gameObject);
							}
						}
					}

					unitCol.within.Clear();
				}
				ccol.hide("selection");
				#endregion
			}
		}

		#endregion

		#region Command

		//Switch command states
		if(Input.GetMouseButtonUp(2))
		{
			if(commandState<1)
				commandState++;
			else
				commandState = 0;
		}

		//Command to go / scavage/ attack
		if(Input.GetMouseButtonDown(1))
		{

			if(commandState == 0)
				Cursor.SetCursor(ccol.t2dcol["cursorw"],new Vector2(0,0),CursorMode.Auto);
			else if(commandState == 1)
				Cursor.SetCursor(ccol.t2dcol["cursorp"],new Vector2(0,0),CursorMode.Auto);

			//Aim arrow
			ccol.relocate("lookArrow",rchit.point+transform.TransformDirection(0,3,0));

			//Scale upon unit count.
			tcf.transform.localScale = new Vector3(cinter.selectedUnits.Count*3,cinter.selectedUnits.Count*3,cinter.selectedUnits.Count*3);

		}
		else if(Input.GetMouseButton(1))
		{
			//Aim arrow.
			ccol.rcol["lookArrow"].transform.rotation = Quaternion.LookRotation(rchit.point-ccol.rcol["lookArrow"].transform.position);
			ccol.rcol["lookArrow"].transform.rotation *= Quaternion.Euler(90,-90,1);

			ccol.relocate("coverFinder",rchit.point);

			
		}
		else if(Input.GetMouseButtonUp(1))
		{

			Cursor.SetCursor(null,new Vector2(0,0),CursorMode.Auto);
			StartCoroutine(doubleClick());

			if(rchit.collider.gameObject.tag=="layout")
			{
				ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Build";

				for(int i=0;i<cinter.selectedUnits.Count;i++)
				{

					cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
					
					StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().buildLayout(rchit.collider.gameObject));
				}
			}
			else if(rchit.collider.gameObject.tag=="item")
			{
				ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Pick Up";

				cinter.selectedUnits[0].GetComponent<actor_control>().StopAllCoroutines();
				
				StartCoroutine(cinter.selectedUnits[0].GetComponent<actor_control>().goPick(rchit.collider.gameObject));
			}
			else if(rchit.collider.gameObject.tag=="storage")
			{
				ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Storage Options";
				cinter.smallMenu.SetActive(true);
				cinter.smallMenu.GetComponent<RectTransform>().position=Input.mousePosition;
				tempStorage = rchit.collider.gameObject.GetComponent<stru_storage>();

				if(tempStorage.canvasFadeStatus)
					tempStorage.toggleCanvas=true;

				specialMode=true;
			}
			else if(rchit.collider.gameObject.tag=="habitable")
			{
				ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Get In";

				for(int i=0;i<cinter.selectedUnits.Count;i++)
				{
					cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
					StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().goGetIn(rchit.collider.gameObject));
				}
			}
			else if(rchit.collider.gameObject.tag=="unit")
			{
				GameObject tempUnit = rchit.collider.gameObject;

				//Enemy check
				if(cinfo.factions[0].fRelations[tempUnit.GetComponent<misc_pdt>().faction])
				{
					ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Attack";

					for(int i=0;i<cinter.selectedUnits.Count;i++)
					{
						if(cinter.selectedUnits[i].GetComponent<actor_info>().weaponItem!="")
						{
							cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
							StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().goAttack(tempUnit));
						}
					}
				}
				else
				{
//					ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Follow.";
//
//					for(int i=0;i<cinter.selectedUnits.Count;i++)
//					{
//						cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
//						StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().followObj(tempUnit,5));
//					}
				}

			}
			else
			{
				if(commandState==0)
				{
					ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Relocate";

					//Remove taken covers from cover collection.
					for(int i=0;i<tcfRO.within.Count;i++)
					{
						if(tcfRO.within[i].GetComponent<cover_info>().taken)
							tcfRO.within.RemoveAt(i);
					}

					//Setting covers for units:
					for(int i=0;i<cinter.selectedUnits.Count;i++)
					{
						actor_info tainfo = cinter.selectedUnits[i].GetComponent<actor_info>();

						if(tainfo.currentCover!=null)
						{
							tainfo.currentCover.GetComponent<cover_info>().taken = false;
							tainfo.currentCover = null;
						}

						//Set in covers
						if(i<tcfRO.within.Count)
						{
							Instantiate(ccol.ocol["cover_dummy"],tcfRO.within[i].transform.position,tcfRO.within[i].transform.rotation);
							cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
							
							StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().walkOnPosition(tcfRO.within[i].transform.position));

							tainfo.currentCover = tcfRO.within[i];
							tcfRO.within[i].GetComponent<cover_info>().taken = true;
						}
						else//Set if no cover.
						{
							cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
							
							StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().walkToPosition(rchit.point+new Vector3(i-tcfRO.within.Count,0,0)));
						}
					}

				}
				else if(commandState==1)
				{
					ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().givenText = "Scavage";

					for(int i=0;i<cinter.selectedUnits.Count;i++)
					{
						cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
						StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().gatherFrom(rchit.point));
					}
				}
			}

			//Set pointer on point and trigger.
			ccol.relocate("pointed_spot",rchit.point);
			ccol.rcol["pointed_spot"].transform.rotation = Quaternion.Euler(-90,1,1);
			StartCoroutine(ccol.rcol["pointed_spot"].GetComponent<pointer_effect>().triggerSelf());
			ccol.hide("lookArrow");
		}

		//Drop items
		if(cinter.selectedUnits.Count>0 && cinter.selectedUnits[0]!=null)
		{
			actrl = cinter.selectedUnits[0].GetComponent<actor_control>();
		}


		#endregion

		#region Build




		if(buildMode && rchit.collider.gameObject!=null)
		{
			if(buildLayout!=null)
			{
				buildLayout.transform.position = rchit.point + new Vector3(0,3,0);


			}

			if(Input.GetMouseButtonDown(0) && buildLayout.GetComponent<build_layout>().hasSpace)
			{
				buildLayout.layer = 11;
				buildLayout.GetComponent<BoxCollider>().size+= new Vector3(0,150,0);
				buildLayout = null;
				buildMode = false;
			}
			else if(Input.GetMouseButtonUp(1))
			{
				Destroy(buildLayout);
				buildLayout = null;
				buildMode=false;
			}

		}
		#endregion

		if(cinter.selectedUnits.Count>0)
		{
			for(int i=0;i<cinter.selectedUnits.Count;i++)
			{
				if(cinter.selectedUnits[i]==null)
				{
					cinter.selectedUnits.RemoveAt(i);
				}
//				if(!cinter.selectedUnits[i].activeInHierarchy || cinter.selectedUnits[i] == null)
//				{
//					cinter.selectedUnits.RemoveAt(i);
//				}
			}
		}
	}

	void LateUpdate()
	{
		if(doubleClicked)
		{
			doubleClicked = false;

			if(cinter.selectedUnits.Count>0)
			{
				for(int i=0;i<cinter.selectedUnits.Count;i++)
				{
					cinter.selectedUnits[i].GetComponent<actor_control>().running = true;
				}
			}
		}
	}

	#endregion

	#region Commands

	//Command group to store carriage in storage.
	public void cmdStore()
	{
		for(int i=0;i<cinter.selectedUnits.Count;i++)
		{
			cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
			
			StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().goStoreAll());
		}
	}
	
	//Command group to store weapon in storage
	public void cmdStoreWep()
	{
		for(int i=0;i<cinter.selectedUnits.Count;i++)
		{
			cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();
			
			StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().goStoreWep());
		}
	}

	//Will command the group to pick a weapon from the storage.
	public void cmdPickWep(string gWep,int amount)
	{
		int pCounter = 0;
		for(int i=0;i<cinter.selectedUnits.Count;i++)
		{
			if(pCounter>=amount)
			{
				break;
			}
			else
			{
				if(cinter.selectedUnits[i].GetComponent<actor_info>().weaponItem == "")
				{
					cinter.selectedUnits[i].GetComponent<actor_control>().StopAllCoroutines();

					StartCoroutine(cinter.selectedUnits[i].GetComponent<actor_control>().goStorePick(gWep)); 

					pCounter++;
				}
			}
		}
	}

	//Double click check.
	private IEnumerator doubleClick()
	{
		yield return new WaitForSeconds(0.1f);
		float i = info_input.dcResponse;
		while(i > 0)
		{
			if(Input.GetMouseButtonUp(1))
				doubleClicked=true;

			i-=1*Time.deltaTime;
			yield return 0;
		}
	}
	#endregion
}
