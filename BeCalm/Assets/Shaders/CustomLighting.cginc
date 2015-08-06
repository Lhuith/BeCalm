#ifdef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED



half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			
			fixed atten = LIGHT_ATTENUATION(i);
			//Texture Unpack
			fixed4 texP = tex2D(_NoiseMap, i.tex.xy * _NoiseMap_ST.xy + _NoiseMap_ST.zw);
			half4 texM = tex2D( _MipMap, i.tex.xy * _MipMap_ST.xy +_MipMap_ST.zw);
			half4 texB = tex2D( _MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
			
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
			
			//Diffuse Reflection
			fixed3 diffuseReflection = atten * i.lightDir.w * _LightColor0.xyz * saturate(nDotL);
			//Specular Reflection
			fixed3 AniospecularReflection = atten * diffuseReflection *( exp(-(tDotHX * tDotHX + bDotHY * bDotHY)) * _Shininess) * _SpecColor.xyz;
			
			
			
			
			//Translucance
		
			
			//NormalReflecion
			fixed3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect( - i.lightDir.xyz, i.normalDir),1.0)),_Shininess);
			
			fixed3 backScatter = i.lightDir.w * _LightColor0.xyz * _BackScatter.xyz * saturate(dot(i.normalDir, -i.lightDir));
			fixed3 translucence = i.lightDir.w * _LightColor0.xyz * _Translucence.xyz * pow(saturate(dot( -i.lightDir.xyz, AniospecularReflection)), _Intensity);
			
			//Reflection Glitter
			//float3 fp2 = saturate( frac(.7 * i.pos + 9 * texP.xyz + ( AniospecularReflection * 0.04).r + 0.3 * i.viewDir));
			//fp2 *= (1 - fp2);
			//texM.y *= 1;
			//float glitter = saturate(1 - 3 * (fp2.x + fp2.y + fp2.z));
			//float sparkle =	 glitter * pow(AniospecularReflection, 2.5) * _SpecColor.xyz;
			
						//General Glitter
			float specBase =	saturate( 4 * dot( normalDirection, i.viewDir )); // JOURNEY!!!!!!!!!!!!!!
			normalDirection.y *= 1;
			//texP.y *= 1;
			float3 fp1 = frac(.7 * i.pos + 9 * texM.xyz + ( specularReflection * 0.04).r + 0.3 * i.viewDir);
			fp1 *= (1 - fp1);
			float Normglitter = saturate(1 - 7 * (fp1.x + fp1.y + fp1.z));
			float Normsparkle =  Normglitter * (pow(AniospecularReflection, 2.5) * _SpecColor.xyz) * _GlitterStrengh;
			
			
			fixed3 lightFinal = AniospecularReflection + Normsparkle + backScatter + translucence + UNITY_LIGHTMODEL_AMBIENT.xyz;
			
			
			
			return fixed4(texB * lightFinal * _Color, 1.0);
			}
			
#endif