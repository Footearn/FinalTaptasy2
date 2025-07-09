// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SpriteMask/ClearStencil" {
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Stencil {
   Pass Zero
  }
  Blend One OneMinusSrcAlpha
  ColorMask 0
  GpuProgramID 59973
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
};

struct OUT_Data_Vert
{
    float4 xlv_COLOR0 :COLOR0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 xlv_COLOR0 :COLOR0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float4 tmpvar_1;
    float4 tmpvar_2;
    tmpvar_2 = clamp(in_v.color, 0, 1);
    tmpvar_1 = tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3.w = 1;
    tmpvar_3.xyz = float3(in_v.vertex.xyz);
    out_v.xlv_COLOR0 = tmpvar_1;
    out_v.vertex = UnityObjectToClipPos(tmpvar_3);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    out_f.color = in_f.xlv_COLOR0;
    return out_f;
}


ENDCG

}
}
}