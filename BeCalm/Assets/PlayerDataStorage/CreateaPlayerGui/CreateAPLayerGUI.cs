using UnityEngine;
using System.Collections;

public class CreateAPLayerGUI : MonoBehaviour {




	public enum CreateAPlayerStates{
		CLASSSELECTION, //display all class types
		STATALLOCATION, //allocate stats where the player wants too
		FINALSETUP, //Add name and misc items
	}
	private DisplayCreatePlayerFunctions displayFunctions = new DisplayCreatePlayerFunctions();
	public static CreateAPlayerStates currentState;

	// Use this for initialization
	void Start () {

		currentState = CreateAPlayerStates.FINALSETUP;
	}

	

	// Update is called once per frame
	void Update () {

		switch(currentState){

		case(CreateAPlayerStates.CLASSSELECTION):
			break;
		case(CreateAPlayerStates.STATALLOCATION):
			break;
		case(CreateAPlayerStates.FINALSETUP):
			break;
	
	}
	}
	void OnGUI(){

				displayFunctions.DisplayMainItems ();

//				if (currentState == CreateAPlayerStates.CLASSSELECTION) {
//						//Display class selection function
//				displayFunctions.DisplayClassSelections();
//				}
//				if (currentState == CreateAPlayerStates.STATALLOCATION) {
//						//Display class selection function
//				displayFunctions.DisplayStatAllocation();
//				}
				if (currentState == CreateAPlayerStates.FINALSETUP) {
						//Display class selection function
				displayFunctions.DisplayFinalSetup();
				}
		}
}
