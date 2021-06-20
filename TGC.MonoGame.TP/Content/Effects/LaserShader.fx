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

VertexShaderOutput DepthVS(in VertexShaderInput input)
{
	return (VertexShaderOutput)0;
}

float4 DepthPS(VertexShaderOutput input) : COLOR
{
	discard;
	return float4(0, 0, 0, 0);
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
