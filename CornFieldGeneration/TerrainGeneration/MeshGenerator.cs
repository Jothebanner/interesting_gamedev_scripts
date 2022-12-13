using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    float[] perlinHeight;

    public int xSize = 20;
    public int zSize = 20;
    public Color[] colors;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;
    
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

	private void Update()
    {
    }

	void CreateShape()
	{
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        perlinHeight = new float[(xSize + 1) * (zSize + 1)];

		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++)
			{
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2;
                perlinHeight[i] = y;
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

                i++;
			}
		}


        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
		for (int z = 0; z < zSize; z++)
		{
            for (int x = 0; x < xSize; x++)
            {
                int a = vert;
                int b = vert + xSize + 1;
                int c = vert + xSize + 2;
                int d = vert + 1;

                triangles[tris + 0] = a;
                triangles[tris + 1] = b;
                triangles[tris + 2] = d;
                triangles[tris + 3] = c;
                triangles[tris + 4] = d;
                triangles[tris + 5] = b;

                vert++;
                tris += 6;
            }
            vert++;
        }

        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);

                i++;
            }
        }

        if (colors.Length == 0)
         colors = new Color[vertices.Length];

		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++)
			{
				float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
				if (colors[i] != Color.green || colors[i] != Color.white)
					colors[i] = Color.cyan; //gradient.Evaluate(perlinHeight[i]);
				i++;
			}
		}
	}

    void UpdateMesh()
	{
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        mesh.RecalculateNormals();
	}
}
