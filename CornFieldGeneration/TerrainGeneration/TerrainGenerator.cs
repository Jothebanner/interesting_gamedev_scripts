using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
	public class TerrainTile {
        GameObject meshObject;
        Vector2 position;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        MeshCollider meshCollider;
        MeshData meshData;

        public TerrainTile(Vector2 tilePosition, int xSize, int zSize, Transform parent, Material material)
		{
            meshObject = new GameObject("Terrain Tile");
            meshObject.transform.position = tilePosition;
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            // set the filter to be a mesh made from perlin noise
            // set collider to the same mesh
            // set the renderer to the same mesh but also with color

            meshObject.transform.parent = parent;
            float[,] noiseMap;

            MapGenerator mapGenerator = parent.GetComponent<MapGenerator>();


            noiseMap = mapGenerator.GenerateNoiseMap(tilePosition);

            meshFilter.mesh = GenerateMeshes.GenerateTerrainMesh(xSize, zSize, noiseMap, mapGenerator.heightMultiplier, mapGenerator.heightCurve);
            meshCollider.sharedMesh = meshFilter.mesh;

            AddTerrainToTerrainList(meshObject, mapGenerator);
        }

        public TerrainTile(Vector2 tilePosition, int xSize, int zSize, Transform parent, Material material, float[,] noiseMap)
        {
            meshObject = new GameObject("Terrain Tile");
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshData = meshObject.AddComponent<MeshData>();
            meshData.tilePosition.x = tilePosition.x;
            meshData.tilePosition.z = tilePosition.y;
            meshData.xSize = xSize;
            meshData.zSize = zSize;
            
            meshRenderer.material = material;

            // set the filter to be a mesh made from perlin noise
            // set collider to the same mesh
            // set the renderer to the same mesh but also with color

            // pretty much fixes everying
            meshObject.transform.SetParent(parent, false);

            // get the map generator that called it (probably)
            MapGenerator mapGenerator = parent.GetComponent<MapGenerator>();

            meshFilter.mesh = GenerateMeshes.GenerateTerrainMesh(tilePosition, xSize, zSize, noiseMap, mapGenerator.heightMultiplier, mapGenerator.heightCurve);
            meshCollider.sharedMesh = meshFilter.mesh;

            AddTerrainToTerrainList(meshObject, mapGenerator);
        }

        //TODO: this should probably belong in the MapGenerator script
        void AddTerrainToTerrainList(GameObject terrainTile, MapGenerator mapGenerator)
		{
            mapGenerator.terrainArray.Add(terrainTile);
		}

    }
}
