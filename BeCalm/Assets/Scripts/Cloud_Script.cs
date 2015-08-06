using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cloud_Script : MonoBehaviour {

	public ParticleSystem cloudParticleSystem;
	ParticleSystem.Particle[] cloudParticles;

	public GameObject lightningObject;

	public Color CloudColor;
	public Color CloudHighlight, CloudStormyColor;
	public float offset = 0.2f;
	public float particleHeight;
	public float CloudAlt;
	public float CloudHeight =60f;
	public float MaxParticles;
	float[] x = new float[2000];
	float[] y = new float[2000];
	float[] z = new float[2000];

	float segLength = 24;

	// Use this for initialization
	void Awake () {

		cloudParticleSystem =  GetComponent<ParticleSystem> ();
		cloudParticles = new ParticleSystem.Particle[cloudParticleSystem.maxParticles]; 
		//if (GetComponent<ParticleEmitter> ().particles.Length < MaxParticles) {
			//GetComponent<ParticleEmitter> ().emit = true;
			//GetComponent<ParticleEmitter> ().Emit (1);
	//	} else {
			//GetComponent<ParticleEmitter> ().emit = false;
			//GetComponent<ParticleEmitter> ().Emit (0);
	//	}


		//GetComponent<ParticleEmitter>().Simulate(0.05f);

		GameObject lightningLoad = 	Resources.Load("Mesh/Lightning", typeof(GameObject)) as GameObject;
		lightningObject = lightningLoad;
		//VolometricClouds();
	}
	
	// Update is called once per frame
	void Update () {
		VolometricClouds();
		//Vector3 transformPos = new Vector3  (transform.position.x, transform.position.y + (float)GetComponent<ParticleEmitter> ().particles.Length/500, transform.position.z);
		if (particleHeight > -1) {
			//transform.position = transformPos;

		}

	}

	void MoveSegment(int i, float xin, float yin, float zin) {
		float dx = xin - x[i];
		float dy = yin - y[i];
		//float dz = zin - z[i];
		
		//float angle2 = Mathf.Atan2(dx, dz);
		//float angle3 = Mathf.Atan2(dy, dz);
		
		//float angle4 = Mathf.Atan2(angle2, angle3);
		
		float angle = Mathf.Atan2(dy, dx);
		
		x[i] = xin - Mathf.Cos(angle) * segLength;
		
		y[i] = yin - Mathf.Sin(angle) * segLength;
		
		//z[i] = zin - Mathf.Cos(angle2) * segLength;
		
		//z[i] = zin - Mathf.Sin(angle3) * segLength;
		
		
		segment(i ,x[i], y[i],z[i], angle);
	}

	void VolometricClouds(){
		//Debug.Log(GetComponent<ParticleEmitter>().particles.Length);
		CloudAlt = transform.position.y - 30;

		//var particles = GetComponent<ParticleSystem.Particle[]> ();

		//particles [0].position = transform.position;

		int numParticlesAlive =  cloudParticleSystem.GetParticles(cloudParticles);

		for( int i = 0; i < numParticlesAlive; i++){

		//for (int i =0; i < particles.Length; i++) {
			//cloudParticles [i].color = Color.Lerp (CloudColor, CloudStormyColor, FeedBackController.calm);
			particleHeight = (((cloudParticles [i].position.y - CloudAlt) / CloudHeight) - offset);

			//cloudParticles [i].color = Color.Lerp (CloudHighlight, CloudColor, particleHeight);
			if(i > cloudParticles.Length){
			//cloudParticles [i].color = Color.Lerp (CloudHighlight, CloudColor, particleHeight);
			}
			//MoveSegment (i + 1, particles [i].position.x, particles [i].position.y, particles [i].position.z);
			//GetComponent<ParticleEmitter> ().particles [i].position = transform.position;

			int diceRoll = Random.Range (1, 1000);
			
			if (particleHeight <= -1 && diceRoll > 997) {
				
				//cloudParticles [i].color = Color.white;
				//Vector3 particlePos = new Vector3 (cloudParticles [i].position.x, cloudParticles [i].position.y, cloudParticles [i].position.z);
				//CreateLighnting (particlePos);
				
			}
				//cloudParticle = particles;
			cloudParticleSystem.SetParticles(cloudParticles, numParticlesAlive);
		//}
		
		
		}
	}
	void segment(int i, float x, float y, float z, float a) {
		if (i == GetComponent<ParticleEmitter>().particles.Length) {
			//linePositions [i] = new Vector3 (x - 1, y - 1, z - 1);
		} else if (i == 0) {
			//linePositions [i] = transform.position;
			
		} else 
		{
			Vector3 movPos = new Vector3 (x, y , z);
			//GetComponent<ParticleEmitter>().particles[i].position = movPos;
			//GetComponent<ParticleEmitter>().Simulate(0.05f);
		}
	}
	void LateUpdate () {
}

	void CreateLighnting(Vector3 pos)
	{
		GameObject lightningInst = Instantiate (lightningObject, new Vector3 (pos.x, pos.y, pos.z), lightningObject.transform.rotation) as GameObject;
		//lightningInst.transform.parent = transform;
	}
}
