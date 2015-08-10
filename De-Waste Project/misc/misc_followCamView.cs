using UnityEngine;
using System.Collections;

/// <summary>
/// A useful small component which sets rotation to face the camera.
/// </summary>

public class misc_followCamView : MonoBehaviour {

	private GameObject cameraObj;

	void Start()
	{
		cameraObj = GameObject.Find("Main Camera");
	}

	void Update()
	{
		transform.rotation = cameraObj.transform.rotation;
	}
}
