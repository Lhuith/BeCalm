using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CreateAPLayerGUI : MonoBehaviour {

	public InputField nameSpace; 

	public enum CreateAPlayerStates{

	}


	// Use this for initialization
	void Start () 
	{
		nameSpace = GetComponentInChildren<InputField>();
		nameSpace.text = "";
	}

	public void ClearText()
	{
		nameSpace.text = "";
	}
	

	public void SaveData()
	{				
					int i = Application.loadedLevel;
					Application.LoadLevel(i + 1);
					GameInformation.PlayerName = nameSpace.text;
//					GameInformation.breathPeaks  = null;
//					GameInformation.breathPerMin  = 0;
//					GameInformation.playTime  = 0;
//					GameInformation.starCount  = 0;
//					GameInformation.topBreathePeak  = 0;
					
					SaveInformation.SaveAllInformation();

	}
	// Update is called once per frame
	void Update () 
	{

//		if (GUI.Button (new Rect (525, 370, 50, 50), "Begin")){
//			int i = Application.loadedLevel;
//			Application.LoadLevel(i + 1);
//			GameInformation.PlayerName = playerFirstName + " " + playerLastName;
//			//GameInformation.PlayerAbilities = GameInformation.PlayerClass.AbilityList;
//			if(genderSelection == 0){
//				GameInformation.IsMale = true;
//			}else if(genderSelection == 1){
//				GameInformation.IsMale = false;
//			}
//			
//			SaveInformation.SaveAllInformation();
//		}
	}
	void OnGUI(){

				
		}
}
