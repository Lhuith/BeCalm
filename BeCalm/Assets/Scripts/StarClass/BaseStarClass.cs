using UnityEngine;
using System.Collections;

public class BaseStarClass  {

	
	//Base Island Color
	private Color starColor;
	
	//Base Island Coordinates
	private Vector3 starCoords;
	
	private float calmLevel;

	private GameObject starObject;

	public Color StarColor{
		get{ return starColor;}
		set{ starColor = value;}
	}

	public GameObject StarObject{
		get{ return starObject;}
		set{ starObject = value;}
	}
	
	public Vector3 StarCoords{
		get{ return starCoords;}
		set{ starCoords = value;}
	}
	
	public float CalmLevel{
		get{ return calmLevel;}
		set{ calmLevel = value;}
	}
}
