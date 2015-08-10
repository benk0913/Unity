using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the idle / self conscious behavior of the units.
/// The units would take cover if needed, retreat if overly 
/// surpressed and attack found enemies while idle.
/// </summary>
public class actor_behave : MonoBehaviour {

	private actor_info    ainfo;
	private actor_control actrl;
	private core_info     cinfo;
	private misc_recordObjects enemyView;

	private float intervalTimer = 0;

	void Start()
	{
		ainfo = GetComponent<actor_info>();
		actrl = GetComponent<actor_control>();
		cinfo = GameObject.Find("CORE").GetComponent<core_info>();
		enemyView = this.gameObject.transform.FindChild("eneView").GetComponent<misc_recordObjects>();
	}

	void Update()
	{
		//Will check every few seconds for enemies in sight. (Only if not in combat)
		if(actrl.idle && !actrl.attacking && ainfo.weaponItem!="")
		{
			if(intervalTimer>0)
			{
				intervalTimer-=1*Time.deltaTime;
			}
			else
			{
				intervalTimer = 1;

				GameObject tempEnemy = checkForEnemies();
				if(tempEnemy!=null)
				{
					actrl.StopAllCoroutines();
					StartCoroutine(actrl.goAttack(tempEnemy));
				}
			}
		}

		//Surpression
		if(ainfo.surpression>30)//RETREAT!!!
		{
			StopAllCoroutines();
			actrl.running = true;
			StartCoroutine(actrl.walkToObject(ainfo.selfStorage));
		}
		else if(ainfo.surpression>5 && ainfo.currentCover == null && !ainfo.lookingForCover)//IN THE OPEN!
		{
			StartCoroutine(actrl.lookForCover());
		}


	}

	//Will search for enemies within sight.
	public GameObject checkForEnemies()
	{
		if(enemyView.within.Count>0)
		{
			int closestEnemy = -1;
			int bestRange = 9999;
			for(int i=0;i<enemyView.within.Count;i++)
			{
				if(enemyView.within[i]!=null)
				{
					int targetFaction = enemyView.within[i].GetComponent<misc_pdt>().faction;
					if(cinfo.factions[ainfo.faction].fRelations[targetFaction])
					{
						int rangeBetween = Mathf.FloorToInt(Vector3.Distance(enemyView.within[i].transform.position , transform.position));
						if(rangeBetween<bestRange)
						{
							bestRange = rangeBetween;
							closestEnemy = i;
						}
					}
				}
			}

			if(closestEnemy!=-1)
				return enemyView.within[closestEnemy];
		}

		return null;
	}






}
