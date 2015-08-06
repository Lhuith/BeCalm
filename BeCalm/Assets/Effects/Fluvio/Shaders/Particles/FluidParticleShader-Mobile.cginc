#include "UnityCG.cginc"

// We need to use higher precisions than I'd like for some of these values in order to deal with issues on the iPhone 3GS/iPhone 4 and similar GPUs

uniform sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform float4 _GrabTex_ST;

struct v2fB {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;	
	float2 uvScreen : TEXCOORD1;	
	fixed4 color : COLOR0;
};
	
v2fB vertB (appdata_full v)
{
	v2fB o;

	// Position, UVs, vertex colors
	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uvScreen = ComputeGrabScreenPos (o.pos);
	o.uvScreen *= _GrabTex_ST.xy;
	o.uvScreen += _GrabTex_ST.zw;
	
	o.color = v.color;
	
	return o;
}