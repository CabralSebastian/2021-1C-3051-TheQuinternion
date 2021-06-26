#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 ViewProjection;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
};

// ===== DepthPass =====

struct DepthPassVertexShaderInput
{
	float4 Position : POSITION0;
};

struct DepthPassVertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 ScreenSpacePosition : TEXCOORD1;
};

DepthPassVertexShaderOutput DepthVS(in DepthPassVertexShaderInput input)
{
	DepthPassVertexShaderOutput output;
	matrix worldViewProjection = mul(World, ViewProjection);
	output.Position = mul(input.Position, worldViewProjection);
	output.ScreenSpacePosition = output.Position;
	return output;
}

float4 DepthPS(DepthPassVertexShaderOutput input) : COLOR
{
	float depth = input.ScreenSpacePosition.z / input.ScreenSpacePosition.w;
	return float4(depth, depth, depth, 1.0);
}

// ===== DrawShadowed =====

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
    float4 worldPosition = mul(input.Position, World);
	output.Position = mul(worldPosition, ViewProjection);
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return float4(1, 0, 0, 1);
}

// ===== BloomPass =====

float4 BloomPS(VertexShaderOutput input) : COLOR
{
	return float4(1, 0, 0, 1);
}

// ===== Techniques =====

technique DepthPass
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL DepthVS();
		PixelShader = compile PS_SHADERMODEL DepthPS();
	}
};

technique DrawShadowed
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

technique BloomPass
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL BloomPS();
	}
};