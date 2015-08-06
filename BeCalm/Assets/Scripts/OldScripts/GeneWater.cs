using UnityEngine;
using System.Collections;

public class GeneWater : MonoBehaviour {

	LineRenderer Body;

	float[] xpositions;
	float[] ypositions;
	//float[] zpositions;
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
	public float z = 0;

	float baseheight;
	//float left;
	float bottom;

	// Use this for initialization
	void Start () {
		Vector3 pos = Camera.main.WorldToViewportPoint (transform.position);
		SpawnWater (pos.x, 100,0, -3);
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

	public void Float(float xpos, float velocity, Rigidbody boat)
	{
		
		if (xpos >= xpositions[0] && xpos <= xpositions[xpositions.Length-1])
		{
			//Offset the x position to be the distance from the left side
			xpos -= xpositions[0];
			
			//Find which spring we're touching
			int index = Mathf.RoundToInt((xpositions.Length-1)*(xpos / (xpositions[xpositions.Length-1] - xpositions[0])));

			float springpos;
			springpos = xpositions[index];

			Vector3 boatpos = new Vector3 (boat.transform.position.x,springpos, boat.transform.position.z);
			boat.transform.position = boatpos;
			//Add the velocity of the falling object to the spring
			//velocities[index] += velocity;
			Vector2 boatfloat = new Vector2 (boat.velocity.x, boat.velocity.y);
			boatfloat.y  -= velocity;
			boat.velocity = boatfloat;
		}
	}

	public void SpawnWater(float Left, float Width, float Top, float Bottom)
	{

		gameObject.AddComponent<BoxCollider>();
		gameObject.GetComponent<BoxCollider>().center = new Vector2(Left + Width / 2, (Top + Bottom) / 2);
		gameObject.GetComponent<BoxCollider>().size = new Vector2(Width, Top - Bottom);
		gameObject.GetComponent<BoxCollider>().isTrigger = true;

		int edgecount = Mathf.RoundToInt (Width) * 5;
		int nodecount = edgecount + 1;

		//Body = gameObject.AddComponent<LineRenderer> ();
		//Body.material = mat;
		//Body.material.renderQueue = 1000;
		//Body.SetVertexCount (nodecount);
		//Body.SetWidth (0.1f, 0.1f);

		xpositions = new float[nodecount];
		ypositions = new float[nodecount];
		//zpositions = new float[nodecount];
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
			//Body.SetPosition (i, new Vector3 (xpositions [i], Top, z));
		}

		for (int i = 0; i < edgecount; i++)
		{
			meshes [i] = new Mesh ();
			Vector3[] Vertices = new Vector3[4];
			Vertices[0] = new Vector3(xpositions[i], ypositions[i], z);
			Vertices[1] = new Vector3(xpositions[i + 1], ypositions[i + 1], z);
			Vertices[2] = new Vector3(xpositions[i], bottom, z);
			Vertices[3] = new Vector3(xpositions[i + 1], bottom, z);

			Vector2[] UVs = new Vector2[4];
			UVs[0] = new Vector2(0,1);
			UVs[1] = new Vector2(1,1);
			UVs[2] = new Vector2(0,0);
			UVs[3] = new Vector2(1,0);

			int[] tris = new int[6]{0,1,3,3,2,0};

			meshes[i].vertices = Vertices;
			meshes[i].uv = UVs;
			meshes[i].triangles = tris;

			meshObjects[i] = Instantiate(waterMesh,Vector3.zero, Quaternion.identity) as GameObject;
			meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
			meshObjects[i].transform.parent = transform;

			colliders[i] = new GameObject();
			colliders[i].name = "Trigger";
			colliders[i].AddComponent<BoxCollider>();
			colliders[i].transform.parent = transform;
			colliders[i].transform.position = new Vector3(Left + Width * (i + 0.5f) / edgecount, Top - 0.5f, 0);
			colliders[i].transform.localScale = new Vector3(Width /edgecount, 1,1);
			colliders[i].GetComponent<BoxCollider>().isTrigger = true;
			colliders[i].AddComponent<GeneWaterDetector>();
		}
	}

	void UpdateMeshes()
	{
		for (int i = 0; i < meshes.Length; i++) 
		{
			Vector3[] Vertices = new Vector3[4];
			Vertices[0] = new Vector3(xpositions[i], ypositions[i], z);
			Vertices[1] = new Vector3(xpositions[i + 1], ypositions[i + 1], z);
			Vertices[2] = new Vector3(xpositions[i], bottom, z);
			Vertices[3] = new Vector3(xpositions[i + 1], bottom, z);

			meshes[i].vertices = Vertices;
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
			//Body.SetPosition(i, new Vector3(xpositions[i], ypositions[i],z));
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


		void OnTriggerStay(Collider Hit)
		{
		if(Hit.tag == "Boat"){
		Float(transform.position.x, Hit.GetComponent<Rigidbody>().velocity.y*Hit.GetComponent<Rigidbody>().mass, Hit.gameObject.GetComponent<Rigidbody>());
		}
		}
	
}
