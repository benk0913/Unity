using UnityEngine;
using System.Collections;

/// <summary>
/// Will contain the hard-coded command collection of the units.
/// The commands are divided in to a some sort of "Tier Hierarchy."
/// High tiers would contain complex commands while low tier command 
/// would be more simple. Low tier commands would never use a command 
/// from a higher tier inside them, while higher tier commands will
/// depend on lower tier methods.
/// 
/// For example, "SearchForItems would use "walk to position" coroutine,
/// "pick item" coroutine, scan, goStore, etc...
/// 
/// This class is meant to be controlled by other classes, and help
/// other classes control UNITS better.
/// </summary>

[RequireComponent (typeof(NavMeshAgent))]
public class actor_control : MonoBehaviour {

	#region Parameters
	
	private CapsuleCollider cld;
	private NavMeshAgent    NMA;
	private actor_info      info;
	private core_collect    collect;
	private core_control    cctrl;
	private core_ref        cref;

	public  GameObject      pview;
	public  Animator        anim;

	public  bool 			running;
	public  bool            attacking;
	public  bool 			idle = true;
	public 	GameObject focusObject;



	void Start()
	{
		cld  = GetComponent<CapsuleCollider>();
		NMA  = GetComponent<NavMeshAgent>();
		info = GetComponent<actor_info>();
		collect = GameObject.Find("CORE").GetComponent<core_collect>();
		cctrl   = collect.gameObject.GetComponent<core_control>();
		cref    = new core_ref();

		running = false;
	}

	#endregion

	#region Flow

	void LateUpdate()
	{
		if(focusObject!=null)
		{
			aimAt(focusObject);
		}
	}
	#endregion

	#region Commands

	#region Tier 4

	public IEnumerator gatherFrom(Vector3 gPos)
	{
		GameObject gRegion =  info.getNearestRegion(gPos);

		while(enabled)
		{
			yield return StartCoroutine(searchForItems(gRegion));
			yield return 0;
		}
	}

	#endregion

	#region Tier 3

	//Will search for items to store and store them.
	public IEnumerator searchForItems(GameObject gRegion)
	{
		if(info.weaponRaise)
			unequipWeapon();

		GameObject gStorage = info.selfStorage;
		int  searchAttempts = 0;
		bool foundObj       = false;

		while(!info.isFull())
		{
			foundObj = false;

			while(!foundObj)
			{
				searchAttempts++;
				Vector3 tPos = info.randomRegionPosition(gRegion);

				yield return StartCoroutine(walkToPosition(tPos));
				idle = false;

				anim.SetInteger("sType", Random.Range(0,2));
				anim.SetTrigger("search");
				float rndTime = Random.Range(0.3f,3);
				yield return new WaitForSeconds(rndTime);
				
				misc_recordObjects mro = pview.GetComponent<misc_recordObjects>();
				
				for(int i=0;i<mro.within.Count;i++)
				{
					if(info.isFull())
					{
						break;
					}
					else
					{
						if(info.canSee(mro.within[i]))
						{
							yield return StartCoroutine(goPick(mro.within[i]));
							idle = false;
							foundObj = true;
						}
					}
				}

				if(searchAttempts>20)
					break;

				yield return 0;
			}

			if(searchAttempts>20)
				break;

			yield return 0;
		}


		yield return StartCoroutine(goStoreAll());
		idle = true;
	}

