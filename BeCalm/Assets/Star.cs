using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {
	public SkyStars skyManager;
	public GameObject player;

	public Renderer renderer;

	public float intensity = 0.1f , constantVelocity;

	public FeedBackController focusData;

	public Color starColor;
	public bool removed, attached;

	public AudioClip hit, attach;

	public WaveController waveScript;
	// Use this for initialization
	void Start () {
		removed = false;
		player = CustomExtensions.GetPlayer ();
		renderer = GetComponentInChildren<Renderer> ();
		focusData = GameObject.FindGameObjectWithTag ("GameController").GetComponent<FeedBackController> ();

		skyManager = GetComponentInParent<SkyStars> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < 5 && removed == true) 
		{
			//transform.GetComponent<Rigidbody> ().velocity = -transform.GetComponent<Rigidbody> ().velocity;
			transform.GetComponent<Rigidbody> ().isKinematic = true;
			transform.position = new Vector3(transform.position.x, player.transform.position.y , transform.position.z);
		}

		if (transform.position.y < 0 && removed == true) 
		{
			//Destroy(this.gameObject);
		}

		intensity = focusData.focus * 10;
		
		starColor = new Color (intensity, intensity, intensity, intensity);
		
		renderer.material.SetColor("_EmissionColor", starColor);

		if (attached) 
		{
			//Vector3 Pos = Vector3.MoveTowards(transform.position,new Vector3 (player.transform.position.x, player.transform.position.y, player.transform.position.z), 10f);
			//transform.position = Pos;
			transform.localScale = new Vector3(.2f,.2f,.2f);
			transform.RotateAround (new Vector3 (player.transform.position.x + 5, player.transform.position.y + 5, player.transform.position.z + 5), Vector3.up, 50 * Time.deltaTime);
			//transform.GetComponent<Rigidbody>().AddForce (Vector3.forward * 1000, ForceMode.Acceleration); 
		}
	}

	public void Attach(Transform hit)
	{
		//transform.parent = hit.transform;
		GetComponent<SphereCollider> ().enabled = false;
		attached = true;

	}

	public void RemoveSelf()
	{
		transform.GetComponent<Rigidbody>().AddForce(new Vector3 (transform.forward.x, -transform.up.y) * 2000, ForceMode.Impulse);
		if (transform.parent != null) {
			Debug.Log ("bleh");
			skyManager.RemoveStar (this.gameObject);
			transform.parent = null;
			removed = true;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.transform.tag == "Player") 
		{
			GameInformation.starCount ++;
			Debug.Log("belh2");
			Attach(col.transform);
		}
	}
}
