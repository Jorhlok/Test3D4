#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

/*
modes:
flat/tex/clut		+ 4*mode (0 flat, 1 tex, 2 tex clut)
tri/quad            + 2 for quad
screendoors         + 1

minimum maximum attributes
gl 2.1  16 min
gles 2  8 min
gles 3  16 min


even smaller:
vec3    vertex
vec4    xy0, xy1
vec4    xy2, xy3
vec4    uv0 (flat color for untextured)
vec4    uv2
vec4    gouraud (each packed into 15 bits or up to 24 bits safely, rgba6666)
vec4    modes (mode, half lumi/shading?, color bank, CLUT index)

7 attr (compatible with gles 2 and webgl 1?)
*/

struct VSInput
{
	float4 Position : POSITION;
	float4 XY01 : TEXCOORD0;
	float4 XY23 : TEXCOORD1;
	float4 UV01 : TEXCOORD2; //flat color for untextured
	float4 UV23 : TEXCOORD3;
	float4 Gouraud : TEXCOORD4;
	float4 Modes : TEXCOORD5; //mode, color bank, clut, color calc
};

struct VSOutput
{
	float4 Position : SV_Position;
	float4 XY01 : TEXCOORD0;
	float4 XY23 : TEXCOORD1;
	float4 UV01 : TEXCOORD2;
	float4 UV23 : TEXCOORD3;
	float4 Gouraud : TEXCOORD4;
	float4 Modes : TEXCOORD5;
};

float4x4 Projection;
float3 Screendoors; //A alpha, B alpha, size

texture Tex; //texture atlas
sampler2D textureSampler = sampler_state {
	Texture = (Tex);
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture ColorTable; //1x32768
sampler2D colorSampler = sampler_state {
	Texture = (ColorTable);
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

int CLUTLen;
texture CLUT; //16xCLUTLen
sampler2D clutSampler = sampler_state {
	Texture = (CLUT);
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};


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

float2 invBilinear(float3 pt, float2 a, float2 b, float2 c, float2 d) {
	float2 p = pt.xy;
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


VSOutput VertexShaderFunction(VSInput input)
{
	VSOutput output;
	output.Position = mul(input.Position, Projection);
	output.XY01 = input.XY01;
	output.XY23 = input.XY23;
	output.UV01 = input.UV01;
	output.UV23 = input.UV23;
	output.Gouraud = input.Gouraud;
	output.Modes = input.Modes;

	return output;
}

float4 PixelShaderFunction(VSOutput input) : COLOR
{
	return input.Gouraud;
}

technique Saturnalia
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