	//Will build the designated layout.
	public IEnumerator buildLayout(GameObject gLayout)
	{
		idle = false;
		misc_recordObjects lobj = gLayout.GetComponent<misc_recordObjects>();
		build_layout       bl   = gLayout.GetComponent<build_layout>();

		int requiredScrap = bl.scrapsAmount - lobj.within.Count;

		while(requiredScrap>0)
		{
			if(gLayout!=null)
			{
				if(info.isFull())
				{
					yield return StartCoroutine(walkToObject(gLayout));
					idle = false;

					if(info.rightCarry!=null)
					{
						dropItem();
					}
					if(info.leftCarry!=null)
					{
						dropItem();
					}
					if(info.backCarry!=null)
					{
						dropItem();
					}

					yield return new WaitForSeconds(1);
					
				}
				else
				{
					GameObject ep = info.selfStorage.GetComponent<stru_storage>().entrancePoint;
					yield return StartCoroutine(walkToObject(ep));
					idle = false;

					string[] prepVariety = new string[3];
					prepVariety[0] = "item_ms_a";
					prepVariety[1] = "item_ms_b";
					prepVariety[2] = "item_ms_c";

					if(info.backCarry==null)
					{
						if(!pickAnyStorage(prepVariety))
						{
							break;
						}
					}

					if(info.leftCarry==null)
					{
						if(!pickAnyStorage(prepVariety))
						{
							break;
						}
					}

					if(info.rightCarry==null)
					{
						if(!pickAnyStorage(prepVariety))
						{
							break;
						}
					}

				}

				requiredScrap = bl.scrapsAmount - lobj.within.Count;

			}
			else
				break;

			yield return 0;
		}

		if(gLayout!=null && requiredScrap<=0)
		{
			yield return StartCoroutine(walkToObject(gLayout));

			int scrapper = 0;
			for(int i=0;i<lobj.within.Count;i++)
			{
				scrapper++;

				if(scrapper<bl.scrapsAmount)
				{
					Destroy(lobj.within[i]);
				}
				else
				{
					lobj.within[i].transform.position += transform.TransformDirection(0,0,-8);
				}

			}

			transform.position += transform.TransformDirection(0,0,-5);
			Instantiate(collect.ocol["build_effect"],gLayout.transform.position+transform.TransformDirection(0,-3.5f,0),gLayout.transform.rotation);
			Instantiate(collect.ocol[bl.targetStructure],gLayout.transform.position+transform.TransformDirection(0,-3.5f,0),gLayout.transform.rotation);
			Destroy(gLayout);
			return true;
		}


	}

	#endregion

	#region Tier 2

	//Will look for closest free cover in the surrounding area.
	public IEnumerator lookForCover()
	{
		info.lookingForCover = true;
		collect.relocate("coverSurpress",transform.position);
		yield return new WaitForSeconds(0.1f);
		
		misc_recordPoints mrp = collect.rcol["coverSurpress"].GetComponent<misc_recordPoints>();
		
		if(mrp.within.Count>0)
		{
			int bestCover = -1;
			int bestRange = 9999;

			//Look for the best cover.
			for(int i=0;i<mrp.within.Count;i++)
			{
				if(!mrp.within[i].GetComponent<cover_info>().taken)
				{
					if(info.legitCover(mrp.within[i], focusObject))
					{
						int currentRange = Mathf.FloorToInt(Vector3.Distance(transform.position , mrp.within[i].transform.position));

						if(currentRange<bestRange)
						{
							bestCover = i;
							bestRange = currentRange;
						}
					}
				}
			}

			if(bestCover!=-1)
			{
				StopAllCoroutines();
				
				running = true;
				
				StartCoroutine(walkOnPosition(mrp.within[bestCover].transform.position));
				
				info.currentCover = mrp.within[bestCover];
				mrp.within[bestCover].GetComponent<cover_info>().taken = true;
			}
		}

		collect.hide("coverSurpress");
		info.lookingForCover = false;
	}

	//Will go to a habitable obj and attempt to get inside.
	public IEnumerator goGetIn(GameObject gObj)
	{
		idle = false;

		if(info.weaponRaise)
			unequipWeapon();
	
		if(info.rightCarry != null)
		{
			dropItem();
		}
		if(info.leftCarry  != null)
		{
			dropItem();
		}
		if(info.backCarry  != null)
		{
			dropItem();
		}

		yield return StartCoroutine(walkToObject(gObj));
		standStill();
		yield return new WaitForSeconds(0.5f);
		getIn(gObj);

		idle = true;
	}

