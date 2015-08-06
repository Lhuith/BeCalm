using UnityEngine;
using System.Collections;

public class ChildTextureTest : MonoBehaviour {
	private int size;
	private int hwidth;
	private int hheight;
	private int riprad;


	public int width;
	public int height;

	public Texture2D sourceimage;
	public Texture2D targettexture;

	// Use this for initialization
	void Start () {
		sourceimage =  new Texture2D (256, 256);
		
		width = sourceimage.width;
		height = sourceimage.height;
		
		targettexture = new Texture2D (width, height);
		
		GetComponent<MeshRenderer> ().material.SetTexture ("_ExtrudeTex", targettexture);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
