﻿#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VSInput
{
	float4 Position : POSITION;
};

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

struct VSInputBatch
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
};

struct VSOutput
{
	float4 PositionPS : SV_Position;
};

struct VSOutputVc
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

struct VSOutputBatch
{
	float4 PositionPS : SV_Position;
	float2 TexCoord   : TEXCOORD0;
};


float4x4 Projection;
bool ScreenDoor;
int ScreenDoorScale;
texture Tex;
bool MagicColEnable;
float4 MagicCol;

/********
A****B
*    *
*    *
D****C
********/
float3 VertexA;
float3 VertexB;
float3 VertexC;
float3 VertexD;

float2 UVA;
float2 UVB;
float2 UVC;
float2 UVD;

float4 GouraudA;
float4 GouraudB;
float4 GouraudC;
float4 GouraudD;

float4 PrimitiveColor;

sampler2D textureSampler = sampler_state {
	Texture = (Tex);
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};


float BatchSize;
texture Quads;
sampler2D quadSampler = sampler_state {
	Texture = (Quads);
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

float CalcScreenDoor(float2 pos)
{
	int checker = trunc(fmod(pos.x / ScreenDoorScale, 2)) + trunc(fmod(pos.y / ScreenDoorScale, 2));
	if (checker != 1) return 0;
	return 1;
}

//original from https://www.shadertoy.com/view/lsBSDm
// Inverse bilinear interpolation: given four points defining a quadrilateral, compute the uv
// coordinates of any point in the plane that would give result to that point as a bilinear 
// interpolation of the four points.
//
// The problem can be solved through a quadratic equation. More information in this article:
//
// http://www.iquilezles.org/www/articles/ibilinear/ibilinear.htm
//!!!!!!!!!!!!!!!!!!!!!!!!
/*fix from page https://www.shadertoy.com/view/lsBSDm
Hackerham, 2017-08-07
Made a fix: https://gist.github.com/ivanpopelyshev/2a75479075286deb8ee5dc1fb2e07f09
*/
//!!!!!!!!!!!!!!!!!!!!!!!!
//As seen in: https://www.shadertoy.com/view/MsccD4
float cross2d(float2 a, float2 b) { return a.x*b.y - a.y*b.x; }
float2 invBilinear(float3 pt) {
	float2 p = pt;
	//p.x -= 0.4;
	//p.y -= 0.4;
	float2 a = VertexA.xy;
	float2 b = VertexB.xy;
	float2 c = VertexC.xy;
	float2 d = VertexD.xy;
	float2 e = b - a;
	float2 f = d - a;
	float2 g = a - b + c - d;
	float2 h = p.xy - a;

	float k2 = cross2d(g, f);
	float k1 = cross2d(e, f) + cross2d(h, g);
	float k0 = cross2d(h, e);

	float k2u = cross2d(e, g);
	float k1u = cross2d(e, f) + cross2d(g, h);
	float k0u = cross2d(h, f);

	float v1, u1, v2, u2;

	if (abs(k2) < 1e-5)
	{
		v1 = -k0 / k1;
		u1 = (h.x - f.x*v1) / (e.x + g.x*v1);
	}
	else if (abs(k2u) < 1e-5)
	{
		u1 = k0u / k1u;
		v1 = (h.y - e.y*u1) / (f.y + g.y*u1);
	}
	else
	{
		float w = k1 * k1 - 4.0*k0*k2;

		if (w<0.0) return -1;

		w = sqrt(w);

		v1 = (-k1 - w) / (2.0*k2);
		v2 = (-k1 + w) / (2.0*k2);
		u1 = (-k1u - w) / (2.0*k2u);
		u2 = (-k1u + w) / (2.0*k2u);
	}
	bool  b1 = v1>0.0 && v1<1.0 && u1>0.0 && u1<1.0;
	bool  b2 = v2>0.0 && v2<1.0 && u2>0.0 && u2<1.0;

	float2 res = -1;

	if (b1 /*&& !b2*/) res = float2(u1, v1);
	if (!b1 &&  b2) res = float2(u2, v2);

	return res;
}
float2 invBilinear(float3 pt, float2 a, float2 b, float2 c, float2 d) {
	float2 p = pt;
	//p.x -= 0.4;
	//p.y -= 0.4;
	//float2 a = VertexA.xy;
	//float2 b = VertexB.xy;
	//float2 c = VertexC.xy;
	//float2 d = VertexD.xy;
	float2 e = b - a;
	float2 f = d - a;
	float2 g = a - b + c - d;
	float2 h = p.xy - a;

	float k2 = cross2d(g, f);
	float k1 = cross2d(e, f) + cross2d(h, g);
	float k0 = cross2d(h, e);

	float k2u = cross2d(e, g);
	float k1u = cross2d(e, f) + cross2d(g, h);
	float k0u = cross2d(h, f);

	float v1, u1, v2, u2;

	if (abs(k2) < 1e-5)
	{
		v1 = -k0 / k1;
		u1 = (h.x - f.x*v1) / (e.x + g.x*v1);
	}
	else if (abs(k2u) < 1e-5)
	{
		u1 = k0u / k1u;
		v1 = (h.y - e.y*u1) / (f.y + g.y*u1);
	}
	else
	{
		float w = k1 * k1 - 4.0*k0*k2;

		if (w<0.0) return -1;

		w = sqrt(w);

		v1 = (-k1 - w) / (2.0*k2);
		v2 = (-k1 + w) / (2.0*k2);
		u1 = (-k1u - w) / (2.0*k2u);
		u2 = (-k1u + w) / (2.0*k2u);
	}
	bool  b1 = v1>0.0 && v1<1.0 && u1>0.0 && u1<1.0;
	bool  b2 = v2>0.0 && v2<1.0 && u2>0.0 && u2<1.0;

	float2 res = -1;

	if (b1 /*&& !b2*/) res = float2(u1, v1);
	if (!b1 &&  b2) res = float2(u2, v2);

	return res;
}

float4 bilinearGouraud(float2 uv) {
	return float4(GouraudA.r * ((1 - uv.x) * (1 - uv.y))
		+ GouraudB.r * (uv.x * (1 - uv.y))
		+ GouraudC.r * (uv.x  * uv.y)
		+ GouraudD.r * ((1 - uv.x) * uv.y)
		, GouraudA.g * ((1 - uv.x) * (1 - uv.y))
		+ GouraudB.g * (uv.x * (1 - uv.y))
		+ GouraudC.g * (uv.x  * uv.y)
		+ GouraudD.g * ((1 - uv.x) * uv.y)
		, GouraudA.b * ((1 - uv.x) * (1 - uv.y))
		+ GouraudB.b * (uv.x * (1 - uv.y))
		+ GouraudC.b * (uv.x  * uv.y)
		+ GouraudD.b * ((1 - uv.x) * uv.y)
		, GouraudA.a * ((1 - uv.x) * (1 - uv.y))
		+ GouraudB.a * (uv.x * (1 - uv.y))
		+ GouraudC.a * (uv.x  * uv.y)
		+ GouraudD.a * ((1 - uv.x) * uv.y));
}
float4 bilinearGouraud(float2 uv, float4 A, float4 B, float4 C, float4 D) {
	return float4(A.r * ((1 - uv.x) * (1 - uv.y))
		+ B.r * (uv.x * (1 - uv.y))
		+ C.r * (uv.x  * uv.y)
		+ D.r * ((1 - uv.x) * uv.y)
		, A.g * ((1 - uv.x) * (1 - uv.y))
		+ B.g * (uv.x * (1 - uv.y))
		+ C.g * (uv.x  * uv.y)
		+ D.g * ((1 - uv.x) * uv.y)
		, A.b * ((1 - uv.x) * (1 - uv.y))
		+ B.b * (uv.x * (1 - uv.y))
		+ C.b * (uv.x  * uv.y)
		+ D.b * ((1 - uv.x) * uv.y)
		, A.a * ((1 - uv.x) * (1 - uv.y))
		+ B.a * (uv.x * (1 - uv.y))
		+ C.a * (uv.x  * uv.y)
		+ D.a * ((1 - uv.x) * uv.y));
}

float2 bilinearUV(float2 uv) {
	return float2(UVA.x * ((1 - uv.x) * (1 - uv.y))
		+ UVB.x * (uv.x * (1 - uv.y))
		+ UVC.x * (uv.x  * uv.y)
		+ UVD.x * ((1 - uv.x) * uv.y)
		, UVA.y * ((1 - uv.x) * (1 - uv.y))
		+ UVB.y * (uv.x * (1 - uv.y))
		+ UVC.y * (uv.x  * uv.y)
		+ UVD.y * ((1 - uv.x) * uv.y));
}
float2 bilinearUV(float2 uv, float2 A, float2 B, float2 C, float2 D) {
	return float2(A.x * ((1 - uv.x) * (1 - uv.y))
		+ B.x * (uv.x * (1 - uv.y))
		+ C.x * (uv.x  * uv.y)
		+ D.x * ((1 - uv.x) * uv.y)
		, A.y * ((1 - uv.x) * (1 - uv.y))
		+ B.y * (uv.x * (1 - uv.y))
		+ C.y * (uv.x  * uv.y)
		+ D.y * ((1 - uv.x) * uv.y));
}


VSOutput PosVertexShaderFunction(VSInput input)
{
	VSOutput output;

	output.PositionPS = mul(input.Position, Projection);
	return output;
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
	textureColor.rgb += input.Diffuse.rgb - 0.5;
	textureColor.a *= input.Diffuse.a;
	if (textureColor.a <= 0) clip(-1);
	return saturate(textureColor);
}

float4 TexPixelShaderFunction2(VSOutput input) : COLOR
{
	if (ScreenDoor == true) clip(CalcScreenDoor(input.PositionPS.xy) - 1);
	float2 uv = invBilinear(input.PositionPS);
	if (uv.x < -0.5 || uv.y < -0.5) clip(-1);
	//if (uv.x < -0.5 || uv.y < -0.5) return float4(1, 0, 1, 1);
	float4 textureColor = tex2D(textureSampler, bilinearUV(uv));
	if (MagicColEnable == true && textureColor.r == MagicCol.r && textureColor.g == MagicCol.g && textureColor.b == MagicCol.b && textureColor.a == MagicCol.a) clip(-1);
	float4 col = bilinearGouraud(uv);
	textureColor.rgb += col.rgb - 0.5;
	textureColor.a *= col.a;
	if (textureColor.a <= 0) clip(-1);
	return saturate(textureColor);
}

VSOutputVc VertexShaderFunction(VSInputVc input)
{
	VSOutputVc output;

	output.PositionPS = mul(input.Position, Projection);

	output.Diffuse = input.Color;
	return output;
}

float4 PixelShaderFunction(VSOutputVc input) : COLOR
{
	if (ScreenDoor == true) clip(CalcScreenDoor(input.PositionPS.xy) - 1);
	if (input.Diffuse.a <= 0) clip(-1);
	return input.Diffuse;
}


VSOutputBatch VertexShaderBatch(VSInputBatch input)
{
	VSOutputBatch output;

	output.PositionPS = mul(input.Position, Projection);
	output.TexCoord = input.TexCoord;
	return output;
}

float4 PixelShaderBatch(VSOutputBatch input) : COLOR
{
	float2 uv = 1.0 / 16;
	uv.y = input.TexCoord.y;
	float4 pos0 = tex2D(quadSampler, uv);
	uv.x += 1.0 / 8;
	float4 pos1 = tex2D(quadSampler, uv);
	float2 invbi = invBilinear(input.PositionPS, pos0.xy, pos0.zw, pos1.xy, pos1.zw);
	if (invbi.x < -0.5 || invbi.y < -0.5) clip(-1);
	//if (invbi.x < -0.5 || invbi.y < -0.5) return float4(1, 0, 1, 1);
	uv.x += 1.0 / 8;
	float4 uv0 = tex2D(quadSampler, uv);
	uv.x += 1.0 / 8;
	float4 uv1 = tex2D(quadSampler, uv);
	float4 textureColor = tex2D(textureSampler, bilinearUV(invbi, uv0.xy, uv0.zw, uv1.xy, uv1.zw));
	if (MagicColEnable == true && textureColor.r == MagicCol.r && textureColor.g == MagicCol.g && textureColor.b == MagicCol.b && textureColor.a == MagicCol.a) clip(-1);
	uv.x += 1.0 / 8;
	float4 col0 = tex2D(quadSampler, uv);
	uv.x += 1.0 / 8;
	float4 col1 = tex2D(quadSampler, uv);
	uv.x += 1.0 / 8;
	float4 col2 = tex2D(quadSampler, uv);
	uv.x += 1.0 / 8;
	float4 col3 = tex2D(quadSampler, uv);
	float4 col = saturate(bilinearGouraud(invbi,col0,col1,col2,col3));
	textureColor.rgb += col.rgb - 0.5;
	textureColor.a *= col.a;
	if (textureColor.a <= 0) clip(-1);
	return saturate(textureColor);
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

	pass Pass3
	{
		VertexShader = compile VS_SHADERMODEL PosVertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL TexPixelShaderFunction2();
	}

	pass Pass4
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderBatch();
		PixelShader = compile PS_SHADERMODEL PixelShaderBatch();
	}
}