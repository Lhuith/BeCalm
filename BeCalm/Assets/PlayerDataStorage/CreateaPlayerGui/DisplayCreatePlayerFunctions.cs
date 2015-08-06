using UnityEngine;
using System.Collections;

public class DisplayCreatePlayerFunctions {

	private StatAllocationModule statAllocationModule = new StatAllocationModule();

	private int classSelection;
	//private string[] classSelectionNames = new string[] {"Mage" , "Warroir", "Archer"}; 
	private string playerFirstName = "What Name Do You go By?!";
	private string playerLastName = "From What House Do You Hail?";
	private PlayerStats playerStats;
	private bool isMale = true;
	private int genderSelection;
	private string[] genderTypes = new string[2] {"Male", "Female"};


	public void DisplayClassSelections(){
//		//a List of toggle buttons and each button will be a diffrent class
//		//selection grid
//		classSelection = GUI.SelectionGrid (new Rect (50,50,250,100), classSelection, classSelectionNames, 4);
//		GUI.Label (new Rect (450, 50, 300, 300), FindClassDesctription (classSelection));
//		GUI.Label (new Rect (450, 100, 300, 300), FindClassStatValues (classSelection));
		}

//	private string FindClassDesctription(int classSelection){
////				if (classSelection == 0) {
////						BaseCharacterClass tempClass = new  BaseMageClass ();
////						return tempClass.CharacterClassDescription;
////				} else if (classSelection == 1) {
////						BaseCharacterClass tempClass = new  BaseWarriorClass ();
////						return tempClass.CharacterClassDescription;
////				} else if (classSelection == 2) {
////						BaseCharacterClass tempClass = new  BaseArcherClass ();
////						return tempClass.CharacterClassDescription;
////				}
////		return "NO CLASS FOUND";
//		}

//	private string FindClassStatValues(int classSelection){
////		if (classSelection == 0) {
////						BaseCharacterClass tempClass = new  BaseMageClass ();
////						string TempStats = "Stamina " + tempClass.Stamina + "\n" + "Endurance " + tempClass.Endurance + "\n" + "Intellect " + tempClass.Intellect + "\n" + "Strength " + tempClass.Strength + "\n" + "Agility " + tempClass.Agility + "\n" + "Resistance " + tempClass.Resistance;
////						return TempStats;
////				} else if (classSelection == 1) {
////						BaseCharacterClass tempClass = new  BaseWarriorClass ();
////						string TempStats = "Stamina " + tempClass.Stamina + "\n" + "Endurance " + tempClass.Endurance + "\n" + "Intellect " + tempClass.Intellect + "\n" + "Strength " + tempClass.Strength + "\n" + "Agility " + tempClass.Agility + "\n" + "Resistance " + tempClass.Resistance;
////						return TempStats;
////				} else if (classSelection == 2) {
////						BaseCharacterClass tempClass = new  BaseArcherClass ();
////						string TempStats = "Stamina " + tempClass.Stamina + "\n" + "Endurance " + tempClass.Endurance + "\n" + "Intellect " + tempClass.Intellect + "\n" + "Strength " + tempClass.Strength + "\n" + "Agility " + tempClass.Agility + "\n" + "Resistance " + tempClass.Resistance;
////						return TempStats;
////				}
////		return "NO STATS FOUND";
//}

	public void DisplayStatAllocation(){
		//a list of stats with plus and minus buttons to add stats to specific stats
		//logic to make sure the player cannot add more then stats given
		//statAllocationModule.DisplayStatAllocationModule();
	}
	public void DisplayFinalSetup(){
		//What our Hero will go by
		playerFirstName = GUI.TextArea (new Rect (30, 10, 200, 35), playerFirstName, 30);
		playerLastName = GUI.TextArea (new Rect (30, 55, 200, 35), playerLastName, 30);
		genderSelection = GUI.SelectionGrid (new Rect (250, 10, 100, 80), genderSelection, genderTypes, 1);
		}

	private void ChooseClass(int classSelection){

		if (classSelection == 0) {
			}
		}
	public void DisplayMainItems(){

		//Transform player = GameObject.FindGameObjectWithTag("Player").transform;

		GUI.Label(new Rect(Screen.width/2, 20, 100, 100), "CREATE NEW PLAYER");

		if (CreateAPLayerGUI.currentState != CreateAPLayerGUI.CreateAPlayerStates.FINALSETUP){ //if were not in setup then show a next button
			if (GUI.Button (new Rect (470, 370, 50, 50), "NEXT")) {
			if (CreateAPLayerGUI.currentState == CreateAPLayerGUI.CreateAPlayerStates.CLASSSELECTION){
						ChooseClass (classSelection);
						CreateAPLayerGUI.currentState = CreateAPLayerGUI.CreateAPlayerStates.STATALLOCATION;
			}else if (CreateAPLayerGUI.currentState == CreateAPLayerGUI.CreateAPlayerStates.STATALLOCATION){
//					GameInformation.Stamina = statAllocationModule.pointsToAllocate[0];
//					GameInformation.Endurance = statAllocationModule.pointsToAllocate[1];
//					GameInformation.Intellect = statAllocationModule.pointsToAllocate[2];
//					GameInformation.Strength = statAllocationModule.pointsToAllocate[3];
//					GameInformation.Agility = statAllocationModule.pointsToAllocate[4];
//					GameInformation.Resistance = statAllocationModule.pointsToAllocate[5];
					CreateAPLayerGUI.currentState = CreateAPLayerGUI.CreateAPlayerStates.FINALSETUP;
			}

				}
		}else if(CreateAPLayerGUI.currentState == CreateAPLayerGUI.CreateAPlayerStates.FINALSETUP){
			if (GUI.Button (new Rect (525, 370, 50, 50), "Begin")){
				int i = Application.loadedLevel;
				Application.LoadLevel(i + 1);
				GameInformation.PlayerName = playerFirstName + " " + playerLastName;
				//GameInformation.PlayerAbilities = GameInformation.PlayerClass.AbilityList;
				if(genderSelection == 0){
					GameInformation.IsMale = true;
				}else if(genderSelection == 1){
					GameInformation.IsMale = false;
				}

				SaveInformation.SaveAllInformation();
			}
		}
		if (CreateAPLayerGUI.currentState != CreateAPLayerGUI.CreateAPlayerStates.CLASSSELECTION) {
//						if (GUI.Button (new Rect (295, 370, 50, 50), "BACK")) {
//								if (CreateAPLayerGUI.currentState == CreateAPLayerGUI.CreateAPlayerStates.STATALLOCATION) {
//								statAllocationModule.didRunOnce = false;
//								GameInformation.PlayerClass = null;
////										CreateAPLayerGUI.currentState = CreateAPLayerGUI.CreateAPlayerStates.CLASSSELECTION;
//								} else if (CreateAPLayerGUI.currentState == CreateAPLayerGUI.CreateAPlayerStates.FINALSETUP) {
////										CreateAPLayerGUI.currentState = CreateAPLayerGUI.CreateAPlayerStates.STATALLOCATION;
//								}
//						}
				}
			    }
}
