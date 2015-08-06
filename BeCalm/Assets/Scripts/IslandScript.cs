using UnityEngine;
using System.Collections;

public class IslandScript : MonoBehaviour {

	public BaseIslandOfEarth newearthIsland; 

	public BaseStar newGuidingStar;
	public SkyStars starManager;

	public GameObject GuidingStar;

	// Use this for initialization
	void Start () {
		newGuidingStar = new BaseStar();
		newearthIsland = new BaseIslandOfEarth();

		gameObject.name = newearthIsland.IslandName;
		starManager = GameObject.FindGameObjectWithTag ("Star_Manager").GetComponent<SkyStars>();

		starManager.CreateGuide (newGuidingStar.StarObject, gameObject.transform, gameObject.name, newearthIsland.IslandBaseColor);

	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine (transform.position, GuidingStar.transform.position, Color.red);

		newearthIsland.IslandCoords = transform.position;

		Debug.Log (newearthIsland.IslandCoords);
	}
	
}
