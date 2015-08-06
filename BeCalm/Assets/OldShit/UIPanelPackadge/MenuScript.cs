using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

	public GameObject top, bot;
	Image topS, botS;
	bool isHCM, isPressing;
	public enum Buttons{
		CONTINUE,
		RETRY
	};
	public Buttons currentButton;
	public AudioClip selectNoise1, selectNoise2;
	AudioSource aS;
	
	void Start () {

		aS = Camera.main.GetComponent<AudioSource> ();
		//aS.clip = selectNoise;
		aS.volume = .2f;
		isHCM = false;
		currentButton = Buttons.CONTINUE;
		topS = top.GetComponent<Image> ();
		botS = bot.GetComponent<Image> ();
		
		topS.enabled = true;
		botS.enabled = false;
	}
	
	void SpriteChecker(){
		if(currentButton == Buttons.CONTINUE){
			topS.enabled = true;
			botS.enabled = false;
		}
		if(currentButton == Buttons.RETRY){
			topS.enabled = false;
			botS.enabled = true;
		}
	}
	
	void Update(){
		int i = Application.loadedLevel;

		if(Input.GetButtonDown("Fire0")){
			if(currentButton == Buttons.CONTINUE){
				Application.LoadLevel(i + 1);
			}
			if(currentButton == Buttons.RETRY){
				Application.LoadLevel(i - 1);
			}
		}
		
		SpriteChecker ();
		if(DpadInput.down || Input.GetKeyDown(KeyCode.W)){
			//if(currentButton == Buttons.CONTINUE){
				aS.PlayOneShot(selectNoise1, 1f);
				currentButton = Buttons.RETRY;
			//}
			//else if(currentButton == Buttons.RETRY){
//				aS.PlayOneShot(selectNoise, .2f);
//				currentButton = Buttons.CONTINUE;
			//}

		}	
		if(DpadInput.up || Input.GetKeyDown(KeyCode.S)){
			//if(currentButton == Buttons.CONTINUE){
				aS.PlayOneShot(selectNoise2, 1f);
				currentButton = Buttons.CONTINUE;
			//}
			//else if(currentButton == Buttons.RETRY){
//				aS.PlayOneShot(selectNoise, .2f);
//				currentButton = Buttons.CONTINUE;
			//}
		
			
		}

	}

}
