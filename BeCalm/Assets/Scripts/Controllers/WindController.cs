using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour {
	public MicrophoneInput micInput;
	public float windSpeed, windSpeedMax, windSpeedMin, windIntensity, windDropRate;
	public Wind windMaker;
	public UserBreathe breatheCreate;


	// Use this for initialization
	void Start () {
//		windSpeedMin = 0;
//		windSpeedMax = 10;
//		windIntensity = .2f;
		windMaker = GameObject.FindGameObjectWithTag("Wind").GetComponent<Wind>();
		//breatheCreate = Camera.main.GetComponent<UserBreathe>();
		//micInput = Camera.main.GetComponent<MicrophoneInput> ();

	}
	
	// Update is called once per frame
	void Update () {

		//windSpeed = Mathf.Lerp (Noise, windSpeedMin, windIntensity);;
		if (windSpeed < 0) 
		{ 
			windSpeed += 1;
		}
		if (FeedBackController.musicPeak && windSpeed < windSpeedMax) {
			windMaker.CreateWind(windSpeed);
			//breatheCreate.CreateBreathe(windSpeed);

			windSpeed += Mathf.Lerp (windSpeedMin, windSpeedMax, windIntensity * Time.deltaTime);

			}else
			{
			if (windSpeed > windSpeedMin) {
				windSpeed -= Mathf.Lerp (windSpeedMin, windSpeedMax, windDropRate * Time.deltaTime);
			}
		}
		}
}
