using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class MusicLoad : MonoBehaviour {
	public AudioClip[] Calmsongslist, NotCalmsongslist;
	// Use this for initialization
	void Start () {
		AudioClip[] CalmsongsListLoad =  Resources.LoadAll<AudioClip>("Music/CalmMusic");
		AudioClip[] NotCalmsongslistload =  Resources.LoadAll<AudioClip>("Music/NotCalmMusic");
		Calmsongslist = CalmsongsListLoad;
		NotCalmsongslist = NotCalmsongslistload;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
