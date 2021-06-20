#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 ViewProjection;
float4x4 InverseTransposeWorld;
float4x4 World;
float4x4 LightViewProjection;

float3 ambientColor;
float3 diffuseColor;
float3 specularColor;
float KAmbient;
float KDiffuse;
float KSpecular;
float shininess;
//float3 lightPosition;
float3 lightDirection;
float3 cameraPosition;

float3 bloomColor;

texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture shadowMap;
sampler2D shadowMapSampler = sampler_state
{
    Texture = <shadowMap>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Border;
    AddressV = Border;
    BorderColor = 0xFFFFFFFF;
};
float2 shadowMapSize;

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
    output.ScreenSpacePosition = mul(input.Position, worldViewProjection);
    return output;
}

float4 DepthPS(in DepthPassVertexShaderOutput input) : COLOR
{
    float depth = input.ScreenSpacePosition.z / input.ScreenSpacePosition.w;
    return float4(depth, depth, depth, 1.0);
}

// ===== DrawShadowed =====

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinates : TEXCOORD0;
	float4 WorldSpacePosition : TEXCOORD1;
	float4 LightSpacePosition : TEXCOORD2;
    float4 Normal : TEXCOORD3;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, mul(World, ViewProjection));
    output.TextureCoordinates = input.TextureCoordinates;
    output.WorldSpacePosition = mul(input.Position, World);
    output.LightSpacePosition = mul(output.WorldSpacePosition, LightViewProjection);
    output.Normal = mul(float4(input.Normal, 1), InverseTransposeWorld);
    return output;
}

static const float modulatedEpsilon = 0.000709990182749889791011810302734375;
static const float maxEpsilon = 0.00023200045689009130001068115234375;

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Base vectors
    //float3 lightDirection = normalize(lightPosition - input.WorldSpacePosition.xyz);
    float3 viewDirection = normalize(cameraPosition - input.WorldSpacePosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);
    float3 normal = normalize(input.Normal.xyz);

    // Diffuse light
    float NdotL = saturate(dot(normal, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

    // Specular light
    float NdotH = dot(normal, halfVector);
    float3 specularLight = sign(NdotL) * KSpecular * specularColor * pow(saturate(NdotH), shininess);

    float3 lightSpacePosition = input.LightSpacePosition.xyz / input.LightSpacePosition.w;
    float2 shadowMapTextureCoordinates = 0.5 * lightSpacePosition.xy + float2(0.5, 0.5);
    shadowMapTextureCoordinates.y = 1.0f - shadowMapTextureCoordinates.y;

    float inclinationBias = min(modulatedEpsilon * (1.0 - NdotL), maxEpsilon); // Bias

    // Shadow Antialiasing
    float notInShadow = 0.0;
    float2 texelSize = 1.0 / shadowMapSize;
    for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
        {
            float pcfDepth = tex2D(shadowMapSampler, shadowMapTextureCoordinates + float2(x, y) * texelSize).r + inclinationBias;
            notInShadow += step(lightSpacePosition.z, pcfDepth) / 9.0;
        }

    // Without Shadow Antialiasing
    /*float shadowMapDepth = tex2D(shadowMapSampler, shadowMapTextureCoordinates).r + inclinationBias;
    float notInShadow = step(lightSpacePosition.z, shadowMapDepth);*/
    
    // Final calculation
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinates);

    float4 result = float4(saturate(ambientColor * KAmbient + diffuseLight) * textureColor.rgb + specularLight, textureColor.a);
    result.rgb *= 0.25 + 0.75 * notInShadow;
    return result;
}

// ===== BloomPass =====

struct BloomVertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
};

BloomVertexShaderOutput BloomVS(in VertexShaderInput input)
{
    BloomVertexShaderOutput output = (BloomVertexShaderOutput)0;
    output.Position = mul(mul(input.Position, World), ViewProjection);
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 BloomPS(BloomVertexShaderOutput input) : COLOR
{
    float4 color = tex2D(textureSampler, input.TextureCoordinates);
    float distanceToTargetColor = distance(color.rgb, float3(1, 0, 0));
    float filter = step(distanceToTargetColor, 0.2);
    return float4(bloomColor * filter, filter);
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
        VertexShader = compile VS_SHADERMODEL BloomVS();
        PixelShader = compile PS_SHADERMODEL BloomPS();
    }
};