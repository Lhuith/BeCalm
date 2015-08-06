using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WaterSpace : MonoBehaviour {

	public GameObject waterTile, player;
	public float spaceHeight;
	public float spaceWidth, watermeshWidth, waterSpaceWidth;
	public float tilesSize;
	public float distance; //Distance from the player
	public WaterV3 waterSize;
	public List<GameObject> WaterSpaceList = new List<GameObject>();
	public bool isOn;
	// Use this for initialization
	void Awake () 
	{
		WaterSpaceList.Clear();
		tilesSize = spaceHeight * spaceWidth;
		watermeshWidth = waterTile.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * waterTile.transform.lossyScale.x;
		waterSpaceWidth = watermeshWidth * spaceWidth;
		player = CustomExtensions.GetPlayer();
		CreateWaterSpace ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{

	
	}

	void CreateWaterSpace()
	{
			for (int x = 0; x < spaceHeight; x ++)
			{
				for (int z = 0; z < spaceWidth; z ++) 
				{
					Debug.Log("im a fag 2");
					GameObject waterspace = Instantiate (waterTile, new Vector3 (this.transform.position.x + (x * watermeshWidth), this.transform.position.y, this.transform.position.z + (z * watermeshWidth)),transform.rotation) as GameObject;
					waterspace.transform.position = new Vector3 (this.transform.position.x + (x * watermeshWidth), 0, this.transform.position.z + (z * watermeshWidth));
					waterspace.transform.parent = transform;
					WaterSpaceList.Add (waterspace);
				}
			}
	}
	

	void OnApplicationQuit() 
	{
		for (int i = 0; i < WaterSpaceList.Count; i++) 
		{
			GameObject.Destroy(WaterSpaceList[i]);
		}

		WaterSpaceList.Clear();
}
}
