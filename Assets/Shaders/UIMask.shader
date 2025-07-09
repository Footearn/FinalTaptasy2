// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Mask" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
 _Color ("Tint", Color) = (1,1,1,1)
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
}
SubShader { 
 Tags { "QUEUE"="Transparent-1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent-1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Stencil {
   Ref 1
   Pass Replace
  }
  Blend One OneMinusSrcAlpha
  ColorMask 0
  GpuProgramID 62947
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
uniform float4 _Color;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR :COLOR;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR :COLOR;
    float2 xlv_TEXCOORD0 :TEXCOORD0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float2 tmpvar_1;
    tmpvar_1 = in_v.texcoord.xy;
    float4 tmpvar_2;
    float2 tmpvar_3;
    tmpvar_3 = tmpvar_1;
    tmpvar_2 = (in_v.color * _Color);
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_COLOR = tmpvar_2;
    out_v.xlv_TEXCOORD0 = tmpvar_3;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 c_1;
    float4 tmpvar_2;
    tmpvar_2 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * in_f.xlv_COLOR);
    c_1.w = tmpvar_2.w;
    if((tmpvar_2.w<0.1))
    {
        discard;
    }
    c_1.xyz = float3((tmpvar_2.xyz * tmpvar_2.w));
    out_f.color = c_1;
    return out_f;
}


ENDCG

}
}
}