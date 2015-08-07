using UnityEngine;
using System.Collections;

public class WindMovment : MonoBehaviour {
	public Transform sphereObject; 
	public Vector3 target;
	public GameObject player;
	public Rigidbody rb;
	public float power, speed, frequency, distance = 180;
	public WaveController waveScript;

	public enum WindStates{
		Breathe,
		Wind
	}
	public  WindStates currentWindType;

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, 9f);
		player = CustomExtensions.GetPlayer ();
		rb = GetComponent<Rigidbody> ();
		
		GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");
		waveScript = gameController.GetComponent<WaveController> ();
	}
	
	// Update is called once per frame

	void FixedUpdate()
	{  
		Vector3 Pos = waveScript.GetWaveYPos (transform.position.x, transform.position.z);

		transform.position = new Vector3 (transform.position.x, transform.position.y + Pos.y, transform.position.z);

		if (currentWindType == WindStates.Wind) {
			if (power < 800 && currentWindType == WindStates.Wind) {
				transform.RotateAround (player.transform.position, Vector3.up, 100 * Time.deltaTime);
			}
		}

			rb.AddForce (target * (power), ForceMode.Acceleration); 

			//float watertileDis = CustomExtensions.GetDistance(WaterGameObjectList[i], player);
			float vecDistance = Vector3.Distance(transform.position, player.transform.position); 
			
			if (vecDistance < distance) {
				//Debug.DrawLine (transform.position, player.transform.position, Color.green);
				//Do Magic Shit
				//WaterGameObjectList [i].TurnOnGameObject();
			} else 
			{
			Destroy(this.gameObject,.4f);
				//WaterGameObjectList [i].TurnOffGameObject();
			}

	}


}
