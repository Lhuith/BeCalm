using UnityEngine;
using System.Collections;

/*

Any objects with this script attached will face the selected camera.

*/

public class SpriteRotator : MonoBehaviour
{
	public new Transform camera;

	// Use this for initialization
	void Start ()
	{
		// Get the first enabled camera tagged "MainCamera"
		if ( !camera ) {
			camera = Camera.main.transform;
		}

		// Makes the player sprite always face the camera
		Update();
	}

	// Update is called once per frame
	void Update ()
	{
		// Makes the player sprite always face the camera
		transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
	}
}
