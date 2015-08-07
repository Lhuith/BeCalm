using UnityEngine;
using System.Collections;
public class LeaveMenu : MonoBehaviour {

	public GameObject pauseMenu;
	public bool isPuased;
	// Use this for initialization
	void Start () {
		isPuased = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire7")) 
		{
			isPuased = !isPuased;
			if(isPuased){
			Time.timeScale = 0;
			pauseMenu.TurnOnGameObject ();
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") {
			pauseMenu.TurnOnGameObject ();
		} else 
		{
			pauseMenu.TurnOffGameObject();
		}
	}

	public void CloseMenu()
	{
		Time.timeScale = 1;
		pauseMenu.TurnOffGameObject();
	}

	public void LeaveGame()
	{
		int i = Application.loadedLevel;
		Application.LoadLevel(i + 1);
	}
}
