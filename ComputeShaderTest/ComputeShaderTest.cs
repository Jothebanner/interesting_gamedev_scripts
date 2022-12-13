using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{

    [SerializeField] ComputeShader comShad;

    [SerializeField] RenderTexture RT;

    void Start()
    {
        RT = new RenderTexture(256, 256, 24);
        RT.enableRandomWrite = true;
        RT.Create();

        comShad.SetTexture(0, "Result", RT);
        comShad.SetFloat("Resolution", RT.width);
        comShad.Dispatch(0, RT.width / 8, RT.height / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
