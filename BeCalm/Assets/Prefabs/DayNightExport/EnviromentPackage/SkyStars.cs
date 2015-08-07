using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyStars : MonoBehaviour {
	public new Camera camera;
	public float duration;
	public int stars;
	public int distance, offset;
	public GameObject[] starPrefabsArray;
	public  List<GameObject> starPrefabsList = new List<GameObject> ();
	public List<GameObject> GudingstarsList = new List<GameObject>();
	public SkyStars isNight;
	public float starAlpha;
	void Start() 
	{

		isNight = this.gameObject.GetComponent<SkyStars>();
				// Get the first enabled camera tagged "MainCamera"
				if (!camera) {
						camera = Camera.main;
				}
		DrawStars();
		}

	void Update() {
		UpdateStars ();
		//StarCheck ();

		distance = distance + offset;
		// Set the position relative to the camera
		transform.position = camera.transform.position;

		if ( duration > 1 ) {
			// Rotate at a constant speed determined by dividing a full rotation by the duration in seconds
			transform.Rotate( 360.0f / duration * Time.deltaTime, 0, 0 );
		}

	}
	public void CreateGuide(GameObject guideStar, Transform islandPos, string Starname, Color islandColor)
	{
		//Vector3 islandlocal = islandPos.InverseTransformPoint (islandPos.position);
		// Get a random direction vector for positioning the star
		Vector3 randomDirection = new Vector3 (islandPos.position.x, islandPos.position.y + 1200f, islandPos.position.z);
		// Instantiate a star prefab in the random direction multiplied by the distance
		GameObject starGameObject = (GameObject)Instantiate (guideStar,  randomDirection , Quaternion.identity);
		// Set the parent transform of the instantiated star to this transform
		//starGameObject.transform.parent = transform;
		starGameObject.layer = transform.gameObject.layer;
		// Blend the star sprite depending on its position in the night sky
		//starGameObject.GetComponentInChildren< MeshRenderer > ().material.color *= new Vector4 (1.0f, 1.0f, 1.0f, 1.0f);
		DynamicGI.SetEmissive (starGameObject.GetComponentInChildren<Renderer>(), islandColor);
		islandPos.GetComponent<IslandScript> ().GuidingStar = starGameObject;
		starGameObject.name = Starname + " Guide";
		GudingstarsList.Add (starGameObject);
	}
	

	public void DrawStars(){
		//normalStars
				for (int i = 0; i < stars; i++) {
						// Get a random direction vector for positioning the star
					Vector3 randomDirection = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1, 1f), Random.Range (-1.0f, 1.0f));
						// Instantiate a star prefab in the random direction multiplied by the distance
					GameObject starGameObject = (GameObject)Instantiate (starPrefabsArray [Random.Range (0, 0)],  randomDirection.normalized * distance, Quaternion.identity);
						// Set the parent transform of the instantiated star to this transform
						starGameObject.transform.parent = transform;

						starGameObject.layer = transform.gameObject.layer;
						starPrefabsList.Add(starGameObject);
						// Blend the star sprite depending on its position in the night sky
	
						//starGameObject.GetComponentInChildren< Renderer > ().material.SetColor("_EmissionColor", starColor); 
		}
		//GuidingStars

		}
	public void StarCheck()
	{	for (int i = 0; i < starPrefabsList.Count; i++) {
			if (starPrefabsList [i].activeInHierarchy) {
				if (starPrefabsList [i].GetComponentInChildren<Renderer> ().material.color.a > 0.5f) {
					//starPrefabsList [i].TurnOnGameObject ();
				} else {
					//starPrefabsList [i].TurnOffGameObject ();
				}
			}
		}

	}

	public void RemoveStar(GameObject star)
	{
		starPrefabsList.Remove (star);
	}

	public void UpdateStars()
	{
		for (int i = 0; i < starPrefabsList.Count; i++) {
			if(starPrefabsList [i].activeInHierarchy){
			//Color starColor = new Vector4 (starPrefabsList [i].GetComponentInChildren< Renderer > ().material.color.r, starPrefabsList [i].GetComponentInChildren< Renderer > ().material.color.g, starPrefabsList [i].GetComponentInChildren< Renderer > ().material.color.b, starAlpha);
			//starPrefabsList [i].GetComponentInChildren<Renderer> ().material.color = starColor;
				if( starPrefabsList [i].transform.position.y < 1)
				{
					starPrefabsList [i].TurnOffGameObject ();
				}else
				{
					starPrefabsList [i].TurnOnGameObject ();
				}
			}
		}

	}
}
