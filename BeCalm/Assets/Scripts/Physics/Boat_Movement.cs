using UnityEngine;
using System.Collections;

public class Boat_Movement : MonoBehaviour {
	public WaterPhysics waterPhys;
	public WindController windPower;
	public float boatThrust;
	public Vector3 x;
	// Use this for initialization
	void Start () {
		waterPhys = GetComponent<WaterPhysics> ();
		windPower = GameObject.FindGameObjectWithTag ("GameController").GetComponent<WindController>();
	}
	
	// Update is called once per frame
	void Update () {
		x = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		//x = transform.TransformDirection(x);
		transform.Rotate(0, x.x, 0);
		boatThrust = windPower.windSpeed;
		waterPhys.AddPropulsion(boatThrust);
	}
}
