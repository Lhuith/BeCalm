using UnityEngine;
using System.Collections;

public class LoadInformation {

	public static void LoadAllInformation(){
		GameInformation.PlayerName = PlayerPrefs.GetString ("PLAYERNAME");

//		if (PlayerPrefs.GetString("EQUIPMENTITEM1") != null){
////			GameInformation .EquipmentOne = (BaseEquipment)PPSerialization.Load("EQUIPMENTITEM1");
//		}
		}
}
