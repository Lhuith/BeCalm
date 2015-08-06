using UnityEngine;
using System.Collections;

public static class CustomExtensions {

	public static float PlayerCheck(Vector3 obj)
	{
		GameObject player = GameObject.FindGameObjectWithTag ("Player");


		float distance = Vector3.Distance (obj, player.transform.position);

		return distance;
	}




	public static GameObject LoadGameobject(string directory)
	{	
		GameObject obj;
		GameObject Grab = Resources.Load(directory, typeof(GameObject)) as GameObject;
		obj = Grab;

		return obj;
	}

	public static void TurnOnOrOffGameObject(this GameObject gO)
	{
		if (gO.activeSelf) 
		{
			gO.SetActive (false);
		} else 
		{
			gO.SetActive (true);
		}
	}

	public static float GridSize(float height_a, float width_a, float width_b)
	{
			float size = (height_a * width_a) * width_b;
			return size;
	}


	public static void TurnOnGameObject(this GameObject gO)
	{
			gO.SetActive (true);

	}

	public static void TurnOffGameObject(this GameObject gO)
	{
		gO.SetActive (false);
	}

	public static string ConvertBoolToString(this bool value)
	{
		if (value) 
		{
			return "Yep"; 
		} else 
		{
			return "Nop";
		}
	}

	public static float GetDistance(this GameObject Me, GameObject gO)
	{
		float distance = Vector3.Distance (Me.transform.position, gO.transform.position);

		return distance;
	}

	public static GameObject GetPlayer()
	{

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		return player;
	}
	
}
