using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class MusicLoad : MonoBehaviour {
	public AudioClip[] songslist;
	// Use this for initialization
	void Start () {
		AudioClip[] songsListLoad =  Resources.LoadAll<AudioClip>("Music");
		songslist = songsListLoad;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
