using UnityEngine;
using System.Collections;

/// <summary>
/// Will contain the actor / units basic information, as well as it's negotiation with 
/// "PDT" (Passive damage taker) / Region Scanning / Unit scanning / Death and wounds / Cover checks.
/// </summary>
public class actor_info : MonoBehaviour {

	#region Parameters
	private core_collect ccol;

	public string aName;
	public int    hp;
	public bool   prone;

	public int 	  rifleVet;
	public int	  meleeVet;
	public int    electVet;
	public int    buildVet;
	public int    mechaVet;

	public GameObject rightCarry;
	public GameObject leftCarry;
	public GameObject backCarry;

	public GameObject rightHand;
	public GameObject leftHand;
	public GameObject backGrip;

	public GameObject backBone;
	

	//Combat
	public actor_control actrl;
	public misc_pdt   	 mpdt;

	public string     weaponItem;
	public GameObject weaponObject;
	public GameObject currentCover;

	public bool weaponRaise;
	
	//Faction
	public int    faction;
	public GameObject selfStorage;

	//Combat
	
	public int bulletsLeft;
	public float surpression;
	public bool crouch;
	public bool lookingForCover;

	//--//

	public GameObject projObj;

	void Start()
	{
		initInfo();
	}
	#endregion

	#region perFrame

	void Update()
	{
		//Surpression
		if(surpression>0)
		{
			surpression-=1*Time.deltaTime;
		}

		//Wound / Death
		if(mpdt.wounded)
		{
			Wound(mpdt.damage);
			actrl.StopAllCoroutines();
			StartCoroutine(actrl.goAttack(mpdt.causeObject));
			Instantiate(ccol.ocol["effect_bloodimpact01"],transform.position+transform.TransformDirection(0,2,0),Quaternion.Inverse(mpdt.causeObject.transform.rotation));
			mpdt.wounded=false;
		}

		if(mpdt.surpressed)
		{
			surpression++;
			mpdt.surpressed = false;
		}

		if(hp<=0)
		{
			Death();
		}

		mpdt.cover = currentCover;
	}

	#endregion

	#region Commands

	public void initInfo()
	{
		core_info cinfo = GameObject.Find("CORE").GetComponent<core_info>();
		ccol = cinfo.GetComponent<core_collect>();
		mpdt  = GetComponent<misc_pdt>();
		actrl = GetComponent<actor_control>();
		aName 	 = "Temp Name";
		hp    	 = 100;

		//Find self storage
		for(int i=0;i<cinfo.storageList.Length;i++)
		{
			if(cinfo.storageList[i].GetComponent<stru_storage>().faction == faction)
			{
				selfStorage = cinfo.storageList[i];
				break;
			}
		}
	}

	//Will find a random region in the scene.
	public GameObject findRandomRegion()
	{
		GameObject[] regions = GameObject.FindGameObjectsWithTag("region");
		int rnd = Random.Range(0,regions.Length);

		return regions[rnd];
	}

	//Will return a random position within a given region.
	public Vector3 randomRegionPosition(GameObject gRegion)
	{
		Bounds bnd = gRegion.GetComponent<BoxCollider>().bounds;

		int cx = Random.Range(Mathf.FloorToInt(bnd.min.x),Mathf.FloorToInt(bnd.max.x));
		int cz = Random.Range(Mathf.FloorToInt(bnd.min.z),Mathf.FloorToInt(bnd.max.z));

		return (new Vector3(cx ,bnd.max.y ,cz));
	}

	//Will get the nearest region to given position.
	public GameObject getNearestRegion(Vector3 gPos)
	{
		GameObject[] regions = GameObject.FindGameObjectsWithTag("region");

		//Within check
		for(int i=0;i<regions.Length;i++)
		{
			if(regions[i].GetComponent<BoxCollider>().bounds.Contains(gPos))
				return regions[i];
		}

		//Closest of all.
		int maxDist = 999;
		GameObject regionTD = null;

		for(int i=0;i<regions.Length;i++)
		{
			int currDist = Mathf.FloorToInt(Vector3.Distance(gPos,regions[i].transform.position));

			if(currDist<maxDist)
			{
				maxDist = currDist;
				regionTD = regions[i];
			}
		}

		return regionTD;
	}

	//Will return a bool if the object could be seen by the actor.
	public bool canSee(GameObject gObj)
	{
		if(gObj!=null)
		{
			RaycastHit rchit;
			if(Physics.Raycast(transform.position,(gObj.transform.position-transform.position),out rchit,100))
			{
				if(rchit.collider.gameObject == gObj)
				{
					return true;
				}
			}
		}

		return false;
	}

	//Will return a bool for the carriage state.
	public bool isFull()
	{
		if(leftCarry != null && rightCarry != null && backCarry != null)
			return true;
		else
			return false;
	}

	//Will execute a 'Wound' on the actor.
	public void Wound(int dmg)
	{
		surpression+=5;
		actrl.anim.SetTrigger("hurt");
		actrl.anim.SetInteger("sType",Random.Range(0,3));
		hp-=dmg;
	}

	//Will execute death on the actor.
	public void Death()
	{
		if(currentCover!=null)
			currentCover.GetComponent<cover_info>().taken=false;

		actrl.unequipWeapon();
		actrl.dropWeapon();
		actrl.anim.gameObject.GetComponent<p_ragcontrol>().enabled=true;
		actrl.anim.gameObject.GetComponent<p_ragcontrol>().DEBUGRAGIT=true;

		transform.FindChild("mdl").transform.parent = null;

		Destroy(this.gameObject);
	}

	//Will check if the given cover protects from given object.
	public bool legitCover(GameObject givenCover, GameObject gObject)
	{
		if(gObject!=null)
		{
			float eulerY = givenCover.transform.eulerAngles.y;
			
			Quaternion lr = Quaternion.LookRotation(gObject.transform.position-transform.position);
			
			if(lr.eulerAngles.y > eulerY-70 && lr.eulerAngles.y < eulerY+70)
			{
				return true;
			}
		}
		return false;
	}





	#endregion


}
