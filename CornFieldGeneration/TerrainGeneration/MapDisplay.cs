using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour
{

	public Renderer textureRender;
	[SerializeField] MapGenerator mapGenerator;

	private void Update()
	{
		//DrawNoiseMap();
	}

	//public void DrawNoiseMap()
	//{
	//	float[,] noiseMap = mapGenerator.GenerateMap();
	//	int width = noiseMap.GetLength(0);
	//	int height = noiseMap.GetLength(1);

	//	Texture2D texture = new Texture2D(width, height);

	//	Color[] colorMap = new Color[width * height];
	//	for (int y = 0; y < height; y++)
	//	{
	//		for (int x = 0; x < width; x++)
	//		{
	//			colorMap[y * width + x] = Color.Lerp(Color.blue, Color.green, noiseMap[x, y]);
	//		}
	//	}
	//	texture.SetPixels(colorMap);
	//	texture.Apply();

	//	textureRender.sharedMaterial.mainTexture = texture;
	//	textureRender.transform.localScale = new Vector3(width, 1, height);
	//}

}
