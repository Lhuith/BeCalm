using UnityEngine;
using System.Collections;

public class BoundsCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider col)
	{
		if (col.tag == "Player") 
		{
			Debug.Log("Bop");
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") 
		{
			col.attachedRigidbody.velocity = -col.attachedRigidbody.velocity;
			//col.attachedRigidbody.AddForce(-col.attachedRigidbody.velocity * col.attachedRigidbody.velocity.magnitude * 10000, ForceMode.Impulse );
		}
	}

}
