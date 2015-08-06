using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudSystem : MonoBehaviour {
		
		public GameObject cloud, player;
		public float systemHeight, systemWidth, systemAlt, cloudSpacing;
		public WaterV3 waterSize;
		
		public AudioClip LightningFoley;
		
		public float maxManagerParticles;

		public List<Vector3> cloudVectorList = new List<Vector3> ();
		
		public List<GameObject> cloudGameObjectList = new List<GameObject>();

		public WaterSpace waterMeshGridSize;

		public float viewDistance;

		public Vector3 cloudVecPos;
		
		public float watertileDis;

		public CloudController cloudController;
		
		public float particleCount;

		public float diceRoll;	

	public ParticleSystem cloudParticleSystem;
	ParticleSystem.Particle[] cloudParticles;
	
	public GameObject lightningObject;
	
	public Color CloudColor;
	public Color CloudHighlight, CloudStormyColor, cloadBaseColor;
	public float offset = 0.2f;
	public float particleHeight;
	public float CloudAlt;
	public float CloudHeight =60f;
	public float MaxParticles;
	float[] x = new float[2000];
	float[] y = new float[2000];
	float[] z = new float[2000];
	
	float segLength = 24;

		//private Mesh OriginalWater;
		// Use this for initialization
		void Awake () 
		{
			

			
			
		}
		
		void Start(){
			
		AudioClip lightningSoundLoad = Resources.Load("Foley/Lightning", typeof(AudioClip)) as AudioClip;
			LightningFoley = lightningSoundLoad;

			GameObject lightningLoad = 	Resources.Load("Mesh/Lightning", typeof(GameObject)) as GameObject;
			lightningObject = lightningLoad;

			GameObject cloudLoad =  Resources.Load("Mesh/Cloud", typeof(GameObject)) as GameObject;
			cloud = cloudLoad;
			cloudController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CloudController> ();
			player = CustomExtensions.GetPlayer();
			
			CreateClouds();
			Debug.Log(cloudVectorList.Count);
			Debug.Log(particleCount);
		cloadBaseColor = CloudColor;
		}
		
		// Update is called once per frame
		void Update () 
		{
			maxManagerParticles = cloudController.cloudDensitiy;
			CloudController ();
			TileCheck ();
			
			
		}
		
		void TileCheck()
		{
		for (int i = 0; i < cloudGameObjectList.Count; i++) {
			float cloudPoint = CustomExtensions.GetDistance(cloudGameObjectList[i], player);
			//float vecDistance = Vector3.Distance(cloudVectorList[i], player.transform.position);  
				
			if (cloudPoint < viewDistance) {
					
					Debug.DrawLine (cloudGameObjectList[i].transform.position, player.transform.position, Color.yellow);
					//Debug.DrawLine (cloudGameObjectList[i].transform.position, player.transform.position, Color.grey);
					cloudGameObjectList [i].TurnOnGameObject();
					cloudGameObjectList [i].GetComponent<ParticleSystem>().Play();

				} else {
					cloudGameObjectList [i].TurnOffGameObject();
					
				}
			}
		}
		


		void CloudController()
	{
	
		for (int i = 0; i < cloudGameObjectList.Count; i ++) 
		{
			if(cloudGameObjectList[i].activeInHierarchy == true){
				//cloudGameObjectList[i].GetComponentInChildren<ParticleSystem>().enableEmission = false;
				cloudGameObjectList [i].GetComponent<ParticleSystem>().maxParticles = (int)cloudController.cloudDensitiy;
				cloudGameObjectList [i].GetComponent<ParticleSystem>().emissionRate  = cloudController.cloudEmisssive / 25;
				cloudGameObjectList [i].GetComponent<ParticleSystem>().startSpeed  = cloudController.cloudSpeed / 25;
				cloudGameObjectList [i].GetComponent<Cloud_Script> ().MaxParticles = maxManagerParticles;
				VolometricClouds(cloudGameObjectList [i]);
			}
		}
	}

	void VolometricClouds(GameObject cloud){

		ParticleSystem cloudParticleSystem =  cloud.GetComponent<ParticleSystem> ();
		ParticleSystem.Particle[] cloudParticles = new ParticleSystem.Particle[cloudParticleSystem.maxParticles]; 
		//Debug.Log(GetComponent<ParticleEmitter>().particles.Length);
		CloudAlt = transform.position.y - 30;
		
		//var particles = GetComponent<ParticleSystem.Particle[]> ();
		
		//particles [0].position = transform.position;
		
		int numParticlesAlive =  cloudParticleSystem.GetParticles(cloudParticles);

		float cloudPercent = (((float)numParticlesAlive * 10)/maxManagerParticles) * 100;
		float cloudLerp = cloudPercent / 100;


		//Debug.Log (cloudPercent/100);

		if (cloudPercent > 60) 
		{
			cloud.GetComponentsInChildren<ParticleSystem>()[1].enableEmission = true;;
		}else
		{
			//cloud.GetComponentInChildren<ParticleSystem>().Stop();
			cloud.GetComponentsInChildren<ParticleSystem>()[1].enableEmission = false;;
		}

		for( int i = 0; i < numParticlesAlive; i++){


			//cloudParticles [i].color = Color.Lerp (CloudColor, CloudStormyColor, FeedBackController.calm);
			particleHeight = (((cloudParticles [i].position.y - CloudAlt) / CloudHeight) - offset);

			if(cloudLerp < 0.9f){
			CloudColor = Color.Lerp (cloadBaseColor, CloudStormyColor, cloudLerp);
			}
			cloudParticles [i].color = Color.Lerp (CloudHighlight, CloudColor, particleHeight);

			//MoveSegment (i + 1, particles [i].position.x, particles [i].position.y, particles [i].position.z);

			int diceRollx = Random.Range (1, 100);

			if (cloudPercent > 80 && diceRollx > 99) 
				{
				cloudParticles [i].color = Color.white;
				Vector3 particlePos = new Vector3 (cloudParticles [i].position.x, cloudParticles [i].position.y, cloudParticles [i].position.z);
				CreateLighnting (particlePos);
				}

			cloudParticleSystem.SetParticles(cloudParticles, numParticlesAlive);
			
		}



	}

		void OnApplicationQuit() 
		{
		for (int i = 0; i < cloudVectorList.Count; i++) 
			{
				//GameObject.Destroy(WaterTileList[i]);
			}
			
			cloudVectorList.Clear();
		}
		
		void CreateClouds()
		{
			
			for (int x = 0; x < systemHeight; x ++) 
			{
				for (int z = 0; z < systemWidth; z ++)
				{
					systemAlt = Random.Range(100, 100);
					Vector3 randomDirection = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (0, 2f), Random.Range (-1.0f, 1.0f));
					Vector3 TempVec = new Vector3(this.transform.position.x + (x * cloudSpacing * randomDirection.normalized.x), this.transform.position.y + (systemAlt), this.transform.position.z + (z * cloudSpacing * randomDirection.normalized.z));
					cloudVecPos = TempVec;
					GenerateCloud(TempVec);
					particleCount += maxManagerParticles;
					cloudVectorList.Add(cloudVecPos);
				}
			}
		}

	void CreateLighnting(Vector3 pos)
	{
		Camera.main.GetComponent<AudioSource> ().PlayOneShot (LightningFoley, 1);
		GameObject lightningInst = Instantiate (lightningObject, new Vector3 (pos.x, pos.y, pos.z), lightningObject.transform.rotation) as GameObject;
		lightningInst.transform.parent = transform;
	}
		
		public void GenerateCloud(Vector3 cloudvector)
		{
				
				//Vector3 randomDirection = new Vector3 ( cloudvector.x + Random.Range (-1.0f, 1.0f), cloudvector.y +  Random.Range (-1, 1f),  cloudvector.z +  Random.Range (-1.0f, 1.0f));
				GameObject cloudInt = Instantiate (cloud, cloudvector, cloud.transform.rotation) as GameObject;
				cloudInt.GetComponent<Cloud_Script> ().MaxParticles = maxManagerParticles;
				cloudInt.transform.parent = transform;
				//watertile.GetComponent<WaveDisplacement> ().sourceimage.Apply();
				//waterMeshGridSize.viewSpaceDistance = viewDistance;
				cloudGameObjectList.Add(cloudInt);
			
		}
	}
