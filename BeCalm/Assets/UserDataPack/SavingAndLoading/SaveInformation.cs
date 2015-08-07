using UnityEngine;
using System.Collections;

public class SaveInformation {

	public static void SaveAllInformation(){

				PlayerPrefs.SetString ("PLAYERNAME", GameInformation.PlayerName);

//				if (GameInformation.EquipmentOne != null){
//						PPSerialization.Save ("EQUIPMENTITEM1", GameInformation.EquipmentOne);
//		}
		Debug.Log ("SAVED ALL INFORMATION");
		}

}
