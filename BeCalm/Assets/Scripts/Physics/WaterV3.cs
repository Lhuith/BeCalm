using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterV3 : MonoBehaviour {
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
	public float alpha = 0.01f;

	//P = kernel size
	//6 is the smallest value tat gives water-like motion
	int P = 8;
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

		boxCollider.center = new Vector3 (waterWidth / 2f, 0f, waterWidth / 2f);
		boxCollider.size = new Vector3 (waterWidth, 0.1f, waterWidth);

		//Center the mesh to make it easier to know where it is
		transform.position = new Vector3 (transform.position.x, 0f, transform.position.z );

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
		storedKernelArray = new float[P * 2 + 1, P * 2 + 1];
		
		PrecomputateKernalValues ();
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
		AddWaterWakes (dt);

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
	//Precomputate the kernel values G(k,1)
	void PrecomputateKernalValues(){
		float G_zero = CalculateG_zero ();

		//P - kernel size
		for (int k = -P; k <= P; k++) 
		{
			for(int l = -P; l <= P; l++)
			{
				//Need "+ P" becuase we itrate from -P and not 0, which is how they are stored in the array
				storedKernelArray[k + P, l + P] =  CalculateG((float)k,(float)l, G_zero);
			}
		}
	}


	//G(k,l)
	private float CalculateG(float k, float l, float G_zero)
	{
		float delta_q = 0.001f;
		float sigma = 1f;
		float r = Mathf.Sqrt (k * k + l * l);

		float G = 0f;

		for (int n = 1; n <= 10000; n++) 
		{
			float q_n = ((float)n * delta_q);
			float q_n_square = q_n * q_n;

			G += q_n_square * Mathf.Exp(-sigma * q_n_square) * BesselFunction(q_n * r);
		}

		G /= G_zero;

		return G;
	}

	private float CalculateG_zero()
	{
		float delta_q = 0.001f;
		float sigma = 1f;

		float G_zero = 0f;
		for (int n = 1; n <= 10000; n++) 
		{
			float q_n_square = ((float) n * delta_q) * ((float)n * delta_q);

			G_zero += q_n_square * Mathf.Exp(-sigma * q_n_square);
		}

		return G_zero;
	}

	private float BesselFunction(float x)
	{
		//from http://people.math.sfu.ca/~cbm/aands/frameindex.htm
		//page 369-370

		float J_zero_of_X = 0f;

		//Test to see if the bessel functions are valid
		//Has to be above -3
		if (x <= -3f) 
		{
			Debug.Log("Smaller");
		}

		//9.4.1
		//-3 <= x <= 3
		if (x <= 3f) 
		{
			//Ignored the small rest term at the end
			J_zero_of_X =
				1f -
						2.2499997f * Mathf.Pow (x/ 3f, 2f) +
						1.2656208f * Mathf.Pow (x/ 3f, 4f) -
						0.3163866f * Mathf.Pow (x/ 3f, 6f) +
						0.0444479f * Mathf.Pow (x/ 3f, 8f) -
						0.0038444f * Mathf.Pow (x/ 3f, 10f) +
						0.0002100f * Mathf.Pow (x/ 3f, 12f) ;
		}
		//9.4.3
		//3 <= x <= infinity
		else
		{
			//Ignored the small rest term at the end
			float f_zero =
				0.79788456f -
					0.00000077f * Mathf.Pow (3f/ x, 1f) -
					0.00552740f * Mathf.Pow (3f/ x, 2f) -
					0.00009512f * Mathf.Pow (3f/ x, 3f) -
					0.00137237f * Mathf.Pow (3f/ x, 4f) -
					0.00072805f * Mathf.Pow (3f/ x, 5f) +
					0.00014476f * Mathf.Pow (3f/ x, 6f) ;
		

		//Ignored the small rest term at the end
		float theta_zero =
			x -
				0.78539816f -
				0.04166397f * Mathf.Pow (3f/ x, 1f) -
				0.00003954f * Mathf.Pow (3f/ x, 2f) -
				0.00262573f * Mathf.Pow (3f/ x, 3f) -
				0.00054125f * Mathf.Pow (3f/ x, 4f) -
				0.00029333f * Mathf.Pow (3f/ x, 5f) +
				0.00013558f * Mathf.Pow (3f/ x, 6f) ;

		//Should be cos and not acos
			J_zero_of_X = Mathf.Pow(x, -1f/3f) * f_zero * Mathf.Cos (theta_zero);
		}

		return J_zero_of_X;
	}

	void AddWaterWakes(float dt)
	{
		//if strange gigantic waves happens, adjust alpha
		
		//add sources and obstructions
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++) 
			{
				//Add height from source
				height[j][i].y += source[j][i].y;
				
				//Clear the source
				source[j][i].y = 0f;
				
				//Add obstruction
				height[j][i].y *= obstruction[j][i].y;
			}
		}
		
		//Convolve to update verticalDerivative
		Convolve ();
		
		//Same for all
		float twoMinusAlphaTimesDt = 2f - alpha * dt;
		float onePlusAlphaTimesDt = 1f + alpha * dt;
		float gravityTimesDtTimesDt = g * dt * dt;
		
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++)
			{
				//Faster to do this once
				float currentHeight = height[j][i].y;
				
				//Calculate the new height
				float newHeight = 0f;
				
				newHeight += currentHeight * twoMinusAlphaTimesDt;
				newHeight -= previousHeight[j][i].y;
				newHeight -= gravityTimesDtTimesDt * verticalDerivative[j][i].y;
				newHeight /= onePlusAlphaTimesDt;
				
				//Save the old height
				previousHeight[j][i].y = currentHeight;
				
				//Add the new height
				height[j][i].y = newHeight;
				
				//if wehave ambientwaves we can add them here
				//just replace this with a call to a method where you find the current height of the ambient wave
				//At the current coorndate
				//waveScript.GetWaveYPos(i,j);
				//waveScript.GetWaveYPos(height[j][i].x, height[j][i].z)
				//float heightAmbientWave = waveScript.GetWaveYPos(height[j][i].x, height[j][i].z);
				float heightAmbientWave = ambientWaves.yCheck;

				heightDiffrence[j][i].y = heightAmbientWave + newHeight;

				//waterMat.SetFloat ("_WaveYPos", heightDiffrence[j][i].y);
				//waterMat.SetFloat ("_WaveZPos", heightDiffrence[j][i].z);
			}
		}
	}


	//Convolve to update verticalDerivative
	//This might seem unnecessary, but we will save one array if we are doing it before the main loop
	void Convolve()
	{
		for (int j = 0; j < arrayLength; j++) 
		{
			for (int i = 0; i < arrayLength; i++)
			{
				float vDeriv = 0f;

				//Will include when k, l = 0
				for (int k = -P; k <= P; k++) 
				{
					for(int l = -P; l <= P; l++)
					{
						//Get the precomputed values
						//Need "+ P" becuase we itirate from -P and not 0, which is how they are stored in the array

						float kernelValue = storedKernelArray[k + P, l + P];

						//Make sure we are within the water
						if(j+k >= 0 && j+k < arrayLength && i+l >= 0 && i+l < arrayLength)
						{
							vDeriv += kernelValue * height[j+k][i+l].y;
						}

						//Outside
						else
						{
							//Right
							if(j+k >= arrayLength && i+l >= 0 && i+l < arrayLength)
							{
								vDeriv += kernelValue * height[2 * arrayLength - j - k - 1][i+l].y;
							}

							//Top
							if(i+l >= arrayLength && j+k >= 0 && j+k < arrayLength)
							{
								vDeriv += kernelValue * height[j+k][2 * arrayLength - i - l - 1].y;
							}

							//Left
							if(j+k < 0 && i+l >= 0 && i+l < arrayLength)
							{
								vDeriv += kernelValue * height[-j-k][i+l].y;
							}

							//Bottom
							else if(i+l < 0 && j+k >= 0 && j+k < arrayLength)
							{
								vDeriv += kernelValue * height[j+k][-i-l].y;
							}
			}
		}

	}

				verticalDerivative[j][i].y = vDeriv;



	}
	}
}
	public void CreateWaterWakes(Transform boatPos,float force){

			
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

							source[j][i].y += -0.01f + distanceCompensator;
						}
					}
				}
			}
		}

	}
}


