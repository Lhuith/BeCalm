
using UnityEngine;
using System.Collections;



public class CamaraFollow : MonoBehaviour
{
	public Transform target, target1, target2;

	public Vector3 reculcPos;

	public Quaternion recalcRot;

	public float targetHeight = 1.7f;
	public float distance = 5.0f;
	public float offsetFromWall = 0.1f;

	public float lookSpeed;

	public float maxDistance = 20;
	public float minDistance = .6f;
	
	public float xSpeed = 200.0f;
	public float ySpeed = 200.0f;
	public float targetSpeed = 5.0f;
	
	
	public float yMinLimit = -80;
	public float yMaxLimit = 80;
	public Vector3 wavePos;
	public float yWaveMinLimit;

	public int zoomRate = 40;
	
	public float rotationDampening = 3.0f;
	public float zoomDampening = 5.0f;
	public float dampening = .2f;

	public LayerMask collisionLayers = -1;
	
	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;
	private float correctedDistance;

	public bool switched, notSwitched, Pressing;

	private WaveController waveheight;

	Quaternion originalRotation;

	void Awake ()
	{
		if (target1 == null) {
			target1 = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		}

		waveheight = GameObject.FindGameObjectWithTag ("GameController").GetComponent<WaveController> ();
		Vector3 angles = transform.eulerAngles;
		xDeg = angles.x;
		yDeg = angles.y;
		currentDistance = 	distance;
		desiredDistance = distance;
		correctedDistance = distance;
		
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
	
	
	void Update()
	{
		if (FeedBackController.currentFocusState == FeedBackController.FocalStates.Focusing) {


			//target = target2;
			//distance = 2;
			//targetHeight = .2f;
			//maxDistance = 1f;
		} else 
		{
			target = target1;
		}

	}
	
	/**
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */
	void LateUpdate ()
	{
		Vector3 vTargetOffset;

		xDeg += Input.GetAxis ("RightH") * xSpeed * 0.2f * Time.deltaTime;
		yDeg -= Input.GetAxis ("RightV") * ySpeed * 0.2f * Time.deltaTime;
		//target.transform.rotation = Quaternion.Euler (0, xDeg, 0);
		//Angle Clamps
	
		

			Quaternion rotation = Quaternion.Euler (yDeg, xDeg - 180, 0);

			recalcRot = rotation;
			yDeg = ClampAngle (yDeg, yWaveMinLimit , yMaxLimit + 5);


			Vector3 globalPosition = transform.position;
			
			
			//Wave Height
		wavePos = waveheight.GetWaveYPos (globalPosition.x, globalPosition.z);
			//yWaveMinLimit = waveheight.GetWaveYPos (globalPosition.x, globalPosition.z);
			yWaveMinLimit = wavePos.y;
			

			//set camera rotation
			

			// calculate the desired distance
			desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs (desiredDistance);
			desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance);
			correctedDistance = desiredDistance;



			// calculate desired camera position
			vTargetOffset = new Vector3 (0, -targetHeight, 0);
			Vector3 position = target.position - (recalcRot * Vector3.up * desiredDistance + vTargetOffset);



			// check for collision using the true target's desired registration point as set by user using height
			RaycastHit collisionHit;
			Vector3 trueTargetPosition = new Vector3 (target.position.x, target.position.y + targetHeight, target.position.z);
		
			// if there was a collision, correct the camera position and calculate the corrected distance
			bool isCorrected = false;
			if (Physics.Linecast (trueTargetPosition, position, out collisionHit, collisionLayers.value)) {
				// calculate the distance from the original estimated position to the collision location,
				// subtracting out a safety "offset" distance from the object we hit.  The offset will help
				// keep the camera from being right on top of the surface we hit, which usually shows up as
				// the surface geometry getting partially clipped by the camera's front clipping plane.
				correctedDistance = Vector3.Distance (trueTargetPosition, collisionHit.point) - offsetFromWall;
				isCorrected = true;
			}
		
			// For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
			currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp (currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;
		
			// keep within legal limits
			currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);
		
			// recalculate position based on the new currentDistance
			transform.rotation = recalcRot;
		
		

			reculcPos = target.position - (recalcRot * Vector3.forward * currentDistance + vTargetOffset);

			transform.position = reculcPos;



	}
	

	private static float ClampAngle (float angle, float min, float max)
	{
		if (angle < 90)
			angle = angle;
		if (angle > 90)
			angle -= 1;
		return Mathf.Clamp (angle, min, max);
	}

}

