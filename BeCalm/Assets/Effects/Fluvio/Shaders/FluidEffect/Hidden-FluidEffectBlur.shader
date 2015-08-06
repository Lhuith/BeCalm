Shader "Hidden/Fluvio/FluidEffectBlur" {
	Properties { _MainTex ("Base (RGB)", 2D) = "white" {} }
	
    CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half2 blurCoordinates[5] : TEXCOORD1; 
	};
	
    sampler2D _MainTex;
	half4 _MainTex_TexelSize;
	
    v2f vert( appdata_img v ) {
		v2f o; 
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv = v.texcoord.xy;
        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0)
            o.uv.y = 1-o.uv.y;
        #endif
		o.blurCoordinates[0] = o.uv;
		o.blurCoordinates[1] = o.uv + _MainTex_TexelSize.xy * 1.407333;
		o.blurCoordinates[2] = o.uv - _MainTex_TexelSize.xy * 3.294215;
		o.blurCoordinates[3] = o.uv + _MainTex_TexelSize.xy * 3.294215;
        o.blurCoordinates[4] = o.uv - _MainTex_TexelSize.xy * 3.294215;
		return o;
	}
	half4 frag(v2f i) : COLOR {
		half4 color = half4(0,0,0,0);
        color += tex2D(_MainTex, i.blurCoordinates[0]) * 0.204164;
        color += tex2D(_MainTex, i.blurCoordinates[1]) * 0.304005;
		color += tex2D(_MainTex, i.blurCoordinates[2]) * 0.304005;
		color += tex2D(_MainTex, i.blurCoordinates[3]) * 0.093913;
		color += tex2D(_MainTex, i.blurCoordinates[4]) * 0.093913; 
		return color;
	}
	ENDCG
	
    SubShader {
		 Pass {
			  ZTest Always Cull Off ZWrite Off
			  Fog { Mode off }      

			  CGPROGRAM
			  #pragma fragmentoption ARB_precision_hint_fastest
			  #pragma vertex vert
			  #pragma fragment frag
			  ENDCG
		  }
	}
	Fallback off
}
