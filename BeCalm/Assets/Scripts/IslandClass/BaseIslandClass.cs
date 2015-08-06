using UnityEngine;
using System.Collections;

public class BaseIslandClass {

	private string islandName;
	private string islandDescription;
	
	//Base Island Color
	private Color islandBaseColor;

	//Base Island Coordinates
	private Vector3 islandCoords;

	private float calmLevel;

	public string IslandName{
		get{ return islandName;}
		set{ islandName = value;}
	}
	public string IslandDescription{
		get{ return islandDescription;}
		set{ islandDescription = value;}
	}
	public Color IslandBaseColor{
		get{ return islandBaseColor;}
		set{ islandBaseColor = value;}
	}

	public Vector3 IslandCoords{
		get{ return islandCoords;}
		set{ islandCoords = value;}
	}

	public float CalmLevel{
		get{ return calmLevel;}
		set{ calmLevel = value;}
	}
}
