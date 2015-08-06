using UnityEngine;
using System.Collections;

public class GuidingStarScript : MonoBehaviour {
	public GameObject player;
	public Renderer renderer;
	public float intensity = 0.1f;
	public FeedBackController focusData;
	public Color starColor;
	// Use this for initialization
	void Start () {
		player = CustomExtensions.GetPlayer ();
		renderer = GetComponentInChildren<Renderer> ();
		focusData = GameObject.FindGameObjectWithTag ("GameController").GetComponent<FeedBackController> ();
	}
	
	// Update is called once per frame
	void Update () {

		StarPath ();
	}

	void StarPath()
	{
		intensity = focusData.focus * 10;

		starColor = new Color (intensity, intensity, intensity, intensity);
		
		renderer.material.SetColor("_EmissionColor", starColor);

			//float watertileDis = CustomExtensions.GetDistance(WaterGameObjectList[i], player);
			float vecDistance = Vector3.Distance(transform.position, player.transform.position); 
			if (FeedBackController.currentFocusState == FeedBackController.FocalStates.Focusing) 
		{
			Debug.DrawLine (transform.position, player.transform.position, Color.blue);
			//WaterGameObjectList [i].TurnOnGameObject();
		}
	}
}
