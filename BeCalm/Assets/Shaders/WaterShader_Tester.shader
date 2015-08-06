Shader "Eugene/WaterShaderV5" {
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
	
    _ExtrudeTex ("Extrusion Texture", 2D) = "white" {}
    _Amount ("Extrusion Amount", Range(-1,1)) = 0.5
     
    _ExtrudeDetail ("Extrusion Detail Texture", 2D) = "white" {}
     
    _SineAmplitude ("Amplitude", Float) = 1.0
   //the following three are vectors so we can control more than one wave easily
   _SineFrequency ("Frequency", Vector) = (1,1,0,0)
   _Speed ("Speed", Vector) = (1,1,0,0)
   _Steepness ("steepness", Vector) = (1,1,0,0)
   //two direction vectors as we are using two gerstner waves
   _Dir ("Wave Direction", Vector) = (1,1,0,0)
   _Dir2 ("2nd Wave Direction", Vector) = (1,1,0,0)
      
}
	SubShader{

	// Grab the screen behind the object into _GrabTexture, using default values
		Pass{
			Tags{"LightMode" = "ForwardBase"}
		//Blend One One 
		//Cull Back
			
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
			
			uniform sampler2D _ExtrudeTex;
			uniform half4 _ExtrudeTex_ST;
			
			uniform sampler2D _ExtrudeDetail;
			uniform half4 _ExtrudeDetail_ST;
			
			uniform half _Amount;
			
			float _SineAmplitude;
			float4 _SineFrequency;
			float4 _Speed;
			float4 _Steepness;
			float4 _Dir;
			float4 _Dir2;
			
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
				float3 posWorld : TEXCOORD7;
		
			};
			
			
			//vertex function
			vertexOutput vert(vertexInput v)
			{
			
			vertexOutput o;	
			
					
				
					
					
											
						 float2 dir = _Dir.xy;
						dir = normalize(dir);
						float dotprod = dot(dir, v.vertex.xz);
						float disp = _Time.y*_Speed.x;

						//do the same for our second wave
						float2 dir2 = _Dir2.xy;
						dir2 = normalize(dir2);
						float dotprod2 = dot(dir2, v.vertex.xz);
						float disp2 = _Time.y*_Speed.y;										
																																						
//						v.vertex.x += _Steepness.x*_SineAmplitude*_Dir.x*cos(_SineFrequency.x*dotprod + disp);
//						v.vertex.z += _Steepness.x*_SineAmplitude*_Dir.y*cos(_SineFrequency.x*dotprod + disp);
//						v.vertex.y += _SineAmplitude*-sin(_SineFrequency.x*dotprod + disp);
//
//						v.vertex.x += _Steepness.y*_SineAmplitude*_Dir2.x*cos(_SineFrequency.y*dotprod2 + disp2);
//						v.vertex.z += _Steepness.y*_SineAmplitude*_Dir2.y*cos(_SineFrequency.y*dotprod2 + disp2);
//						v.vertex.y += _SineAmplitude*sin(_SineFrequency.y*dotprod2 + disp2);
	
						//o.posWorld = mul(_Object2World,(v.vertex)).xyz;
						
						o.posWorld = mul(_Object2World, v.vertex).xyx;	
					
//						o.posWorld.x += _Steepness.x*_SineAmplitude*_Dir.x*cos(_SineFrequency.x*dotprod + disp);
//						o.posWorld.z += _Steepness.x*_SineAmplitude*_Dir.y*cos(_SineFrequency.x*dotprod + disp);
//						o.posWorld.y += _SineAmplitude*-sin(_SineFrequency.x*dotprod + disp);
//
//						o.posWorld.x += _Steepness.y*_SineAmplitude*_Dir2.x*cos(_SineFrequency.y*dotprod2 + disp2);
//						o.posWorld.z += _Steepness.y*_SineAmplitude*_Dir2.y*cos(_SineFrequency.y*dotprod2 + disp2);
//						o.posWorld.y += _SineAmplitude*sin(_SineFrequency.y*dotprod2 + disp2);	
						
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
							
					float4 tex = tex2Dlod (_ExtrudeTex, float4(v.texcoord.xy,0,0));
					float4 tex2 = tex2Dlod (_ExtrudeDetail, float4(v.texcoord.xy,0,0));
	
						
					v.vertex.y += (tex.rgb) * _Amount;
					v.normal.y += (tex.rgb ) * _Amount;
			
					TRANSFER_VERTEX_TO_FRAGMENT(o)
					return o;
					
		
		
}
			
        
			//fragment function
			fixed4 frag(vertexOutput i) : COLOR
			{
			
			half4 texwave = tex2D (_ExtrudeTex, i.tex.xy * _ExtrudeTex_ST.xy + _ExtrudeTex_ST.zw);
			fixed4 texDetWave = tex2D(_ExtrudeDetail, i.tex.xy * _ExtrudeDetail_ST.xy + _ExtrudeDetail_ST.zw);
	
		  half blurSizeY = 2;
		  half blurSizeX = 2;
		  half foams = texwave.rgb;
		  //if (foams>0.8)  o.Emission = foams*0.5;
//		  if (foams>0.8)  o.Specular = foams*0.8;

		   half4 sum = half4(0.0, 0.0, 0.0, 0.0);
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 4.0 * blurSizeX, texwave.y - 4.0 * blurSizeY)) * 0.05;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 3.0 * blurSizeX, texwave.y - 3.0 * blurSizeY)) * 0.09;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 2.0 * blurSizeX, texwave.y - 2.0 * blurSizeY)) * 0.12;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 1.0 * blurSizeX, texwave.y - 1.0 * blurSizeY)) * 0.15;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x, texwave.y)) * 0.16;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x + 1.0 * blurSizeX, texwave.y + 1.0 * blurSizeY)) * 0.15;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x + 2.0 * blurSizeX, texwave.y + 2.0 * blurSizeY)) * 0.12;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x + 3.0 * blurSizeX, texwave.y + 3.0 * blurSizeY)) * 0.09;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x+ 4.0 * blurSizeX, texwave.y + 4.0 * blurSizeY)) * 0.05;
		   
		   
		   half4 sum2 = half4(0.0, 0.0, 0.0, 0.0);
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x - 4.0 * blurSizeX, texDetWave.y - 4.0 * blurSizeY)) * 0.05;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x - 3.0 * blurSizeX, texDetWave.y - 3.0 * blurSizeY)) * 0.09;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x - 2.0 * blurSizeX, texDetWave.y - 2.0 * blurSizeY)) * 0.12;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x - 1.0 * blurSizeX, texDetWave.y - 1.0 * blurSizeY)) * 0.15;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x, texDetWave.y)) * 0.16;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x + 1.0 * blurSizeX, texDetWave.y + 1.0 * blurSizeY)) * 0.15;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x + 2.0 * blurSizeX, texDetWave.y + 2.0 * blurSizeY)) * 0.12;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x + 3.0 * blurSizeX, texDetWave.y + 3.0 * blurSizeY)) * 0.09;
//		   sum2 += tex2D(_ExtrudeDetail, float2(texDetWave.x+ 4.0 * blurSizeX, texDetWave.y + 4.0 * blurSizeY)) * 0.05;
		   
		  // uvs += _Dir2.xy*disp;
		   
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
			
			fixed3 Reflection = atten * saturate(dot(reflect(-i.lightDir.xyz, normalDirection), i.viewDir * texwave));
			//texB.rbg += texwave.rbg;
			//Diffuse Reflection
			fixed3 diffuseReflection = atten * i.lightDir.w * texDetWave.r * texwave.r *  _LightColor0.xyz * saturate(nDotL);
			//Specular Reflection
			fixed3 AniospecularReflection = atten * diffuseReflection *( exp(-(tDotHX * tDotHX + bDotHY * bDotHY)) * _Shininess) * _SpecColor.xyz;

			
			//NormalReflecion
			fixed3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect( - i.lightDir.xyz, i.normalDir),1.0)),_Shininess);
			
			//Translucance
			fixed3 backScatter = i.lightDir.w * _LightColor0.xyz * _BackScatter.xyz * saturate(dot(i.normalDir, -i.lightDir));
			fixed3 translucence = i.lightDir.w * _LightColor0.xyz * _Translucence.xyz * pow(saturate(dot( -i.lightDir.xyz, i.viewDir)), _Intensity);
			
			//Reflection Glitter
			float3 fp2 = saturate( frac(.7 * i.pos + 9 * texP.xyz + ( AniospecularReflection * 0.04).r + 0.3 * i.viewDir));
			fp2 *= (1 - fp2);
			texM.y *= 1;
			float glitter = saturate(1 - 3 * (fp2.x + fp2.y + fp2.z));
			float sparkle =	 glitter * pow(AniospecularReflection, 2.5) * _SpecColor.xyz;
			
			//General Glitter
			float specBase =	saturate( 4 * dot( normalDirection, i.viewDir )); // JOURNEY!!!!!!!!!!!!!!
			normalDirection.y *= 1;
			//texP.y *= 1;
			float3 fp1 = frac(.7 * i.pos + 9 * texM.xyz + ( specularReflection * 0.04).r + 0.3 * i.viewDir);
			fp1 *= (1 - fp1);
			float Normglitter = saturate(1 - 7 * (fp1.x + fp1.y + fp1.z));
			float Normsparkle =  Normglitter * (pow(AniospecularReflection, 2.5) * _SpecColor.xyz);
			
			texB *= texwave.r + texDetWave.r;
			
			//+ backScatter + translucence
			fixed3 lightFinal =  AniospecularReflection + (sparkle * _GlitterStrengh) +  backScatter + translucence + UNITY_LIGHTMODEL_AMBIENT.xyz;
			
			
			
			return fixed4((texB) + lightFinal * _Color, 1.0);
			}
			
			
			ENDCG
		}
 
        Pass {
        	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    		//Blend One One
    		Blend SrcAlpha OneMinusSrcAlpha 
   			AlphaTest Greater .01
    		ColorMask RGB
   			Cull Off Lighting Off Zwrite Off
   			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

					
			uniform fixed4 _TintColor;
			uniform	fixed _DepthFactor;
			
				uniform sampler2D _DepthTexture;
			uniform half4 _DepthTexture_ST;
			
			
			uniform float _Curvature;
			
			uniform sampler2D _ExtrudeTex;
			uniform half4 _ExtrudeTex_ST;

			uniform half _Amount;
			
			
			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };
 
            struct v2f {
                float4 vertex : SV_POSITION;
              	half4 tex : TEXCOORD0;
                float4 projPos : TEXCOORD1;
            };
           
 
            v2f vert (appdata_t v)
            {
         
 
		   
            // Transform the vertex coordinates from model space into world space
            //float4 vv = mul( _Object2World, v.vertex );
 
            // Now adjust the coordinates to be relative to the camera position
            //vv.xyz -= _WorldSpaceCameraPos.xyz;
 
            // Reduce the y coordinate (i.e. lower the "height") of each vertex based
            // on the square of the distance from the camera in the z axis, multiplied
            // by the chosen curvature factor
            //vv = float4( 0.0f, (vv.z * vv.z) * - _Curvature, 0.0f, 0.0f );
 
            // Now apply the offset back to the vertices in model space
           
            	v2f o;
					
					
						#if !defined(SHADER_API_OPENGL)
					float4 tex = tex2Dlod (_ExtrudeTex, float4(v.texcoord.xy,0,0));
					
					if (v.normal.y > 0)
					{
					v.vertex.y += tex.rgb * _Amount;
					v.normal.y += tex.rgb * _Amount;
					}
					#endif

 				o.tex = v.texcoord;
            	
               	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				
				            
					
				//v.vertex += mul(_World2Object, vv);
                return o;
            }
 			
            sampler2D_float _CameraDepthTexture;
           
            fixed4 frag (v2f i) : COLOR
            {
           // _DepthTexture
         		 half4 texwave = tex2D (_ExtrudeTex, i.tex.xy * _ExtrudeTex_ST.xy + _ExtrudeTex_ST.zw);
               half4 texDepth = tex2D( _DepthTexture, i.tex.xy * _DepthTexture_ST.xy + _DepthTexture_ST.zw);
          
              half blurSizeY = 2;
		  half blurSizeX = 2;
		  half foams = texwave.rgb;
		  
               half4 sum = half4(0.0, 0.0, 0.0, 0.0);
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 4.0 * blurSizeX, texwave.y - 4.0 * blurSizeY)) * 0.05;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 3.0 * blurSizeX, texwave.y - 3.0 * blurSizeY)) * 0.09;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 2.0 * blurSizeX, texwave.y - 2.0 * blurSizeY)) * 0.12;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x - 1.0 * blurSizeX, texwave.y - 1.0 * blurSizeY)) * 0.15;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x, texwave.y)) * 0.16;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x + 1.0 * blurSizeX, texwave.y + 1.0 * blurSizeY)) * 0.15;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x + 2.0 * blurSizeX, texwave.y + 2.0 * blurSizeY)) * 0.12;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x + 3.0 * blurSizeX, texwave.y + 3.0 * blurSizeY)) * 0.09;
		   sum += tex2D(_ExtrudeTex, float2(texwave.x + 4.0 * blurSizeX, texwave.y + 4.0 * blurSizeY)) * 0.05;
		   
            //half4 texB = tex2D( _MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
            
            float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD(i.projPos)));
			float partZ = i.projPos.z;
			float fade = saturate (_DepthFactor / (sceneZ-partZ));
			texDepth.a -= saturate(abs(1 - fade));
			//texB.a *= saturate(abs(1 - fade));
  			texwave.a *= saturate(abs(1 - fade));
             
             return texDepth;
            }
            ENDCG
        }
    }  
   //Fallback "Diffuse"
    }