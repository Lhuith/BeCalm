//basic phong CG shader with spec map


Shader "CG Shaders/Alternative Lighting/OrenNayarFast"
{
	Properties
	{
		_diffuseColor("Diffuse Color", Color) = (1,1,1,1)
		_diffuseMap("Diffuse", 2D) = "white" {}
		_FrenselPower ("Rim Power", Range(1.0, 10.0)) = 2.5
		_FrenselPower (" ", Float) = 2.5
		_rimColor("Rim Color", Color) = (1,1,1,1)
		_normalMap("Normal", 2D) = "bump" {}
		_Roughness ("Roughness", Range(0.0, 1.0)) = 1
		_Roughness (" ", Float) = 1
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" } 
            
			CGPROGRAM
			
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			#pragma multi_compile_fwdbase
			
			uniform fixed3 _diffuseColor;
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;			
			uniform fixed4 _LightColor0; 
			uniform half _FrenselPower;
			uniform fixed4 _rimColor;
			uniform sampler2D _normalMap;
			uniform half4 _normalMap_ST;	
			uniform half _Roughness;
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : 	TEXCOORD0;
				fixed4 normal 	:	NORMAL;
				fixed4 tangent : TANGENT;
				
			};
			struct vert2Pixel
			{
				float4 pos 						: 	SV_POSITION;
				fixed2 uvs						:	TEXCOORD0;
				fixed3 normalDir						:	TEXCOORD1;	
				fixed3 binormalDir					:	TEXCOORD2;	
				fixed3 tangentDir					:	TEXCOORD3;	
				half3 posWorld						:	TEXCOORD4;	
				fixed3 viewDir						:	TEXCOORD5;
				fixed3 lighting						:	TEXCOORD6;
			};
			
			fixed lambert(fixed3 N, fixed3 L)
			{
				return saturate(dot(N, L));
			}
			fixed frensel(fixed3 V, fixed3 N, half P)
			{	
				return pow(1 - saturate(dot(V,N)), P);
			}
			fixed orenNayar(fixed3 N, fixed3 L, fixed3 V )
			{
				fixed nDotL = dot(N, L);
				fixed nDotV = dot(N, V);
				fixed lambert = saturate(nDotL);
				//fakey fake
				fixed softRim = saturate(1-nDotV/2);
				fixed fakey = 1-lambert*softRim;
				fakey *= fakey;
				//suspiciously golden ratioish...
				fixed fakeyNumber = .62;
				fixed fakeyFix = 1-(saturate(dot(L,V)) * lambert);
				fakey = fakeyNumber - fakey * (fakeyNumber*fakeyFix);
				return lerp(lambert,fakey,_Roughness);
			}
			vert2Pixel vShader(app2vert IN)
			{
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = _World2Object; 
				float4x4 World = _Object2World;
							
				OUT.pos = mul(WorldViewProjection, IN.vertex);
				OUT.uvs = IN.texCoord;					
				OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
				OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
				OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir)); 
				OUT.posWorld = mul(World, IN.vertex).xyz;
				OUT.viewDir = normalize( OUT.posWorld - _WorldSpaceCameraPos);

				fixed3 vertexLighting = fixed3(0.0, 0.0, 0.0);
				#ifdef VERTEXLIGHT_ON
				 for (int index = 0; index < 4; index++)
					{    						
						half3 vertexToLightSource = half3(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index]) - OUT.posWorld;
						fixed attenuation  = (1.0/ length(vertexToLightSource)) *.5;	
						fixed3 diffuse = unity_LightColor[index].xyz * lambert(OUT.normalDir, normalize(vertexToLightSource)) * attenuation;
						vertexLighting = vertexLighting + diffuse;
					}
				vertexLighting = saturate( vertexLighting );
				#endif
				OUT.lighting = vertexLighting ;
				
				return OUT;
			}
			
			fixed4 pShader(vert2Pixel IN): COLOR
			{
				half2 normalUVs = TRANSFORM_TEX(IN.uvs, _normalMap);
				fixed4 normalD = tex2D(_normalMap, normalUVs);
				normalD.xyz = (normalD.xyz * 2) - 1;
				
				//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
				//deriving the z component
				//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
               // alternatively you can approximate deriving the z component without sqrt like so:  
				//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);
				
				fixed3 normalDir = normalD.xyz;	
				normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));
	
				fixed3 ambientL = UNITY_LIGHTMODEL_AMBIENT.xyz;
	
				//Main Light calculation - includes directional lights
				half3 pixelToLightSource =_WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
				fixed attenuation  = lerp(1.0, 1.0/ length(pixelToLightSource), _WorldSpaceLightPos0.w);				
				fixed3 lightDirection = normalize(pixelToLightSource);
				//replace lambert with OrenNayar
				//fixed diffuseL = lambert(normalDir, lightDirection);	
				fixed diffuseL = orenNayar(normalDir, lightDirection, -IN.viewDir);
				
				//rimLight calculation
				fixed rimLight = frensel(normalDir, -IN.viewDir, _FrenselPower);
				rimLight *= saturate(dot(fixed3(0,1,0),normalDir)* 0.5 + 0.5)* saturate(dot(fixed3(0,1,0),-IN.viewDir)+ 1.75);	
				fixed3 diffuse = _LightColor0.xyz * (diffuseL+ (rimLight * diffuseL)) * attenuation;
				rimLight *= (1-diffuseL);
				diffuse = saturate(IN.lighting + ambientL + diffuse+ (rimLight*_rimColor));
		
				
				fixed4 outColor;							
				half2 diffuseUVs = TRANSFORM_TEX(IN.uvs, _diffuseMap);
				fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
				fixed3 diffuseS = (diffuse * texSample.xyz) * _diffuseColor.xyz;
				outColor = fixed4( diffuseS ,1.0);
				return outColor;
			}
			
			ENDCG
		}	
		
		//the second pass for additional lights
		Pass
		{
			Tags { "LightMode" = "ForwardAdd" } 
			Blend One One 
			
			CGPROGRAM
			#pragma vertex vShader
			#pragma fragment pShader
			#include "UnityCG.cginc"
			
			uniform fixed3 _diffuseColor;
			uniform sampler2D _diffuseMap;
			uniform half4 _diffuseMap_ST;
			uniform fixed4 _LightColor0; 	
			uniform sampler2D _normalMap;
			uniform half4 _normalMap_ST;	
			uniform half _Roughness;
			
			
			struct app2vert {
				float4 vertex 	: 	POSITION;
				fixed2 texCoord : 	TEXCOORD0;
				fixed4 normal 	:	NORMAL;
				fixed4 tangent : TANGENT;
			};
			struct vert2Pixel
			{
				float4 pos 						: 	SV_POSITION;
				fixed2 uvs						:	TEXCOORD0;	
				fixed3 normalDir						:	TEXCOORD1;	
				fixed3 binormalDir					:	TEXCOORD2;	
				fixed3 tangentDir					:	TEXCOORD3;	
				half3 posWorld						:	TEXCOORD4;	
				fixed3 viewDir						:	TEXCOORD5;
				fixed4 lighting					:	TEXCOORD6;	
			};
			
			fixed lambert(fixed3 N, fixed3 L)
			{
				return saturate(dot(N, L));
			}			
			fixed orenNayar(fixed3 N, fixed3 L, fixed3 V )
			{
				fixed nDotL = dot(N, L);
				fixed nDotV = dot(N, V);
				fixed lambert = saturate(nDotL);
				//fakey fake
				fixed softRim = saturate(1-nDotV/2);
				fixed fakey = 1-lambert*softRim;
				fakey *= fakey;
				//suspiciously golden ratioish...
				fixed fakeyNumber = .62;
				fixed fakeyFix = 1-(saturate(dot(L,V)) * lambert);
				fakey = fakeyNumber - fakey * (fakeyNumber*fakeyFix);
				return lerp(lambert,fakey,_Roughness);
			}
			vert2Pixel vShader(app2vert IN)
			{
				vert2Pixel OUT;
				float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
				float4x4 WorldInverseTranspose = _World2Object; 
				float4x4 World = _Object2World;
				
				OUT.pos = mul(WorldViewProjection, IN.vertex);
				OUT.uvs = IN.texCoord;	
				
				OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
				OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
				OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir)); 
				OUT.posWorld = mul(World, IN.vertex).xyz;
				OUT.viewDir = normalize( OUT.posWorld - _WorldSpaceCameraPos);
				return OUT;
			}
			fixed4 pShader(vert2Pixel IN): COLOR
			{
				half2 normalUVs = TRANSFORM_TEX(IN.uvs, _normalMap);
				fixed4 normalD = tex2D(_normalMap, normalUVs);
				normalD.xyz = (normalD.xyz * 2) - 1;
				
				//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
				//deriving the z component
				//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
               // alternatively you can approximate deriving the z component without sqrt like so: 
				//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);
				
				fixed3 normalDir = normalD.xyz;	
				normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));
							
				//Fill lights
				half3 pixelToLightSource = _WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
				fixed attenuation  = lerp(1.0, 1.0/ length(pixelToLightSource), _WorldSpaceLightPos0.w);				
				fixed3 lightDirection = normalize(pixelToLightSource);
				
				//replace lambert with OrenNayar
				//fixed diffuseL = lambert(normalDir, lightDirection);	
				fixed diffuseL = orenNayar(normalDir, lightDirection, -IN.viewDir);
				fixed3 diffuseTotal = _LightColor0.xyz * diffuseL * attenuation;
			
				
				fixed4 outColor;							
				half2 diffuseUVs = TRANSFORM_TEX(IN.uvs, _diffuseMap);
				fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
				fixed3 diffuseS = (diffuseTotal * texSample.xyz) * _diffuseColor.xyz;
				outColor = fixed4( diffuseS ,1.0);
				return outColor;
			}
			
			ENDCG
		}	
		
	}
}