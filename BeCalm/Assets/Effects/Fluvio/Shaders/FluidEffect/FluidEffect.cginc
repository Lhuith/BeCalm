#include "UnityStandardCore.cginc"

half _AlphaTestRef;

#define FLUVIO_FRAGMENT_SETUP(x) FragmentCommonData x = \
	FluvioFragmentSetup(i.tex, i.eyeVec, WorldNormal(i.tangentToWorldAndColor), ExtractTangentToWorldPerPixel(i.tangentToWorldAndColor), IN_WORLDPOS(i), vertex_color);

#define FLUVIO_FRAGMENT_SETUP_FWDADD(x) FragmentCommonData x = \
	FluvioFragmentSetup(i.tex, i.eyeVec, WorldNormal(i.tangentToWorldAndLightDir), ExtractTangentToWorldPerPixel(i.tangentToWorldAndLightDir), half3(0,0,0), half3(0,0,0));

#ifndef FLUVIO_SETUP_BRDF_INPUT
	#define FLUVIO_SETUP_BRDF_INPUT FluvioSpecularSetup
#endif

// ------------------------------------------------------------------
// Custom structs
struct FluvioVertexInput
{
	float4 vertex	: POSITION;
	half3 normal	: NORMAL;
	float4 color	: COLOR;
	float2 uv0		: TEXCOORD0;
	#ifdef _TANGENT_TO_WORLD
		half4 tangent	: TANGENT;
	#endif
};

// ------------------------------------------------------------------
// Fluvio-specific helper functions
int _Fluvio_OverrideNormals = 1;
float4x4 _Fluvio_FluidToObject;
float3 FluvioGetWorldNormal(float3 norm)
{
	if (_Fluvio_OverrideNormals)
		norm = mul((float3x3)_Fluvio_FluidToObject, float3(0,0,1));

	return UnityObjectToWorldNormal(norm);
}
float4 FluvioGetWorldTangent(float4 dir)
{
	if (_Fluvio_OverrideNormals)
		dir = float4(mul((float3x3)_Fluvio_FluidToObject, float3(1,0,0)), -1);

	return float4(UnityObjectToWorldDir(dir.xyz), dir.w);
}
half FluvioAlpha(float2 texcoords)
{
	return tex2D(_MainTex, texcoords).a;
}		
half3 FluvioEmission(float2 uv, float3 color)
{
	#ifndef _EMISSION
		return 0;
	#else
		return tex2D(_EmissionMap, uv).rgb * _EmissionColor.rgb * color;
	#endif
}
half3 FluvioAlbedo(float2 texcoords, float3 color)
{
	return _Color.rgb * tex2D (_MainTex, texcoords).rgb * color;
}
float2 FluvioTexCoords(FluvioVertexInput v)
{
	return TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
}
half4 FluvioOutputForward (half3 color, half alpha)
{
	return half4(color, alpha);
}
inline FragmentCommonData FluvioSpecularSetup (float2 i_tex, half3 vertex_color)
{
	half4 specGloss = SpecularGloss(i_tex);
	
    #ifdef _VERTEXCOLORMODE_ALBEDO
        half3 albedo = FluvioAlbedo(i_tex, vertex_color);
    #else
        half3 albedo = FluvioAlbedo(i_tex, float3(1,1,1));
    #endif
    
    #ifdef _VERTEXCOLORMODE_SPECULAR
        half3 specColor = specGloss.rgb * vertex_color;
    #else
        half3 specColor = specGloss.rgb;
    #endif

	half oneMinusRoughness = specGloss.a;

	half oneMinusReflectivity;

    half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (albedo, specColor, /*out*/ oneMinusReflectivity);
	
    FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.oneMinusRoughness = oneMinusRoughness;
	return o;
}

#ifdef _NORMALMAP
half3 FluvioNormalInTangentSpace(float2 texcoords)
{
	return UnpackScaleNormal(tex2D (_BumpMap, texcoords), _BumpScale);
}
#endif

inline FragmentCommonData FluvioFragmentSetup (float2 i_tex, half3 i_eyeVec, half3 i_normalWorld, half3x3 i_tanToWorld, half3 i_posWorld, half3 vertex_color)
{
	half alpha = FluvioAlpha(i_tex);
	
    clip(alpha - 0.001f);

	#ifdef _NORMALMAP
		half3 normalWorld = NormalizePerPixelNormal(mul(FluvioNormalInTangentSpace(i_tex), i_tanToWorld));
	#else
		// Should get compiled out, isn't being used in the end.
	 	half3 normalWorld = i_normalWorld;
	#endif
	
	half3 eyeVec = i_eyeVec;
	eyeVec = NormalizePerPixelNormal(eyeVec);
	
	FragmentCommonData o = FLUVIO_SETUP_BRDF_INPUT(i_tex, vertex_color);
	o.normalWorld = normalWorld;
	o.eyeVec = eyeVec;
	o.posWorld = i_posWorld;
	o.alpha = alpha;
	return o;
}

