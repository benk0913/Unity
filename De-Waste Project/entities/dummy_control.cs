using UnityEngine;
using System.Collections;

/// <summary>
/// The basic AI of a training dummy.
/// </summary>

public class dummy_control : MonoBehaviour {

	public misc_pdt mpdt;
	public Animator anim;
	public int      health;
	public core_collect ccol;

	void Start()
	{
		mpdt = GetComponent<misc_pdt>();
		health = 100;
		ccol = GameObject.Find("CORE").GetComponent<core_collect>();
	}

	void Update()
	{
		if(mpdt.wounded)
		{
			Wound(mpdt.damage);
			mpdt.wounded=false;
		}

		if(health<=0)
		{
			StartCoroutine(death());
			health = 9999;
		}
	}

	private void Wound(int damage)
	{
		anim.SetInteger("hitType",Random.Range(0,3));
		anim.SetTrigger("hit");
		health-=damage;

		Instantiate(ccol.ocol["effect_impact01"],transform.position,Quaternion.Inverse(mpdt.causeObject.transform.rotation));
	}

	private IEnumerator death()
	{
		anim.SetInteger("hitType",Random.Range(0,3));
		anim.SetTrigger("hit");
		Instantiate(ccol.ocol["effect_impact01"],transform.position,Quaternion.Inverse(mpdt.causeObject.transform.rotation));
		yield return new WaitForSeconds(0.3f);
		Destroy(this.gameObject);
	}
}
