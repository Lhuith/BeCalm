using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
	LineRenderer Body;

	public Material mat;

	public float[] xpositions;
	float[] ypositions;
	float[] zpositions;
	float[] velocities;

	//Nodes V2
	public Vector3[] Springnodes;

	float[] accelerations;

	public int TileWidth = 16;
	public int TileHeight = 16;
	public int NumTilesX = 16;
	public int NumTilesY = 16;
	public int TileGridWidth = 100;
	public int TileGridHeight = 100;
	public int DefaultTileX;
	public int DefaultTileY;
	public Texture2D Texture;

	//Hooks Law veraibles
	public float springconstant = 0.02f;
	public float damping = 0.04f;
	public float spread = 0.05f;
	public float z = 1f;

	float baseheight;
	//float left;
	//float bottom;

	void OnEnable()
	{
		CreatePlane(TileWidth, TileHeight, TileGridWidth, TileGridHeight);
	}
	
	void Update()
	{
		var tileColumn = Random.Range(0, NumTilesX);
		var tileRow = Random.Range(0, NumTilesY);
		
		var x = Random.Range(0, TileGridWidth);
		var y = Random.Range(0, TileGridHeight);
		
		UpdateGrid(new Vector2(x, y), new Vector2(tileColumn, tileRow), TileWidth, TileHeight, TileGridWidth);
	}
	
	public void UpdateGrid(Vector2 gridIndex, Vector2 tileIndex, int tileWidth, int tileHeight, int gridWidth)
	{


		var mesh = GetComponent<MeshFilter>().mesh;
		var uvs = mesh.uv;
		
		float tileSizeX = 1.0f / NumTilesX;
		float tileSizeY = 1.0f / NumTilesY;
		
		mesh.uv = uvs;
		
		uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 0] = new Vector2(tileIndex.x * tileSizeX, tileIndex.y * tileSizeY);
		uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 1] = new Vector2((tileIndex.x + 1) * tileSizeX, tileIndex.y * tileSizeY);
		uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 2] = new Vector2((tileIndex.x + 1) * tileSizeX, (tileIndex.y + 1) * tileSizeY);
		uvs[(int)(gridWidth * gridIndex.x + gridIndex.y) * 4 + 3] = new Vector2(tileIndex.x * tileSizeX, (tileIndex.y + 1) * tileSizeY);
		
		mesh.uv = uvs;
	}
	
	void CreatePlane(int tileHeight, int tileWidth, int gridHeight, int gridWidth)
	{
		int edgecount = Mathf.RoundToInt (tileWidth) * 5;
		int nodecount = edgecount + 1;
		
		Body = gameObject.AddComponent<LineRenderer> ();
		Body.material = mat;
		Body.material.renderQueue = 1000;
		Body.SetVertexCount (nodecount);
		Body.SetWidth (0.1f, 0.1f);
		
		xpositions = new float[nodecount];
		ypositions = new float[nodecount];
		velocities = new float[nodecount];
		accelerations = new float[nodecount];
		
		baseheight = gridHeight;
		//bottom = 0;
		//left = 0;

		Mesh mesh = new Mesh();
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.GetComponent<Renderer>().material.SetTexture("_MainTex", Texture);
		mf.mesh = mesh;
		
		float tileSizeX = 1.0f / NumTilesX;
		float tileSizeY = 1.0f / NumTilesY;
		
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		
		var index = 0;
		for (var x = 0; x < gridWidth; x++) {
			for (var y = 0; y < gridHeight; y++) {
				AddVertices(tileHeight, tileWidth, y, x, vertices);
				index = AddTriangles(index, triangles);
				AddNormals(normals);
				AddUvs(DefaultTileX, tileSizeY, tileSizeX, uvs, DefaultTileY);
			}
		}
		
		mesh.vertices = vertices.ToArray();
		Springnodes = mesh.vertices;
		mesh.normals = normals.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();
	}
	
	private static void AddVertices(int tileHeight, int tileWidth, int y, int x, ICollection<Vector3> vertices)
	{
		vertices.Add(new Vector3((x * tileWidth), (y * tileHeight), 0));
		vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight), 0));
		vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight) + tileHeight, 0));
		vertices.Add(new Vector3((x * tileWidth), (y * tileHeight) + tileHeight, 0));
	}
	
	private static int AddTriangles(int index, ICollection<int> triangles)
	{
		triangles.Add(index + 2);
		triangles.Add(index + 1);
		triangles.Add(index);
		triangles.Add(index);
		triangles.Add(index + 3);
		triangles.Add(index + 2);
		index += 4;
		return index;
	}
	
	private static void AddNormals(ICollection<Vector3> normals)
	{
		normals.Add(Vector3.forward);
		normals.Add(Vector3.forward);
		normals.Add(Vector3.forward);
		normals.Add(Vector3.forward);
	}
	
	private static void AddUvs(int tileRow, float tileSizeY, float tileSizeX, ICollection<Vector2> uvs, int tileColumn)
	{
		uvs.Add(new Vector2(tileColumn * tileSizeX, tileRow * tileSizeY));
		uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, tileRow * tileSizeY));
		uvs.Add(new Vector2((tileColumn + 1) * tileSizeX, (tileRow + 1) * tileSizeY));
		uvs.Add(new Vector2(tileColumn * tileSizeX, (tileRow + 1) * tileSizeY));
	}

	void FixedUpdate()
	{
		for (int i = 0; i < xpositions.Length; i ++)
		{
			float force = springconstant * (ypositions[i] - baseheight) + velocities[i] * damping;
			accelerations[i] = -force;
			ypositions[i] += velocities[i];
			velocities[i] += accelerations[i];
			Body.SetPosition(i, new Vector3(xpositions[i], ypositions[i],z));
		}
		
		float[] leftDeltas = new float[xpositions.Length];
		float[] rightDeltas = new float[xpositions.Length];
		
		for (int j = 0; j < 8; j++) {
			for (int i = 0; i < xpositions.Length; i++) 
			{
				if (i > 0) 
				{
					leftDeltas [i] = spread * (ypositions[i] - ypositions[i - 1]);
					velocities [i - 1] += leftDeltas [i];
				}
				
				if (i < xpositions.Length - 1) 
				{
					rightDeltas [i] = spread * (ypositions[i] - ypositions [i + 1]);
					velocities[i + 1] += rightDeltas[i];
				}
			}
			
			for (int i = 0; i < xpositions.Length; i++) 
			{
				if (i > 0) 
				{
					ypositions [i - 1] += leftDeltas [i];
				}
				if (i < xpositions.Length - 1) 
				{
					ypositions [i + 1] += rightDeltas [i];
				}
				
			}
		}
		
		//UpdateGrid ();
	}
}