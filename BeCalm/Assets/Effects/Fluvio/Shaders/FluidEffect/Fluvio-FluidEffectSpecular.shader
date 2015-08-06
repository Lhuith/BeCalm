Shader "Fluvio/Fluid Effect (Specular setup)"
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
        
        // UI-only data
        [HideInInspector] _EmissionScaleUI("Scale", Float) = 1.0
        [HideInInspector] _EmissionColorUI("Color", Color) = (0,0,0,0)
        
        // Image effect (passed to composite shader)
        [HideInInspector] _DownsampleFactorUI("Downsample Factor", Float) = 1.0
        [Toggle] [HideInInspector] _FluidBlurUI("Blur Fluid", Int) = 1
        [Toggle] [HideInInspector] _FluidBlurBackgroundUI("Blur Background", Int) = 0
        [HideInInspector] _FluidSpecularScaleUI("Fake Specular Effect", Range(0.0, 1.0)) = 0
        [HideInInspector] _FluidTintUI("Composite Tint Color", Color) = (1,1,1,1)     
    }

    CGINCLUDE
        #define _GLOSSYENV 1
        #define FLUVIO_SETUP_BRDF_INPUT FluvioSpecularSetup
    ENDCG

    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Fluvio" "PerformanceChecks"="False" }
        LOD 300

        // ------------------------------------------------------------------
        // Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD" 
            Tags { "LightMode" = "ForwardBase" }

            Blend SrcAlpha OneMinusSrcAlpha, One One
            ZWrite Off ZTest LEqual Cull Off
            
            CGPROGRAM
            #pragma target 3.0
            // GLES2.0 disabled to prevent errors spam on devices without textureCubeLodEXT
            #pragma exclude_renderers gles
            
            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _EMISSION
            // ALWAYS ON shader_feature _GLOSSYENV
            #pragma shader_feature _SPECGLOSSMAP 
            
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
                
            #pragma vertex FluvioVertForwardBase
            #pragma fragment FluvioFragForwardBase

            #include "FluidEffect.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend SrcAlpha One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off ZTest LEqual Cull Off
           
            CGPROGRAM
            #pragma target 3.0
            // GLES2.0 disabled to prevent errors spam on devices without textureCubeLodEXT
            #pragma exclude_renderers gles
            
            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

            #pragma shader_feature _NORMALMAP
            // ALWAYS ON shader_feature _GLOSSYENV
            #pragma shader_feature _SPECGLOSSMAP
            
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            
            #pragma vertex FluvioVertForwardAdd
            #pragma fragment FluvioFragForwardAdd

            #include "FluidEffect.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
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
        // ------------------------------------------------------------------
        //  Deferred pass
        Pass
        {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            ZWrite Off ZTest LEqual Cull Off
            
            CGPROGRAM
			#pragma target 3.0
			// GLES2.0 disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers nomrt gles
            
            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _EMISSION
			// ALWAYS ON shader_feature _GLOSSYENV
			#pragma shader_feature _SPECGLOSSMAP
            
			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            
			#pragma vertex FluvioVertDeferred
			#pragma fragment FluvioFragDeferred

			#include "FluidEffect.cginc"
            ENDCG
        }
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Fluvio" "PerformanceChecks"="False" }
        LOD 150

        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD" 
            Tags { "LightMode" = "ForwardBase" }

            Blend SrcAlpha OneMinusSrcAlpha, One One
            ZWrite Off ZTest LEqual Cull Off
            
            CGPROGRAM
            #pragma target 2.0

            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _EMISSION
            // ALWAYS ON shader_feature _GLOSSYENV
            #pragma shader_feature _SPECGLOSSMAP 
            #pragma skip_variants SHADOWS_SOFT
            #pragma skip_variants DYNAMICLIGHTMAP_ON
            
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
    
            #pragma vertex FluvioVertForwardBase
            #pragma fragment FluvioFragForwardBase

            #include "FluidEffect.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend SrcAlpha One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off ZTest LEqual Cull Off
            
            CGPROGRAM
            #pragma target 2.0
           
           #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _SPECGLOSSMAP
            #pragma skip_variants SHADOWS_SOFT
            
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            
            #pragma vertex FluvioVertForwardAdd
            #pragma fragment FluvioFragForwardAdd

            #include "FluidEffect.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On ZTest LEqual
            
            CGPROGRAM
            #pragma target 2.0

            #pragma multi_compile _VERTEXCOLORMODE_NONE _VERTEXCOLORMODE_ALBEDO _VERTEXCOLORMODE_SPECULAR _VERTEXCOLORMODE_EMISSION

            #pragma skip_variants SHADOWS_SOFT
            #pragma multi_compile_shadowcaster

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }
    }
    CustomEditor "Thinksquirrel.FluvioEditor.Inspectors.FluidEffectShaderInspector"
}

