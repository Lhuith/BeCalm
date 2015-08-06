using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GenerateWaterMesh : MonoBehaviour {

	public static List<Vector3[]> GenerateWater(MeshFilter waterMeshFilter, float size, float spacing){
		//Determine the number of vertices per row/column (is always a square)
		int totalVertices = (int)Mathf.Round (size / spacing) + 1;
		//Normals
		List<Vector3> normals = new List<Vector3>();;
		//UVS
		//Vector2[] uvs = waterMeshFilter.mesh.uv;
		//Vertices 2d List
		List<Vector3[]> vertices2dArray = new List<Vector3[]> ();
		//Triangles
		List<int> tris = new List<int> ();

		for (int z = 0; z < totalVertices; z++) 
		{
			vertices2dArray.Add(new Vector3[totalVertices]);

			for(int x = 0; x < totalVertices; x++)
			{


				//Fill this array every loop
				Vector3 current_point = new Vector3 ();

				//Get the coordinates of the vertices
				current_point.x = x * spacing;
				current_point.z = z * spacing;
				current_point.y = 0f; //assume always at 0

				vertices2dArray[z][x] = current_point;

				//Dont generate a traingle the first coordinate on each row
				if(x <= 0 || z <= 0){
					continue;
				}

				//Build the triangles
				//Each Square consists of 2 triangles

				//The traingle south-west of the vertice
				tris.Add(x         + z * totalVertices);
				tris.Add(x         + (z-1) * totalVertices);
				tris.Add((x-1)         + (z-1) * totalVertices);

				//The traingle west-south of the vertice
				tris.Add(x         + z * totalVertices);
				tris.Add((x-1)         + (z-1) * totalVertices);
				tris.Add((x-1)         + z * totalVertices);
			
				//Normals Added
				normals.Add(Vector3.forward);
				normals.Add(Vector3.forward);
				normals.Add(Vector3.forward);
				normals.Add(Vector3.forward);

				//Uvs Added

				//uvs.Add(new Vector2(x * spacing, z * spacing));
				//uvs.Add(new Vector2((x + 1) * spacing, z * spacing));
				//uvs.Add(new Vector2((x + 1) * spacing, (z + 1) * spacing));
				//uvs.Add(new Vector2(x * spacing, (z + 1) * spacing));
			
			}
		}

		//Unfold the 2d array of the verticies into a 1d array
		Vector3[] unfold_verts = new Vector3[totalVertices*totalVertices];
		for (int i = 0; i < vertices2dArray.Count; i++) {
			//Copies all the elements of the current array to the specified array
			vertices2dArray[i].CopyTo(unfold_verts, i * totalVertices);
		}

		Mesh waterMesh = new Mesh ();
		//Generate the mes
		waterMesh.vertices = unfold_verts;
		waterMesh.triangles = tris.ToArray();
		//waterMesh.uv = uvs;
		//waterMesh.normals = normals.ToArray();
		//ensure that the bounding volume is correct
		waterMesh.RecalculateBounds ();
		//Update the normals to reflect the change
		waterMesh.RecalculateNormals ();
		waterMesh.name = "WaterMesh";
		//Add the generated mesh to the gameObject
		waterMeshFilter.mesh.Clear ();
		waterMeshFilter.mesh = waterMesh;
		//Return the 2d array
		return vertices2dArray;
	}
}
