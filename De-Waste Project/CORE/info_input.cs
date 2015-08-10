using UnityEngine;
using System.Collections;

/// <summary>
/// Will contain the tweakable input / hotkeys.
/// </summary>

public class info_input : MonoBehaviour {

	public static int     mouseSens;
	public static float   dcResponse;
	public static KeyCode iForward;
	public static KeyCode iBackward;
	public static KeyCode iLeft;
	public static KeyCode iRight;
	public static KeyCode iCamPan;

	void Start()
	{
		setDefault();
	}

	//Will set the default keys.
	public static void setDefault()
	{
		dcResponse = 0.5f;
		mouseSens = 50;
		iForward  = KeyCode.W;
		iBackward = KeyCode.S;
		iLeft 	  = KeyCode.A;
		iRight 	  = KeyCode.D;
		iCamPan   = KeyCode.LeftAlt;
	}

}
