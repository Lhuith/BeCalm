using UnityEngine;
using System.Collections;

public class InteractController : MonoBehaviour {
	public UIController uiController;
	public Transform seatPos, camSeatPos;
	public bool inICUse;
	public WaterPhysics boatUse;
	public GameObject player;
	// Use this for initialization
	void Start () {
		uiController = GameObject.FindGameObjectWithTag ("UI").GetComponent<UIController> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		seatPos = GameObject.FindGameObjectWithTag ("Seat").GetComponent<Transform> ();
		camSeatPos = GameObject.FindGameObjectWithTag ("SeatCamPos").GetComponent<Transform> ();
		boatUse = transform.root.GetComponent<WaterPhysics> ();

		player.transform.position = seatPos.position;
		player.transform.parent = seatPos.parent;
		player.gameObject.GetComponent<Character_Movement> ().enabled = false;
		player.gameObject.GetComponent<CharacterController> ().enabled = false;
		Camera.main.GetComponent<CamaraFollow>().distance = 4.1f;
		Camera.main.transform.position = camSeatPos.position;
		Camera.main.transform.rotation = camSeatPos.rotation;
		inICUse = true;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
	
	}

	void OnTriggerStay(Collider col)
	{
		if (col.transform.tag == "Player" && this.transform.root.tag == "Vehicle") {
			if (!inICUse) {
				uiController.TextBoxInput ("press B to Enter");
				if (Input.GetButtonDown ("Fire5")) {
					col.transform.position = seatPos.position;
					col.transform.parent = seatPos.parent;
					col.gameObject.GetComponent<Character_Movement> ().enabled = false;
					col.gameObject.GetComponent<CharacterController> ().enabled = false;
					Camera.main.GetComponent<CamaraFollow>().distance = 4.1f;
					Camera.main.transform.position = camSeatPos.position;
					Camera.main.transform.rotation = camSeatPos.rotation;
					inICUse = true;
					uiController.TextBoxInput ("");
			} else {
			}
		}
}
}
	void OnTriggerExit(Collider col)
	{
		uiController.TextBoxInput ("");
	}

}
