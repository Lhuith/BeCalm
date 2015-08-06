using UnityEngine;
using System.Collections;

public class TeleScopeCam : MonoBehaviour {
	public Camera cam;
	public RaycastHit hit;

		Vector2 _joyAbsolute;
		Vector2 _smoothJoy;
		
		public Vector2 clampInDegrees = new Vector2(360, 180);
		public bool lockCursor;
		public Vector2 sensitivity = new Vector2(2, 2);
		public Vector2 smoothing = new Vector2(3, 3);
		public Vector2 targetDirection;
		public Vector2 targetCharacterDirection;
		
		// Assign this if there's a parent object controlling motion, such as a Character Controller.
		// Yaw rotation will affect this object instead of the camera if set.
		public GameObject characterBody;
		
		void Start()
		{
			cam = GetComponent<Camera> ();
			// Set target direction to the camera's initial orientation.
			targetDirection = transform.localRotation.eulerAngles;
			
			// Set target direction for the character body to its inital state.
			if (characterBody) targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
		}
		
		void Update()
		{
		cam.backgroundColor = Camera.main.backgroundColor;
		float dist = 30;
		Vector3 dir = new Vector3(0,-1,0);
		
		//edit: to draw ray also//
		Debug.DrawRay(transform.position,dir*dist,Color.green);
		
		
		if (Physics.Raycast (transform.position, dir, out hit, dist)) 
		{
		}
			// Ensure the cursor is always locked when set
			Screen.lockCursor = lockCursor;
			
			// Allow the script to clamp based on a desired target value.
			var targetOrientation = Quaternion.Euler(targetDirection);
			var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);
			
			// Get raw mouse input for a cleaner reading on more sensitive mice.
		var mouseDelta = new Vector2(Input.GetAxisRaw("RightH"), Input.GetAxisRaw("RightV"));
			
			// Scale input against the sensitivity setting and multiply that against the smoothing value.
			mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
			
			// Interpolate mouse movement over time to apply smoothing delta.
		_smoothJoy.x = Mathf.Lerp(_smoothJoy.x, mouseDelta.x, 1f / smoothing.x);
		_smoothJoy.y = Mathf.Lerp(_smoothJoy.y, mouseDelta.y, 1f / smoothing.y);
			
			// Find the absolute mouse movement value from point zero.
		_joyAbsolute += _smoothJoy;
			
			// Clamp and apply the local x value first, so as not to be affected by world transforms.
			if (clampInDegrees.x < 360)
			_joyAbsolute.x = Mathf.Clamp(_joyAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
			
		var xRotation = Quaternion.AngleAxis(-_joyAbsolute.y, targetOrientation * Vector3.right);
			transform.localRotation = xRotation;
			
			// Then clamp and apply the global y value.
			if (clampInDegrees.y < 360)
			_joyAbsolute.y = Mathf.Clamp(_joyAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
			
			transform.localRotation *= targetOrientation;
			
			// If there's a character body that acts as a parent to the camera
			if (characterBody)
			{
			var yRotation = Quaternion.AngleAxis(_joyAbsolute.x, characterBody.transform.up);
				characterBody.transform.localRotation = yRotation;
				characterBody.transform.localRotation *= targetCharacterOrientation;
			}
			else
			{
			var yRotation = Quaternion.AngleAxis(_joyAbsolute.x, transform.InverseTransformDirection(Vector3.up));
				transform.localRotation *= yRotation;
			}
		}
	}
