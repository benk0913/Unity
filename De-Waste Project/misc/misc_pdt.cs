using UnityEngine;
using System.Collections;

/// <summary>
/// Misc_pdt. Passive Damage Taker.
/// Would take damage and mark that it has been wounded.
/// Other components could make use of this class to indicate wounds.
/// 
/// It's reset comes upon use of the components.
/// This "passive class" helps out with different
/// types of units and different types of unit classes.
/// The attacks would always contact this method and the 
/// 'Hits recieved' will always carry out from this method aswell.
/// </summary>

public class misc_pdt : MonoBehaviour {

	public int faction;
	
	public bool wounded;
	public int  damage;
	public int  damageType;
	public GameObject causeObject;
	public bool surpressed;
	public GameObject cover;
	

}
