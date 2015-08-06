using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveDisplacementGrid : MonoBehaviour {

	public Texture2D sourceimage;
	private Texture2D targettexture;

	GameObject player;

	private int size;
	private int hwidth;
	private int hheight;
	private int riprad;
	
	private int[] ripplemap;
	private int data;
	
	private Color[] ripple;
	private Color[] texture;
	private int[] pixels;
	
	private int oldind;
	private int newind;
	private int mapind;
	
	private int i;
	private int a;
	private int b;
	
	private int width;
	private int height;
	
	public int disturbsize = 512;
	
	private float dmin = 999999999;
	private float dmax = -999999999;
	public MeshFilter[] meshFilters;
	public Renderer[] tileRenderers;

	public MeshFilter originalMesh;

	public Vector2[] uvList;

	public Material _mat;
	// init 

	public void Awake ()
	{
		originalMesh = GetComponent<MeshFilter>();
		uvList = originalMesh.mesh.uv;

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
		}

		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
		transform.GetComponent<MeshCollider> ().sharedMesh = transform.GetComponent<MeshFilter>().mesh;

		transform.gameObject.SetActive(true);


		_mat = new Material(Shader.Find("mShaders/WaterBumpShader3"));
		_mat.mainTexture = sourceimage;


		width = sourceimage.width;
		height = sourceimage.height;

		targettexture = new Texture2D(width,height);

		_mat.SetTexture("_ExtrudeTex", targettexture);

		MeshRenderer renderer = GetComponent<MeshRenderer>();
		renderer.sharedMaterial = _mat;

	
		tileRenderers = GetComponentsInChildren<Renderer> ();
		//sourceimage = GetComponentInChildren<Renderer>().material.GetTexture("_ExtrudeTex",);
		
		//sourceimage = GetComponentInParent<WaterGridSystem> ().sourceimage; //GetComponentInChildren<Renderer>().material.GetTexture("_ExtrudeTex",);
		

		//Single Texture set
		
		//targettexture = GetComponent<MeshRenderer> ().material.GetTexture ("_ExtrudeTex") as Texture2D;
		
		//GetComponent<MeshRenderer> ().material.mainTexture = targettexture;
		
		//this.GetComponent<MeshRenderer> ().material.SetTexture ("_ExtrudeTex", targettexture);

		//Setting Up Rendering Textures

			hwidth = width >>1;
			hheight = height >>1;
			riprad = 3; //test with 3
			
			size = width * (height + 2) * 2;
			
			ripplemap = new int[size];
			ripple = new Color[width*height];
			texture = new Color[width*height];
			pixels = new int[width*height];
			
			oldind = width;
			newind = width * (height+3);
			
			int counter = 0;
			
			for (int y =0; y < height; y++) 
			{
				for (int x = 0; x < width; x++) 
				{
					texture[counter] = sourceimage.GetPixel (x,y);
					counter++;
				}
			}



	}
	void Start(){
		
	}
	public void Update()
	{
		//player = CustomExtensions.GetPlayer();
		//float watertileDis = CustomExtensions.GetDistance(this.gameObject, player);
		//  image(img, 0, 0); //Displays images to the screen
		//  loadPixels(); // Loads the pixel data for the display window into the pixels[] array
		//  texture = pixels;


		//RaycastHit Playerhit;
		//Vector3 fwd = raycastObject.transform.TransformDirection(Vector3.forward);
		//Vector3 playerPos = player.transform.TransformDirection(Vector3.forward);
		//Debug.DrawLine (player.transform.position, player.transform.up, Color.red);
		//Physics.Raycast (player.transform.position, transform.forward, out Playerhit);

		newframe();
		int px = 0;
		int py = 0;

		for (int i = 0; i < pixels.Length; i++) 
		{
			//	todo: use Texture2D.SetPixels instead..
			//	targettexture.SetPixel (px, py, ripple[i]);

			targettexture.SetPixel (px, py, ripple[i]);
			px++;
			if (px >= width) {px=0; py++;}
		}
			targettexture.Apply();
		//updatePixels(); //Updates the display window with the data in the pixels[] array
		//targettexture.Apply();
		
		// left mouse button is pressed down
		if(Input.GetMouseButton(0))
		{
			// raycast to mousecursor location
			RaycastHit hit;
			if (!Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit))	return;


			WaterWake(hit);
		}
	}
	
	// ripples 
	public void disturb(int dx, int dy)
	{
		for (int j = dy-riprad; j < dy + riprad;j++) {
			for (int k = dx-riprad; k < dx + riprad;k++) {
				if (j >= 0 && j<height && k >= 0 && k<width) {
					ripplemap[oldind+(j*width)+k] += disturbsize;
				}
			}
		}
	}
	
	
	// processing
	public void newframe() 
	{
		//Toggle maps each frame
		i = oldind;
		oldind = newind;
		newind = i;
		
		
		i = 0;
		mapind = oldind;
		for (int y = 0;y < height;y++) {
			for (int x =0;x<width;x++) {
				data = (ripplemap[mapind-width]+ripplemap[mapind+width]+ripplemap[mapind-1]+ripplemap[mapind+1])>>1;
				data -= ripplemap[newind+i];
				data -= data >> 5;
				
				ripplemap[newind+i]=data;
				float col = remap(data,-2560,1537,0,1); //Mathf.Abs(data);
				
				if (data<dmin) dmin=data;
				if (data>dmax) dmax=data;
				//if (x==50 && y==50)	print (col);
				
				//where data=0 then still, where data>0 then wave
				data = (1024-data);
				
				//offsets
				a=((x-hwidth)*data/1024)+hwidth;
				b=((y-hheight)*data/1024)+hheight;
				
				//bounds check
				if (a>=width) a=width-1;
				if (a<0) a=0;
				if (b>=height) b=height-1;
				if (b<0) b=0;
				
				//      ripple[i]=texture[a+(b*width)];
				//ripple[i]=A[a+(b*w)];
				
				ripple[i]= new Color(col,col,col,1);
				mapind++;
				i++;
			}
		}
		
		//  print ("data:"+dmin+","+dmax);
		
	}


	public void WaterWake(RaycastHit hitTexCoords)
	{

		Vector2 pixelUV = hitTexCoords.textureCoord;
		pixelUV.x *= width;
		pixelUV.y *= height;
		
		// then apply waves on that position
			disturb((int)pixelUV.x, (int)pixelUV.y);
		}

	public float remap(float value, float from1, float to1, float from2, float to2) 
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

}
