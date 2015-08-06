Shader "Custom/EugeneShader/GlitterWater" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Scale("Scale", Float) = 1
		_Speed("Speed", Float) = 1
		_Frequency("Frequency", Float) = 1
	}
	SubShader {
	
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float3 WorldPos;
		float _Scale, _Speed, _Frequency;
		half4 _Color;
		float3 worldPos;
		//Ripple Amplitudes and there offets
		float _WaveAmplitude1, _WaveAmplitude2, _WaveAmplitude3, _WaveAmplitude4, _WaveAmplitude5, _WaveAmplitude6, _WaveAmplitude7, _WaveAmplitude8;
		float _OffsetX1, _OffsetZ1, _OffsetX2, _OffsetZ2, _OffsetX3, _OffsetZ3, _OffsetX4, _OffsetZ4, _OffsetX5, _OffsetZ5, _OffsetX6, _OffsetZ6, _OffsetX7, _OffsetZ7, _OffsetX8, _OffsetZ8;
		float _Distance1, _Distance2, _Distance3, _Distance4, _Distance5, _Distance6 , _Distance7 , _Distance8;
		float _xImpact1, z_Impact1, _xImpact2, z_Impact2, _xImpact3, z_Impact3, _xImpact4, z_Impact4, _xImpact5, z_Impact5, _xImpact6, z_Impact6, _xImpact7, z_Impact7, _xImpact8, z_Impact8;
		
		struct Input {
			float2 uv_MainTex;
		};
	
			
		
		void vert(inout appdata_full v)
		{
		
		worldPos = mul(_Object2World, v.vertex).xyz;
		
		half offsetvert = ((v.vertex.x * v.vertex.x) + (v.vertex.z * v.vertex.z));
		
		half value1 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX1) + (v.vertex.z * _OffsetZ1));
		half value2 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX2) + (v.vertex.z * _OffsetZ2));
		half value3 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX3) + (v.vertex.z * _OffsetZ3));
		half value4 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX4) + (v.vertex.z * _OffsetZ4));
		half value5 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX5) + (v.vertex.z * _OffsetZ5));
		half value6 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX6) + (v.vertex.z * _OffsetZ6));
		half value7 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX7) + (v.vertex.z * _OffsetZ7));
		half value8 = _Scale * sin(_Time.w * _Speed * _Frequency + offsetvert + (v.vertex.x * _OffsetX8) + (v.vertex.z * _OffsetZ8));
		
		
		if (sqrt(pow(worldPos.x - _xImpact1,2) + pow (WorldPos.z - z_Impact1, 2)) < _Distance1)
		{
		v.vertex.y += value1 * _WaveAmplitude1;
		v.normal.y += value1 * _WaveAmplitude1;
		}
		
		if (sqrt(pow(worldPos.x - _xImpact2,2) + pow (WorldPos.z - z_Impact2, 2)) < _Distance2)
		{
		v.vertex.y += value2 * _WaveAmplitude2;
		v.normal.y += value2 * _WaveAmplitude2; 
		}
		if (sqrt(pow(worldPos.x - _xImpact3,2) + pow (WorldPos.z - z_Impact3, 2)) < _Distance3)
		{
		v.vertex.y += value3 * _WaveAmplitude3;
		v.normal.y += value3 * _WaveAmplitude3; 
		}
		if (sqrt(pow(worldPos.x - _xImpact4,2) + pow (WorldPos.z - z_Impact4, 2)) < _Distance4)
		{
		v.vertex.y += value4 * _WaveAmplitude4;
		v.normal.y += value4 * _WaveAmplitude4; 
		}
		if (sqrt(pow(worldPos.x - _xImpact5,2) + pow (WorldPos.z - z_Impact5, 2)) < _Distance5)
		{
		v.vertex.y += value5 * _WaveAmplitude5;
		v.normal.y += value5 * _WaveAmplitude5; 
		}
		if (sqrt(pow(worldPos.x - _xImpact6,2) + pow (WorldPos.z - z_Impact6, 2)) < _Distance6)
		{
		v.vertex.y += value6 * _WaveAmplitude6;
		v.normal.y += value6 * _WaveAmplitude6; 
		}
		if (sqrt(pow(worldPos.x - _xImpact7,2) + pow (WorldPos.z - z_Impact7 ,2)) < _Distance7)
		{
		v.vertex.y += value7 * _WaveAmplitude7;
		v.normal.y += value7 * _WaveAmplitude7; 
		}
		if (sqrt(pow(worldPos.x - _xImpact8,2) + pow (WorldPos.z - z_Impact8, 2)) < _Distance8)
		{
		v.vertex.y += value8 * _WaveAmplitude8;
		v.normal.y += value8 * _WaveAmplitude8;  
		}
		}


		void surf (Input IN, inout SurfaceOutput o) {
			
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = _Color.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	//FallBack "Diffuse"
}
