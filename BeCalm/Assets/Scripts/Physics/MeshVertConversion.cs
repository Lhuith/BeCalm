using UnityEngine;
using System.Collections;

public class MeshVertConversion : MonoBehaviour {

	public float waveScale = 0.1f;
	public float waveSpeed = .5f, waveSpeedMax = 2.3f, waveSpeedMin = 1.0f, waveIntensity = 0.3f, waveRefreshRate = 0.3f;
	
	//The width between the wave peaks
	public float waveDistance =1f;

	public int MaxLengh;

	//Noise parameters
	public float noiseStrengh = 1f;
	public float noiseWalk = 1f;
	public MeshRenderer waterMeshMat;
	public float YPos, Zpos;

	// Use this for initialization
	void Start () {
		waterMeshMat = this.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		//waterMeshMat.material.SetFloat("_AnimationPowerY", YPos);
		waterMeshMat.material.color = Color.red;

		Zpos = waterMeshMat.material.GetFloat ("_WaveZPos");

		for (int i = 0; i < MaxLengh; i++) {
			YPos += Mathf.Sin (
			(Time.time * waveSpeed +
				Zpos)
				/ waveDistance) * waveScale;

			waterMeshMat.material.SetFloat ("_WaveYPos", YPos);



		}

	}
}
