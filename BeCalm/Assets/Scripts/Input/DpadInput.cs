using UnityEngine;
using System.Collections;

public class DpadInput : MonoBehaviour {

	public static bool up;
	public static bool down;
	public static bool left;
	public static bool right;
	
	float lastX;
	float lastY;
	
	void Start() {
		up = false; down = false; left = false; right = false;
		lastX = Input.GetAxis("DPadX");
		lastY = Input.GetAxis("DPadY");
	}
	
	void Update() {
		if(Input.GetAxis ("DPadX") == 1 && lastX != 1) 
		{ 
			right = true; 
		} 
		else 
		{ 
			right = false; 
		}

		if(Input.GetAxis ("DPadX") == -1 && lastX != -1) 
		{ 
			left = true; 
		} 
		else 
		{ 
			left = false; 
		}
		if(Input.GetAxis ("DPadY") == 1 && lastY != 1) 
		{ 
			up = true; 
		} 
		else 
		{ 
			up = false; 
		}
		if(Input.GetAxis ("DPadY") == -1 && lastY != -1) 
		{ 
			down = true; 
		} else 
		{ 
			down = false; 
		}
	}
}
