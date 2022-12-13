using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMeshes : MonoBehaviour
{
	
	// Create mesh data from the desired x and z mesh squares
	public static Mesh GenerateTerrainMesh(int xSize, int zSize, float[,] noiseMap, float heightMultiplier, AnimationCurve _heightCurve)
	{
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

		MeshData meshData = new MeshData(xSize, zSize);

		// calculate triangles in mesh
		int verticesIndex = 0;
		for (int z = 0; z < zSize; z++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float y = heightCurve.Evaluate(noiseMap[x, z]) * heightMultiplier;
				Debug.Log(y);
				meshData.vertices[verticesIndex] = new Vector3(x, y, z);

				// if not out of bounds then add the triangles to make the mesh unit (square)
				if (x < xSize - 1 && z < zSize - 1)
				{
					meshData.AddTriangle(verticesIndex, verticesIndex + xSize + 1, verticesIndex + 1);
					meshData.AddTriangle(verticesIndex + xSize + 2, verticesIndex + 1, verticesIndex + xSize + 1);
				}

				verticesIndex++;
			}
			// increment at end of z to prevent triangles being created from the end of a line to the beginning of a line
			verticesIndex++;
		}

		return meshData.CreateMesh();
	}

	// include position in which we want to create the mesh in the case of one big noisemap for every mesh to pull from
	public static Mesh GenerateTerrainMesh(Vector2 position, int xSize, int zSize, float[,] noiseMap, float heightMultiplier, AnimationCurve _heightCurve)
	{
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

		MeshData meshData = new MeshData(xSize, zSize);

		// calculate triangles in mesh
		int verticesIndex = 0;
		// we just want the noisemap height from the start of the chunked mesh to the end
		for (int z = (int)position.y; z < position.y + zSize; z++)
		{
			for (int x = (int)position.x; x < position.x + xSize; x++)
			{
				Debug.Log("hitting " + position.x + " " + position.y);
				// result will be between 0 and the heightMultiplier
				float y = heightCurve.Evaluate(noiseMap[x, z]) * heightMultiplier;
				//Debug.Log(y);
				meshData.vertices[verticesIndex] = new Vector3(x, y, z);

				// check if the x is out of range. Add position to range since we're using one big noisemap
				if (x < position.x + xSize - 1 && z < position.y + zSize - 1)
				{
					meshData.AddTriangle(verticesIndex, verticesIndex + xSize + 1, verticesIndex + 1);
					meshData.AddTriangle(verticesIndex + xSize + 2, verticesIndex + 1, verticesIndex + xSize + 1);
				}

				verticesIndex++;
			}
			// increment at end of z to prevent triangles being created from the end of a line across the mesh to the beginning of a line
			verticesIndex++;
		}

		return meshData.CreateMesh();
	}

	public class MeshData {
		public Vector3[] vertices;
		public int[] triangles;

		int triangleIndex;
		public MeshData(int xSize, int zSize)
		{
			// idk I tried a lot of things and this one worked...
			//TODO: when smarter think through and fix this
			vertices = new Vector3[(xSize +1) * (zSize +1)];
			triangles = new int[(xSize-1) * (zSize-1) * 6];
		}

		// absolutly stunning
		public void AddTriangle(int a, int b, int c)
		{
			triangles[triangleIndex] = a;
			triangles[triangleIndex + 1] = b;
			triangles[triangleIndex + 2] = c;
			triangleIndex += 3;
		}

		public Mesh CreateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			// we're not going to worry too much about weird normals with chunks yet
			//TODO: worry about weird normals when chunking
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}
