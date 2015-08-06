using UnityEngine;
using System.Collections;

public class UserBreathe : MonoBehaviour {

		public GameObject breatheSpline, player;
		
		// Use this for initialization
		void Start () {

			GameObject breatheSplineLoad =  Resources.Load("Mesh/Breathe", typeof(GameObject)) as GameObject;
			breatheSpline = breatheSplineLoad;
			//player = CustomExtensions.GetPlayer ();
		}
		
		// Update is called once per frame
		void Update () {
			
		}
		
		public void CreateBreathe(float windPower)
		{
			//int dicex = Random.Range ((int)transform.transform.position.x, (int)transform.transform.position.x);
			//int dicey = Random.Range (0 , 10);

			GameObject windcreate = Instantiate(breatheSpline, new Vector3(transform.position.x, transform.position.y, transform.position.z),  Quaternion.Inverse(transform.rotation)) as GameObject;
			windcreate.GetComponent<WindMovment> ().currentWindType = WindMovment.WindStates.Breathe;
			windcreate.GetComponent<WindMovment>().power = 400;
			windcreate.GetComponent<WindMovment>().distance = 600;
			windcreate.GetComponent<WindMovment>().target = transform.right;
			//windcreate.transform.parent = transform;
		}
	}

