using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class WaveController : MonoBehaviour {
	


	//Wave Parametres
	//public float waveScale = 0.1f;
	public float waveSpeedMax, waveSpeedMin, waveSpeedIncreament, waveSpeedwaveDecayRate;
	public float amplitude , amplitudeMax, amplitudeMin, amplitudeDecayRate, amplitudeIncreaseRate;
	public Vector2 sineFrequency;
	public Vector2 waveSpeed;
	public Vector2 steepness;

	public Vector3 waveDirection1, waveDirection2;
	

	public float waveHeightCheckY, waveHeightCheckZ, waveHeightCheckX;


	void Update(){
		//waveScale = FeedBackController.noise/2;

		if (FeedBackController.musicPeak && amplitude < amplitudeMax) {
			amplitude += Mathf.Lerp (amplitudeMin, amplitudeMax, amplitudeIncreaseRate * Time.deltaTime) /10;
		}else
		{
			if (amplitude > amplitudeMin) {
				amplitude -= Mathf.Lerp (amplitudeMin, amplitudeMax, amplitudeDecayRate * Time.deltaTime)/10;
				
			}
		}

		if (FeedBackController.musicPeak && waveSpeed.x < (waveSpeedMax - .2f) ) {
			waveSpeed.x += Mathf.Lerp (waveSpeedMin, waveSpeedMax, waveSpeedIncreament * Time.deltaTime);
		}else
		{
			if (waveSpeed.x > waveSpeedMin) {
				waveSpeed.x -= Mathf.Lerp (waveSpeedMin, waveSpeedMax, waveSpeedwaveDecayRate * Time.deltaTime);

			}
		}




		if (FeedBackController.musicPeak && waveSpeed.y < (waveSpeedMax - .2f)) {
			waveSpeed.y += Mathf.Lerp (waveSpeedMin, waveSpeedMax, waveSpeedIncreament * Time.deltaTime);
		}else
		{
			if (waveSpeed.y > waveSpeedMin) {
				waveSpeed.y -= Mathf.Lerp (waveSpeedMin, waveSpeedMax, waveSpeedwaveDecayRate * Time.deltaTime);
				
			}
		}

	}


	public Vector3 GetWaveYPos(float x_coord, float z_coord)
	{
		float y_coord = 0f;

		//Using only x_coord or z_coord will produce straight waves
		//Using only vertex.y will prodice an up/down movment
		//x_coord + y_coord + z_coord rolling waves sideways
		//x_coord * z_coord produces a moving sea without rolling waves
//
//		y_coord += Mathf.Sin (
//			(Time.time * waveSpeed +
//		 z_coord)
//				/ waveDistance) * waveScale;

		Vector3 vertPos = new Vector3 (x_coord, y_coord, z_coord);

		Vector3 dir = new Vector2 (waveDirection1.x, waveDirection1.y);

		dir = Vector3.Normalize(dir);

		float dotprod = Vector3.Dot(dir, new Vector2 ( vertPos.x,  vertPos.z));
		float disp = waveSpeed.x * Time.time * 100;
		
		//do the same for our second wave
		Vector3 dir2 = new Vector2 (waveDirection2.x, waveDirection2.y);
		dir2 = Vector3.Normalize(dir2);
		float dotprod2 = Vector3.Dot(dir2,  new Vector2 ( vertPos.x, vertPos.z));
		float disp2 =  waveSpeed.y * Time.time / 100;	


		vertPos.z +=  (steepness.x) * ((amplitude * dir.x) * Mathf.Cos(sineFrequency.x * (dotprod + disp)));
		vertPos.x +=  (steepness.x) * ((amplitude * dir.y) * Mathf.Cos(sineFrequency.x * (dotprod + disp)));
		vertPos.y +=  amplitude  *  -Mathf.Sin(sineFrequency.x *  (dotprod + disp)); //+ ((tex.rgb * tex2.rgb) * _Amount);

		vertPos.z += steepness.y * amplitude * dir2.x * Mathf.Cos(sineFrequency.y * dotprod2 + disp2);
		vertPos.x += steepness.y * amplitude * dir2.y *  Mathf.Cos(sineFrequency.y * dotprod2 + disp2);
		vertPos.y += amplitude * Mathf.Sin(sineFrequency.y * dotprod2 + disp2);

		//Debug.Log (steepness.x);
		return vertPos; //y_coord;
	}

	public void SetWaveMat(Material mat, GameObject gamin)
	{
		float meshWidth =  gamin.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
		//float meshWidth = gamin.transform.lossyScale.x * gamin.transform.lossyScale.x;
		mat.SetFloat ("_TimeCostum", Time.time);
		mat.SetVector ("_ObjectScale", gamin.transform.lossyScale);
		mat.SetFloat ("_SineAmplitude", amplitude);
		mat.SetVector ("_SineFrequency", new Vector4 (sineFrequency.x, sineFrequency.y, 0, 0));
		mat.SetVector ("_Speed", new Vector4 (waveSpeed.x, waveSpeed.y, 0, 0));
		mat.SetVector ("_Steepness", new Vector4 (steepness.x, steepness.y, 0, 0));
		mat.SetVector ("_Dir", new Vector4 (waveDirection1.x, waveDirection1.y, waveDirection1.z, 0));
		mat.SetVector ("_Dir2", new Vector4 (waveDirection2.x, waveDirection2.y, waveDirection2.z, 0));

	}
}
