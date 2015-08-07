using UnityEngine;
using System.Collections;

public class TsetScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		LoadInformation.LoadAllInformation ();
		Debug.Log ("Player Name : " + GameInformation.PlayerName);
	
	}

}
