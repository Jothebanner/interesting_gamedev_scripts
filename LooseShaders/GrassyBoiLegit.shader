Shader "Custom/GrassyBoiLegit"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    {
        _AwfulSauce("Sauce", Color) = (0,.5,0,1)
    }


        HLSLINCLUDE

#pragma vertex vert
#pragma fragment frag
#pragma require geometry
#pragma geometry geom

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"  

        struct Attributes
    {
        float4 positionOS   : POSITION;
    };

    struct v2g
    {
        float4 pos  : SV_POSITION;
    };

    v2g vert(Attributes v)
    {
        v2g OUT;
        OUT.pos = v.positionOS;
        return OUT;
    }

    struct g2f
    {
        float4 pos : SV_POSITION;
    };


    [maxvertexcount(12)]
    void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
    {
        float3 v0 = IN[0].pos.xyz;

        g2f o;

        // first grass (0) segment does not get displaced by interactivity
        float3 newPos = v0;

        // every segment adds 2 new triangles
        float3 offsetvertices = v0 + float3(.2, 1, 0);


        float3 positionWS = TransformObjectToWorld(offsetvertices.xyz);

        float4 positionCS = TransformWorldToHClip(positionWS);
        o.pos = positionCS;
        triStream.Append(o);

        offsetvertices = v0 + float3(-.2, 1, 0);


        positionWS = TransformObjectToWorld(offsetvertices.xyz);

        positionCS = TransformWorldToHClip(positionWS);
        o.pos = positionCS;
        triStream.Append(o);

        offsetvertices = v0 + float3(0, 3, 0);


        positionWS = TransformObjectToWorld(offsetvertices.xyz);

        positionCS = TransformWorldToHClip(positionWS);
        o.pos = positionCS;

        // Add just below the loop to insert the vertex at the tip of the blade.
        triStream.Append(o);
        // restart the strip to start another grass blade
        triStream.RestartStrip();
    }
    ENDHLSL

        SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        Cull Off
        Pass
    {

        HLSLPROGRAM
        float4 _AwfulSauce;

            half4 frag(g2f i) : SV_Target
            {
                return _AwfulSauce;
            }
       ENDHLSL
        }
    }
}
