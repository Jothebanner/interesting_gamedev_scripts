using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Material material;

    [Header("Detail Controls")]
    public int mapXSize = 20;
    public int mapZSize = 20;
    public int XTiles = 1;
    public int ZTiles = 1;

    [Header("Overall Size Controls")]
    [SerializeField] float mapXSizeMultiplier = 1f;
    [SerializeField] float mapZSizeMultiplier = 1f;

    [Header("Calculated Tile Size")]
    public int tileXSize;
    public int tileZSize;

    [Header("Noise Map Controls")]
    public float noiseScale;

    public int octaves;
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public bool useOneNoiseMap;

    [Header("Edges")]
    public bool addEdge;
    [SerializeField] float edgeParamA = 2.0f;
    [SerializeField] float edgeParamB = 3.0f;


    float[,] bigBoiNoiseMap;

    public List<GameObject> terrainArray = new List<GameObject>();

    [SerializeField] GameObject sphere;

    //const int maxMeshSize = 254;

	private void Update()
    {
        // overall map size
        transform.localScale = new Vector3(mapXSizeMultiplier, transform.localScale.y, mapZSizeMultiplier);


        // map detail size

        // calculate the tile size based on desired tiles and total map size
        tileXSize = mapXSize / XTiles;
        tileZSize = mapZSize / ZTiles;


        if (Input.GetKeyDown(KeyCode.G))
		{
            GenerateTheMap();
		}

        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach(GameObject tile in terrainArray)
			{
                Destroy(tile);
			}
            terrainArray.Clear();
        }

		//if (Input.GetKeyDown(KeyCode.U))
		//{
		//	float xIncrement = 0.25f;
		//	float zIncrement = 0.4f;
		//	int numberOfZCorn = Mathf.RoundToInt(mapZSize / zIncrement); // number of corn to be placed on the z coord
		//	int numberOfXCorn = Mathf.RoundToInt(mapXSize / xIncrement); // number of corn to be placed on the x coord
  //          float cornScaleFactor = numberOfZCorn * numberOfXCorn; // get a number both are divizible by even if it's stupid crazy big
  //          Debug.Log(numberOfZCorn + " " + numberOfXCorn + " " + cornScaleFactor);
  //          Vector2 position = new Vector2(0, 0);
		//	float[,] cornNoiseMap = Noise.GenerateNoiseMap(position, numberOfXCorn, numberOfZCorn, seed, cornScaleFactor, octaves, persistance, lacunarity, offset, Noise.NormalizeMode.Global);

  //          int cornInZ = numberOfZCorn;

		//	for (int z = 0; z < numberOfZCorn; z++)
		//	{
		//		for (int x = 0; x < numberOfXCorn; x++)
		//		{
		//			float boundary = 100f;
  //                  int fixedX = x * numberOfXCorn;
  //                  int fixedZ = x * numberOfZCorn;
		//			Vector3 point = new Vector3(fixedX, cornNoiseMap[x, z] * heightMultiplier, fixedZ);
  //                  Quaternion randRotation = Random.rotation;
  //                  randRotation.x = Quaternion.identity.x;
  //                  randRotation.z = Quaternion.identity.z;
  //                  GameObject newSphere = Instantiate(sphere, point, randRotation);
  //                  newSphere.transform.position += sphere.transform.position;
  //                  //if (z > boundary && z < mapZSize * mapZSizeMultiplier - boundary && x > boundary && x < mapXSize * mapXSizeMultiplier - boundary)
  //                  //{
  //                  //	Quaternion randRotation = Random.rotation;
  //                  //	randRotation.x = Quaternion.identity.x;
  //                  //	randRotation.z = Quaternion.identity.z;
  //                  //	GameObject newSphere = Instantiate(sphere, point, randRotation);
  //                  //	newSphere.transform.position += sphere.transform.position;
  //                  //}
  //                  //// if outside boundary then make sure the ground is low enough
  //                  //else if (cornNoiseMap[x, z] * heightMultiplier < 10)
  //                  //{
  //                  //	Quaternion randRotation = Random.rotation;
  //                  //	randRotation.x = Quaternion.identity.x;
  //                  //	randRotation.z = Quaternion.identity.z;
  //                  //	GameObject newSphere = Instantiate(sphere, point, randRotation);
  //                  //	newSphere.transform.position += sphere.transform.position;
  //                  //}
  //              }
		//	}

		//}



		if (Input.GetKeyDown(KeyCode.Y))
        {
            int cornCount = 0;
            RaycastHit hit;
            float maxHeight = heightMultiplier;
            GameObject parentMesh = GameObject.Find("MeshHolder");

            foreach (GameObject tile in terrainArray)
            {
                Vector3 tilePosition = tile.GetComponent<MeshData>().tilePosition;
                Debug.Log("tile position " + tilePosition);
                float xIncrement = 0.25f;
                float zIncrement = 0.4f;
				for (float z = tilePosition.z; z < (tile.GetComponent<MeshData>().zSize -1 + tilePosition.z); z += zIncrement)
				{
					for (float x = tilePosition.x; x < (tile.GetComponent<MeshData>().xSize -1 + tilePosition.x); x += xIncrement)
					{
						Vector3 vertex = new Vector3(x, 0, z);
						Ray ray = new Ray(new Vector3(vertex.x * mapXSizeMultiplier, maxHeight * 2, vertex.z * mapZSizeMultiplier), Vector3.down);
						tile.GetComponent<MeshCollider>().Raycast(ray, out hit, maxHeight * 4.0f);

                        //Instantiate(sphere, new Vector3(vertex.x * mapXSizeMultiplier, maxHeight * 2, vertex.z * mapZSizeMultiplier), Quaternion.identity);

                        // spawn inside boundaries
                        float boundary = 100f;
                        if (hit.point.z > boundary && hit.point.z < mapZSize * mapZSizeMultiplier - boundary && hit.point.x > boundary && hit.point.x < mapXSize * mapXSizeMultiplier - boundary)
                        {
                            Quaternion randRotation = Random.rotation;
                            randRotation.x = Quaternion.identity.x;
                            randRotation.z = Quaternion.identity.z;
                            GameObject newSphere = Instantiate(sphere, hit.point, randRotation, parentMesh.transform);
                            newSphere.transform.position += sphere.transform.position;
                            cornCount++;
                        }
                        // if outside boundary then make sure the ground is low enough
                        else if(hit.point.y < 10)
						{
                            Quaternion randRotation = Random.rotation;
                            randRotation.x = Quaternion.identity.x;
                            randRotation.z = Quaternion.identity.z;
                            GameObject newSphere = Instantiate(sphere, hit.point, randRotation, parentMesh.transform);
                            newSphere.transform.position += sphere.transform.position;
                            cornCount++;
                        }

						Debug.Log("Vertex " + vertex + " Hit Point " + hit.point + " Tile Position " + tilePosition);
					}
				}

                Debug.Log(cornCount);
			}
        }
    }

	public void GenerateTheMap()
    {
		if (useOneNoiseMap) {

            Vector2 tilePosition = new Vector2(0, 0);
            bigBoiNoiseMap = GenerateNoiseMap(tilePosition);

            for (int z = 0; z < ZTiles; z++)
            {
                tilePosition = new Vector2(0, tilePosition.y);
                for (int x = 0; x < XTiles; x++)
                {
                    new TerrainGenerator.TerrainTile(tilePosition, tileXSize, tileZSize, transform, material, bigBoiNoiseMap);
                    tilePosition += new Vector2(tileXSize - 1, 0);
                }

                // increment z after row is drawn
                tilePosition += new Vector2(0, tileZSize - 1);
            }
        }
    }

    public float[,] GenerateNoiseMap(Vector2 position)
	{
        float[,] noiseMap = Noise.GenerateNoiseMap(position, mapXSize, mapZSize, seed, noiseScale, octaves, persistance, lacunarity, offset, Noise.NormalizeMode.Global);

        if (addEdge)
		{
            float[,] edgeMap = EdgeGenerator.GenerateUpwardEdges(mapXSize, mapZSize, edgeParamA, edgeParamB);

			for (int z = 0; z < mapZSize; z++)
			{
				for (int x = 0; x < mapXSize; x++)
				{

                    noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] + edgeMap[x, z]);

                }
			}

		}            

        return noiseMap;
    }
}
