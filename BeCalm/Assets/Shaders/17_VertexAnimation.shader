Shader"EugeneShader/ShaderTestDemos/17_VertexAnimation"{
Properties{
	_Color("Color_Tint", Color) = (1.0,1.0,1.0,1.0)
	_SpecColor("Specular Color", Color) = (1.0,1.0,1.0,1.0)
	_Shininess("Shininess", Float) = 10.0
	_WaveXPos("Wave_X_Pos", Float) = 0.0
	_WaveYPos("Wave_Y_Pos", Float)  = 0.0
	_WaveZPos("Wave_Z_Pos", Float) = 0.0
	_WaveOffSetX("Wave OffSet X", Float) = 10.0
	_WaveOffSetY("Wave OffSet Y", Float) = 0.0
	_WaveOffSetZ("Wave OffSet Z", Float) = 0.0
	_PlayerPos("Player Position", Vector) = (1,1,1)
	_WaveScale("Wave Scale", Float) = 1.0
	_WaveSpeed("Wave Speed", Float) = 1.0
	_WaveDistance("Wave Distance", Float) = 1.0

}
	SubShader{
		Pass{
			Tags{"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			//user defined verables
			uniform fixed4 _Color;
			uniform fixed4 _SpecColor;
			uniform half _Shininess;
			//Wave INput
			uniform half _WaveXPos;
			uniform half _WaveYPos;
			uniform half _WaveZPos;
			uniform half _WaveOffSetX;
			uniform half _WaveOffSetY;
			uniform half _WaveOffSetZ;
			uniform half _WaveScale;
			uniform half _WaveSpeed;
			uniform half _WaveDistance;
			uniform half3 _PlayerPos;
			//unity defined verables
			uniform half4 _LightColor0;
			
			//Base Input structs
			
			struct vertexInput{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
			};
			
			struct vertexOutput{
				half4 pos : SV_POSITION;
				fixed3 normalDir : TEXCOORD0;
				fixed4 lightDir : TEXCOORD1;
				fixed3 viewDir : TEXCOORD2;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v)
			{
					vertexOutput o;
					
					//animation
					half3 wavePos = half3 (_WaveXPos, _WaveYPos, _WaveZPos);
					 v.vertex.z +  v.vertex.z + _WaveZPos;
					 
					//v.vertex.x = saturate(v.vertex.x + _WaveXPos);
					//_WaveZPos = v.vertex.z;
					
					//newPos.xyz = newPos.xyz + sin(_Time.x *_AnimSpeed + animOffset * _AnimFreq) * _WaveScale;
					
					//float oldPos = _AnimationPowerY;
					//float time = 10;
					//float Curtime = time + _Time.y; 
					
					
					//Normal Direction
					o.normalDir = normalize(mul(half4(v.normal, 0.0), _World2Object).xyz);
					
					//Unity transform Position
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					
					//world Position
					half4 posWorld = mul(_Object2World, v.vertex);
					//view Direction
					o.viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);
					//light Direction
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
				
			i.pos.z += sin (
			(_Time.y * _WaveSpeed +
			i.pos.y)
			/ _WaveDistance) * _WaveScale;

			//Lighting
			//dotProduct
			fixed nDotL = saturate(dot(i.normalDir, i.lightDir.xyz));
			
			//diffuse
			fixed3 diffuseReflection = i.lightDir.w * _LightColor0 * nDotL;
			//specular
			fixed3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect( - i.lightDir.xyz,i.normalDir),i.viewDir)),_Shininess);
			
			fixed3 lightFinal = diffuseReflection + specularReflection  + UNITY_LIGHTMODEL_AMBIENT.xyz;
			
			return fixed4(lightFinal * _Color.xyz, 1.0);
			}
			ENDCG
		}
}
//Fallback "Diffuse"
}





















