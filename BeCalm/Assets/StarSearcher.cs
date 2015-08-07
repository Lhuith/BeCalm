using UnityEngine;
using System.Collections;

public class StarSearcher : MonoBehaviour {
	bool addedForce;
	public RaycastHit hit;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float dist = 15000;
		Vector3 dir = transform.forward;//new Vector3(0,0,transform.rotation.z);

		Debug.DrawRay(transform.position,dir*dist,Color.green);

		if(Physics.Raycast(transform.position, dir, out hit , dist)){
			if(hit.transform.tag == "Star")
			{

				if(!hit.transform.GetComponent<Star>().removed)
				{
				Debug.Log("Hit Star");
				hit.rigidbody.isKinematic = false;
				hit.transform.GetComponent<TrailRenderer>().enabled = true;
				hit.transform.GetComponent<Star>().RemoveSelf();
				}
			}
		}
	}
}
