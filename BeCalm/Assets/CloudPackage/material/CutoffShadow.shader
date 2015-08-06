 Shader "JM/CutoffShadowCast" {
 Properties {
     _Cutoff ("Outline Thickness", Range(0.0,1.0)) = 0.5
     _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
     _MainTex ("Particle Texture", 2D) = "white"
 }
  
 SubShader {
     Cull Off
     Lighting Off
     ZWrite On
     //Fog { color (0,0,0,0) }
     AlphaTest Greater [_Cutoff]
     ColorMask RGB
     BindChannels {
         Bind "Color", color
         Bind "Vertex", vertex
         Bind "TexCoord", texcoord
     }
 
     Pass{
     Tags {  "LightMode" = "ShadowCaster" }
     SetTexture [_MainTex]
     }
 
 
     Pass {
 
     Tags { "Queue" = "AlphaTest" "RenderType"="Transparentcutout" }
 
         SetTexture [_MainTex] {
             constantColor [_TintColor]
             combine constant * primary DOUBLE
         }
         SetTexture [_MainTex] {
             combine previous * texture
         }
     }
 }
 Fallback "VertexLit"
 }