using UnityEngine;
using System.Collections;

public class WaterV2 : MonoBehaviour {
	
	LineRenderer Body;
	
	float[] xpositions;
	float[] ypositions;
	float[] zpositions;
	float[] velocities;
	float[] accelerations;
	
	GameObject[] meshObjects;
	GameObject[] colliders;
	Mesh[] meshes;
	
	public GameObject waterMesh;
	public GameObject splash;
	
	public Material mat;
	
	public float springconstant = 0.02f;
	public float damping = 0.04f;
	public float spread = 0.05f;
	public float z = 1f;

	public int size = 1;
	float baseheight;
	//float left;
	float bottom;
	
	// Use this for initialization
	void Start () {
		SpawnWater (-10, 20, 0, -3);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void Splash(float xpos, float velocity)
	{
		
		if (xpos >= xpositions[0] && xpos <= xpositions[xpositions.Length-1])
		{
			//Offset the x position to be the distance from the left side
			xpos -= xpositions[0];
			
			//Find which spring we're touching
			int index = Mathf.RoundToInt((xpositions.Length-1)*(xpos / (xpositions[xpositions.Length-1] - xpositions[0])));
			
			//Add the velocity of the falling object to the spring
			velocities[index] += velocity;
		}
	}
	
	public void SpawnWater(float Left, float Width, float Top, float Bottom)
	{
		int edgecount = Mathf.RoundToInt (Width) * 5;
		int nodecount = edgecount + 1;
		
		Body = gameObject.AddComponent<LineRenderer> ();
		Body.material = mat;
		Body.material.renderQueue = 1000;
		Body.SetVertexCount (nodecount);
		Body.SetWidth (0.1f, 0.1f);
		
		xpositions = new float[nodecount];
		ypositions = new float[nodecount];
		velocities = new float[nodecount];
		accelerations = new float[nodecount];
		
		meshObjects = new GameObject[edgecount];
		meshes = new Mesh[edgecount];
		colliders = new GameObject[edgecount];
		
		baseheight = Top;
		bottom = Bottom;
		//left = Left;
		
		for (int i = 0; i < nodecount; i++) {
			ypositions [i] = Top;
			xpositions [i] = Left + Width * i / edgecount;
			accelerations [i] = 0;
			velocities [i] = 0;
			Body.SetPosition (i, new Vector3 (xpositions [i], Top, z));
		}
		
		for (int i = 0; i < edgecount; i++)
		{
			meshes [i] = new Mesh ();
			Vector3[] Vertices = new Vector3[24];
			Vector3 p0 = new Vector3(xpositions[i], ypositions[i], 1);
			Vector3 p1 =  new Vector3(xpositions[i + 1], ypositions[i + 1], 1);
			Vector3 p2 = new Vector3(xpositions[i], bottom, 1);
			Vector3 p3 = new Vector3(xpositions[i + 1], bottom, 1);
			
			Vector3 p4 = new Vector3(xpositions[i], ypositions[i], -1);
			Vector3 p5 =  new Vector3(xpositions[i + 1], ypositions[i + 1], -1);
			Vector3 p6 = new Vector3(xpositions[i], bottom, -1);
			Vector3 p7 = new Vector3(xpositions[i + 1], bottom, -1);

				// Bottom
				Vertices[0] = p0;
				Vertices[1] = p1;
				Vertices[2] = p2;
				Vertices[3] = p3;
				// Left

				Vertices[4] = p7;
				Vertices[5] = p4;
				Vertices[6] = p0;
				Vertices[7] = p3;
				// Front

				Vertices[8] = p4;
				Vertices[9] = p5;
				Vertices[10] = p1;
				Vertices[11] = p0;
				// Back

				Vertices[12] = p6;
				Vertices[13] = p7;
				Vertices[14] = p3;
				Vertices[15] = p2;
				// Right

				Vertices[16] = p5;
				Vertices[17] = p6;
				Vertices[18] = p2;
				Vertices[19] = p1;
				// Top
				Vertices[20] = p7;
				Vertices[21] = p6;
				Vertices[22] = p5;
				Vertices[23] = p4;

			Vector2 _00 = new Vector2( 0f, 0f );
			Vector2 _10 = new Vector2( 1f, 0f );
			Vector2 _01 = new Vector2( 0f, 1f );
			Vector2 _11 = new Vector2( 1f, 1f );
			
			Vector2[] UVs = new Vector2[24];

			UVs[0] = _00;
			UVs[1] = _10;
			UVs[2] = _01;
			UVs[3] = _11;

			UVs[4] = _00;
			UVs[5] = _10;
			UVs[6] = _01;
			UVs[7] = _11;

			UVs[8] = _00;
			UVs[9] = _10;
			UVs[10] = _01;
			UVs[11] = _11;

			UVs[12] = _00;
			UVs[13] = _10;
			UVs[14] = _01;
			UVs[15] = _11;

			UVs[16] = _00;
			UVs[17] = _10;
			UVs[18] = _01;
			UVs[19] = _11;

			UVs[20] = _00;
			UVs[21] = _10;
			UVs[22] = _01;
			UVs[23] = _11;

			int[] tris = new int[]{
				// Bottom
				3, 1, 0,
				3, 2, 1,			
				
				// Left
				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
				
				// Front
				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
				
				// Back
				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
				
				// Right
				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
				
				// Top
				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
			};
			
			meshes[i].vertices = Vertices;
			meshes[i].uv = UVs;
			meshes[i].triangles = tris;
			
			meshObjects[i] = Instantiate(waterMesh,Vector3.zero, Quaternion.identity) as GameObject;
			meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
			meshObjects[i].transform.parent = transform;
			
			colliders[i] = new GameObject();
			colliders[i].name = "Trigger";
			colliders[i].AddComponent<BoxCollider2D>();
			colliders[i].transform.parent = transform;
			colliders[i].transform.position = new Vector3(Left + Width * (i + 0.5f) / edgecount, Top - 0.5f, 0);
			colliders[i].transform.localScale = new Vector3(Width /edgecount, 1,1);
			colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
			colliders[i].AddComponent<GeneWaterDetector>();
		}
	}
	
	void UpdateMeshes()
	{
		for (int i = 0; i < meshes.Length; i++) 
		{
			Vector3[] Vertices = new Vector3[24];
			Vector3 p0 = new Vector3(xpositions[i], ypositions[i], z);
			Vector3 p1 =  new Vector3(xpositions[i + 1], ypositions[i + 1], z);
			Vector3 p2 = new Vector3(xpositions[i], bottom, z);
			Vector3 p3 = new Vector3(xpositions[i + 1], bottom, z);
			
			Vector3 p4 = new Vector3(xpositions[i], ypositions[i], -z);
			Vector3 p5 =  new Vector3(xpositions[i + 1], ypositions[i + 1], -z);
			Vector3 p6 = new Vector3(xpositions[i], bottom, -z);
			Vector3 p7 = new Vector3(xpositions[i + 1], bottom, -z);

			// Bottom
			Vertices[0] = p0;
			Vertices[1] = p1;
			Vertices[2] = p2;
			Vertices[3] = p3;
			// Left
			
			Vertices[4] = p7;
			Vertices[5] = p4;
			Vertices[6] = p0;
			Vertices[7] = p3;
			// Front
			
			Vertices[8] = p4;
			Vertices[9] = p5;
			Vertices[10] = p1;
			Vertices[11] = p0;
			// Back
			
			Vertices[12] = p6;
			Vertices[13] = p7;
			Vertices[14] = p3;
			Vertices[15] = p2;
			// Right
			
			Vertices[16] = p5;
			Vertices[17] = p6;
			Vertices[18] = p2;
			Vertices[19] = p1;
			// Top
			Vertices[20] = p7;
			Vertices[21] = p6;
			Vertices[22] = p5;
			Vertices[23] = p4;
		}
	}
	
	void FixedUpdate()
	{
		for (int i = 0; i < xpositions.Length; i ++)
		{
			float force = springconstant * (ypositions[i] - baseheight) + velocities[i] * damping;
			accelerations[i] = -force;
			ypositions[i] += velocities[i];
			velocities[i] += accelerations[i];
			Body.SetPosition(i, new Vector3(xpositions[i], ypositions[i],z));
		}
		
		float[] leftDeltas = new float[xpositions.Length];
		float[] rightDeltas = new float[xpositions.Length];
		
		for (int j = 0; j < 8; j++) {
			for (int i = 0; i < xpositions.Length; i++) 
			{
				if (i > 0) 
				{
					leftDeltas [i] = spread * (ypositions[i] - ypositions[i - 1]);
					velocities [i - 1] += leftDeltas [i];
				}
				
				if (i < xpositions.Length - 1) 
				{
					rightDeltas [i] = spread * (ypositions[i] - ypositions [i + 1]);
					velocities[i + 1] += rightDeltas[i];
				}
			}
			
			for (int i = 0; i < xpositions.Length; i++) 
			{
				if (i > 0) 
				{
					ypositions [i - 1] += leftDeltas [i];
				}
				if (i < xpositions.Length - 1) 
				{
					ypositions [i + 1] += rightDeltas [i];
				}
				
			}
		}
		
		UpdateMeshes ();
	}
}
