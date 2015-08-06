using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GenerateWaves : MonoBehaviour {
	public bool isNewMesh;
	//The water mesh
	Mesh waterMesh;

	public float refreshRate, time, yCheck, xCheck, zCheck;


	//The new values of the vertices after we applied the wave algorithm are stored here
	private Vector3[] newVertices;
	//The initial values of the vertices are stored here
	private Vector3[] originalVertices;

	//To get the y position
	private WaveController waveScript;
	
	private Material waterMat;

	void Start () {

	//Get the water mesh
		waterMesh = this.GetComponent<MeshFilter> ().mesh;

		waterMat = this.GetComponent<MeshRenderer> ().material;

		originalVertices = waterMesh.vertices;

		//Get the waveScript
		GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");

		waveScript = gameController.GetComponent<WaveController> ();
	}
	
	// Update is called once per frame
	void Update () {
		refreshRate = FeedBackController.noise;


		waterMat.SetVector ("_PlayerPos", transform.position);

	
		{
			MoveSea ();
		
		}
	}

	void MoveSea(){

//		newVertices = new Vector3[originalVertices.Length];
//
//	
//
//		for (int i = 0; i < originalVertices.Length; i++) {
//
//				Vector3 vertice = originalVertices [i];
//
//				//Now we need to modify this coordinate's y-position
//				//From local to global
//				vertice = transform.TransformPoint (vertice);
//
//
//
//			//Vertice Information Changed Behind the Scenes To keep track of the Waves Y Position
//			Vector3 wavePos = vertice;
//			wavePos += waveScript.GetWaveYPos (vertice.x, vertice.z);
//			vertice.y = wavePos.y;
//			//Convert Back (Porbably an easier way but fuck it)
//			//Debug.Log(vertice.x);
//			yCheck = vertice.y;
//
//			newVertices [i] = transform.InverseTransformPoint (vertice);
//
//			//yCheck = vertice.y;
//			zCheck = vertice.z;
//
////
////		}
//////
	//waveScript.SetWaveMat (waterMat, this.gameObject);
////
//		//Add the new position of the water to the water mesh
//		waterMesh.vertices = newVertices;
//		//After modifying the vertices it is often useful to update the normals to reflect the change
//		waterMesh.RecalculateBounds ();
//////		//After modifying the Faces it is often useful to update the normals to reflect the change
//		waterMesh.RecalculateNormals ();
	}
}
