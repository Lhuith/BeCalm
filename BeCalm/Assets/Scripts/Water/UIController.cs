using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	public MusicLoad musicLoader;
	public FeedBackController FeedBackeReader;
	public SpectrumAnalyzer specAny;
	public Text songName, playButton, textBox;
	public bool isPause, isPlay, isManual;
	public static float calmnessUIValue;
	public Slider calmSlider;
	// Use this for initialization
	void Start () {
		//musicLoader = Camera.main.GetComponent<MusicLoad> ();
		specAny = GetComponent<SpectrumAnalyzer> ();
		FeedBackeReader = GetComponent<FeedBackController> ();
//		songName = GameObject.FindGameObjectWithTag ("NAME").GetComponent<Text> ();
//		playButton = GameObject.FindGameObjectWithTag ("PLAY").GetComponent<Text> ();
//		textBox = GameObject.FindGameObjectWithTag ("TEXT").GetComponent<Text> ();
		calmSlider =  GameObject.FindGameObjectWithTag ("UI_Canvas").GetComponentInChildren<Slider> ();
	}
	
	// Update is called once per frame
	void Update () {
		isManual = FeedBackeReader.notManual;

		if (isManual) {
			calmSlider.value = calmnessUIValue;
		} else 
		{
			calmnessUIValue = 	calmSlider.value;
		}

		if (Input.GetButtonDown ("Fire9")) 
		{
			PrevSong();
		}

		if (Input.GetButtonDown ("Fire6")) 
		{
			NextSong();
		}

		if (Input.GetButtonDown ("Fire7")) 
		{
			//Application.LoadLevel(Application.loadedLevel);
		}

		if (isPlay) {
			//playButton.text = "Pause";
		} else {
			//playButton.text = "Play";
		}
			//songName.text = Camera.main.GetComponent<AudioSource> ().clip.name;
	}


	public void TextBoxInput(string text)
	{
		//textBox.text = text;
	}

	public void NextSong(){
		//specAny.NextSong ();
	}

	public void PrevSong(){
		//specAny.PrevSong ();
	}

	public void PlayPuaseSong(){
		if (!isPlay) {
			//specAny.PlaySong ();
		} else {
			//specAny.PauseSong ();
		}
	}
}
