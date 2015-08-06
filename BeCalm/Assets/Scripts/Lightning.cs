using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	private LineRenderer lineRenderer;

	private float maxZ = 20f;

	private int noSegments = 26;

	private Color color = Color.white;

	public float posRange;

	private float radius = 1f;

	private Vector2 midPoint;

	public float duration;

	private bool HitMax;
	// Use this for initialization
	void Start () {

		lineRenderer = GetComponent<LineRenderer> ();

		lineRenderer.SetVertexCount (noSegments);

		for (int i = 1; i < noSegments -1; i++) 
		{
			float z = ((float) i)*(maxZ)/(float)(noSegments -1);

			lineRenderer.SetPosition(i, new Vector3(Random.Range(- posRange, posRange), Random.Range(-posRange, posRange), z));
		}

		lineRenderer.SetPosition (0, new Vector3 (0f, 0f, 0f));
		lineRenderer.SetPosition (noSegments - 1, new Vector3 (0f, 0f, maxZ));
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Increase Alpha then Decrease
		//if (!HitMax) {
			//color.a += duration * Time.deltaTime;

			//if (color.a <= 0.99f) 
			//{
				//HitMax = false;
			//}
		//}

		color.a -= duration * Time.deltaTime;

		lineRenderer.SetColors (color, color);

		if (color.a <= 0.01f) 
		{
			Destroy(this.gameObject);
		}
	}
}
