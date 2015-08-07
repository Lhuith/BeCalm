using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour {
	public float  cloudDensitiy, Noise, cloudDensitiyMin, cloudDensitiyMax, cloudIntensity, cloudDropRate;
	public float cloudEmissMin, cloudEmissMax, cloudEmissIntensity, cloudEmissDropRate, cloudEmisssive;
	public float cloudSpeedMin, cloudSpeedMax, cloudSpeedIntesity, cloudSpeedDropRate, cloudSpeed;
	public bool cloudStorming;
	public GameObject cloudSystem;
	public ParticleSystem Clouds, Mist;
	// Use this for initialization
	void Start () {
		//cloudDensitiyMin = 0;
		//cloudDensitiyMax = 1000;
	}
	
	// Update is called once per frame
	void Update () {
		//cloudDensitiy = Clouds.particleCount;



		cloudDensitiy = Mathf.Lerp (cloudDensitiyMin, cloudDensitiyMax, FeedBackController.calm);
		//cloudEmisssive = Mathf.Lerp (cloudEmissMin, cloudDensitiyMax, FeedBackController.calm);

			if (FeedBackController.musicPeak && cloudDensitiy < cloudDensitiyMax) {
				//cloudDensitiy += Mathf.Lerp (cloudDensitiyMin, cloudDensitiyMax, cloudIntensity * Time.deltaTime);
			} else {

				if (cloudDensitiy > cloudDensitiyMin) {
					//cloudDensitiy -= Mathf.Lerp (cloudDensitiyMin, cloudDensitiyMax, cloudDropRate * Time.deltaTime);
				}
			}
			//
			if (FeedBackController.musicPeak && cloudEmisssive < cloudEmissMax) {
				//cloudEmisssive += Mathf.Lerp (cloudEmissMin, cloudEmissMax, cloudEmissIntensity * Time.deltaTime);
			}else{

				if (cloudEmisssive > cloudDensitiyMin) {
					//cloudEmisssive -= Mathf.Lerp (cloudEmissMin, cloudEmissMax, cloudEmissDropRate * Time.deltaTime);
				}
			}

			if (cloudSpeed < cloudSpeedMax) {
				cloudSpeed += Mathf.Lerp (cloudSpeedMin, cloudSpeedMax, cloudSpeedIntesity * Time.deltaTime);
			} else {
			
				if (cloudSpeed > cloudSpeedMin) {
					cloudSpeed += Mathf.Lerp (cloudSpeedMin, cloudSpeedMax, cloudSpeedIntesity * Time.deltaTime);
				}

				//Clouds.startColor -= Color.Lerp (white, black, cloudIntensity);
			}
		}
	}

