using UnityEngine;
using System.Collections;

public class XYFolow : MonoBehaviour {
	public GameObject player;
	// Use this for initialization
	void Start () {
		player = CustomExtensions.GetPlayer ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
	}
}
