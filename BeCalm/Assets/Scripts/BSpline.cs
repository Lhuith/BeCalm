using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSpline : MonoBehaviour {

	float[] x = new float[40];
	float[] y = new float[40];
	float[] z = new float[40];

	public float segLength = 16;

	public LineRenderer windLine;
	public List<Vector3> linePositions = new List<Vector3>();

	// Use this for initialization
	void Start () {
		windLine = GetComponent<LineRenderer> ();

		for(int i = 0; i < segLength; i++) {
			linePositions.Add(new Vector3(transform.position.x * i, transform.position.y +  i, transform.position.z +  i));;
		}
		windLine.SetVertexCount (linePositions.Count);

	}
	
	// Update is called once per frame
	void Update () {

		linePositions [0] = transform.position; 
		//linePositions [linePositions.Count] = new Vector3 (linePositions [linePositions.Count - 1].x , linePositions [linePositions.Count - 1].y, linePositions [linePositions.Count - 1].z); 
		for(int i = 0; i < linePositions.Count; i++) {
			windLine.SetPosition(i, linePositions[i]);
			MoveSegment(i + 1, linePositions[i].x, linePositions[i].y, linePositions[i].z);
		}
	}
	void MoveSegment(int i, float xin, float yin, float zin) {
		float dx = xin - x[i];
		float dy = yin - y[i];
		float dz = zin - z[i];


		float angle2 = Mathf.Atan2(dx , dz);

		float angle = Mathf.Atan2(dy, dx);

		x[i] = xin - Mathf.Cos(angle) * segLength;

		y[i] = yin - Mathf.Sin(angle) * segLength;

		z[i] = zin - Mathf.Cos(angle2) * segLength;
		                      

		segment(i ,x[i], y[i],z[i], angle);
		}
		
		void segment(int i, float x, float y, float z, float a) {
		if (i == linePositions.Count) {
			//linePositions [i] = new Vector3 (x - 1, y - 1, z - 1);
		} else if (i == 0) {
			//linePositions [i] = transform.position;

		} else 
		{
			linePositions [i] = new Vector3 (x, y, z);
		}
	}

	void OnDrawGizmos()
	{
		for (int i = 0; i < linePositions.Count; i++) 
		{
			//Gizmos.DrawWireSphere(linePositions[i], 0.3f);
		}
	}

}
