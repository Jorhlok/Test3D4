#if OPENGL
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
	p.x -= 0.4;
	float2 a = VertexD.xy;
	float2 b = VertexC.xy;
	float2 c = VertexB.xy;
	float2 d = VertexA.xy;
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

	if (b1 && !b2) res = float2(u1, v1 * -1 + 1);
	if ( /*!b1 &&  */b2) res = float2(u2, v2 * -1 + 1);

	return res;
}

float4 bilinearGouraud(float2 uv) {
	return GouraudA * ((1 - uv.x) * (1 - uv.y))
		+ GouraudB * (uv.x * (1 - uv.y))
		+ GouraudD * (uv.x  * uv.y)
		+ GouraudC * ((1 - uv.x) * uv.y);
}

float2 bilinearUV(float2 uv) {
	return UVA * ((1 - uv.x) * (1 - uv.y))
		+ UVB * (uv.x * (1 - uv.y))
		+ UVD * (uv.x  * uv.y)
		+ UVC * ((1 - uv.x) * uv.y);
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

float4 TexPixelShaderFunction2(VSOutputTx input) : COLOR
{
	if (ScreenDoor == true) clip(CalcScreenDoor(input.PositionPS.xy) - 1);
	float2 uv = invBilinear(input.PositionPS);
	if (uv.x < -0.5 || uv.y < -0.5) return float4(1,0,1,1); //clip(-1);
	//return saturate(float4(uv.x,uv.y,0,1));
	float4 textureColor = tex2D(textureSampler, uv); // bilinearUV(uv));
	if (MagicColEnable == true && textureColor.r == MagicCol.r && textureColor.g == MagicCol.g && textureColor.b == MagicCol.b && textureColor.a == MagicCol.a) clip(-1);
	//float4 col = bilinearGouraud(uv);
	//textureColor.rgb += (col.rgb - 0.5);
	//textureColor.a *= col.a;
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
		VertexShader = compile VS_SHADERMODEL TexVertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL TexPixelShaderFunction2();
	}
}