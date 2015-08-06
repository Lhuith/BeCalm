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

	// Use this for initialization
	void Start () {
		fogDensity = RenderSettings.fogDensity;
		skydome = GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ();
		skydomeLight = skydome.m_sunIntensity;
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

		if (GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity > 10f) {
			GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity = skydomeLight - (FeedBackController.calm * 100);
		} else 
		{
			//GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity = skydomeLight - (FeedBackController.calm);
		}
		Camera.main.GetComponent<ColorCorrectionCurves> ().saturation = colorSat - (FeedBackController.calm * 2);
		if (Camera.main.GetComponent<Tonemapping> ().exposureAdjustment > 0.5) {
			Camera.main.GetComponent<Tonemapping> ().exposureAdjustment = toneExp - (FeedBackController.calm * 4);
		}
		Camera.main.GetComponent<SunShafts> ().sunShaftIntensity = sunShaftIntens - (FeedBackController.calm);

	}
}
