using UnityEngine;
using System.Collections;

public class Rooji_Manager : MonoBehaviour {

	public Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(FeedBackController.currentFocusState == FeedBackController.FocalStates.Focusing)
		{
			anim.SetBool("Focusing", true);
			anim.SetBool("Music", false);
		}else if(FeedBackController.currentFocusState == FeedBackController.FocalStates.PlayingMusic)
		{
			anim.SetBool("Music", true);
		}
		else
		{
			anim.SetBool("Focusing", false);
			anim.SetBool("Music", false);
		}
	}
}
