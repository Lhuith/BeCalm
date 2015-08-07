using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class WeatherController : MonoBehaviour {

	public Sky skydome;
	public float sunLight;
	public float fogDensity;
	public float colorSat, toneExp, sunShaftIntens, skydomeLight;
	public ColorCorrectionCurves colorCurve;
	public Tonemapping toneMapper;
	public SunShafts sunShafts;
	public GameObject teleCam;
	// Use this for initialization
	void Start () {
		fogDensity = RenderSettings.fogDensity;
		//skydome = GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ();
		//skydomeLight = skydome.m_sunIntensity;
		colorCurve = Camera.main.GetComponent<ColorCorrectionCurves> ();
		toneMapper = Camera.main.GetComponent<Tonemapping> ();
		sunShafts = Camera.main.GetComponent<SunShafts> ();

		sunShaftIntens = sunShafts.sunShaftIntensity;
		colorSat = colorCurve.saturation;
		toneExp = toneMapper.exposureAdjustment; 
	}
	
	// Update is called once per frame
	void Update () {
		RenderSettings.fogDensity = fogDensity;

//			if (GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity > 5f) {
//				GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity = skydomeLight - (FeedBackController.calm * 50);
//			} else {
//				//GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity = skydomeLight - (FeedBackController.calm);
//			}
		if (Camera.main.GetComponent<Tonemapping> ().exposureAdjustment > 0.4) {
			Camera.main.GetComponent<ColorCorrectionCurves> ().saturation = colorSat - (FeedBackController.calm * 1.5f);
			teleCam.GetComponent<ColorCorrectionCurves> ().saturation = colorSat - (FeedBackController.calm * 1.5f);
		}
		if (Camera.main.GetComponent<Tonemapping> ().exposureAdjustment > 0.2) {
			Camera.main.GetComponent<Tonemapping> ().exposureAdjustment = toneExp - (FeedBackController.calm * .6f);
			teleCam.GetComponent<Tonemapping> ().exposureAdjustment = toneExp - (FeedBackController.calm * .6f);
		}else if( Camera.main.GetComponent<Tonemapping> ().exposureAdjustment < 0.4f)
		{
			//Camera.main.GetComponent<Tonemapping> ().exposureAdjustment += 0.1f;
		}
	
		Camera.main.GetComponent<SunShafts> ().sunShaftIntensity = sunShaftIntens - (FeedBackController.calm);
		teleCam.GetComponent<SunShafts> ().sunShaftIntensity = sunShaftIntens - (FeedBackController.calm);

	}
}
