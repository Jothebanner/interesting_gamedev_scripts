Shader "Example/LitTessellationTest"
{
    Properties
    {
        _Tessellation("Tessellation", Range(1, 32)) = 20
        _MaxTessDistance("Max Tess Distance", Range(1, 32)) = 20
        _Noise("NoiseTexture", 2D) = "gray" {}
        _StackColor("StackColor", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _AmbientColor("AmbientColor", Color) = (1,1,1,1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5

        _Weight("Displacement Amount", Range(0, 1)) = 0
        _LightPoint("LightPointPosition", Vector) = (0,0,0,0)
        _ShadowStrength("Shadow Strength", Range(0, 1)) = 0.5 // Add this line
        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

    }
        SubShader
        {

            Pass
            {
                Tags{ "LightMode" = "UniversalForward" }

                Cull off


                // The HLSL code block. Unity SRP uses the HLSL language.
                HLSLPROGRAM
                // The Core.hlsl file contains definitions of frequently used HLSL
                // macros and functions, and also contains #include references to other
                // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"

                #if defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_VULKAN) || defined(SHADER_API_METAL) || defined(SHADER_API_PSSL)
                #define UNITY_CAN_COMPILE_TESSELLATION 1
                #   define UNITY_domain                 domain
                #   define UNITY_partitioning           partitioning
                #   define UNITY_outputtopology         outputtopology
                #   define UNITY_patchconstantfunc      patchconstantfunc
                #   define UNITY_outputcontrolpoints    outputcontrolpoints
                #endif

                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _SHADOWS_SOFT
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

                TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
                half _OcclusionStrength;

                // The structure definition defines which variables it contains.
                // This example uses the Attributes structure as an input structure in
                // the vertex shader.

                // vertex to fragment struct
                struct Varyings
                {
                    float4 color : COLOR;

                    float3 normal : NORMAL;

                    float4 vertex : SV_POSITION;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                        UNITY_VERTEX_OUTPUT_STEREO

                    float2 uv : TEXCOORD0;

                    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                        float3 positionWS : TEXCOORD2;
                    #endif

                        float4 shadowCoord : TEXCOORD3;
                };


                // tessellation data
                struct TessellationFactors
                {
                    float edge[3] : SV_TessFactor;
                    float inside : SV_InsideTessFactor;
                };

                // Extra vertex struct
                struct ControlPoint
                {
                    float4 vertex : INTERNALTESSPOS;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                    float3 normal : NORMAL;
                };

                // the original vertex struct
                struct Attributes
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                    UNITY_VERTEX_INPUT_INSTANCE_ID

                };

                // tessellation variables, add these to your shader properties
                float _Tessellation;
                float _MaxTessDistance;

                // info so the GPU knows what to do (triangles) and how to set it up , clockwise, fractional division
                // hull takes the original vertices and outputs more
                [UNITY_domain("tri")]
                [UNITY_outputcontrolpoints(3)]
                [UNITY_outputtopology("triangle_cw")]
                [UNITY_partitioning("fractional_odd")]
                //[UNITY_partitioning("fractional_even")]
                //[UNITY_partitioning("pow2")]
                //[UNITY_partitioning("integer")]
                [UNITY_patchconstantfunc("patchConstantFunction")]
                ControlPoint hull(InputPatch<ControlPoint, 3> patch, uint id : SV_OutputControlPointID)
                {
                    return patch[id];
                }


                // fade tessellation at a distance
                float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
                {
                    float3 worldPosition = TransformObjectToWorld(vertex.xyz);
                    float dist = distance(worldPosition, _WorldSpaceCameraPos);
                    float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
                    return (f);
                }


                // tessellation
                TessellationFactors patchConstantFunction(InputPatch<ControlPoint, 3> patch)
                {
                    // values for distance fading the tessellation
                    float minDist = 5.0;
                    float maxDist = _MaxTessDistance;

                    TessellationFactors f;

                    float edge0 = CalcDistanceTessFactor(patch[0].vertex, minDist, maxDist, _Tessellation);
                    float edge1 = CalcDistanceTessFactor(patch[1].vertex, minDist, maxDist, _Tessellation);
                    float edge2 = CalcDistanceTessFactor(patch[2].vertex, minDist, maxDist, _Tessellation);

                    // make sure there are no gaps between different tessellated distances, by averaging the edges out.
                    f.edge[0] = (edge1 + edge2) / 2;
                    f.edge[1] = (edge2 + edge0) / 2;
                    f.edge[2] = (edge0 + edge1) / 2;
                    f.inside = (edge0 + edge1 + edge2) / 3;
                    return f;
                }


                #pragma require tessellation
                // This line defines the name of the vertex shader. 
                #pragma vertex TessellationVertexProgram
                        // This line defines the name of the fragment shader. 
                #pragma fragment frag
                        // This line defines the name of the hull shader. 
                #pragma hull hull
                        // This line defines the name of the domain shader.
                #pragma domain domain


                // pre tesselation vertex program
                ControlPoint TessellationVertexProgram(Attributes v)
                {
                    ControlPoint p;

                    p.vertex = v.vertex;
                    p.uv = v.uv;
                    p.normal = v.normal;
                    p.color = v.color;

                    return p;
                }

                float _Weight;
                sampler2D _Noise;

                // after tesselation
                Varyings vert(Attributes input)
                {
                    Varyings output;

                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_TRANSFER_INSTANCE_ID(input, output);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                    float Noise = tex2Dlod(_Noise, float4(input.uv, 0, 0)).r;

                    input.vertex.xyz += (input.normal) * Noise * _Weight;


                    output.vertex = TransformObjectToHClip(input.vertex.xyz);

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);

                    output.normal = normalize(input.normal);
                    output.uv = input.uv;
                    output.color = input.color;
                    output.shadowCoord = GetShadowCoord(vertexInput);
                    return output;
                }

                [UNITY_domain("tri")]
                Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
                {
                    Attributes v;

                #define DomainPos(fieldName) v.fieldName = \
					                        patch[0].fieldName * barycentricCoordinates.x + \
					                        patch[1].fieldName * barycentricCoordinates.y + \
					                        patch[2].fieldName * barycentricCoordinates.z;

                                        DomainPos(vertex)
                                        DomainPos(uv)
                                        DomainPos(color)
                                        DomainPos(normal)

                                        return vert(v);
                                    }

                SurfaceData CheatInitializeSurfaceData(half3 albedo, half metallic, half3 specular,
                    half smoothness, half occlusion, half3 emission, half alpha)
                {
                    SurfaceData s;
                    s.albedo = albedo;
                    s.metallic = metallic;
                    s.specular = specular;
                    s.smoothness = smoothness;
                    s.occlusion = occlusion;
                    s.emission = emission;
                    s.alpha = alpha;
                    s.clearCoatMask = 0.0;
                    s.clearCoatSmoothness = 1.0;
                    return s;
                }


                half SampleOcclusion(float2 uv)
                {
                    #ifdef _OCCLUSIONMAP
                        // TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
                    #if defined(SHADER_API_GLES)
                        return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
                    #else
                        half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
                        return LerpWhiteTo(occ, _OcclusionStrength);
                    #endif
                    #else
                        return 1.0;
                    #endif
                }

                Texture2D _StackColor;

                SamplerState sampler_StackColor;

                half4 _AmbientColor;

                half4 _LightPoint;

                // The fragment shader definition.            
                half4 frag(Varyings input) : SV_Target
                {
                    InputData inputData = (InputData)0;

                    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                        inputData.positionWS = input.positionWS;
                    #endif

                    // Initialize inputs
                    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.vertex);

                    half3 albedo = half3(1, 0, 0);
                    half metallic = 0; half3 specular = half3(1, 1, 1);
                    half smoothness = 0; half occlusion = 1; half3 emission = half3(0, 0, 0); half alpha = 0;
                    SurfaceData surfaceData = CheatInitializeSurfaceData(albedo, metallic, specular, smoothness, occlusion, emission, alpha);


                    //half4 baseMap = SAMPLE_TEXTURE2D(_StackColor, sampler_StackColor, input.uv);

                    //half3 color = baseMap.xyz * _BaseColor.xyz * input.color.xyz;

                    Light mainLight = GetMainLight(input.shadowCoord);

                    #if defined(_SCREEN_SPACE_OCCLUSION)
                        AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
                        mainLight.color *= aoFactor.directAmbientOcclusion;
                        surfaceData.occlusion = min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
                    #endif

                    #if !_USE_WEBGL1_LIGHTS
                    #define LIGHT_LOOP_BEGIN(lightCount) \
                        for (uint lightIndex = 0u; lightIndex < lightCount; ++lightIndex) {

                    #define LIGHT_LOOP_END }
                    #else
                                            // WebGL 1 doesn't support variable for loop conditions
                    #define LIGHT_LOOP_BEGIN(lightCount) \
                        for (int lightIndex = 0; lightIndex < _WEBGL1_MAX_LIGHTS; ++lightIndex) { \
                            if (lightIndex >= (int)lightCount) break;

                    #define LIGHT_LOOP_END }
                    #endif

                    half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
                    half3 diffuseColor = LightingLambert(attenuatedLightColor, mainLight.direction, input.normal);
                    //half3 specularColor = LightingSpecular(attenuatedLightColor, mainLight.direction, inputData.normalWS, inputData.viewDirectionWS, specularGloss, smoothness);
                
                   
                        uint pixelLightCount = GetAdditionalLightsCount();
                        LIGHT_LOOP_BEGIN(pixelLightCount)
                            Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
                            #if defined(_SCREEN_SPACE_OCCLUSION)
                                light.color *= aoFactor.directAmbientOcclusion;
                            #endif
                            half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
                            diffuseColor += LightingLambert(attenuatedLightColor, light.direction, inputData.normalWS);
                            //specularColor += LightingSpecular(attenuatedLightColor, light.direction, inputData.normalWS, inputData.viewDirectionWS, specularGloss, smoothness);
                        LIGHT_LOOP_END

                    half3 color = diffuseColor;

                    return half4(color, 1);

                }
                ENDHLSL
            }
    
            Pass
            {
                Name "ShadowCaster"
                Tags{"LightMode" = "ShadowCaster"}

                ZWrite On
                ZTest LEqual
                ColorMask 0
                Cull[_Cull]

                HLSLPROGRAM
                #pragma exclude_renderers gles gles3 glcore
                #pragma target 4.5

                        // -------------------------------------
                        // Material Keywords
                        #pragma shader_feature_local_fragment _ALPHATEST_ON
                        #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

                        //--------------------------------------
                        // GPU Instancing
                        #pragma multi_compile_instancing
                        #pragma multi_compile _ DOTS_INSTANCING_ON

                        #pragma vertex ShadowPassVertex
                        #pragma fragment ShadowPassFragment

                        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
                        ENDHLSL
                    }

            Pass
            {
                // Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
                // no LightMode tag are also rendered by Universal Render Pipeline
                Name "GBuffer"
                Tags{"LightMode" = "UniversalGBuffer"}

                ZWrite[_ZWrite]
                ZTest LEqual
                Cull[_Cull]

                HLSLPROGRAM
                #pragma exclude_renderers gles gles3 glcore
                #pragma target 4.5

                // -------------------------------------
                // Material Keywords
                #pragma shader_feature_local _NORMALMAP
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                //#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
                #pragma shader_feature_local_fragment _EMISSION
                #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
                #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                #pragma shader_feature_local_fragment _OCCLUSIONMAP
                #pragma shader_feature_local _PARALLAXMAP
                #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

                #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
                #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
                #pragma shader_feature_local_fragment _SPECULAR_SETUP
                #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

                // -------------------------------------
                // Universal Pipeline keywords
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile _ _SHADOWS_SOFT
                #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile _ DIRLIGHTMAP_COMBINED
                #pragma multi_compile _ LIGHTMAP_ON
                #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON

                #pragma vertex LitGBufferPassVertex
                #pragma fragment LitGBufferPassFragment

                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitGBufferPass.hlsl"
                ENDHLSL
            }

            Pass
            {
                Name "DepthOnly"
                Tags{"LightMode" = "DepthOnly"}

                ZWrite On
                ColorMask 0
                Cull[_Cull]

                HLSLPROGRAM
                #pragma exclude_renderers gles gles3 glcore
                #pragma target 4.5

                #pragma vertex DepthOnlyVertex
                #pragma fragment DepthOnlyFragment

                // -------------------------------------
                // Material Keywords
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON

                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
                ENDHLSL
            }
            // This pass is used when drawing to a _CameraNormalsTexture texture
            Pass
            {
                Name "DepthNormals"
                Tags{"LightMode" = "DepthNormals"}

                ZWrite On
                Cull[_Cull]

                HLSLPROGRAM
                #pragma exclude_renderers gles gles3 glcore
                #pragma target 4.5

                #pragma vertex DepthNormalsVertex
                #pragma fragment DepthNormalsFragment

                // -------------------------------------
                // Material Keywords
                #pragma shader_feature_local _NORMALMAP
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON

                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
                ENDHLSL
            }

                // This pass it not used during regular rendering, only for lightmap baking.
            Pass
            {
                Name "Meta"
                Tags{"LightMode" = "Meta"}

                Cull Off

                HLSLPROGRAM
                #pragma exclude_renderers gles gles3 glcore
                #pragma target 4.5

                #pragma vertex UniversalVertexMeta
                #pragma fragment UniversalFragmentMeta

                #pragma shader_feature_local_fragment _SPECULAR_SETUP
                #pragma shader_feature_local_fragment _EMISSION
                #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
                #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

                #pragma shader_feature_local_fragment _SPECGLOSSMAP

                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

                ENDHLSL
            }

            Pass
            {
                Name "Universal2D"
                Tags{ "LightMode" = "Universal2D" }

                Blend[_SrcBlend][_DstBlend]
                ZWrite[_ZWrite]
                Cull[_Cull]

                HLSLPROGRAM
                #pragma exclude_renderers gles gles3 glcore
                #pragma target 4.5

                #pragma vertex vert
                #pragma fragment frag
                #pragma shader_feature_local_fragment _ALPHATEST_ON
                #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
                ENDHLSL
            }
            }

            FallBack "Lit"
}