	//Will go to an item and pick it up.
	public IEnumerator goPick(GameObject gItem)
	{
		idle = false;
		attacking   = false;
		focusObject = gItem;
		yield return StartCoroutine(walkToObject(gItem));
		pickItem(gItem);
		idle = true;
	}

	//Will go to a storage and store everything.
	public IEnumerator goStoreAll()
	{
		idle = false;
		attacking   = false;
		focusObject = info.selfStorage;
		GameObject gStorage = info.selfStorage;
		GameObject ep = gStorage.GetComponent<stru_storage>().entrancePoint;
		yield return StartCoroutine(walkToObject(ep));
		dropStorageAll();
		standStill();
		idle = true;
	}

	//Will go to a storage and pick an item.
	public IEnumerator goStorePick( string gItem)
	{
		idle = false;
		attacking   = false;
		focusObject = info.selfStorage;
		GameObject gStorage = info.selfStorage;
		GameObject ep = gStorage.GetComponent<stru_storage>().entrancePoint;
		yield return StartCoroutine(walkToObject(ep));
		pickStorage(gItem);
		idle = true;
	}

	//Will go to a storage and store it's weapon.
	public IEnumerator goStoreWep()
	{
		idle = false;
		attacking = false;
		focusObject = info.selfStorage;
		GameObject ep = info.selfStorage.GetComponent<stru_storage>().entrancePoint;
		yield return StartCoroutine(walkToObject(ep));
		storeWeapon();
		idle = true;
	}

	//Will follow the object within range and attack it.
	public IEnumerator goAttack(GameObject gUnit)
	{
		idle = false;
		standStill();
		focusObject = gUnit;
		int range = cref.item[info.weaponItem].weaponRange;
		if(!info.weaponRaise)
		{
			equipWeapon();
		}

		//Procedure:
		while(gUnit!=null)
		{
			attacking = true;
			int cRange = Mathf.FloorToInt(Vector3.Distance(this.gameObject.transform.position,gUnit.transform.position));


			if(range<cRange)
			{
				if(cref.item[info.weaponItem].iType==3)
				{
					running = true;
					yield return StartCoroutine(walkToObject(gUnit));
					idle = false;
				}
				else
				{
					yield return StartCoroutine(walkToRange(gUnit,range));
					idle = false;
				}
			}
			else
			{
				//Reload if needed:
				if(cref.item[info.weaponItem].iType != 3 && info.bulletsLeft<=0)
					yield return StartCoroutine(reloadWeapon());

				//Cooldown:
				float tempCD = cref.item[info.weaponItem].weaponCD/10;
				yield return new WaitForSeconds(Random.Range(tempCD/2,tempCD+tempCD/2));

				if(gUnit!=null)
				{
					if(cref.item[info.weaponItem].iType!=3)
					{
						if(!seeTarget(gUnit))
						{
							yield return StartCoroutine(walkToRange(gUnit,range));
							idle = false;
						}
					}

					yield return StartCoroutine(attackUnit(gUnit));
					idle = false;
				}
			}
			
			yield return 0;
		}

		attacking = false;
		idle = true;
		focusObject = null;
	}

	#endregion

	#region Tier 1

	//Will navigate towards a given position.
	public IEnumerator walkToPosition(Vector3 gPos)
	{
		idle = false;
		if(info.crouch)
			standUp();

		attacking = false;
		focusObject = null;

		yield return new WaitForSeconds(Random.Range(0,0.5f));

		NMA.SetDestination(gPos);
		NMA.stoppingDistance = 3;
		anim.SetBool("walk",true);

		yield return new WaitForSeconds(0.5f);
		
		while(NMA.remainingDistance>3)
		{
			yield return 0;
			if(running)
			{
				NMA.speed = 10;
				anim.SetBool("run",true);
			}
		}

		if(running)
		{
			running = false;
			NMA.speed = 3.5f;
			anim.SetBool("run",false);
		}

		idle = true;
		standStill();
		
	}

