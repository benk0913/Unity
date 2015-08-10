using UnityEngine;
using System.Collections;

/// <summary>
/// Will kill itself upon a given amount of time.
/// </summary>

public class self_desctruct : MonoBehaviour {

	public float time;

	public bool with2DFade = false;

	void Update()
	{
		time-=1*Time.deltaTime;
		if(time<=0)
		{
			Destroy(this.gameObject);
		}

		if(with2DFade)
		{
			Color origC = GetComponent<SpriteRenderer>().color;
			GetComponent<SpriteRenderer>().color = new Color(origC.r,origC.g,origC.b,time);
		}
	}
}