// ------------------------------------------------------------------
// Forward base
struct FluvioVertexOutputForwardBase
{
	float4 pos							: SV_POSITION;
	float2 tex							: TEXCOORD0;
	half3 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndColor[3]		: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:color]
	half4 ambientOrLightmapUV			: TEXCOORD5;	// SH or Lightmap UV
	SHADOW_COORDS(6)
	UNITY_FOG_COORDS(7)

	// next ones would not fit into SM2.0 limits, but they are always for SM3.0+
	#if UNITY_SPECCUBE_BOX_PROJECTION
		float3 posWorld					: TEXCOORD8;
	#endif
};

FluvioVertexOutputForwardBase FluvioVertForwardBase (FluvioVertexInput v)
{
	FluvioVertexOutputForwardBase o;
	UNITY_INITIALIZE_OUTPUT(FluvioVertexOutputForwardBase, o);

	float4 posWorld = mul(_Object2World, v.vertex);
	#if UNITY_SPECCUBE_BOX_PROJECTION
		o.posWorld = posWorld.xyz;
	#endif
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.tex = FluvioTexCoords(v);
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = FluvioGetWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = FluvioGetWorldTangent(v.tangent);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndColor[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndColor[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndColor[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndColor[0].xyz = 0;
		o.tangentToWorldAndColor[1].xyz = 0;
		o.tangentToWorldAndColor[2].xyz = normalWorld;
	#endif
		
    #ifdef _VERTEXCOLORMODE_NONE
	    o.tangentToWorldAndColor[0].w = 1;
	    o.tangentToWorldAndColor[1].w = 1;
	    o.tangentToWorldAndColor[2].w = 1;    
    #else
        o.tangentToWorldAndColor[0].w = v.color.r;
	    o.tangentToWorldAndColor[1].w = v.color.g;
	    o.tangentToWorldAndColor[2].w = v.color.b;
    #endif
	
    //We need this for shadow receving
	TRANSFER_SHADOW(o);

	// Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
	#if UNITY_SHOULD_SAMPLE_SH
		#if UNITY_SAMPLE_FULL_SH_PER_PIXEL
			o.ambientOrLightmapUV.rgb = 0;
		#elif (SHADER_TARGET < 30)
			o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
		#else
			// Optimization: L2 per-vertex, L0..L1 per-pixel
			o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
		#endif
		// Add approximated illumination from non-important point lights
		#ifdef VERTEXLIGHT_ON
			o.ambientOrLightmapUV.rgb += Shade4PointLights (
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, posWorld, normalWorld);
		#endif
	#endif
	
	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}


half4 FluvioFragForwardBase (FluvioVertexOutputForwardBase i) : SV_Target
{
    // FLUVIO - VERTEX COLORS
	half3 vertex_color = half3(i.tangentToWorldAndColor[0].w, i.tangentToWorldAndColor[1].w, i.tangentToWorldAndColor[2].w);
	
	FLUVIO_FRAGMENT_SETUP(s)
	
    UnityLight mainLight = MainLight (s.normalWorld);
	half atten = SHADOW_ATTENUATION(i);
	
	half occlusion = 1;

	UnityGI gi = FragmentGI (
		s.posWorld, occlusion, i.ambientOrLightmapUV, atten, s.oneMinusRoughness, s.normalWorld, s.eyeVec, mainLight);

    half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);

	#ifdef _VERTEXCOLORMODE_EMISSION
        c.rgb += FluvioEmission(i.tex, vertex_color);
    #else
        c.rgb += FluvioEmission(i.tex, float3(1,1,1));
    #endif

	UNITY_APPLY_FOG(i.fogCoord, c.rgb);
	return FluvioOutputForward (c, s.alpha);
}

// ------------------------------------------------------------------
// Forward add
struct FluvioVertexOutputForwardAdd
{
	float4 pos							: SV_POSITION;
	float2 tex							: TEXCOORD0;
	half3 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndLightDir[3]	: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:lightDir]
	LIGHTING_COORDS(5,6)
	UNITY_FOG_COORDS(7)
};

FluvioVertexOutputForwardAdd FluvioVertForwardAdd (FluvioVertexInput v)
{
	FluvioVertexOutputForwardAdd o;
	UNITY_INITIALIZE_OUTPUT(FluvioVertexOutputForwardAdd, o);

	float4 posWorld = mul(_Object2World, v.vertex);
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.tex = FluvioTexCoords(v);
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = FluvioGetWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = FluvioGetWorldTangent(v.tangent);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndLightDir[0].xyz = 0;
		o.tangentToWorldAndLightDir[1].xyz = 0;
		o.tangentToWorldAndLightDir[2].xyz = normalWorld;
	#endif
	//We need this for shadow receving
	TRANSFER_VERTEX_TO_FRAGMENT(o);

	float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
	#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = NormalizePerVertexNormal(lightDir);
	#endif
	o.tangentToWorldAndLightDir[0].w = lightDir.x;
	o.tangentToWorldAndLightDir[1].w = lightDir.y;
	o.tangentToWorldAndLightDir[2].w = lightDir.z;
	
	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}

half4 FluvioFragForwardAdd (FluvioVertexOutputForwardAdd i) : SV_Target
{
	FLUVIO_FRAGMENT_SETUP_FWDADD(s)

	UnityLight light = AdditiveLight (s.normalWorld, IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));
	UnityIndirect noIndirect = ZeroIndirect ();

	half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, light, noIndirect);
	
	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
	return FluvioOutputForward (c, s.alpha);
}

