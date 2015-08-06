Shader"EugeneShader/ShaderTestDemos/18b_ToonOutLine"{
Properties{
	_Color("Lit Color", Color) = (1,1,1,1)
	_UnlitColor("Unlit Color", Color) = (0.5,0.5,0.5,0.5)
	_DiffuseThreshold("Lighting Threshold", Range(-1.1,1)) = 0.1
	_Diffusion("Diffusion", Range(0,0.99)) = 0.0
	_SpecColor("Specular Color", Color) = (1,1,1,1)
	_Shininess("Shininess", Range(0.5, 1)) = 1
	_SpecDiffusion("Specular Diffusion", Range (0, 0.99)) = 0.0
	_OutlineColor("Outline Color", Color) = (1,1,1,1)
	_OutlineThickness("Outline Thickness", Range(0,1)) = 0.1
	_OutlineDiffusion("Outline Diffusion", Range(0,1)) = 0.0
	_NoiseMap("Noise_Map", 2D) = "white" {}
	_GlitterStrengh("Glitter Strengh", Range(0.0, 3.0)) = 0.3
}
	SubShader{
		Pass{
		Name "ShadowPass"
			Tags{"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			//user defined verables
			uniform fixed4 _Color;
			uniform fixed4 _UnlitColor;
			uniform fixed _DiffuseThreshold;
			uniform fixed _Diffusion;
			uniform fixed4 _SpecColor;
			uniform fixed _Shininess;
			uniform half _SpecDiffusion;
			uniform fixed4 _OutlineColor;
			uniform fixed _OutlineThickness;
			uniform fixed _OutlineDiffusion;
			uniform sampler2D _NoiseMap;
			uniform half4 _NoiseMap_ST;
			uniform fixed _GlitterStrengh;
			//unity defined verables
			
			uniform half4 _LightColor0;
			
			//Base Input structs
			
			struct vertexInput{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 texcoord : TEXCOORD0;
			};
			
			struct vertexOutput{
				half4 pos : SV_POSITION;
				half4 tex : TEXCOORD0;
				fixed3 normalDir : TEXCOORD1;
				fixed4 lightDir : TEXCOORD2;
				fixed3 viewDir : TEXCOORD3;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v)
			{
					vertexOutput o;
					
					//Normal Direction
					o.normalDir = normalize(mul(half4(v.normal, 0.0), _World2Object).xyz);
					
					//Unity transform Position
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					
					//Texture
					//world Position
					half4 posWorld = mul(_Object2World, v.vertex);
					//view Direction
					o.viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
					//light Direction
					o.tex = v.texcoord;
					half3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
					o.lightDir = fixed4(
					normalize(lerp(_WorldSpaceLightPos0.xyz, fragmentToLightSource, _WorldSpaceLightPos0.w)),
					lerp(1.0, 1.0/length(fragmentToLightSource), _WorldSpaceLightPos0.w)
					);
					
					return o;
			
			}
			//fragment function
			fixed4 frag(vertexOutput i) : COLOR
			{

			fixed4 texP = tex2D(_NoiseMap, i.tex.xy * _NoiseMap_ST.xy + _NoiseMap_ST.zw);
			//Lighting
			//dotProduct
			fixed nDotL = saturate(dot(i.normalDir, i.lightDir.xyz));
			
			//Diffuse Color
			fixed diffuseCutoff = saturate((max (_DiffuseThreshold, nDotL) -  _DiffuseThreshold) * pow((2-_Diffusion),10));
			fixed specularCutoff = saturate((max(_Shininess, dot(reflect(-i.lightDir, i.normalDir), i.viewDir))- _Shininess)* pow((2-_SpecDiffusion), 10));
			
			//calculate outlines
			fixed outlineStrengh = saturate((dot(i.normalDir, i.viewDir) - _OutlineThickness) * pow((2-_OutlineDiffusion),10) + _OutlineThickness);
			fixed3 outlineOverlay = (_OutlineColor.xyz * (1-outlineStrengh)) + outlineStrengh;
			
			
			fixed3 ambientLight = (1-diffuseCutoff) * _UnlitColor.xyz;
			fixed3 diffuseReflection = (1-specularCutoff) * _Color.xyz * diffuseCutoff;
			fixed3 specularReflection = _SpecColor.xyz * specularCutoff;
			
			float specBase = saturate(dot(reflect(-normalize(i.viewDir), i.normalDir),
			i.lightDir));

			float3 fp = frac(0.7 * i.pos + 9 * texP.xyz + ( i.pos * 0.04).r + 0.1 * i.viewDir);
			fp *= (1 - fp);
			float glitter = saturate(1 - 7 * (fp.x + fp.y + fp.z));
			float sparkle = glitter * pow(specBase, 3);
			
			fixed3 lightFinal = (ambientLight + diffuseReflection) * outlineOverlay + specularReflection + (sparkle * _GlitterStrengh);
			
			return fixed4(lightFinal, 1.0);
			}
			ENDCG
		}
		
}
//Fallback "Diffuse"
}





















