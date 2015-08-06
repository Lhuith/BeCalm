using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterV4 : MonoBehaviour {
	public Mesh waterMesh;
	MeshFilter waterMeshFilter;
	
	public bool newMesh;
	
	//The total size in m
	public float waterWidth = 3f;
	
	//Width of one square (=dinstance between vertices)
	public float gridSpacing = 0.1f;
	
	//Water wakes parameters
	//Velocity damping term
	//Useful to help suppress numerical instabilities tat can arise
	public float alpha = 0.99f;
	
	//P = kernel size
	//6 is the smallest value tat gives water-like motion
	//int P = 8;
	//Should be the neg or the waves will movein the wrong direction
	float g = -9.81f;
	
	//Store the precomputed kernel values here
	float [,] storedKernelArray;
	
	//Used in the Iwave loop
	Vector3[][] height;
	Vector3[][] previousHeight;
	Vector3[][] verticalDerivative;
	public Vector3[][] source;
	public Vector3[][] obstruction;
	
	//To update the mesh we need a 1d array
	Vector3[] unfold_verts;
	//To be able to add ambient waves
	public Vector3[][] heightDiffrence;
	//Faster to calculate this once
	public int arrayLength;
	
	//private Material waterMat;
	
	public float updateTimer = 0f;
	
	//private WaveController waveScript;
	
	public List<Vector3[]> height_tmp = new List<Vector3[]>();
	
	public GenerateWaves ambientWaves;
	
	public GameObject player;
	
	public float distance;
	
	
	void Start () {
		
		//GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");
		//waveScript = gameController.GetComponent<WaveController> ();
		
		waterMeshFilter = this.GetComponent<MeshFilter> ();
		//waterMat = this.GetComponent<MeshRenderer> ().material;
		
		ambientWaves = GetComponent<GenerateWaves> ();
		//Create the water mesh
		//dont forget to write "using System.Collections.Generic;" at the top
		
		player = CustomExtensions.GetPlayer ();
		
		
		if (newMesh) {
			height_tmp = GenerateWaterMesh.GenerateWater (waterMeshFilter, waterWidth, gridSpacing);
		} else {
			height_tmp = GrabWaterMeshData.GrabWaterData (waterMeshFilter, waterMeshFilter.mesh.bounds.size.z, gridSpacing);
			
		}
		//List<Vector3[]> height_tmp = GenerateWaterMesh.GenerateWater (waterMeshFilter, waterWidth, gridSpacing);
		waterMesh = waterMeshFilter.mesh;
		
		//Resize box collider
		//Need a box collider so the mouse can interact with the water
		BoxCollider boxCollider = this.GetComponent<BoxCollider> ();
		
		boxCollider.center = new Vector3 (waterWidth, 0f, waterWidth);
		boxCollider.size = new Vector3 (waterWidth/gridSpacing, 0.1f, waterWidth/gridSpacing);
		
		//Center the mesh to make it easier to know where it is
		transform.position = new Vector3 (transform.position.x, 0f, transform.position.z);
		
		//Init the arrays we need
		//these are now filled with heights at 0;
		//Need to clone these
		height = height_tmp.ToArray ();
		previousHeight = CloneList (height);
		verticalDerivative = CloneList (height);
		source = CloneList (height);
		obstruction = CloneList (height);
		heightDiffrence = CloneList (height);
		
		//Create this once here, so we dont need to create it each update
		unfold_verts = new Vector3[height.Length * height.Length];
		
		arrayLength = height.Length;
		
		//Precompute the convolution kernel values	
		
		PreComputekernels.ComputeKernel ();
		storedKernelArray = PreComputekernels.storedKernelArray;

		//Need refrence to the meshFilter so we can add the water
		
		//Add obstruction when the wave hits the walls
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++)
			{
				if (j == 0 || j == arrayLength - 1 || i == 0 || i == arrayLength -1)
				{
					obstruction[j][i].y = 0f;
				}
				else
				{
					obstruction[j][i].y = 1f;
				}
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		distance = CustomExtensions.GetDistance(this.gameObject, player);
		
		//Move water wakes
		CreateWaterWakesWithMouse ();
		updateTimer += Time.deltaTime;
		if (updateTimer > 0.01f) {
			MoveWater (0.03f);
			updateTimer = 0f;
		}
	}
	
	
	void MoveWater(float dt)
	{
		//this will update height[j][i]
		CreateWaterWakes.WaterWakes (storedKernelArray, dt, arrayLength, height, source, heightDiffrence, previousHeight, alpha, obstruction, g, verticalDerivative);
		//Update the mesh with the new heights
		//Unfold the 2d array of the verticies into a 1d array
		for (int i = 0; i < arrayLength; i++) 
		{
			//Copies all the elements of the current array to the specified array
			heightDiffrence[i].CopyTo(unfold_verts, i * heightDiffrence.Length);
		}
		
		//Add the new positions of the water to the water mesh
		waterMesh.vertices = unfold_verts;
		//Ensure the bounding volume is correct
		waterMesh.RecalculateBounds ();
		//After modifing the vertices it is often useful to update the normals to reflect the change
		waterMesh.RecalculateNormals ();
	}
	
	
	
	//Clone an array and the inner array
	Vector3[][] CloneList(Vector3[][] arrayToClone)
	{
		//first clone the outer array
		Vector3[][] newArray = arrayToClone.Clone () as Vector3[][];
		
		//then clone the inner arrays
		for (int i = 0; i < newArray.Length; i++) 
		{
			newArray[i] = newArray[i].Clone() as Vector3[];
		}
		
		return newArray;
	}

	public void CreateWakes(Transform boatPos,float force){
		
		
		//Convert the mouse position fom global to local
		Vector3 localPos = transform.InverseTransformPoint(boatPos.position);
		
		//Loop through all the vertices of the water mesh
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++)
			{
				//Find the closest vertice within a certain distance from the mouse
				float sqrDistanceToVertice = (height[j][i] - localPos).sqrMagnitude;
				
				//If the vertice is within a certain range
				float sqrDistance = 0.2f * 0.2f;
				if(sqrDistanceToVertice < sqrDistance) 
				{
					Debug.Log(source[j][i].y);
					//Get a smaller value the greater the distance is to make it look better
					//float distanceCompensator = 2 - (sqrDistanceToVertice / sqrDistance);
					
					//Add the force that now depends on how far the vertice is from the mouse
					
					source[j][i].y += -0.001f * force;
				}
			}
		}
	}
	
	void CreateWaterWakesWithMouse()
	{
		//Fire rayt from the current mouse position
		if (Input.GetKey (KeyCode.Mouse0)) 
		{
			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				
				//Convert the mouse position fom global to local
				Vector3 localPos = transform.InverseTransformPoint(hit.point);
				
				//Loop through all the vertices of the water mesh
				for (int j = 0; j < arrayLength; j++) 
				{
					for (int i = 0; i < arrayLength; i++)
					{
						//Find the closest vertice within a certain distance from the mouse
						float sqrDistanceToVertice = (height[j][i] - localPos).sqrMagnitude;
						
						//If the vertice is within a certain range
						float sqrDistance = 0.2f * 0.2f;
						if(sqrDistanceToVertice < sqrDistance) 
						{
							//Debug.Log(source[j][i].y);
							//Get a smaller value the greater the distance is to make it look better
							float distanceCompensator = 1 - (sqrDistanceToVertice / sqrDistance);
							
							//Add the force that now depends on how far the vertice is from the mouse
							
							source[j][i].y += -0.01f * distanceCompensator;
						}
					}
				}
			}
		}
		
	}
}


