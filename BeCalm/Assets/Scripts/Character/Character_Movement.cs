using UnityEngine;
using System.Collections;

public class Character_Movement : MonoBehaviour {

		public float pushPower = 2.0F;
		public Animator anim;
		public float speed = 6.0f;
		public float turnSpeed = 2f;
		public float jumpSpeed = 8.0f;
		public float gravity = 20.0f;
		public float x, y, z;
		public Vector3 moveDirection = Vector3.zero;
		public bool isFalling, Pressing;
		public float HorizontelMov, VerticalMov;
		public CharacterController controller;
		
	void Awake(){
			controller = GetComponent<CharacterController> ();
			anim = GetComponentInChildren<Animator> ();
		}
		
		void FixedUpdate()
		{
			isFalling = controller.isGrounded;
			if (controller.isGrounded) 
			{
				moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;
				y = transform.rotation.y;
			if(Input.GetButton("JumpAButton"))
				{
					moveDirection.y = jumpSpeed;
				}
			}
			
			controller.Move (moveDirection * Time.deltaTime);
			//Apply Gravity
			moveDirection.y -= gravity * Time.deltaTime;
			
		if(Input.GetButtonDown("JumpAButton")) {
				//y = Camera.main.transform.position.y;
			} else {
				
				//transform.LookAt(Vector3.up);
				
			}
			
		}
		
		
		void Update () 
		{
			
			if (Input.GetAxisRaw ("Vertical") < 0.0f) // if input is lower than 0 turn to targetf
			{
				//transform.rotation = Quaternion.Lerp(transform.rotation, targetf, Time.deltaTime * turnSpeed);    
			}
			if (Input.GetAxisRaw ("Vertical") > 0.0f) // if input is higher than 0 turn to targetb
			{
				//transform.rotation = Quaternion.Lerp(transform.rotation, targetb, Time.deltaTime * turnSpeed);
			}
		}
		
		void OnControllerColliderHit(ControllerColliderHit hit) {
			Rigidbody body = hit.collider.attachedRigidbody;
			if (body == null || body.isKinematic)
				return;
			
			if (hit.moveDirection.y < -0.3F)
				return;
			
			Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
			body.velocity = pushDir * pushPower;
		}
		
		
	}