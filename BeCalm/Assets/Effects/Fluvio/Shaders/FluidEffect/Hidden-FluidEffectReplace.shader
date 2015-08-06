Shader "Hidden/Fluvio/FluidEffectReplace"
{
    Properties
    {
        // Diffuse/alpha
        [LM_Albedo] [LM_Transparency] _Color ("Color", Color) = (1,1,1,1)
        [LM_MasterTilingOffset] [LM_Albedo] _MainTex ("Albedo", 2D) = "white" {}
		[LM_TransparencyCutOff] _AlphaTestRef("Alpha Cutoff", Range(0.001, 1.0)) = 0.5

        // Specular/smoothness
        [LM_Glossiness] _Glossiness ("Smoothness", Range(0,1)) = 0.2
        [LM_Specular] _SpecColor ("Specular", Color) = (0.8,0.8,0.8,1)
        [LM_Specular] [LM_Glossiness] _SpecGlossMap ("Specular", 2D) = "white" {}
        
        // Normal
        _BumpScale("Scale", Float) = 1.0
        [LM_NormalMap] _BumpMap ("Normal Map", 2D) = "bump" {}
        
        // Emission
        [LM_Emission] _EmissionColor("Color", Color) = (0,0,0,0)
        [LM_Emission] _EmissionMap("Emission", 2D) = "white" {}

        // Vertex colors
        [KeywordEnum(None, Albedo, Specular, Emission)] _VertexColorMode ("Vertex color mode", Float) = 0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        ZWrite On
    
        Pass 
        {  
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                };

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    return o;
                }
            
                fixed4 frag (v2f i) : COLOR
                {
                    return 0;
                }
            ENDCG
        }

        Pass
        { 
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
 
			Fog {Mode Off}
			ZWrite On ZTest Less Cull Off
			Offset 1, 1
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

            sampler2D _MainTex;
            float _AlphaTestRef;

            struct v2f {
                V2F_SHADOW_CASTER; 
                float4 uv : TEXCOORD1;
            };
 
			v2f vert (appdata_full v)
			{
                v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			    return o;
			}
 
			float4 frag( v2f i ) : COLOR
			{
				fixed4 texcol = tex2D( _MainTex, i.uv );
				clip( texcol.a - _AlphaTestRef );
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
        } 
    }
    SubShader 
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True"}
        Fog { Mode Off }
        Lighting Off
        ZWrite Off
        ZTest LEqual

        Blend One One
    
        Pass { Color(0, 0, 0, 0) }
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Fluvio"  "PerformanceChecks"="False" }
        
        UsePass "Fluvio/Fluid Effect (Specular setup)/FORWARD"
        UsePass "Fluvio/Fluid Effect (Specular setup)/FORWARD_DELTA"
        
        Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles
			
			// -------------------------------------

            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "FluidEffectShadow.cginc"

			ENDCG
		}

        UsePass "Fluvio/Fluid Effect (Specular setup)/DEFERRED"
    }
}

