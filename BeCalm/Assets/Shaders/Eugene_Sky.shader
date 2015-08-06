Shader"EugeneShader/Eugene_Sky"{
Properties{

	_Color("Color_Tint", Color) = (1.0,1.0,1.0,1.0)
	_MainTex("Diffuse Texture", 2D) = "white" {}
	_SpecColor("Specular Color", Color) = (1.0,1.0,1.0,1.0)
	
	_AniX("Anisotropic X", Range(0.0, 2.0)) = 1.0
	_AniY("Anisotropic Y", Range(0.0, 2.0)) = 1.0
	
	_Shininess("Shininess", Float) = 1.0
	_NoiseMap("Noise_Map", 2D) = "white" {}
	_MipMap("Mip_Map", 2D) = "white"{}
	_GlitterStrengh("Glitter Strengh", Range(0.0, 20)) = 0.3
	
	_NormalMap("Bump Map", 2D) = "white" {}
	_BumpDepth("Bump Depth", Float) = 0.1
	_Curvature ("Curvature", Float) = 0.001
	
	_DepthTexture("Depth Texture", 2D) = "white" {}
	_DepthFactor("Deepness", Float) = 0.1

	//Translucant Variables
	_BackScatter("Back Translucent Color", Color) = (1.0,1.0,1.0,1.0)
	_Translucence("Forward Translucent Color", Color) = (1.0,1.0,1.0,1.0)
	_Intensity("Translucent Intensity", Float) = 10.0
	
      
}
	SubShader{

	// Grab the screen behind the object into _GrabTexture, using default values
		Pass{
		Cull Front
		
			Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		//Blend One One 

			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"
			
			//user defined verables
			uniform fixed4 _Color;
			uniform fixed4 _SpecColor;
			uniform fixed _AniX;
			uniform fixed _AniY;
			uniform half _Shininess;
			
			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			
			uniform sampler2D _NoiseMap;
			uniform half4 _NoiseMap_ST;
			
			uniform sampler2D _NormalMap;
			uniform half4 _NormalMap_ST;
			
			uniform sampler2D _MipMap;
			uniform half4 _MipMap_ST;
			
			uniform fixed _GlitterStrengh;
			uniform half _BumpDepth;
			
			uniform half _WaveXPos;
			uniform half _WaveYPos;
			uniform half _WaveZPos;
			
			uniform fixed _WaveScale;
			uniform fixed _WaveSpeed;
			uniform fixed _WaveDistance;
			
			
			uniform fixed4 _BackScatter;
			uniform fixed4 _Translucence;
			uniform half _Intensity;
			
			
			//Ripple Amplitudes and there offets
				
			
			//unity defined verables
			uniform half4 _LightColor0;
			//Base Input structs
		    	
			struct vertexInput{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 tangent : TANGENT;
				half4 texcoord : TEXCOORD0;
				LIGHTING_COORDS(TEXCOORD2,TEXCOORD3)
			};
			
			struct vertexOutput{
				half4 pos : SV_POSITION;
				half4 tex : TEXCOORD0;
				fixed3 normalDir : TEXCOORD1;
				fixed4 lightDir : TEXCOORD2;
				fixed3 viewDir : TEXCOORD3;
				fixed3 tangentDir : TEXCOORD4;
				half4 uv : TEXCOORD5;
				fixed3 binormalDir : TEXCOORD6;
				fixed3 posWorld : TEXCOORD7;
		
			};
			
			
			//vertex function
			vertexOutput vert(vertexInput v)
			{
			
			vertexOutput o;	
										
					
					o.posWorld = mul(_Object2World, v.vertex);		
					o.tex = v.texcoord;
					o.uv = float4( v.texcoord.xy, 0, 0 );
					//Normal Direction
					o.normalDir = normalize(mul(half4(v.normal, 0.0), _World2Object).xyz);
					//tangent Direction
					o.tangentDir = normalize(mul(_Object2World, half4(v.tangent.xyz, 0.0)).xyz);
					//Binormal direction
					o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
					//Unity transform Position
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

					//view Direction
					o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.posWorld.xyz);
					//light Direction
					half3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - o.posWorld.xyz;
					
					o.lightDir = fixed4(
					normalize(lerp(_WorldSpaceLightPos0.xyz, fragmentToLightSource, _WorldSpaceLightPos0.w)),
					lerp(1.0, 1.0/length(fragmentToLightSource), _WorldSpaceLightPos0.w)
					);
					
					TRANSFER_VERTEX_TO_FRAGMENT(o)
					return o;
					
		
		
}
			
        
			//fragment function
			fixed4 frag(vertexOutput i) : COLOR
			{
			
					  //half4 col = tex2D (_MainTex, IN.uv_MainTex+(_Time.x*0.1));
		 // half4 col= tex2D (_MainTex, IN.uv_MainTex+(_Time.x*0.1))*0.5;
			//col+=tex2D (_MainTex, IN.uv_MainTex*0.95+(_Time.x*0.1))*0.25;
			//col+=tex2D (_MainTex, IN.uv_MainTex*0.90+(_Time.x*0.1))*0.25;
		  //half4 wave = tex2D (_ExtrudeTex, IN.uv_ExtrudeTex);
//          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
        //  o.Albedo = col.rgb;
		  //if (foams>0.8)  o.Emission = foams*0.5;
//		  if (foams>0.8)  o.Specular = foams*0.8;

		   
			//Texture Unpack
			fixed4 texP = tex2D(_NoiseMap, i.tex.xy * _NoiseMap_ST.xy + _NoiseMap_ST.zw);
			half4 texM = tex2D( _MipMap, i.tex.xy * _MipMap_ST.xy +_MipMap_ST.zw);
			half4 texB = tex2D( _MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
			//half4 texwave = tex2D (_ExtrudeTex, IN.uv_ExtrudeTex);
		   
			fixed atten = LIGHT_ATTENUATION(i);
			//float2 uv_dx = clamp( ddx( i.uv ), 1,5 );
			//float2 uv_dy = clamp( ddy( i.uv ), 1, 5 );
			//half4 texM = tex2D( _MipMap, i.uv.xy , uv_dx, uv_dy );
			
			//Normal Texture
			half4 texN = tex2D( _NormalMap, i.tex.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
			
			//unpackNormal function
			fixed3 localCoords = float3(2.0 * texN.ag - float2(1.0, 1.0), _BumpDepth);
			
			//normal transpose matrix
			fixed3x3 local2WorldTranspose = fixed3x3(
			i.tangentDir,
			i.binormalDir,
			i.normalDir
			);
			
			fixed3 normalDirection = normalize( mul(localCoords, local2WorldTranspose));
			//Lighting
			fixed3 h = normalize(i.lightDir.xyz + i.viewDir);
			fixed3 binormalDir = cross(normalDirection, i.tangentDir);
			
			//dotProduct
			fixed nDotL = dot(normalDirection, i.lightDir.xyz);
			fixed nDotH = dot(normalDirection, h);
			fixed nDotV = dot(normalDirection, i.viewDir);
			fixed tDotHX = dot(i.tangentDir, h)/ _AniX;
			fixed bDotHY = dot(binormalDir, h)/ _AniY;
			
			fixed3 Reflection = atten * saturate(dot(reflect(-i.lightDir.xyz, normalDirection), i.viewDir));
			//texB.rbg += texwave.rbg;
			//Diffuse Reflection
			fixed3 diffuseReflection = atten * i.lightDir.w * _LightColor0.xyz * saturate(nDotL);
			//Specular Reflection
			fixed3 AniospecularReflection = atten * diffuseReflection *( exp(-(tDotHX * tDotHX + bDotHY * bDotHY)) * _Shininess) * _SpecColor.xyz;

			
			//NormalReflecion
			fixed3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect( - i.lightDir.xyz, i.normalDir),1.0)),_Shininess);
			
			//Translucance
			fixed3 backScatter = i.lightDir.w * _LightColor0.xyz * _BackScatter.xyz * saturate(dot(i.normalDir, -i.lightDir));
			fixed3 translucence = i.lightDir.w * _LightColor0.xyz * _Translucence.xyz * pow(saturate(dot( -i.lightDir.xyz, AniospecularReflection)), _Intensity);
			
			//General Glitter
			float specBase =	saturate( 4 * dot( normalDirection, i.viewDir )); // JOURNEY!!!!!!!!!!!!!!
			normalDirection.y *= 1;
			//texP.y *= 1;
			float3 fp1 = frac(.7 * i.pos + 9 * texM.xyz + ( specularReflection * 0.04).r + 0.3 * i.viewDir);
			fp1 *= (1 - fp1);
			float Normglitter = saturate(1 - 7 * (fp1.x + fp1.y + fp1.z));
			float Normsparkle =  Normglitter * (pow(normalDirection, 2.5) * _SpecColor.xyz);
			
 
			//+ backScatter + translucence
			fixed3 lightFinal =  UNITY_LIGHTMODEL_AMBIENT.xyz;
			
			
			
			return fixed4((texB) * lightFinal * _Color, 1.0);
			}
			
			
			ENDCG
		}
    }  
   // Fallback "Diffuse"
    }
		
	
	






















