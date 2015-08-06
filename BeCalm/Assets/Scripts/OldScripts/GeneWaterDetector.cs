using UnityEngine;
using System.Collections;

public class GeneWaterDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider Hit)
	{
		if (Hit.GetComponent<Rigidbody>() != null)
		{
			//transform.parent.GetComponent<GeneWater>().Splash(transform.position.x, Hit.GetComponent<Rigidbody2D>().velocity.y*Hit.GetComponent<Rigidbody2D>().mass / 20f);
			//transform.parent.GetComponent<GeneWater>().Splash(transform.position.x, Hit.GetComponent<Rigidbody>().velocity.y*Hit.GetComponent<Rigidbody>().mass /400f);
		}
	}

	void OnTriggerStay(Collider Hit)
	{
		if (Hit.GetComponent<Rigidbody>() != null)
		{
			//transform.parent.GetComponent<GeneWater>().Splash(transform.position.x, Hit.GetComponent<Rigidbody2D>().velocity.y*Hit.GetComponent<Rigidbody2D>().mass / 20f);
			transform.parent.GetComponent<GeneWater>().Splash(transform.position.x, Hit.GetComponent<Rigidbody>().velocity.x*Hit.GetComponent<Rigidbody>().mass /400f);
		}
	}
}