	//Will navigate towards a given position and stand on it
	public IEnumerator walkOnPosition(Vector3 gPos)
	{
		idle = false;
		if(info.crouch)
			standUp();

		attacking = false;
		focusObject = null;
		
		yield return new WaitForSeconds(Random.Range(0,0.5f));
		
		NMA.SetDestination(gPos);
		NMA.stoppingDistance = 0.1f;
		anim.SetBool("walk",true);
		
		yield return new WaitForSeconds(0.5f);
		
		while(NMA.remainingDistance>0.1f)
		{
			yield return 0;
			if(running)
			{
				NMA.speed = 10;
				anim.SetBool("run",true);
			}
		}
		
		if(running)
		{
			running = false;
			NMA.speed = 3.5f;
			anim.SetBool("run",false);
		}

		idle = true;
		standStill();
		
	}

	//Will navigate towards a given gameobject.
	public IEnumerator walkToObject(GameObject gObj)
	{
		idle = false;
		if(info.crouch)
			standUp();

		focusObject = gObj;

		yield return new WaitForSeconds(Random.Range(0,0.5f));
		if(gObj!=null)
		{
			Vector3 gPos = gObj.transform.position;
			NMA.SetDestination(gPos);
			NMA.stoppingDistance = 2;
			anim.SetBool("walk",true);

			yield return new WaitForSeconds(0.5f);

			while(NMA.remainingDistance>2)
			{
				if(gObj==null)
				{
					break;
				}

				yield return 0;
				if(running)
				{
					NMA.speed = 10;
					anim.SetBool("run",true);
				}
			}
		}
		if(running)
		{
			running = false;
			NMA.speed = 3.5f;
			anim.SetBool("run",false);
		}

		idle = true;
		standStill();
	}

	//Will navigate towards a given position.
	public IEnumerator walkToRange(GameObject gTarget, int gRange)
	{
		idle = false;
		if(info.crouch)
			standUp();

		focusObject = null;
		
		yield return new WaitForSeconds(Random.Range(0,0.5f));

		if(gTarget!=null)
		{
			NMA.SetDestination(gTarget.transform.position);
			NMA.stoppingDistance = 2;
			anim.SetBool("walk",true);
		}
		else
		{
			return false;
		}
		
		yield return new WaitForSeconds(0.5f);

		//Out of range.
		while(NMA.remainingDistance>gRange)
		{
			if(gTarget==null)
			{
				break;
			}

			if(running)
			{
				NMA.speed = 10;
				anim.SetBool("run",true);
			}

			yield return 0;

		}

		//In range
		while(NMA.remainingDistance<gRange)
		{
			if(gTarget == null)
			{
				break;
			}

			//Check if could see target.
			yield return new WaitForSeconds(0.1f);

			if(seeTarget(gTarget))
			{
				break;
			}

			yield return 0;
			if(running)
			{
				NMA.speed = 10;
				anim.SetBool("run",true);
			}
		}
		
		if(running)
		{
			running = false;
			NMA.speed = 3.5f;
			anim.SetBool("run",false);
		}

		idle = true;
		standStill();
		
	}

	//Will navigate towards a given gameobject.
	public IEnumerator followObj(GameObject gObj,int keepDistance)
	{	
		if(info.crouch)
			standUp();

		attacking = false;
		focusObject = gObj;
		NMA.SetDestination(gObj.transform.position);
		NMA.stoppingDistance = keepDistance;
		anim.SetBool("walk",true);
		
		while(true)
		{
			while(NMA.remainingDistance>keepDistance)
			{
				NMA.SetDestination(gObj.transform.position);
			}

			standStill();

			yield return 0;
		}
	}

	//Halt in the current position.
	public void standStill()
	{
		attacking = false;
		focusObject = null;
		NMA.SetDestination(transform.position);
		NMA.stoppingDistance = 0;
		anim.SetBool("walk",false);

		if(info.currentCover!=null)
		{
			if(info.currentCover.GetComponent<cover_info>().type==1)
			{
				goCrouch();
			}
		}
	}

