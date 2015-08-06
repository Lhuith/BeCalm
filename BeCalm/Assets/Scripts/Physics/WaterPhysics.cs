using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterPhysics : MonoBehaviour {
	public RaycastHit hit;
	
	public GameObject underWaterMeshObj;
	private Mesh BoatMesh;

	private WaveController waveScript;
	
	//These are always constant and comes from the original hull
	//Coordinates of all vertices

	private Vector3[] originalVerticesArray;
	//Positions in allVerticesArray, such as 0, 3, 5, to build traingles

	private int[] originalTrianglesArray;

	//The part of the boat thats underwater
	private Mesh UnderWaterMesh;

	//These will be transformed into arrays later
	private List<Vector3> underwaterVertices;
	public List<int> underwaterTriangles;
	
	public WaterV3 waterWakes;

	Rigidbody boatRB;
	
	public float thrust;

	public float updateTimer;
	void Start () {

		//waterWakes = GameObject.FindGameObjectWithTag ("Water").GetComponent<WaterV3> ();

		GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");
		waveScript = gameController.GetComponent<WaveController> ();

		UnderWaterMesh = underWaterMeshObj.GetComponent<MeshFilter> ().mesh;
		BoatMesh = GetComponent<MeshFilter> ().mesh;

		originalVerticesArray = BoatMesh.vertices;
		originalTrianglesArray = BoatMesh.triangles;

		boatRB = GetComponent<Rigidbody> ();

		boatRB.maxAngularVelocity = 0.5f;


	}

	void FixedUpdate(){

		//Will add bouyance so it can float, and drifting from the waves
		if (underwaterTriangles.Count > 0) {

			AddForceToBoat();
		}
	}

	void Update(){
	
		GenerateUnderwaterMesh ();
		if (underwaterTriangles.Count > 0) {
			updateTimer += Time.deltaTime;
			if (updateTimer > .1f) 
			{
				float dist = 30;
				Vector3 dir = new Vector3(0,-1,0);
				
				//edit: to draw ray also//
				Debug.DrawRay(transform.position,dir*dist,Color.green);
				
				
				if(Physics.Raycast(transform.position, dir, out hit , dist)){
					//the ray collided with something, you can interact
					// with the hit object now by using hit.collider.gameObject
					if(hit.collider.gameObject.GetComponent<WaveDisplacement>()){
					//hit.collider.gameObject.GetComponent<WaveDisplacement>().enabled = true;
					hit.collider.gameObject.GetComponent<WaveDisplacement>().WaterWake(hit);
					Debug.Log("WaterWakingFromBoat");
					}
				}
				else{
					//hit.collider.gameObject.GetComponent<WaveDisplacement>().enabled = false;
				}

				updateTimer = 0f;
			}else{
			}
		}
	}
	


	//will Generate the mesh thats under the water
	//you can Display the mesh, but it will float without it, we just need the data
	

	public void GenerateUnderwaterMesh(){
		//These sore the final data we need to creat the mesh thats underwater
		underwaterVertices = new List<Vector3> ();
		underwaterTriangles = new List<int> ();

		//Loop through all the traingles (3 traingles at a time)
		int i = 0;
		while (i < originalTrianglesArray.Length) {
			//Find the distance from each vertice in the current traingle to the water
			//Negetive distance means below the water

			//That position of the vertice in Vector3 format (need to save this position for later)
			Vector3 vertice_1_pos = originalVerticesArray [originalTrianglesArray [i]];
			float? distance1 = DistanceToWater (vertice_1_pos);

			i++;

			Vector3 vertice_2_pos = originalVerticesArray [originalTrianglesArray [i]];
			float? distance2 = DistanceToWater (vertice_2_pos);

			i++;

			Vector3 vertice_3_pos = originalVerticesArray [originalTrianglesArray [i]];
			float? distance3 = DistanceToWater (vertice_3_pos);

			i++;

			//Continue to the next triangle if all the traingles are above the water
			if (distance1 > 0f && distance2 > 0f && distance3 > 0f) {
				continue;
			}

			//Continue to the next traingle if there is no water
			if (distance1 == null || distance2 == null || distance3 == null) {
				continue;
			}

			DistanceHelper distance1OBJ = new DistanceHelper ();
			DistanceHelper distance2OBJ = new DistanceHelper ();
			DistanceHelper distance3OBJ = new DistanceHelper ();

			distance1OBJ.distance = (float)distance1; //from float? to float
			distance1OBJ.name = "one";
			distance1OBJ.verticePos = vertice_1_pos;

			distance2OBJ.distance = (float)distance2; //from float? to float
			distance2OBJ.name = "two";
			distance2OBJ.verticePos = vertice_2_pos;

			distance3OBJ.distance = (float)distance3; //from float? to float
			distance3OBJ.name = "three";
			distance3OBJ.verticePos = vertice_3_pos;

			//Add the objects to a list so we can sort them
			List<DistanceHelper> allDistancesHelperList = new List<DistanceHelper> ();
			allDistancesHelperList.Add (distance1OBJ);
			allDistancesHelperList.Add (distance2OBJ);
			allDistancesHelperList.Add (distance3OBJ);

			allDistancesHelperList.Sort ();
			allDistancesHelperList.Reverse ();

			//All vertices are underwater
			if (allDistancesHelperList [0].distance < 0f && allDistancesHelperList [1].distance < 0f && allDistancesHelperList [2].distance < 0f) {
				//Make sure these coordinates are unsorted
				AddCoordinateToMesh (distance1OBJ.verticePos);
				AddCoordinateToMesh (distance2OBJ.verticePos);
				AddCoordinateToMesh (distance3OBJ.verticePos);
			}

			//Just Adds one Coordinate to the new mesh


			//One Vertice is above the water, the rest is below
			else if (allDistancesHelperList [0].distance > 0f && allDistancesHelperList [1].distance < 0f && allDistancesHelperList [2].distance < 0f) {

				//H is always at position 0
				Vector3 H = allDistancesHelperList [0].verticePos;

				//left of H is M
				//Right of H is L

				//Find the name of M
				string M_name = "temp";
				if (allDistancesHelperList [0].name == "one") {
					M_name = "three";
				} 

				else if (allDistancesHelperList [0].name == "two") {
					M_name = "one";
				}

				else {
					M_name = "two";
				}

				//We also need the hights to the water
				float h_H = allDistancesHelperList [0].distance;
				float h_M = 0f;
				float h_L = 0f;

				Vector3 M = Vector3.zero;
				Vector3 L = Vector3.zero;


				//This means M is at the position 1 in the Lust
				if (allDistancesHelperList [1].name == M_name) {
					M = allDistancesHelperList [1].verticePos;
					L = allDistancesHelperList [2].verticePos;

					h_M = allDistancesHelperList [1].distance;
					h_L = allDistancesHelperList [2].distance;

				} else {
					M = allDistancesHelperList [2].verticePos;
					L = allDistancesHelperList [1].verticePos;
					
					h_M = allDistancesHelperList [2].distance;
					h_L = allDistancesHelperList [1].distance;
				}

				//Now we can calculate where we should cut the traingle to form 2 new traingles
				//becuase to the resulting area will always form a square

				//Point I_M
				Vector3 MH = H - M;

				float t_M = -h_M / (h_H - h_M);

				Vector3 MI_M = t_M * MH;

				Vector3 I_M = MI_M + M;

				//Point I_L

				Vector3 LH = H - L;

				float t_L = -h_L / (h_H - h_L);

				Vector3 LI_L = t_L * LH;

				Vector3 I_L = LI_L + L;

				//Building the 2 new triangles
				AddCoordinateToMesh (M);
				AddCoordinateToMesh (I_M);
				AddCoordinateToMesh (I_L);

				AddCoordinateToMesh (M);
				AddCoordinateToMesh (I_L);
				AddCoordinateToMesh (L);
			}
			//Two vertices are above the water, the other is below
			else if (allDistancesHelperList [0].distance > 0f && allDistancesHelperList [1].distance > 0f && allDistancesHelperList [2].distance < 0f) 
			{
				//H and M are aboe the water
				//H is after the verice that's below water, which is L
				//So we know which one is L becuase it is the last in the sorted list

				Vector3 L = allDistancesHelperList [2].verticePos;

				//Find the name of H
				string H_name = "temp";
				if (allDistancesHelperList [2].name == "one") {
					H_name = "two";
				} 

				else if (allDistancesHelperList [2].name == "two") {
					H_name = "three";
				} 

				else {
					H_name = "one";
				}
				
				//We also need the hights to the water
				float h_L = allDistancesHelperList [2].distance;
				float h_H = 0f;
				float h_M = 0f;
				
				Vector3 H = Vector3.zero;
				Vector3 M = Vector3.zero;
				
				
				//This means M is at the position 1 in the List
				if (allDistancesHelperList [1].name == H_name) {
					H = allDistancesHelperList [1].verticePos;
					M = allDistancesHelperList [0].verticePos;

					h_H = allDistancesHelperList [1].distance;
					h_M = allDistancesHelperList [0].distance;
					
				} else {
					H = allDistancesHelperList [0].verticePos;
					M = allDistancesHelperList [1].verticePos;
					
					h_H = allDistancesHelperList [0].distance;
					h_M = allDistancesHelperList [1].distance;
				}
				
				//Now we can find where to cut the traingle
				
				//Point J_M
				Vector3 LM = M - L;
				
				float t_M = -h_L / (h_M - h_L);
				
				Vector3 LJ_M = t_M * LM;
				
				Vector3 J_M = LJ_M + L;
				
				//Point J_H
				
				Vector3 LH = H - L;
				
				float t_H = -h_L / (h_H - h_L);
				
				Vector3 LJ_H = t_H * LH;

				Vector3 J_H = LJ_H + L;
				
				//Create the triangle
				AddCoordinateToMesh (L);
				AddCoordinateToMesh (J_H);
				AddCoordinateToMesh (J_M);
			}

		

			UnderWaterMesh.Clear ();
			UnderWaterMesh.name = "UnderWaterMesh";
			UnderWaterMesh.vertices = underwaterVertices.ToArray ();
			//underwatermesh uv = uvs.toArray();
			UnderWaterMesh.triangles = underwaterTriangles.ToArray ();

			//Ensure that the bounding volume is correct
			UnderWaterMesh.RecalculateBounds ();
			//Update the normals to reflect the change
			UnderWaterMesh.RecalculateNormals ();

		}
	}

	float? DistanceToWater(Vector3 position){
		//calculate the coordinate of the vertice in global space
		Vector3 globalVerticePosition = transform.TransformPoint (position);
		
		float? y_pos = 0f;
		Vector3 wave_Pos = new Vector3(0,0,0);
		wave_Pos += waveScript.GetWaveYPos (globalVerticePosition.x, globalVerticePosition.z);
		y_pos = wave_Pos.y;
		
		return globalVerticePosition.y - y_pos;
		
	}
	
	void AddCoordinateToMesh(Vector3 coord){
		underwaterVertices.Add (coord);
		underwaterTriangles.Add (underwaterVertices.Count - 1);
	}


	public void AddForceToBoat(){
		
		int i = 0;
		while (i < underwaterTriangles.Count) {
			
			//the Position of the vertice in the Vector 3 format
			Vector3 vertice_1_pos = underwaterVertices [underwaterTriangles [i]];
			
			i++;
			
			Vector3 vertice_2_pos = underwaterVertices [underwaterTriangles [i]];
			
			i++;
			
			Vector3 vertice_3_pos = underwaterVertices [underwaterTriangles [i]];
			
			i++;
			
			//Calculate the center of the triangle
			Vector3 centrePoint = (vertice_1_pos + vertice_2_pos + vertice_3_pos) / 3f;
			
			
			//Calculate the distance to the surface from the centure of the traingle
			//DistancetoWater() will transform to worldSpace
			float distance_to_surface = Mathf.Abs ((float)DistanceToWater (centrePoint));
			
			//From localspace to worldSpace
			centrePoint = transform.TransformPoint (centrePoint);
			vertice_1_pos = transform.TransformPoint (vertice_1_pos);
			vertice_2_pos = transform.TransformPoint (vertice_2_pos);
			vertice_3_pos = transform.TransformPoint (vertice_3_pos);
			
			
			//Calculate the normal to the triagnle
			Vector3 crossProduct = Vector3.Cross (vertice_2_pos - vertice_1_pos, vertice_3_pos - vertice_1_pos).normalized;
			
			//Test that everything is working
			Debug.DrawRay (centrePoint, crossProduct * 3f);
			
			//Calculte that the area of the triangle by using ther Hero's formula
			float a = Vector3.Distance(vertice_1_pos, vertice_2_pos);
//			float b = Vector3.Distance(vertice_2_pos, vertice_3_pos);
			float c = Vector3.Distance(vertice_3_pos, vertice_1_pos);
//
//			float s = (a + b + c) /2f;
//			
//			float area_heron = Mathf.Sqrt(s * (s-a) * (s-b) * (s-c));
			
			//Alterntive 2 - area of the traingle by using sinus
			
			float area_sin = (a * c * Mathf.Sin(Vector3.Angle(vertice_2_pos-vertice_1_pos, vertice_3_pos-vertice_1_pos) * Mathf.Deg2Rad))/2f;
			
			float area = area_sin;
			
			//float area = area_heron;
			
			//The Buoyancy + Wave Drift + Prupolsion force
			AddBuoyancy(distance_to_surface,area, crossProduct, centrePoint);
			AddWaveDrifting (area, crossProduct, centrePoint);


		}
	}
	void AddWaveDrifting(float area, Vector3 normal, Vector3 centrePoint){
		
		//Drifting from the waves according to:
		//F = 0.5f * rho * g * S * S * n
		//rho - density of the water or whatever medium you have
		//g - gravity
		//s - Surface Area
		//n - normal to the surface
		
		Vector3 F = 1000f * Physics.gravity.y * area * area * normal;
		
		
		//The vertical component of the hydrostatic forces do not cancel out
		//this will cancel out the movoment of the waves, which is good becuase
		//becuase of the waves from another force, such as wave drift, and not bouyancy
		
		F = new Vector3 (F.x, 0f, F.z);
		
		//Should be in worldSpace
		boatRB.AddForceAtPosition (F, centrePoint);
	}

	void AddBuoyancy(float distance_to_surface, float area, Vector3 crossProduct, Vector3 centrePoint){
		//the hydrostatic force df = rho * g * z * ds * n
		//rho - density of the water or whatever medium you have
		//g - gravity
		//z - distance to the surface
		//ds - surface area
		//n - normal to the surface

		Vector3 F = 1000f * Physics.gravity.y * distance_to_surface * area * crossProduct; 

		//The vertical component of the hydrostatic forces do not cancel out
		//this will cancel out the movoment of the waves, which is good becuase
		//becuase of the waves from another force, such as wave drift, and not bouyancy

		F = new Vector3 (0f, F.y, 0f);

		//Should be in worldSpace
		//boatRB.AddForce  (F/10, ForceMode.Acceleration);
		boatRB.AddForceAtPosition (F, centrePoint);

	}

	

	public void AddPropulsion(float thrust){
		//F = 0.5f * rho * g * S * S * n
		//rho - density of the water or whatever medium you have
		//g - gravity
		//s - Surface Area
		//n - normal to the surface
		
		//Vector3 F =  0.5f * 1000f * Physics.gravity.y * area * area * normal * thrust;
		
		//transform.rotation += transform.Rotate(0, 0, rotation);
		//The vertical component of the hydrostatic forces do not cancel out
		//this will cancel out the movoment of the waves, which is good becuase
		//becuase of the waves from another force, such as wave drift, and not bouyancy
		
		//Vector3 F = new Vector3 (0,0, thrust);
		Vector3 v3Force = thrust * -transform.right;
		//Should be in worldSpace
		boatRB.AddForce  (v3Force, ForceMode.Acceleration);
	}
	

}























