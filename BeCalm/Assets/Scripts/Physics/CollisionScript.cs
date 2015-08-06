using UnityEngine;
using System.Collections;

public class CollisionScript : MonoBehaviour {
	private int waveNumber;
	public float distanceX, distanceZ;
	public float[] waveAmplitude;
	public float magnitudeDivider;
	public Mesh mesh;
	public Material meshMat;
	public Renderer renderer;
	public Vector2[] impactPos;
	public float[] distance;
	public float speedWaveSpread;

	// Use this for initialization
	void Awake () {
		mesh = GetComponent<MeshFilter> ().mesh;
		meshMat = GetComponent<MeshRenderer> ().material;
		renderer = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < 8; i++) 
		{
			waveAmplitude[i] = 	renderer.material.GetFloat("_WaveAmplitude" + (i+1));

			if(waveAmplitude[i] > 0)
			{
				distance[i] += speedWaveSpread;
				renderer.material.SetFloat("_Distance" + (i+1), distance[i]);
				renderer.material.SetFloat("_WaveAmplitude" + (i+1), waveAmplitude[i] * 0.98f);
			}
			if(waveAmplitude[i] < 0.01f)
			{
				renderer.material.SetFloat("_WaveAmplitude" + (i+1), 0);
				distance[i] = 0;
			}
		} 
	}

	void OnTriggerStay(Collider col){
		if (col.attachedRigidbody) 
		{
			waveNumber ++;
			if(waveNumber == 9)
			{
				waveNumber = 1;
			}
			waveAmplitude[waveNumber-1] = 0;
			distance[waveNumber -1] = 0;


			distanceX  = transform.position.x - col.gameObject.transform.position.x;
			distanceZ  = transform.position.z - col.gameObject.transform.position.z;

			impactPos[waveNumber -1].x = col.transform.position.x;
			impactPos[waveNumber -1].y = col.transform.position.z;

			renderer.material.SetFloat("_xImpact" + waveNumber, col.transform.position.x);
			renderer.material.SetFloat("_zImpact" + waveNumber, col.transform.position.z);

			renderer.material.SetFloat("_OffsetX" + waveNumber, distanceX / mesh.bounds.size.x/ 2.5f);
			renderer.material.SetFloat("_OffsetZ" + waveNumber, distanceZ / mesh.bounds.size.z / 2.5f);

			renderer.material.SetFloat("_WaveAmplitude" + waveNumber, -col.attachedRigidbody.velocity.z * magnitudeDivider);
		}
}
}
