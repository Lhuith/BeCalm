using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : MonoBehaviour {
	//Has to be least 4 so-called control points
	public GameObject controlpoint;
	public List<Transform> ControlPointList = new List<Transform>();
	public int pointListSize;
	public bool isLooping = true;
	public LineRenderer starline;
	public GameObject player;

	void Start () {
		player = CustomExtensions.GetPlayer ();
		GameObject loadPoint = Resources.Load("Mesh/controlPoint", typeof(GameObject)) as GameObject;
		controlpoint = loadPoint;

		for (int i = 0; i < pointListSize; i++) 
		{
			GameObject controlPointInstance = Instantiate (controlpoint, new Vector3 (transform.position.x + i, transform.position.y + i, transform.position.z + i), transform.rotation) as GameObject;
			controlPointInstance.transform.parent = transform;
			ControlPointList.Add (controlPointInstance.transform);

		}
		ControlPointList[0].transform.position = this.transform.position;
		ControlPointList[ControlPointList.Count - 1].transform.position = player.transform.position;
		starline = GetComponent<LineRenderer> ();
		for(int i=0; i < ControlPointList.Count; i++) 
		{
			starline.SetVertexCount(ControlPointList.Count);
		}

	}
	

	void Update () {
	

		for(int i = 0; i < ControlPointList.Count; i++)
		{
			//Cant draw between the endpoints
			//Neither do we need to draw from the second to the last endpoint
			//...if we are not making a loop line
			if(( i == 0 || i == ControlPointList.Count - 2 || i == ControlPointList.Count - 1) && !isLooping)
			{
				continue;
			}
			DisplayCatmullRomSpline (i);
		}
		for(int i=0; i < ControlPointList.Count; i++) {
			starline.SetPosition(i ,ControlPointList[i].position);
		}

	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		//Draw the Cutmall_Rom Lines betwen the points
		for(int i = 0; i < ControlPointList.Count; i++)
		{
			//Cant draw between the endpoints
			//Neither do we need to draw from the second to the last endpoint
			//...if we are not making a loop line
			if(( i == 0 || i == ControlPointList.Count - 2 || i == ControlPointList.Count - 1) && !isLooping)
			{
				continue;
			}
			//DisplayCatmullRomSpline (i);
		}
		//Draw a sphere at each control point
		for (int i = 0; i < ControlPointList.Count; i++) 
		{
		//	Gizmos.DrawWireSphere(ControlPointList[i].position, 0.3f);
		}


}

	void DisplayCatmullRomSpline (int pos)
	{
		//Clamp to allow looping
		Vector3 p0 = ControlPointList [ClampListPos (pos - 1)].position;
		Vector3 p1 = ControlPointList [pos].position;
		Vector3 p2 = ControlPointList [ClampListPos (pos + 1)].position;
		Vector3 p3 = ControlPointList [ClampListPos (pos + 2)].position;

		//Just assign a tmp value to this
		Vector3 lastPos = Vector3.zero;

		//t is alawys between 0 and 1 and determines the resolution of the spline
		//0 is always at p1

		for (float t = 0; t < 1; t+= 0.1f) 
		{
			//Find the coordinates between the control points with the Catmull-Rom spline
			Vector3 newPos = ReturnCutmullRom(t, p0, p1, p2, p3);

			//Cant display anything the first itration

			if(t == 0)
			{
				lastPos = newPos;
				continue;
			}

			//Gizmos.DrawLine(lastPos, newPos);
			lastPos = newPos;
		}

		//Also draw the last line since it is always less then 1, so we will always miss it
		//Gizmos.DrawLine (lastPos, p2);


	}

	int ClampListPos(int pos)
	{
		if (pos < 0) 
		{
			pos = ControlPointList.Count - 1;
		}

		if (pos > ControlPointList.Count) 
		{
			pos = 1;
		}

		else if (pos > ControlPointList.Count - 1) 
		{
			pos = 0;
		}

		return pos;
	}

	//Returns a position betwen 4 Vector3 with Catmull - Rom Spline alogorithim
	//http://www.iquilezles.org/www/articles/minispline/minispline.htm
	Vector3 ReturnCutmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2 , Vector3 p3)
	{
		Vector3 a = 0.5f * (2f * p1);
		Vector3 b = 0.5f * (p2 - p0);
		Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
		Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);
		
		Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);
		
		return pos;
	}

}
            