	//Equips weapon in belongings.
	public void equipWeapon()
	{
		if(info.rightCarry != null)
		{
			dropItem();
		}
		if(info.leftCarry  != null)
		{
			dropItem();
		}
		if(info.backCarry  != null)
		{
			dropItem();
		}

		info.bulletsLeft = cref.item[info.weaponItem].magSize;

		anim.SetInteger("weaponType",cref.item[info.weaponItem].iType-2);
		anim.SetBool("aim",true);

		info.weaponObject = (GameObject) Instantiate(collect.ocol[info.weaponItem],info.rightHand.transform.position,info.rightHand.transform.rotation);
		info.weaponObject.transform.parent = info.rightHand.transform;
		info.weaponObject.GetComponent<Rigidbody>().isKinematic = true;
		info.weaponObject.GetComponent<BoxCollider>().enabled = false;
		info.weaponRaise = true;
	}

	public void unequipWeapon()
	{
		anim.SetInteger("weaponType",0);
		anim.SetBool("aim",false);
		
		Destroy(info.weaponObject.gameObject);
		info.weaponObject = null;
		info.weaponRaise = false;
	}

	//Will pick an item.
	public void pickItem(GameObject gItem)
	{
		if(gItem!=null && gItem.tag=="item")
		{
			if(info.weaponRaise)
				unequipWeapon();

			anim.SetTrigger("pickup");
			unprepareWeapon();

			if(gItem.name.Substring(0,6) == "weapon" && info.weaponItem == "")
			{
				info.weaponItem = gItem.name;
				Destroy(gItem);
			}
			else
			{
				gItem.GetComponent<Rigidbody>().useGravity  = false;
				gItem.GetComponent<Rigidbody>().isKinematic = true;
				gItem.GetComponent<BoxCollider>().enabled=false;
				gItem.tag = "Untagged";

				if(info.backCarry==null)
				{
					info.backCarry = gItem;
					gItem.transform.position = info.backGrip.transform.position;
					gItem.transform.rotation = info.backGrip.transform.rotation;
					gItem.transform.parent   = info.backGrip.transform; 
				}
				else if(info.leftCarry==null)
				{
					info.leftCarry = gItem;
					gItem.transform.position = info.leftHand.transform.position;
					gItem.transform.rotation = info.leftHand.transform.rotation;
					gItem.transform.parent   = info.leftHand.transform; 
				}
				else
				{
					if(info.rightCarry!=null)
						dropItem();

					info.rightCarry = gItem;
					gItem.transform.position = info.rightHand.transform.position;
					gItem.transform.rotation = info.rightHand.transform.rotation;
					gItem.transform.parent   = info.rightHand.transform; 
				}
			}
		}
	}

	//Will drop an item
	public void dropItem()
	{
		if(info.weaponRaise)
		{
			unequipWeapon();
		}

		if(info.rightCarry!=null)
		{
			info.rightCarry.transform.parent=null;
			info.rightCarry.GetComponent<Rigidbody>().useGravity  = true;
			info.rightCarry.GetComponent<Rigidbody>().isKinematic = false;
			info.rightCarry.GetComponent<BoxCollider>().enabled=true;
			info.rightCarry.tag = "item";
			info.rightCarry = null;
		}
		else if(info.leftCarry!=null)
		{
			info.leftCarry.transform.parent=null;
			info.leftCarry.GetComponent<Rigidbody>().useGravity  = true;
			info.leftCarry.GetComponent<Rigidbody>().isKinematic = false;
			info.leftCarry.GetComponent<BoxCollider>().enabled=true;
			info.leftCarry.tag = "item";
			info.leftCarry = null;
		}
		else if(info.backCarry!=null)
		{
			info.backCarry.transform.parent=null;
			info.backCarry.GetComponent<Rigidbody>().useGravity  = true;
			info.backCarry.GetComponent<Rigidbody>().isKinematic = false;
			info.backCarry.GetComponent<BoxCollider>().enabled=true;
			info.backCarry.tag = "item";
			info.backCarry = null;
		}
	}

