#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 InverseViewProjection;
float4x4 PrevViewProjection;

texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture bloomTexture;
sampler2D bloomTextureSampler = sampler_state
{
    Texture = (bloomTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

/*static const int kernelRadius = 6;
static const int kernelSize = 13;
static const float Kernel[kernelSize] = {
    0.002216, 0.008764, 0.026995, 0.064759, 0.120985, 0.176033, 0.199471, 0.176033, 0.120985, 0.064759, 0.026995, 0.008764, 0.002216,
};

static const int SAMPLE_COUNT = 10;
static const float RAND_SAMPLES[SAMPLE_COUNT] = {
    0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1
};*/


static const int kernelSize = 3;
static const float offset[kernelSize] = { 0.0, 1.3846153846, 3.2307692308 };
static const float weight[kernelSize] = { 0.2270270270, 0.3162162162, 0.0702702703 };

float2 screenSize;

// ===== FinalPass =====

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Position = input.Position;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 BloomPS(VertexShaderOutput input) : COLOR0
{
    float4 sceneColor = tex2D(textureSampler, input.TextureCoordinates);
    float4 bloomColor = tex2D(bloomTextureSampler, input.TextureCoordinates);
    return (sceneColor * 0.7) + bloomColor;
}

float4 BlurPS(in VertexShaderOutput input) : COLOR
{
    float4 tex = tex2D(textureSampler, input.TextureCoordinates);
    float depth = tex.w;
    float4 worldPos = float4(input.TextureCoordinates.x * 2.0f - 1.0f, input.TextureCoordinates.y * 2.0f - 1.0f, depth, 1);
    worldPos = mul(worldPos, InverseViewProjection);
    worldPos /= worldPos.w;

    // Prev UV
    float4 prevUV = mul(worldPos, PrevViewProjection);
    prevUV.xy = float2(0.5f, 0.5f) + float2(0.5f, 0.5f) * prevUV.xy / prevUV.w;
    float3 prevUVDepth = float3(prevUV.xy, prevUV.z / prevUV.w);

    // UV Movement vector
    float distanceToCenter = distance(input.TextureCoordinates, float2(0.5, 0.5));
    float centerDistanceFactor = max(0, distanceToCenter - 0.2);
    float2 blendVector = (input.TextureCoordinates - prevUVDepth.xy) * centerDistanceFactor;

    // Blur
    float4 blurredColor = float4(0, 0, 0, 1);
    for (int i = 0; i < kernelSize; i++)
    {
        float2 texCoord = input.TextureCoordinates + offset[i] * blendVector;
        blurredColor += tex2D(textureSampler, texCoord) * weight[i];
    }
    for (int i = 0; i < kernelSize; i++)
    {
        float2 texCoord = input.TextureCoordinates - offset[i] * blendVector;
        blurredColor += tex2D(textureSampler, texCoord) * weight[i];
    }

    return blurredColor;
}

// ===== Techniques =====

technique BloomPass
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL BloomPS();
    }
}

technique BlurPass
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL BlurPS();
    }
}