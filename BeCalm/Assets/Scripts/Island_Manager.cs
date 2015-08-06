using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Island_Manager : MonoBehaviour {

	public List<IslandScript> islandList = new List<IslandScript>();
	public int listSize;
	// Use this for initialization
	void Start () 
	{
		islandList = gameObject.GetComponentsInChildren<IslandScript>().ToList ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
