using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData : MonoBehaviour
{
	public Vector3 tilePosition;
	public int xSize;
	public int zSize;

	public MeshData(Vector3 _tilePosition, int _xSize, int _zSize)
	{
		tilePosition = _tilePosition;
		xSize = _xSize;
		zSize = _zSize;
	}
}
