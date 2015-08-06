using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {
	public GameObject player;

	void Start(){
		//player = GameObject.FindGameObjectWithTag("Player");
		//transform.LookAt(Camera.main.transform.position, Vector3.up); 
		//if(!Terrain.activeTerrain)
				//renderer.receiveShadows = false;
				//renderer.castShadows = false;
		}

	 void Update() { 
		transform.LookAt(Camera.main.transform.position, Vector3.up); 
	} 
}