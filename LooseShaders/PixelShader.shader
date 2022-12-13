Shader "Custom/PixelShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }

    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 postionOS : POSITION;
        };

        struct v2f
        {
            float4 positionHCS : SV_POSITION;
        };


        // vertex shader
        v2f vert(Attributes vertex)
        {
            v2f dataForFragShader;

            dataForFragShader.positionHCS = TransformObjectToHClip(vertex.postionOS.xyz);

            return dataForFragShader;
        }

        half4 _Color;

        // fragment shader
        half4 frag() : SV_Target
        {
            //half4 color = half4(0, 0.5, 0, 0);
            return _Color;
        }


    ENDHLSL



    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        LOD 200

        Pass
        {

            HLSLPROGRAM

                // run vert and frag shaders
                #pragma vertex vert
                #pragma fragment frag


            ENDHLSL

        }
    }
}
