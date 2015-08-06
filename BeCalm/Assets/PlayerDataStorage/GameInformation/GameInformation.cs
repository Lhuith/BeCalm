using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInformation : MonoBehaviour {

	void Awake(){
				DontDestroyOnLoad (transform.gameObject);
		}
	public static bool IsMale{ get; set; }
	public static string PlayerName{ get; set;}
	public static int PlayerLevel{ get; set;}
	public static Sprite PlayerSprite{ get; set;}
	public static int Stamina { get; set;}
	public static int Endurance{ get; set;}
	public static int Intellect{ get; set;}
	public static int Strength{ get; set;}
	public static int Agility{ get; set;}
	public static int Resistance{ get; set;}
	public static int Gold{ get; set;}
	public static int CurrentXP{ get; set;}
	public static int RequiredXP{ get; set;}

}
