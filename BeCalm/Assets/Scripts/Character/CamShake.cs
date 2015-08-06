using UnityEngine;
using System.Collections;

public class CamShake : MonoBehaviour {

	public bool Shaking; 
	private float ShakeDecay;
	public float ShakeIntensity;   
	public Vector3 OriginalPos, OriginalPosStart;
	public Quaternion OriginalRot, OriginalRotStart;
	public Camera MainCamera;
	
	void Start()
	{
		OriginalPosStart = transform.position;
		OriginalRotStart = transform.rotation;
		MainCamera = Camera.main;
		Shaking = false;   
	}

	
	void Update () 
	{

		if(ShakeIntensity > 0)
		{
			transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
			transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity)*.2f,
			                                    OriginalRot.w + Random.Range(-ShakeIntensity,     ShakeIntensity)*.2f);
			
			ShakeIntensity -= ShakeDecay;
		}
		else if (Shaking)
		{
			Shaking = false;  
		}
		
	}
	
	

	
	public void DoShake()
	{
		OriginalPos = transform.position;
		OriginalRot = transform.rotation;
		
		ShakeIntensity = 0.1f;
		ShakeDecay = 0.02f;
		Shaking = true;
	}   
}
