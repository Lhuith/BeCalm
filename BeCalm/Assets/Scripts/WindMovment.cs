using UnityEngine;
using System.Collections;

public class WindMovment : MonoBehaviour {
	public Transform sphereObject; 
	public Vector3 target;
	public GameObject player;
	public Rigidbody rb;
	public float power, speed, frequency, distance = 180;

	public enum WindStates{
		Breathe,
		Wind
	}
	public  WindStates currentWindType;

	// Use this for initialization
	void Start () {
		player = CustomExtensions.GetPlayer ();
		rb = GetComponent<Rigidbody> ();
	
	}
	
	// Update is called once per frame

	void FixedUpdate()
	{  
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
