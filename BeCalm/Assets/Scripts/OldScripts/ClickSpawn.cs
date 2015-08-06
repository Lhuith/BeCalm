using UnityEngine;
using System.Collections;

public class ClickSpawn : MonoBehaviour {
	public GameObject Cube;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			GameObject cube = Instantiate(Cube, Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,46.1f)), Cube.transform.rotation) as GameObject;
			cube.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 0));
			cube.transform.localScale = new Vector3(UnityEngine.Random.Range(1f, 2f),0.6f,1)*0.4f;
			cube.GetComponent<Rigidbody>().mass = cube.transform.localScale.x * cube.transform.localScale.y*5f;
			Destroy(cube, 1.4f);
		}
	}
}
