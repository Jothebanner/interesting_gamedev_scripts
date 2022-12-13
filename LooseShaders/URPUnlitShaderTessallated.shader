// This shader adds tessellation in URP
Shader "Example/URPUnlitShaderTessallated"
{

	// The properties block of the Unity shader. In this example this block is empty
	// because the output color is predefined in the fragment shader code.
	Properties
	{
		_Tess("Tessellation", Range(1, 32)) = 20
		_MaxTessDistance("Max Tess Distance", Range(1, 32)) = 20
		_Noise("Noise", 2D) = "gray" {}
		_StackColor("StackColor", 2D) = "white" {}

	_Weight("Displacement Amount", Range(0, 1)) = 0
	}

		// The SubShader block containing the Shader code. 
		SubShader
	{
		// SubShader Tags define when and under which conditions a SubShader block or
		// a pass is executed.
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

		Pass
	{
		Tags{ "LightMode" = "UniversalForward" }


		// The HLSL code block. Unity SRP uses the HLSL language.
		HLSLPROGRAM
		// The Core.hlsl file contains definitions of frequently used HLSL
		// macros and functions, and also contains #include references to other
		// HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		#if defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_VULKAN) || defined(SHADER_API_METAL) || defined(SHADER_API_PSSL)
#define UNITY_CAN_COMPILE_TESSELLATION 1
#   define UNITY_domain                 domain
#   define UNITY_partitioning           partitioning
#   define UNITY_outputtopology         outputtopology
#   define UNITY_patchconstantfunc      patchconstantfunc
#   define UNITY_outputcontrolpoints    outputcontrolpoints
#endif



// The structure definition defines which variables it contains.
// This example uses the Attributes structure as an input structure in
// the vertex shader.

// vertex to fragment struct
struct Varyings
{
	float4 color : COLOR;
	float3 normal : NORMAL;
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
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

	};

	// tessellation variables, add these to your shader properties
	float _Tess;
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

		float edge0 = CalcDistanceTessFactor(patch[0].vertex, minDist, maxDist, _Tess);
		float edge1 = CalcDistanceTessFactor(patch[1].vertex, minDist, maxDist, _Tess);
		float edge2 = CalcDistanceTessFactor(patch[2].vertex, minDist, maxDist, _Tess);

		// make sure there are no gaps between different tessellated distances, by averaging the edges out.
		f.edge[0] = (edge1 + edge2) / 2;
		f.edge[1] = (edge2 + edge0) / 2;
		f.edge[2] = (edge0 + edge1) / 2;
		f.inside = (edge0 + edge1 + edge2) / 3;
		return f;
	}

	// second vertex, copy to shader
	//Varyings vert(Attributes input)
	//{
	//  Varyings output;
	//  // put your vertex manipulation here , ie: input.vertex.xyz += distortiontexture
	//  output.vertex = TransformObjectToHClip(input.vertex.xyz);
	//  output.color = input.color;
	//  output.normal = input.normal;
	//  output.uv = input.uv;
	//  return output;
	//}

	// domain, copy to shader
	//[UNITY_domain("tri")]
	//Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
	//{
	//  Attributes v;
	//
	//#define Tesselationing(fieldName) v.fieldName = \
    //              patch[0].fieldName * barycentricCoordinates.x + \
    //              patch[1].fieldName * barycentricCoordinates.y + \
    //              patch[2].fieldName * barycentricCoordinates.z;
	//
	//  Tesselationing(vertex)
	//      Tesselationing(uv)
	//      Tesselationing(color)
	//      Tesselationing(normal)
	//
	//      return vert(v);
	//}


#pragma require tessellation
		// This line defines the name of the vertex shader. 
#pragma vertex TessellationVertexProgram
		// This line defines the name of the fragment shader. 
#pragma fragment frag
		// This line defines the name of the hull shader. 
#pragma hull hull
		// This line defines the name of the domain shader.
#pragma domain domain






	sampler2D _Noise;
	sampler2D _StackColor;
	float _Weight;

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

	// after tesselation
	Varyings vert(Attributes input)
	{
		Varyings output;
		float Noise = tex2Dlod(_Noise, float4(input.uv, 0, 0)).r;
		float StackColor = tex2Dlod(_StackColor, float4(input.uv, 0, 0)).r;

		input.vertex.xyz += (input.normal) * Noise * _Weight;
		output.vertex = TransformObjectToHClip(input.vertex.xyz);
		output.normal = input.normal;
		output.uv = input.uv;
		output.color = input.color;
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

	// The fragment shader definition.            
	half4 frag(Varyings IN) : SV_Target
	{
		half4 tex = tex2D(_StackColor, IN.uv);

		return tex;
	}
		ENDHLSL
	}
	}
}