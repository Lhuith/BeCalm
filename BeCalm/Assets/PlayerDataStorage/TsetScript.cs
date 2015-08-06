using UnityEngine;
using System.Collections;

public class TsetScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		LoadInformation.LoadAllInformation ();
		Debug.Log ("Player Name : " + GameInformation.PlayerName);
		Debug.Log ("Player Sex : " + GameInformation.IsMale);
//		Debug.Log ("Player Level : " + GameInformation.PlayerLevel);
//		Debug.Log ("Player Stamina : " + GameInformation.Stamina);
//		Debug.Log ("Player Endurance : " + GameInformation.Endurance);
//		Debug.Log ("Player Intellect : " + GameInformation.Intellect);
//		Debug.Log ("Player Strength : " + GameInformation.Strength);
//		Debug.Log ("Player Agility: " + GameInformation.Agility);
//		Debug.Log ("Player Resistance: " + GameInformation.Resistance);
//		Debug.Log ("Player Gold: " + GameInformation.Gold);
	
	}

}
