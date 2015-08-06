using UnityEngine;
using System.Collections;

public class telescopeCam : MonoBehaviour {

	private Camera cam;
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		cam.backgroundColor = Camera.main.backgroundColor;
	}
}
