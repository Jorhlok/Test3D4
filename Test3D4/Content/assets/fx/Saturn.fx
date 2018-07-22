#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VSInputVc
{
    float4 Position : POSITION;
    float4 Color    : COLOR;
};

struct VSInputTxVc
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD;
    float4 Color    : COLOR;
};

struct VSOutput
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
};

struct VSOutputTx
{
    float4 PositionPS : SV_Position;
    float4 Diffuse    : COLOR0;
    float2 TexCoord   : TEXCOORD0;
};


float4x4 Projection;
bool ScreenDoor;
int ScreenDoorScale;
texture Tex;
bool MagicColEnable;
float4 MagicCol;

sampler2D textureSampler = sampler_state {
    Texture = (Tex);
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float CalcScreenDoor(float2 pos)
{
	int checker = trunc(fmod(pos.x/ScreenDoorScale,2)) + trunc(fmod(pos.y/ScreenDoorScale,2));
	if (checker != 1) return 0;
	return 1;
}

VSOutputTx TexVertexShaderFunction(VSInputTxVc input)
{
    VSOutputTx output;
 
    output.PositionPS = mul(input.Position, Projection);
 
    output.Diffuse = input.Color;
 
    output.TexCoord = input.TexCoord;
    return output;
}
 
float4 TexPixelShaderFunction(VSOutputTx input) : COLOR
{
	if (ScreenDoor == true) clip(CalcScreenDoor(input.PositionPS.xy) - 1);
    float4 textureColor = tex2D(textureSampler, input.TexCoord);
	if (MagicColEnable == true && textureColor.r == MagicCol.r && textureColor.g == MagicCol.g && textureColor.b == MagicCol.b && textureColor.a == MagicCol.a) clip(-1);
	float4 col = input.Diffuse * 2 - 1;
	textureColor.rgb += col.rgb;
	textureColor.a *= col.a;
	if (textureColor.a <= 0) clip(-1);
    return saturate(textureColor);
}

VSOutput VertexShaderFunction(VSInputVc input)
{
    VSOutput output;
 
    output.PositionPS = mul(input.Position, Projection);
 
    output.Diffuse = input.Color;
    return output;
}
 
float4 PixelShaderFunction(VSOutput input) : COLOR
{
	if (ScreenDoor == true) clip(CalcScreenDoor(input.PositionPS.xy) - 1);
	if (input.Diffuse.a <= 0) clip(-1);
    return input.Diffuse;
}

technique Simple
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL TexVertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL TexPixelShaderFunction();
    }
	
	pass Pass2
	{
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}