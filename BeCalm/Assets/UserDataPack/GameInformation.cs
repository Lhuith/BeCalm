using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInformation : MonoBehaviour {

	void Awake(){
				DontDestroyOnLoad (transform.gameObject);
		}
	public static string PlayerName{ get; set;}
	public static float playTime{ get; set;}
	public static float topBreathePeak{ get; set;}
	public static List<float> breathPeaks { get; set;}
	public static int starCount{ get; set;}
	public static float breathPerMin{ get; set;}

}
