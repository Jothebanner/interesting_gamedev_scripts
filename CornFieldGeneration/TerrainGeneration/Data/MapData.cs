using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MapData : ScriptableObject
{
    #pragma warning disable 0414
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
    public float noiseScale = 27;

    public int octaves = 4;
    public float persistance = .5f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public float heightMultiplier = 5;
    public AnimationCurve heightCurve;

    public bool useOneNoiseMap;

    [Header("Edges")]
    public bool addEdge;
    [SerializeField] float edgeParamA = 2.0f;
    [SerializeField] float edgeParamB = 3.0f;
}