	//Will crouch.
	public void goCrouch()
	{
		info.crouch = true;
		anim.SetBool("crouch",true);
		NMA.height = 4.5f;
		cld.height = 4.5f;
	}

	//Will uncrouch.
	public void standUp()
	{
		info.crouch = false;
		anim.SetBool("crouch",false);
		NMA.height = 9;
		cld.height = 9;
	}

	//Aim at object (Execute from "LateUpdate()"
	public void aimAt(GameObject gObj)
	{
		info.backBone.transform.LookAt(gObj.transform.position,transform.right);

		Quaternion tempQ = Quaternion.LookRotation(gObj.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation,tempQ,Time.deltaTime*5f);
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y , 0);
	}

	//Drop all storage.
	public void dropStorageAll()
	{

		GameObject gStorage = info.selfStorage;
		stru_storage store = gStorage.GetComponent<stru_storage>();

		anim.SetTrigger("grab");
		
		if(info.backCarry!=null)
		{
			store.storage.Add(info.backCarry.name);
			Destroy(info.backCarry);
		}
		if(info.leftCarry!=null)
		{
			store.storage.Add(info.leftCarry.name);
			Destroy(info.leftCarry);
		}
		if(info.rightCarry!=null)
		{	
			store.storage.Add(info.rightCarry.name);
			Destroy(info.rightCarry);
		}
	}

	//Will pick a specific item from storage if exists.
	public bool pickStorage(string gItem)
	{
		stru_storage store = info.selfStorage.GetComponent<stru_storage>();
		for(int i=0;i<store.storage.Count;i++)
		{
			if(store.storage[i] == gItem)
			{
				store.storage.RemoveAt(i);
				GameObject tempItem = (GameObject) Instantiate(collect.ocol[gItem],transform.position,transform.rotation);
				tempItem.name = gItem;
				pickItem(tempItem);
				return true;
			}
		}

		return false;

	}

	//Will pick any item from the given list.
	public bool pickAnyStorage(string[] gVariety)
	{
		stru_storage store = info.selfStorage.GetComponent<stru_storage>();

		for(int i=0;i<store.storage.Count;i++)
		{
			for(int a = 0;a<gVariety.Length;a++)
			{
				if(store.storage[i]==gVariety[a])
				{
					store.storage.RemoveAt(i);
					GameObject tempVarObj = (GameObject) Instantiate(collect.ocol[gVariety[a]]);
					tempVarObj.name = gVariety[a];
					pickItem(tempVarObj);
					return true;
				}
			}
		}

		return false;
	}

	//Will raise his weapon.
	public void prepareWeapon()
	{
		if(info.weaponObject==null);
		{
			dropItem();
			dropItem();
			dropItem();

			info.weaponObject = (GameObject) Instantiate(collect.ocol[info.weaponItem],info.rightHand.transform.position,info.rightHand.transform.rotation);
			info.weaponObject.name = info.weaponItem;
			info.weaponObject.transform.parent = info.rightHand.transform;

			info.weaponObject.GetComponent<Rigidbody>().isKinematic = true;
			info.weaponObject.GetComponent<Rigidbody>().useGravity  = false;


		}
	}

	//Will lower his weapon.
	public void unprepareWeapon()
	{
		if(info.weaponObject!=null)
		{
			Destroy(info.weaponObject);
			info.weaponObject = null;
		}
	}

	//Will drop the weapon he carries.
	public void dropWeapon()
	{
		anim.SetTrigger("grab");
		unprepareWeapon();
		GameObject tempItem = (GameObject)Instantiate(collect.ocol[info.weaponItem],info.rightHand.transform.position,info.rightHand.transform.rotation);
		tempItem.name = info.weaponItem;
		info.weaponItem = "";
	}

	//Will store the weapon in storage
	public void storeWeapon()
	{
		GameObject gStorage = info.selfStorage;
		stru_storage store = gStorage.GetComponent<stru_storage>();
		store.storage.Add(info.weaponItem);

		info.weaponItem="";

		if(info.weaponObject!=null)
			Destroy(info.weaponObject);
	}

	//Will reload the weapon
	public IEnumerator reloadWeapon()
	{
		anim.SetTrigger("reload");
		yield return new WaitForSeconds(2);
		info.bulletsLeft = cref.item[info.weaponItem].magSize;
	}

	//Will attempt to get in a habitable object.
	public void getIn(GameObject gHabitable)
	{
		gHabitable.GetComponent<misc_habitable>().insertMember(this.gameObject);
	}
	

	//Will attack the given unit.
	public IEnumerator attackUnit(GameObject gUnit)
	{
		idle = false;
		focusObject = gUnit;

		if(gUnit!=null)
		{
			misc_pdt epdt = gUnit.GetComponent<misc_pdt>();

			anim.SetTrigger("fire");

			//Hit calculation.
			int rangeFromTarget = Mathf.FloorToInt(Vector3.Distance(transform.position,gUnit.transform.position));
			int hit = 0;
			bool cover = false;

			if(cref.item[info.weaponItem].iType==3)
			{
				anim.SetInteger("meleeType",Random.Range(0,3));
				hit = Random.Range(0,cref.item[info.weaponItem].weaponHitJam);
			}
			else
			{
				//Range hit calc:


				//Cover hit:
				if(epdt.cover!=null)
				{
					if(info.legitCover(epdt.cover,this.gameObject))
					{
						hit = Random.Range(0,cref.item[info.weaponItem].weaponHitJam+Mathf.FloorToInt(rangeFromTarget/10));

						if(hit>info.rifleVet)
						{
							cover = true;
						}
					}
				}



				//Bulletwork
				info.bulletsLeft--;
				GameObject tempBullet = (GameObject) Instantiate(collect.ocol[cref.item[info.weaponItem].projectile]);
				GameObject gunTip = info.weaponObject.transform.GetChild(0).transform.GetChild(0).gameObject;
				tempBullet.transform.position = gunTip.transform.position;

				if(cover)
				{
					print ("cover");
					tempBullet.transform.rotation = Quaternion.LookRotation(epdt.cover.transform.position - tempBullet.transform.position);
					tempBullet.transform.rotation *= Quaternion.Euler(1, Random.Range(-hit/2,hit/2), 0);
				}
				else
				{
					hit = Random.Range(0,cref.item[info.weaponItem].weaponHitJam+Mathf.FloorToInt(rangeFromTarget/10));
					tempBullet.transform.rotation = Quaternion.LookRotation(gUnit.transform.position - tempBullet.transform.position);
					tempBullet.transform.rotation *= Quaternion.Euler(Random.Range(-hit,0), Random.Range(-hit,hit), 0);
				}
				
			}


			yield return new WaitForSeconds(0.2f);


			if(gUnit!=null)
			{
				if(hit<=info.rifleVet && !cover)
				{
					epdt.wounded = true;
					epdt.surpressed = true;
					epdt.damage  = Random.Range(cref.item[info.weaponItem].weaponMinDmg,cref.item[info.weaponItem].weaponMaxDmg);
					epdt.causeObject = this.gameObject;
				}
			}
		}
		idle = true;
	}

	//Will check by ray if the target could be seen.
	public bool seeTarget(GameObject gTarget)
	{
		if(gTarget!=null)
		{
			Vector3 fixedPos = transform.position+transform.TransformDirection(0,2.5f,0);
			Vector3 fixedTPos = gTarget.transform.position+transform.TransformDirection(0,2,0);

			Debug.DrawRay(fixedPos,fixedTPos-fixedPos,Color.red,1);
			RaycastHit rchit;
			if(Physics.Raycast(fixedPos,fixedTPos-fixedPos, out rchit ,1000))
			{
				if(rchit.collider.gameObject == gTarget)
					return true;
			}
		}

		return false;
	}

	#endregion



	#endregion

	#region Testing
	
	#endregion
}
