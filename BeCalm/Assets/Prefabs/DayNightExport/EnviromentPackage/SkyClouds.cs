using UnityEngine;
using System.Collections;

public class SkyClouds : MonoBehaviour {
	public static SkyClouds inst;

	public new Camera camera;
	public int clouds;
	public Vector3 minPosition;
	public Vector3 maxPosition;
	public float minSpeed;
	public float maxSpeed;
	public Vector3 direction;
	public GameObject[] cloudPrefabs;
	public GameObject lightningPrefab;
	public float thunderstorm;

	GameObject[] cloudGameObjects;
	int[] cloudSpeeds;

	void Awake() {
		inst = this;
	}

	void Start() {
		// Get the first enabled camera tagged "MainCamera"
		if ( !camera ) {
			camera = Camera.main;
		}

		// Declare two arrays to store the list of clouds and their speeds
		cloudGameObjects = new GameObject[ clouds ];
		cloudSpeeds = new int[ clouds ];

		for ( int i = 0; i < clouds; i++ ) {
			// Get a random position within the minimum and maximum extents to instantiate a cloud
			Vector3 randomPosition = new Vector3( Random.Range( minPosition.x, maxPosition.x ), Random.Range( minPosition.y, maxPosition.y ), Random.Range( minPosition.z, maxPosition.z ) );
			// Instantiate a random cloud at the random position
			cloudGameObjects[ i ] = ( GameObject )Instantiate( cloudPrefabs[ Random.Range( 0, cloudPrefabs.Length - 1 ) ], randomPosition, Quaternion.Euler( 90.0f, 0.0f, 0.0f ) );
			// Set the parent transform of the instantiated cloud to this transform
			cloudGameObjects[ i ].transform.parent = transform;
			// Set the speed of the cloud to a random value between the minimum and maximum speed
			cloudSpeeds[ i ] = Random.Range( (int)minSpeed, (int)maxSpeed );
		}
	}

	void Update() {
		// Set the position relative to the camera
		transform.position = camera.transform.position;

		for ( int i = 0; i < clouds; i++ ) {
			// Get the position of the cloud after having been moved based on its speed
			Vector3 position = cloudGameObjects[ i ].transform.localPosition + direction * cloudSpeeds[ i ] * Time.deltaTime;

			// When the cloud is no longer visible by any camera
			if ( !cloudGameObjects[ i ].GetComponent<Renderer>().isVisible ) {
				// Wrap the cloud along the X axis
				if ( position.x < minPosition.x || position.x > maxPosition.x ) {
					position.x = -position.x;
				}

				// Wrap the cloud along the Y axis
				if ( position.y < minPosition.y || position.y > maxPosition.y ) {
					position.y = -position.y;
				}

				// Wrap the cloud along the Z axis
				if ( position.z < minPosition.z || position.z > maxPosition.z ) {
					position.z = -position.z;
				}
			}

			// Set the final position of the cloud
			cloudGameObjects[ i ].transform.localPosition = position;
		}

		if ( thunderstorm > 0 && Random.value < thunderstorm ) {
			Instantiate( lightningPrefab, cloudGameObjects[ Random.Range( 0, clouds - 1 ) ].transform.position, Quaternion.identity );
		}
	}
}
