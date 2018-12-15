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
flat/tex/clut   + 8*mode (0 flat, 1 tex, 2 tex clut)
tri/quad            + 4 for quad
screendoors         + 2
first/second tri    + 1

combined:
mode
x0, y0, z0
x1, y1, z1
x2, y2, z2
x3, y3, z3
u0, v0
u1, v1
u2, v2
u3, v3
r0, g0, b0, a0
r1, g1, b1, a1
r2, g2, b2, a2
r3, g3, b3, a3
c0 (r in flat) (offset in nonCLUT index)
c1 (g in flat) (half luminance for rgba textures?)
c2 (b in flat)
c3 (a in flat)
c4
c5
c6
c7
c8
c9
ca
cb
cc
cd
ce
cf

17 attributes if group clut
29 attributes
53 floats

packed vec:
vec4    vertex,mode
vec3    xyz0
vec3    xyz1
vec3    xyz2
vec3    xyz3
vec4    uv0
vec4    uv2
vec4    gouraud0
vec4    gouraud1
vec4    gouraud2
vec4    gouraud3
vec4    clut0
vec4    clut4
vec4    clut8
vec4    clutc

15 attr

gl 2.1  16 min
gles 2  8 min
gles 3  16 min
*/

struct VSInput
{
	float4 Position : POSITION;
	float3 XYZ0 : TEXCOORD0;
	float3 XYZ1 : TEXCOORD1;
	float3 XYZ2 : TEXCOORD2;
	float3 XYZ3 : TEXCOORD3;
	float4 UV0 : TEXCOORD4;
	float4 UV1 : TEXCOORD5;
	float4 GOURAUD0 : TEXCOORD6;
	float4 GOURAUD1 : TEXCOORD7;
	float4 GOURAUD2 : TEXCOORD8;
	float4 GOURAUD3 : TEXCOORD9;
	float4 CLUT0 : TEXCOORD10;
	float4 CLUT1 : TEXCOORD11;
	float4 CLUT2 : TEXCOORD12;
	float4 CLUT3 : TEXCOORD13;
};

struct VSOutput
{
	float3 Position : SV_Position;
	float3 XYZ0 : TEXCOORD0;
	float3 XYZ1 : TEXCOORD1;
	float3 XYZ2 : TEXCOORD2;
	float3 XYZ3 : TEXCOORD3;
	float4 UV0 : TEXCOORD4;
	float4 UV1 : TEXCOORD5;
	float4 GOURAUD0 : TEXCOORD6;
	float4 GOURAUD1 : TEXCOORD7;
	float4 GOURAUD2 : TEXCOORD8;
	float4 GOURAUD3 : TEXCOORD9;
	float4 CLUT0 : TEXCOORD10;
	float4 CLUT1 : TEXCOORD11;
	float4 CLUT2 : TEXCOORD12;
	float4 CLUT3 : TEXCOORD13;
	float  Mode : TEXCOORD14;
};

VSOutput VertexShaderFunction(VSInput input)
{
	VSOutput output;
	output.Position = input.Position.xyz;
	output.XYZ0 = input.XYZ0;
	output.XYZ1 = input.XYZ1;
	output.XYZ2 = input.XYZ2;
	output.XYZ3 = input.XYZ3;
	output.UV0 = input.UV0;
	output.UV1 = input.UV1;
	output.GOURAUD0 = input.GOURAUD0;
	output.GOURAUD1 = input.GOURAUD1;
	output.GOURAUD2 = input.GOURAUD2;
	output.GOURAUD3 = input.GOURAUD3;
	output.CLUT0 = input.CLUT0;
	output.CLUT1 = input.CLUT1;
	output.CLUT2 = input.CLUT2;
	output.CLUT3 = input.CLUT3;
	output.Mode = input.Position.w

	return output;
}

float4 PixelShaderFunction(VSOutput input) : COLOR
{
	return 1;
}

technique Saturnalia
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
