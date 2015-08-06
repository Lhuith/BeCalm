using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour {
	public Rigidbody rb;
	public float thrust;
	public GameObject Water;
	public Vector3 pos;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
	}

	void OnTriggerStay(Collider Hit)
	{
		if (Hit.GetComponent<Rigidbody>() != null)
		{

		}
	}

}
