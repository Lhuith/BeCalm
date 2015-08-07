using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SpectrumAnalyzer : MonoBehaviour {

	public MusicLoad musicListLoad;
	public AudioSource[] AudPlayer;

	public AudioClip[] songsCalm, songsNotCalm;
	public AudioClip CurrentCalmSong, CurrentNotCalmSong;

	public GameObject[] cubes;
	public Vector3 cubeScale, sphereScale;
	public MeshRenderer meshColor;
	public float freqBoost, cellfreqBoost;

	public CamShake shakeCam;

	public int counter;
	public bool isPause, isPlay, musicMode;

	public float green, red, blue, tempEstregh;

	public float musicSum;
	private float windPower0, windPower1, windPower2, windPower3, windPower4, windPower5, windPower6, windPower7, windPower8, windPower9, windPower10,
	windPower11, windPower12, windPower13, windPower14, windPower15, windPower16, windPower17, windPower18, windPower19, windPower20, windPower21, windPower22,
	windPower23, windPower24, windPower25, windPower26, windPower27, windPower28;


	void Start () {
		Application.runInBackground = true;



		cubes = GameObject.FindGameObjectsWithTag ("Cubes");
		AudPlayer = Camera.main.GetComponents<AudioSource> ();

		counter = 0;
		//Loading Music from Music Loading Script
		if (musicMode) {
			musicListLoad = Camera.main.GetComponent<MusicLoad> ();
			songsCalm = musicListLoad.Calmsongslist;
			songsNotCalm = musicListLoad.NotCalmsongslist;

			NextCalmSong ();

			CurrentCalmSong = songsCalm[0];
			CurrentNotCalmSong = songsNotCalm[0];

			NextNotCalmSong ();
		}
	}

	public void togglemusic()
	{
		//musicMode = !musicMode;
	}
	// Update is called once per frame
	void Update () {
		musicSum = (windPower0 + windPower1 + windPower2 + windPower3 + windPower4 + windPower5 + windPower6 + windPower7 + windPower8 + windPower9 + windPower10 +
		            windPower11 + windPower12 + windPower13 + windPower14 + windPower15 + windPower16 + windPower17 + windPower18 + windPower19 + windPower20 + windPower21 + windPower22 +
		            windPower23 + windPower24 + windPower25 + windPower26 + windPower27 + windPower28)/29;
	
			
		if (!AudPlayer[0].isPlaying) {
			if(isPlay){
			NextSongCalmSong();
			}
		}

		if (!AudPlayer[1].isPlaying) {
			if(isPlay){
				NextSongNotCalmSong();
			}
		}
			
		cubeScale = new Vector3 (1,1,1);
		//float[] spectrum = AudioListener.GetOutputData(1024, 0);

		//cubes = GameObject.FindGameObjectsWithTag ("Cubes");
		float[] spectrum = AudioListener.GetSpectrumData (1024, 0, FFTWindow.Hamming);


		//Cubes
		float c0 = spectrum[1] + spectrum[2];
		float c022 = spectrum[2] + spectrum[3];
		float c025 = spectrum[3] + spectrum[4];
		float c1 = spectrum[4] + spectrum[5];
		float c2 = spectrum[4] + spectrum[5] + spectrum[6];
		float c22 = spectrum[6] + spectrum[7]+ spectrum[8];
		float c25 = spectrum[7] + spectrum[8] + spectrum[9];
		float c3 = spectrum[9] + spectrum[10] + spectrum[11];
		float c32 = spectrum[11] + spectrum[12] + spectrum[13];
		float c35 = spectrum[13] + spectrum[14] + spectrum[15];
		float c4 = spectrum[15] + spectrum[16] + spectrum[17];
		float c42 = spectrum [17] + spectrum [18] + spectrum [19];
		float c45 = spectrum[19] + spectrum[20] + spectrum[21];
		float c47 = spectrum[21] + spectrum[22] + spectrum[23];
		float c5 = spectrum[23] + spectrum[24] + spectrum[25];
		float c55 = spectrum [25] + spectrum [26] + spectrum [27];
		float c6 = spectrum [27] + spectrum [28] + spectrum [29];
		float c62 = spectrum [29] + spectrum [30] + spectrum [31];
		float c65 = spectrum [31] + spectrum [32] + spectrum [33];
		float c67 = spectrum [33] + spectrum [34] + spectrum [35];
		float c7 = spectrum [35] + spectrum [36] + spectrum [37];
		float c72 = spectrum [37] + spectrum [38] + spectrum [39];
		float c75 = spectrum [39] + spectrum [41] + spectrum [41];
		float c77 = spectrum [41] + spectrum [42] + spectrum [43];
		float c8 = spectrum [43] + spectrum [44] + spectrum [45];
		float c82 = spectrum [45] + spectrum [46] + spectrum [47];
		float c85 = spectrum [47] + spectrum [48] + spectrum [49];
		float c87 = spectrum [49] + spectrum [50] + spectrum [51];
		float c9 = spectrum [51] + spectrum [52] + spectrum [53];
		float c92 = spectrum [53] + spectrum [54] + spectrum [55];


		for (int i = 0; i < cubes.Length; i++) {
			switch (cubes [i].name) {
			case "C0":
				cubeScale.y = c0 * freqBoost;
				windPower0 = c0;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c0 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C022":
				cubeScale.y = c022 * freqBoost;
				windPower1 = c022;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c022 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C025":
				cubeScale.y = c025 * freqBoost;
				windPower2 = c025;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c025 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C1":
				cubeScale.y = c1 * freqBoost;
				windPower3 = c1;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c1 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C2":
				cubeScale.y = c2 * freqBoost;
				windPower4 = c2;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c2 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C22":
				cubeScale.y = c22 * freqBoost;
				windPower5 = c22;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c22 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C25":
				cubeScale.y = c25 * freqBoost;
				windPower6 = c25;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c25 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C3":
				cubeScale.y = c3 * freqBoost;
				windPower7 = c3;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c3 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C32":
				cubeScale.y = c32 * freqBoost;
				windPower8 = c32;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c32 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C35":
				cubeScale.y = c35 * freqBoost;
				windPower9 = c25;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c35 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C4":
				cubeScale.y = c4 * freqBoost;
				windPower10 = c4;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c4 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C42":
				cubeScale.y = c42 * freqBoost;
				windPower11 = c42;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c42 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C45":
				cubeScale.y = c45 * freqBoost;
				windPower11 = c45;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c45 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C47":
				cubeScale.y = c47 * freqBoost;
				windPower12 = c47;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c47 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C5":
				cubeScale.y = c5 * freqBoost;
				windPower13 = c5;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c5 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C55":
				cubeScale.y = c55 * freqBoost;
				windPower14 = c55;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c55 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C6":
				cubeScale.y = c6 * freqBoost;
				windPower15= c6;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c6 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C62":
				cubeScale.y = c62 * freqBoost;
				windPower16 = c62;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c62 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C65":
				cubeScale.y = c65 * freqBoost;
				windPower17 = c65;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c65 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C67":
				cubeScale.y = c67 * freqBoost;
				windPower18 = c67;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c67 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C7":
				cubeScale.y = c7 * freqBoost;
				windPower19 = c7;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c7 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C72":
				cubeScale.y = c72 * freqBoost;
				windPower20 = c72;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c72 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C75":
				cubeScale.y = c75 * freqBoost;
				windPower21 = c75;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c75 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C77":
				cubeScale.y = c77 * freqBoost;
				windPower22 = c77;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c77 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C8":
				cubeScale.y = c8 * freqBoost;
				windPower23 = c8;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c8 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C82":
				cubeScale.y = c82 * freqBoost;
				windPower24 = c82;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c82 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C85":
				cubeScale.y = c85 * freqBoost;
				windPower25 = c85;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c85 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C87":
				cubeScale.y = c87 * freqBoost;
				windPower26 = c87;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c87 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C9":
				cubeScale.y = c9 * freqBoost;
				windPower27 = c9;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c9 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;
			case "C92":
				cubeScale.y = c92 * freqBoost;
				windPower28 = c92;
				cubes [i].transform.localScale = cubeScale;
				GetVolumeColor (c92 * 10, cubes [i].transform.GetComponent<MeshRenderer> ());
				break;

				
			}
		}


		//Debug.Log("LOW: " + spectrum[0]);
		/* c1 = 64hz
		 * c3 = 256hz
		 * c4 = 512hz
		 * c5 = 1024hz
		 * 
		 * 
*/
	}

	public void NextSongCalmSong(){
		AudPlayer[0].clip = CurrentCalmSong;
		AudPlayer[0].Play ();
		isPlay = true;
	}

	public void NextSongNotCalmSong(){
		AudPlayer[1].clip = CurrentNotCalmSong;
		AudPlayer[1].Play ();
		isPlay = true;
	}

	public void NextCalmSong(){
		if (counter < songsCalm.Length) {
			counter ++;
			CurrentCalmSong = songsCalm [counter];
			AudPlayer[0].clip = CurrentCalmSong;

			NextSongCalmSong ();
		}
	}

	public void NextNotCalmSong(){
		if (counter < songsNotCalm.Length) {
			counter ++;
			CurrentNotCalmSong = songsNotCalm [counter];
			AudPlayer[1].clip = CurrentNotCalmSong;
			NextSongNotCalmSong ();
		}
	}

	public void PrevNotCalmSong(){
		if (counter > 0) {
			counter --;
			CurrentNotCalmSong = songsNotCalm [counter];
			AudPlayer[1].clip = CurrentNotCalmSong;
			NextSongNotCalmSong ();
		}
	}

	public void PauseCalmSong(){
		AudPlayer[0].Pause ();
		isPlay = false;
	}

	public void PauseNotCalmSong(){
		AudPlayer[1].Pause ();
		isPlay = false;
	}

	void GetVolumeColor (float volume, MeshRenderer meshcolor) {

		Color tempCol = new Color (meshcolor.material.color.r, meshcolor.material.color.g, meshcolor.material.color.b, tempEstregh);

		if (volume > 4f) {
			tempEstregh = 3f;
			meshcolor.material.color = Color.red;
			//meshcolor.material.SetColor("_EmissionColor", new Color(.3f, 0, 0, 1.0f));
			meshcolor.material.SetColor ("_EmissionColor", tempCol);
			//meshcolor.material.SetColor("_EmissionColor", new Color(.1f, 0, 0, 1.0f));
			//meshcolor.material.SetFloat("_EmissionScaleUI", .9f);

			//DynamicGI.SetEmissive(meshcolor, new Color(3f, 3.1f, 1.5f, 1.0f) * 100);
		} else if (volume > 2f) {
			tempEstregh = .2f;
			//DynamicGI.SetEmissive(meshcolor, new Color(2f, 2.1f, 1.5f, 1.0f) * 100);
			meshcolor.material.color = Color.yellow;
			//meshcolor.material.SetColor("_EmissionColor", new Color(0, .3f, .3f, 1.0f));
			meshcolor.material.SetColor ("_EmissionColor", tempCol);
			//meshcolor.material.SetColor("_EmissionColor", new Color(0, .1f, .1f, 1.0f));
			//meshcolor.material.SetFloat("_EmissionScaleUI", .6f);

		} else if (volume > .1f) {
			tempEstregh = .1f;
			//DynamicGI.SetEmissive(meshcolor, new Color(1f, 1.1f, 1.5f, 1.0f) * 100);
			meshcolor.material.color = Color.green;
			meshcolor.enabled = true;
			meshcolor.material.SetColor ("_EmissionColor", tempCol);
			meshcolor.material.color = Color.green;
			//meshcolor.material.SetColor("_EmissionColor", new Color(0, .1f, 0, 1.0f));
			//meshcolor.material.SetFloat("_EmissionScaleUI", .3f);
		} else {
			meshcolor.enabled = true;
			tempEstregh = 0f;
			meshcolor.enabled = false;
		}
	}

	void MeshChange(float volume, MeshRenderer meshcolor){
		if (volume > 5f) {
		
		} else if (volume > 2f) {

		} else if (volume <= 0f) {
		}
		else if (volume > .1f) {
			//meshcolor.enabled = true;
		}else {
			//meshcolor.enabled = false;
		}
	}
}
