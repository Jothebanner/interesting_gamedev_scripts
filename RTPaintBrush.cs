using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTPaintBrush : MonoBehaviour
{
    [SerializeField] float distanceBetweenDrawPoints = 0.1f;
    [SerializeField] Material mat;
    public int resolution = 512;
    Texture2D whiteMap;
    public float brushSize;
    public Texture2D brushTexture;
    Vector2 stored;
    public static Dictionary<Collider, RenderTexture> paintTextures = new Dictionary<Collider, RenderTexture>();
    void Start()
    {
        CreateClearTexture();// clear white texture to draw on
    }

    public void Paint(RaycastHit _hit)
    {
        Debug.Log(_hit.collider.gameObject.name);
        // I still don't understand how target - position gives the direction towards the target
        //Debug.DrawRay(transform.position, hit.point - transform.position, Color.red);
        Collider coll = _hit.collider;
        if (coll != null)
        {
            if (!paintTextures.ContainsKey(coll)) // if there is already paint on the material, add to that
            {
                Renderer rend = _hit.transform.GetComponent<Renderer>();
                paintTextures.Add(coll, getWhiteRT());
                
                rend.material.SetTexture("_RenderTexture", paintTextures[coll]);
            }
            if (stored != _hit.lightmapCoord) // stop drawing on the same point
            {
                if ((_hit.lightmapCoord - stored).magnitude > distanceBetweenDrawPoints)
                {
                    stored = _hit.lightmapCoord;
                    Vector2 pixelUV = _hit.lightmapCoord;
                    pixelUV.y *= resolution;
                    pixelUV.x *= resolution;
                    DrawTexture(paintTextures[coll], pixelUV.x, pixelUV.y);
                }
            }
        }
    }

    void DrawTexture(RenderTexture rt, float posX, float posY)
    {
        RenderTexture.active = rt; // activate rendertexture for drawtexture;
        GL.PushMatrix(); // save matrixes
        GL.LoadPixelMatrix(0, resolution, resolution, 0);      // setup matrix for correct size

        // draw brushtexture
        Graphics.DrawTexture(new Rect(posX - brushTexture.width / brushSize, (rt.height - posY) - brushTexture.height / brushSize, brushTexture.width / (brushSize * 0.5f), brushTexture.height / (brushSize * 0.5f)), brushTexture, mat);
        GL.PopMatrix();
        RenderTexture.active = null; // turn off rendertexture
    }

    RenderTexture getWhiteRT()
    {
        RenderTexture rt = new RenderTexture(resolution, resolution, 32);
        //Graphics.Blit(whiteMap, rt);
        return rt;
    }

    void CreateClearTexture()
    {
        whiteMap = new Texture2D(1, 1);
        whiteMap.SetPixel(0, 0, Color.black);
        whiteMap.Apply();
    }
}
