using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	public int playerHealth;
	public int playerMagicPower, playerPhysicalPower, playerRangePower, playerResistance, playerDefence;
	public Sprite playerSprite;
	private SpriteRenderer myRenderer;
	public float playerDodgeChance;
//	public BaseCharacterClass playerClass;

	// Use this for initialization
	void Awake(){
				LoadInformation.LoadAllInformation ();
		}
	void Start () {
		playerSprite = GameInformation.PlayerSprite;
		myRenderer = gameObject.GetComponent<SpriteRenderer>();

//		Debug.Log (GameInformation.PlayerClass.CharacterClassName);
		Debug.Log ("Player Name : " + GameInformation.PlayerName);
		Debug.Log ("Player Level : " + GameInformation.PlayerLevel);

		Debug.Log ("Player Endurance : " + GameInformation.Endurance);
		//Magical Attacks
		Debug.Log ("Player Magic Power : " + GameInformation.Intellect);
		//Magical Attacks
		Debug.Log ("Player Strength : " + GameInformation.Strength);
		//Physical Attacks
		Debug.Log ("Player Agility: " + GameInformation.Agility);
		//Dodgle + Range Attack
		Debug.Log ("Player Resistance: " + GameInformation.Resistance);
		//Less Damage
		Debug.Log ("Player Gold: " + GameInformation.Gold);

//		playerHealth = GameInformation.Stamina + GameInformation.PlayerClass.Stamina;
//		playerMagicPower = GameInformation.Intellect + GameInformation.PlayerClass.Intellect;
//		playerPhysicalPower = GameInformation.Strength + GameInformation.PlayerClass.Strength;
//		playerRangePower = GameInformation.Agility + GameInformation.PlayerClass.Agility;
//		playerResistance = GameInformation.Resistance + GameInformation.PlayerClass.Resistance;
//		playerDodgeChance = (GameInformation.Agility + GameInformation.PlayerClass.Agility) / 5;
//		playerDefence = playerHealth + playerResistance;

	}

	// Update is called once per frame
	void Update () {
		myRenderer.sprite = playerSprite;
	}
	public void StatManager(int staminaBoost, int intellectBoost, int strengthBoost, int agilityBoost, int resistanceBoost)
	{
		playerHealth += staminaBoost;
		playerMagicPower += intellectBoost;
		playerPhysicalPower += strengthBoost;
		playerRangePower += agilityBoost;
		playerResistance += resistanceBoost;
	}
	

	public void DamageManager(int damage)
	{
		if(Random.Range(0, 101) < playerDodgeChance)
		   {
			playerDefence -= damage;
		}
		else
		{ 
			Debug.Log("Missed");
		}
	}

	void OnGUI(){
		//GUI.Label (new Rect (Screen.width / 2, 20, 100, 100),"Player Health : " + playerHealth);
		//GUI.Label (new Rect (Screen.width / 2, 40, 100, 100),"Player Magic Power : " + playerMagicPower);
		//GUI.Label (new Rect (Screen.width / 2, 80, 150, 100),"Player Physical Strength : " + playerPhysicalPower);
	}
}
