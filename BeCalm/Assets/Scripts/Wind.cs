using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {
	public GameObject windSpline, player;
	// Use this for initialization
	void Start () {
		GameObject windSplineLoad =  Resources.Load("Mesh/WindV2", typeof(GameObject)) as GameObject;
		windSpline = windSplineLoad;
		player = CustomExtensions.GetPlayer ();
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	public void CreateWind(float windPower)
	{
		int dicex = Random.Range (- 600, 600);
		int dicez = Random.Range ( -600, 600);
		int dicey = Random.Range (0 , 600);
		GameObject windcreate = Instantiate(windSpline, new Vector3((player.transform.position.x) + dicex, player.transform.position.y + dicey,( player.transform.position.z) + dicez), transform.rotation) as GameObject;
		windcreate.GetComponent<WindMovment> ().currentWindType = WindMovment.WindStates.Wind;
		windcreate.GetComponent<WindMovment>().power = windPower * 300;
		windcreate.GetComponent<WindMovment>().distance = 600;
		windcreate.GetComponent<WindMovment> ().target = -player.transform.right;
		windcreate.transform.parent = transform;
	}
}
