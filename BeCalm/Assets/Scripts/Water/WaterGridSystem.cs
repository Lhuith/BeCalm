using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterGridSystem : MonoBehaviour {

	public GameObject waterSpaceTile, player, watertileLoad;
	public float worldHeight;
	public float worldWidth, meshWidth, spaceGridheight, spaceGridWidth;

	public float distance, visCount; //Distance from the player
	public WaterV3 waterSize;

	public List<Vector3> WaterVectorList = new List<Vector3> ();
	public List<Vector3> VisibleVectors = new List<Vector3>();

	public List<GameObject> WaterGameObjectList = new List<GameObject>();

	public bool isOn, Generated, counted;
	public WaterSpace waterMeshGridSize;
	public float viewDistance = 60f;
	public Vector3 WaveSpacePos;

	public float watertileDis;

	public GameObject underWaterMeshObj;

	public Texture2D displaceMentTexure;
	public Texture2D Gridsourceimage;

	private int width;
	private int height;

	public Material[] waterMat;

	public WaveController waveScript;

	//private Mesh OriginalWater;
	// Use this for initialization
	void Awake () 
	{
		visCount = 0;

	}

	void Start(){

		Gridsourceimage = new Texture2D (256 , 256);
		//Gridsourceimage.Apply();

		//Gridsourceimage = //Resources.Load("Textures/Water", typeof(Texture2D)) as Texture2D;
		waveScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<WaveController> ();

		watertileLoad =  Resources.Load("Mesh/WaterV6", typeof(GameObject)) as GameObject;

		meshWidth = (watertileLoad.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * watertileLoad.transform.lossyScale.x);

	
		
		player = CustomExtensions.GetPlayer();
		
		//waterSize = GetComponentInChildren<WaterV3> ();
		//TileCheck ();

		CreateGrid();

		Debug.Log(WaterVectorList.Count);
	}
	
	// Update is called once per frame
	void Update () 
	{
		TileCheck ();
		WaveGridController ();
	}

	void TileCheck()
	{
		for (int i = 0; i < WaterVectorList.Count; i++) {
			//float watertileDis = CustomExtensions.GetDistance(WaterGameObjectList[i], player);
			float tileDis = Vector3.Distance(WaterGameObjectList[i].transform.position, player.transform.position); 

			if (tileDis < viewDistance) {

				//Debug.DrawLine (WaterGameObjectList[i].transform.position, player.transform.position, Color.green);

				WaterGameObjectList [i].TurnOnGameObject();
			}else {
				//visCount --;
				WaterGameObjectList [i].TurnOffGameObject();

			}
		}
	}

	void WaveGridController ()
	{
		for (int i = 0; i < WaterGameObjectList.Count; i ++) 
		{
			if(WaterGameObjectList[i].activeInHierarchy == true)
			{
				waveScript.SetWaveMat (WaterGameObjectList[i].GetComponent<MeshRenderer>().material, WaterGameObjectList[i]);
			}
		}
	}

	void OnApplicationQuit() 
	{
		for (int i = 0; i < WaterVectorList.Count; i++) 
		{
			//GameObject.Destroy(WaterTileList[i]);
		}

		WaterVectorList.Clear();
}
	
	void CreateGrid()
	{

		for (int x = 0; x < worldHeight; x ++) 
		{
			for (int z = 0; z < worldWidth; z ++)
			{
				WaveSpacePos = new Vector3(this.transform.position.x + (x * meshWidth), this.transform.position.y, this.transform.position.z + (z * meshWidth));
				WaterVectorList.Add(WaveSpacePos);
				GenerateTile(WaveSpacePos);
			}
		}
	}

	
	public void GenerateTile(Vector3 WaterVector)
	{
		if(WaterGameObjectList.Count < WaterVectorList.Count) {
			GameObject watertile = Instantiate (watertileLoad, new Vector3 (WaterVector.x, WaterVector.y, WaterVector.z), transform.rotation) as GameObject;
			watertile.transform.parent = transform;
			//watertile.GetComponent<WaveDisplacement> ().sourceimage = Gridsourceimage;
			//watertile.GetComponent<WaveDisplacement> ().targettexture = Gridsourceimage;
			//watertile.GetComponent<WaveDisplacement> ().sourceimage.Apply();
			//watertile.GetComponent<WaveDisplacement> ().width = Gridsourceimage.width;
			//watertile.GetComponent<WaveDisplacement> ().height = Gridsourceimage.height;
			//watertile.GetComponent<MeshRenderer> ().material.SetTexture ("_ExtrudeTex", Gridsourceimage);
			//watertile.GetComponent<MeshRenderer> ().material.name = "Dick" + WaterVector.x + 1;
			//waterMeshGridSize.viewSpaceDistance = viewDistance;
			WaterGameObjectList.Add(watertile);
		}
	}
}
