using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EdgeGenerator
{
    public static float[,] GenerateUpwardEdges(int XSize , int ZSize, float a, float b)
	{
		float[,] map = new float[XSize, ZSize];

		for (int i = 0; i < ZSize; i++)
		{
			for (int j = 0; j < XSize; j++)
			{
				float x = i / (float)XSize * 2 - 1;
				float z = j / (float)ZSize * 2 - 1;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
				map[i, j] = Evaluate(value, a, b);
			}
		}

		return map;
	}

	static float Evaluate(float value, float a, float b)
	{
		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}
}
