using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	public AudioSource[] audioPlayers;
	

	// Use this for initialization
	void Start () {
		audioPlayers = Camera.main.GetComponents<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		audioPlayers [0].volume = Mathf.Lerp (1, 0, FeedBackController.calm);
		audioPlayers [1].volume = Mathf.Lerp (0, 1, FeedBackController.calm);
		//audioPlayers [0].volume = FeedBackController.calm;
		//audioPlayers [1].volume = FeedBackController.calm;
	
	}
}
