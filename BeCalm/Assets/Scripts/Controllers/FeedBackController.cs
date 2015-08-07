using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FeedBackController : MonoBehaviour {

	SpectrumAnalyzer audioInput;
	WaveController waveVisController;
	public GameObject telescope;
	public float focusMin, focusMax, focusIncriment, focusDecayRate, focus;
	public AudioSource audioPlayer;
	public AudioClip G, C, E, A;
	public static float noise;
	public static bool musicPeak, playingMusic;
	public static float calm;
	public bool notManual;
	public SpectrumAnalyzer specAny;
	public Toggle toggle;
	public float calmMax, calmMin, calmDecayRate, calmIncramentRate;

	public enum FocalStates{
		Focusing,
		NotFocusing,
		PlayingMusic
	}
	public static FocalStates currentFocusState;

	// Use this for initialization
	void Start () {
		calm = 0;
		specAny = GetComponent<SpectrumAnalyzer> ();
		audioPlayer = Camera.main.GetComponent<AudioSource> ();
		audioInput = GetComponent<SpectrumAnalyzer> ();
		currentFocusState = FocalStates.NotFocusing;
		toggle = GameObject.FindGameObjectWithTag("ManToggle").GetComponent<Toggle>();
	}
	
	// Update is called once per frame
	void Update () {

		notManual = toggle.enabled;
		if (notManual) {
			UIController.calmnessUIValue = calm; 
		} else {
			calm = UIController.calmnessUIValue;
		}
		//Debug.Log (currentFocusState);
		//Debug.Log ( Input.GetAxis("Fire10"));
		//D//ebug.Log (calm);

		noise = audioInput.musicSum * 100;

		if (!specAny.musicMode) {
			if (Input.GetAxis ("Fire10") > .5) {
				audioPlayer.pitch = 1.4f;
			} else {
				audioPlayer.pitch = 1f;
			}
		}

		if (noise > 1.0f) {
			musicPeak = true;
		} else {
			musicPeak = false;
		}


		if (currentFocusState != FocalStates.Focusing) {
		
			if (Input.GetButtonDown ("Fire0")) 
			{
				audioPlayer.PlayOneShot (G, 1);
				playingMusic = true;
				currentFocusState = FocalStates.PlayingMusic;
			}

			else if (Input.GetButtonDown ("Fire1")) 
			{
				audioPlayer.PlayOneShot (C, 1);
				playingMusic = true;
				currentFocusState = FocalStates.PlayingMusic;
			}
			else if (Input.GetButtonDown ("Fire2")) 
			{
				audioPlayer.PlayOneShot (A, 1);
				playingMusic = true;
				currentFocusState = FocalStates.PlayingMusic;
			}
			else if (Input.GetButtonDown ("Fire3")) 
			{
				audioPlayer.PlayOneShot (E, 1);
				playingMusic = true;
				currentFocusState = FocalStates.PlayingMusic;
			}else 
			{

			}
		}
		if (Input.GetButton ("Fire4")) {

			currentFocusState = FocalStates.Focusing;
			telescope.TurnOnGameObject();
			//Camera.main.gameObject.TurnOffGameObject();
			playingMusic = false;
		} 
		else 
		{
			if(!playingMusic){
			telescope.TurnOffGameObject();
			//Camera.main.gameObject.TurnOnGameObject();
			currentFocusState = FocalStates.NotFocusing;
			}
		}


		//calm, calmMax, calmMin, calmDecayRate, calmIncramentRate;
	
			if (FeedBackController.musicPeak && calm < calmMax) {
				//windMaker.CreateWind(windSpeed);
				calm += Mathf.Lerp (calmMin, calmMax, calmIncramentRate * Time.deltaTime);
			} else {
				if (calm > calmMin) {
					calm -= Mathf.Lerp (calmMin, calmMax, calmDecayRate * Time.deltaTime);
				}
			}

		//Current Focusing States
		if (currentFocusState == FocalStates.Focusing && focus < focusMax) {
			//windMaker.CreateWind(windSpeed);
			focus += Mathf.Lerp (focusMin, focusMax, focusIncriment * Time.deltaTime);
		}else
		{
			if (focus > focusMin) {
				focus -= Mathf.Lerp (focusMin, focusMax, focusDecayRate * Time.deltaTime);
			}
		}
		
		if (Input.GetButtonDown ("Fire5")) 
		{

		}


	}
	
	public void Manual()
	{
		//notManual = !notManual;
	}
}
