using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UserTracking : MonoBehaviour {

	public SpectrumAnalyzer specany;
	public List<float> sumCount = new List<float>();
	public List<float> sumTopCount = new List<float>();
	public int listMax;
	public float perMin;
	// Use this for initialization
	void Start () {
		specany = GetComponent<SpectrumAnalyzer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (sumCount.Count < listMax) 
		{
				sumCount.Add (specany.musicSum);
		} else 
		{
			for (int i = 0; i < sumCount.Count; i ++) 
			{
				if(i != sumCount.Count)
				{
				sumCount[i] = sumCount[i + 1];
				}
				else if(i == sumCount.Count)
				{
					sumCount[i] = 0;
				}
				else if(i == 0)
				{
					sumCount[i] = specany.musicSum;
				}
			}
		}


		sumCount.Sort ();

//		if (sumTopCount.Count < listMax) 
//		{
//			sumTopCount.Add (sumCount[0]);
//		} 
//
//		else 
//		{
//			for (int i = 0; i < listMax; i ++) 
//			{
//				if(i != sumTopCount.Count)
//				{
//
//					sumTopCount[i] = sumTopCount[i + 1];
//				}else if(i == sumTopCount.Count)
//				{
//					sumTopCount[i] = 0;
//				}
//				else if(i == 0)
//				{
//				{
//						sumTopCount[i] = sumCount[0];
//				}
//			}
//		}


		GameInformation.playTime = Time.time;
		GameInformation.breathPeaks = sumCount;
		//GameInformation.breathPerMin = sumCount / 60;
		GameInformation.topBreathePeak = sumCount [0];
//		if (FeedBackController.musicPeak) 
//		{
//
//		}
	}
	
	void OnApplicationQuit()
	{
		SaveInformation.SaveAllInformation();
	}

	//Store Data
}
