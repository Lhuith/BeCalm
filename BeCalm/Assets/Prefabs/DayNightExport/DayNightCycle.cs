using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {
	public static float time;
	public static bool night;
	public static bool dawn;
	public static bool day;
	public static bool dusk;

	public Camera camera, sky;
	public float duration;
	public Color[] colors;
	public float tint;
	public float dawnDuration;
	public float duskDuration;
	public bool preservePosition;

	public GameObject SkyDome;
	public Material SkyDomeColor;

	float rotation = 0.0f;
	//public float skydomeSunLight;
	Light[] lights;

	//public GameObject sky;

	void Start() {
		//skydomeSunLight = GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity;
		// Get the first enabled camera tagged "MainCamera"
		if ( !camera ) {
			camera = Camera.main;
		}

		// Get references to child lights
		lights = GetComponentsInChildren< Light >();

		//SkyDomeColor = SkyDome.GetComponent<MeshRenderer> ().material;
	}

	void Update() {
		//GameObject.FindGameObjectWithTag ("SkyDome").GetComponent<Sky> ().m_sunIntensity = skydomeSunLight;
		// Set the position relative to the camera
		if ( !preservePosition ) {
			transform.position = camera.transform.position;
		}

		if ( duration > 1 ) {
			// Rotate at a constant speed determined by dividing a full rotation by the duration in seconds
			rotation = ( rotation + 360.0f / duration * Time.deltaTime ) % 360.0f;
			time = rotation / 360.0f;
			// Set the rotation and color of the sky depending on what time it is
			//skydomeSunLight = skydomeSunLight + (rotation / 360.0f) * 10;
			transform.localRotation = Quaternion.Euler( rotation, 0, 0 );
			SetSkyColor( time );

		}

		float nightToDay = 0.25f;
		float dayToNight = 0.75f;
		float dawnNormalized = dawnDuration / duration / 2;
		float duskNormalized = duskDuration / duration / 2;

		time = ( time + nightToDay ) % 1.0f;

		// Set night and day variables depending on what time it is
		if ( time > nightToDay + dawnNormalized && time < dayToNight - dawnNormalized ) {
			day = true;
			night = dawn = dusk = false;
		} else {
			if ( time < nightToDay - duskNormalized || time > dayToNight + duskNormalized ) {
				night = true;
				day = dawn = dusk = false;
			} else {
				if ( time < ( nightToDay + dayToNight ) / 2 ) {
					dawn = true;
					day = night = dusk = false;
				} else {
					dusk = true;
					day = night = dawn = false;
				}
			}
		}

		//print( "Night: " + night + " Dawn: " + dawn + " Day: " + day + " Dusk: " + dusk );
	}

	// Set the color of the sky by interpolating across sky colors by t
	void SetSkyColor( float t ) {
		// Get the index of the current color
		float index = colors.Length * t;

		// Get the first color by rounding the index down
		Color a = colors[ Mathf.FloorToInt( index ) ];
		// Get the second color by rounding the index up
		Color b = colors[ Mathf.CeilToInt( index ) % colors.Length ];

		// Set the background color of the camera by interpolating between the two colors
		//Color SkyColor = Color.Lerp (a, b, index - Mathf.Floor (index));

		Camera.main.backgroundColor  = Color.Lerp( a, b, index - Mathf.Floor( index ) );
		// Set the color of each light to the interpolated color
		for ( int i = lights.Length - 1; i >= 0; i-- ) {
			//lights[ i ].intensity = (rotation / 360.0f);//camera.backgroundColor;
			lights[ i ].color = camera.backgroundColor;
		}

		// Set the fog to the same color as the sky
		RenderSettings.fogColor = camera.backgroundColor;
		// Set the ambient light color by interpolating between the sky color and its grayscale variant as the tint decreases
		//RenderSettings.ambientLight = Color.Lerp( new Color( camera.backgroundColor.grayscale, camera.backgroundColor.grayscale, camera.backgroundColor.grayscale, camera.backgroundColor.a ), camera.backgroundColor, Mathf.Clamp01( tint ) );
	}
}