// ------------------------------------------------------------------
// Deferred
struct FluvioVertexOutputDeferred
{
	float4 pos							: SV_POSITION;
	float2 tex							: TEXCOORD0;
	half3 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndColor[3]		: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:color]
	half4 ambientOrLightmapUV			: TEXCOORD5;	// SH or Lightmap UVs			
	#if UNITY_SPECCUBE_BOX_PROJECTION
		float3 posWorld					: TEXCOORD6;
	#endif
};

FluvioVertexOutputDeferred FluvioVertDeferred (FluvioVertexInput v)
{
	FluvioVertexOutputDeferred o;
	UNITY_INITIALIZE_OUTPUT(FluvioVertexOutputDeferred, o);

	float4 posWorld = mul(_Object2World, v.vertex);
	#if UNITY_SPECCUBE_BOX_PROJECTION
		o.posWorld = posWorld;
	#endif
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.tex = FluvioTexCoords(v);
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = FluvioGetWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = FluvioGetWorldTangent(v.tangent);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndColor[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndColor[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndColor[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndColor[0].xyz = 0;
		o.tangentToWorldAndColor[1].xyz = 0;
		o.tangentToWorldAndColor[2].xyz = normalWorld;
	#endif

    #ifdef _VERTEXCOLORMODE_NONE
	    o.tangentToWorldAndColor[0].w = 1;
	    o.tangentToWorldAndColor[1].w = 1;
	    o.tangentToWorldAndColor[2].w = 1;    
    #else
        o.tangentToWorldAndColor[0].w = v.color.r;
	    o.tangentToWorldAndColor[1].w = v.color.g;
	    o.tangentToWorldAndColor[2].w = v.color.b;
    #endif

    #if UNITY_SHOULD_SAMPLE_SH
		#if (SHADER_TARGET < 30)
			o.ambientOrLightmapUV.rgb = ShadeSH9(half4(normalWorld, 1.0));
		#else
			// Optimization: L2 per-vertex, L0..L1 per-pixel
			o.ambientOrLightmapUV.rgb = ShadeSH3Order(half4(normalWorld, 1.0));
		#endif
	#endif
	
	return o;
}

void FluvioFragDeferred (
		FluvioVertexOutputDeferred i,
		out half4 outDiffuse : SV_Target0,			// RT0: diffuse color (rgb), --unused-- (a)
		out half4 outSpecSmoothness : SV_Target1,	// RT1: spec color (rgb), smoothness (a)
		out half4 outNormal : SV_Target2,			// RT2: normal (rgb), --unused-- (a)
		out half4 outEmission : SV_Target3			// RT3: emission (rgb), --unused-- (a)
	)
{
	#if (SHADER_TARGET < 30)
		outDiffuse = 1;
		outSpecSmoothness = 1;
		outNormal = 0;
		outEmission = 0;
		return;
	#endif

    // FLUVIO - VERTEX COLORS
	half3 vertex_color = half3(i.tangentToWorldAndColor[0].w, i.tangentToWorldAndColor[1].w, i.tangentToWorldAndColor[2].w);
	
	FLUVIO_FRAGMENT_SETUP(s)

	// no analytic lights in this pass
	UnityLight dummyLight = DummyLight (s.normalWorld);
	half atten = 1;

	// only GI
	half occlusion = 1;
	UnityGI gi = FragmentGI (
		s.posWorld, occlusion, i.ambientOrLightmapUV, atten, s.oneMinusRoughness, s.normalWorld, s.eyeVec, dummyLight);
    
	half3 color = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;
	color += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.oneMinusRoughness, s.normalWorld, -s.eyeVec, occlusion, gi);
    
    #ifdef _VERTEXCOLORMODE_EMISSION
        color += FluvioEmission(i.tex, vertex_color);
    #else
        color += FluvioEmission(i.tex, float3(1,1,1));
    #endif
        
	#ifndef UNITY_HDR_ON
		color.rgb = exp2(-color.rgb);
	#endif

	outDiffuse = half4(s.diffColor, 1);
	outSpecSmoothness = half4(s.specColor, s.oneMinusRoughness);
	outNormal = half4(s.normalWorld,1);
}
