using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Will supply basic reference of items / different types of objects.
/// Basicly meant to be the only class which is moddable in the game.
/// Now contains item information / values / icon resources / etc...
/// </summary>

public class core_ref {

    public Dictionary<string, itemInfo> item = new Dictionary<string, itemInfo>();

	public Dictionary<string, string> icons = new Dictionary<string, string>();
	public List<string> clothing = new List<string>();
	public List<string> faces    = new List<string>();

	public core_ref()
	{
		#region Items Setup

		item.Add("item_ms_a",   new itemInfo());
		item["item_ms_a"].iName = "Cheap Scrap";
		item["item_ms_a"].iValue= 5;
		item["item_ms_a"].iIcon = "item_scrap";
		item["item_ms_a"].iType = 0;

		item.Add("item_ms_b",   new itemInfo());
		item["item_ms_b"].iName = "Fine Scrap";
		item["item_ms_b"].iValue= 8;
		item["item_ms_b"].iIcon = "item_scrap";
		item["item_ms_b"].iType = 0;

		item.Add("item_ms_c",   new itemInfo());
		item["item_ms_c"].iName = "Good Scrap";
		item["item_ms_c"].iValue= 10;
		item["item_ms_c"].iIcon = "item_scrap";
		item["item_ms_c"].iType = 0;

		item.Add("item_wheel",  new itemInfo());
		item["item_wheel"].iName = "Wheel";
		item["item_wheel"].iValue= 25;
		item["item_wheel"].iIcon = "item_wheel";
		item["item_wheel"].iType = 1;

		item.Add("weapon_spear",new itemInfo());
		item["weapon_spear"].iName     = "Spear";
		item["weapon_spear"].iValue	   = 25;
		item["weapon_spear"].iIcon     = "item_spear";
		item["weapon_spear"].iBigIcon  = "UI_spear";
		item["weapon_spear"].iType     = 3;
		item["weapon_spear"].weaponMinDmg = 10;
		item["weapon_spear"].weaponMaxDmg = 30;
		item["weapon_spear"].weaponHitJam = 2;
		item["weapon_spear"].weaponCD = 10;
		item["weapon_spear"].weaponRange  = 2;
		item["weapon_spear"].iParts    = new Dictionary<int, int>();
		item["weapon_spear"].iParts[0] = 4;

		item.Add("weapon_imp_rifle",new itemInfo());
		item["weapon_imp_rifle"].iName     = "Improvised Rifle";
		item["weapon_imp_rifle"].iValue	   = 120;
		item["weapon_imp_rifle"].iIcon     = "item_imp_rifle";
		item["weapon_imp_rifle"].iBigIcon  = "UI_imp_rifle";
		item["weapon_imp_rifle"].iType     = 5;
		item["weapon_imp_rifle"].projectile= "proj_bullet";
		item["weapon_imp_rifle"].weaponMinDmg = 10;
		item["weapon_imp_rifle"].weaponMaxDmg = 30;
		item["weapon_imp_rifle"].weaponHitJam = 2;
		item["weapon_imp_rifle"].weaponCD = 1;
		item["weapon_imp_rifle"].weaponRange  = 120;
		item["weapon_imp_rifle"].magSize = 30;
		item["weapon_imp_rifle"].iParts    = new Dictionary<int, int>();
		item["weapon_imp_rifle"].iParts[0] = 4;
		item["weapon_imp_rifle"].iParts[1] = 6;

		//cd=30
		//size=1

		#endregion

		#region Look Textures

		clothing.Add("human_male_set1");
		clothing.Add("human_male_set2");
		clothing.Add("human_male_set3");
		clothing.Add("human_male_set4");
		clothing.Add("human_male_set5");
		clothing.Add("human_male_set6");
		clothing.Add("human_male_set7");
		clothing.Add("human_male_set8");
		clothing.Add("human_male_set9");

		faces.Add("tex_face_01");
		faces.Add("tex_face_02");
		faces.Add("tex_face_03");
		faces.Add("tex_face_04");
		faces.Add("tex_face_05");
		faces.Add("tex_face_06");
		faces.Add("tex_face_07");
		faces.Add("tex_face_08");
		faces.Add("tex_face_09");

		#endregion

		#region Icons

		icons.Add("actor","icon_actor");

		#endregion

	}

}

/// <summary>
/// Item info.
/// 
/// Types: 
/// 0 - scrap
/// 1 - Mechanical Part
/// 2 - Electronic
/// 3 - Melee
/// 4 - Pistol
/// 5 - Rifle
/// </summary>
/// 
/// 

public class itemInfo {
	public string iName;
	public string iIcon;
	public string iBigIcon;
	public string projectile;
	public int iValue;
	public int iType;
	public int weaponMinDmg;
	public int weaponMaxDmg;
	public int weaponHitJam;
	public int weaponCD;
	public int weaponRange;
	public int magSize;
	//Type - Amount
	public Dictionary<int,int> iParts;
}
