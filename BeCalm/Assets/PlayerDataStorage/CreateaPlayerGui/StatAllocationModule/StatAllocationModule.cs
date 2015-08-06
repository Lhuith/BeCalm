using UnityEngine;
using System.Collections;

public class StatAllocationModule {

	private string[] statNames = new string[6] {"Stamina", "Endurance", "Intellect", "Strength", "Agility", "Resistance"};
	private string[] statDescriptions = new string[6]{"Health Modifier","Energy Modifier","Magical Damage Modifier","Physical Damage Modifier","Haste and Crit Modifier", "All Damage Reduction Modifier"};
	private bool[] statSelections = new bool [6];

	public int[] pointsToAllocate = new int[6]; //starting stat values for the chosen class
	private int[] baseStatPoints = new int[6]; // starting stat values for the choosen class

	private int availPoints = 5;
	public bool didRunOnce = false;


	public void DisplayStatAllocationModule(){

		if (!didRunOnce) {
			RetrieveStatBaseStatPoints ();
			didRunOnce = true;
		}

		DisplayStatToggleSwitches ();
		DisplayStatIncreaseDecreaseButtons ();
		}

	private void DisplayStatToggleSwitches(){

				for (int i = 0; i < statNames.Length; i++) {
						statSelections [i] = GUI.Toggle (new Rect (10, 60 * i + 10, 100, 50), statSelections [i], statNames [i]);
						GUI.Label(new Rect(100,60 * i + 10,50,50), pointsToAllocate[i].ToString()); 
						if(statSelections[i]){
						GUI.Label(new Rect(20,60*i + 30,150,100), statDescriptions[i]);
			}
				}
		}

	private void DisplayStatIncreaseDecreaseButtons(){
		for(int i = 0; i < pointsToAllocate.Length; i++){
			if(pointsToAllocate [i] >= baseStatPoints [i] && availPoints > 0){
			if(GUI.Button (new Rect(200,60 * i + 10,50,50), "+")){
					pointsToAllocate [i] +=1;
					--availPoints;
			}
			}
			if(pointsToAllocate [i] > baseStatPoints [i])
			if(GUI.Button (new Rect(260,60 * i + 10,50,50), "-")){
				pointsToAllocate [i] -= 1;
				++availPoints;
		}
			}
			}

	private void RetrieveStatBaseStatPoints(){
//		BaseCharacterClass cClass = GameInformation.PlayerClass;
//
//		pointsToAllocate [0] = cClass.Stamina;
//		baseStatPoints [0] = cClass.Stamina;
//		pointsToAllocate [1] = cClass.Endurance;
//		baseStatPoints [1] = cClass.Endurance;
//		pointsToAllocate [2] = cClass.Intellect;
//		baseStatPoints[2] = cClass.Intellect;
//		pointsToAllocate [3] = cClass.Strength;
//		baseStatPoints [3] = cClass.Strength;
//		pointsToAllocate [4] = cClass.Agility;
//		baseStatPoints [4] = cClass.Agility;
//		pointsToAllocate [5] = cClass.Resistance;
//		baseStatPoints [5] = cClass.Resistance;
	}

}
