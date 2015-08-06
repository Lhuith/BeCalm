using UnityEngine;
using System.Collections;

public class LoadInformation {

	public static void LoadAllInformation(){
		GameInformation.PlayerName = PlayerPrefs.GetString ("PLAYERNAME");
		GameInformation.PlayerLevel= PlayerPrefs.GetInt ("PLAYERLEVEL");
		GameInformation.Stamina = PlayerPrefs.GetInt ("STAMINA");
		GameInformation.Endurance = PlayerPrefs.GetInt ("ENDURANCE");
		GameInformation.Intellect = PlayerPrefs.GetInt ("INTELLECT");
		GameInformation.Strength = PlayerPrefs.GetInt ("STRENGTH");
		GameInformation.Agility = PlayerPrefs.GetInt ("AGILITY");
		GameInformation.Resistance = PlayerPrefs.GetInt ("RESISTANCE");
		GameInformation.Gold = PlayerPrefs.GetInt ("GOLD");

		if (PlayerPrefs.GetString("EQUIPMENTITEM1") != null){
//			GameInformation .EquipmentOne = (BaseEquipment)PPSerialization.Load("EQUIPMENTITEM1");
		}
		}
}
