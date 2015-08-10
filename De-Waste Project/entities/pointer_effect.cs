using UnityEngine;
using System.Collections;

/// <summary>
/// Basic information of a repositionable pointer effect.
/// (Appears after mouse clicks / etc...)
/// </summary>
public class pointer_effect : MonoBehaviour {
	
	public GameObject     textObj;
	public string         givenText;

	public IEnumerator triggerSelf()
	{
		if(GetComponent<ParticleSystem>()!=null)
		{
			GetComponent<ParticleSystem>().Stop();
			GetComponent<ParticleSystem>().Play();
		}

		textObj.GetComponent<Animator>().SetTrigger("start");
		textObj.GetComponent<TextMesh>().text = givenText;

		yield return new WaitForSeconds(1);
		this.gameObject.SetActive(false);
	}

}
