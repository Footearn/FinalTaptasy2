// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SpriteMask/Diffuse" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
 _Color ("Tint", Color) = (1,1,1,1)
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
 _Stencil ("Stencil Ref", Float) = 0
 _StencilComp ("Stencil Comparison", Float) = 8
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "SHADOWSUPPORT"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Stencil {
   Ref [_Stencil]
   Comp [_StencilComp]
  }
  Blend One OneMinusSrcAlpha
  GpuProgramID 56619
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


#define CODE_BLOCK_VERTEX
//uniform float4 unity_SHAr;
//uniform float4 unity_SHAg;
//uniform float4 unity_SHAb;
//uniform float4 unity_SHBr;
//uniform float4 unity_SHBg;
//uniform float4 unity_SHBb;
//uniform float4 unity_SHC;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
// uniform float4x4 _World2Object;
uniform float4 _Color;
uniform float4 _MainTex_ST;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD4 :TEXCOORD4;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD4 :TEXCOORD4;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float3 worldNormal_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    float4 tmpvar_4;
    tmpvar_4 = (in_v.color * _Color);
    tmpvar_3 = tmpvar_4;
    float4 v_5;
    v_5.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_5.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_5.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_5.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).y.x;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).y.x;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).y.x;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).y.x;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(unity_WorldToObject).z.x;
    v_7.y = conv_mxt4x4_1(unity_WorldToObject).z.x;
    v_7.z = conv_mxt4x4_2(unity_WorldToObject).z.x;
    v_7.w = conv_mxt4x4_3(unity_WorldToObject).z.x;
    float3 tmpvar_8;
    tmpvar_8 = normalize((((v_5.xyz * in_v.normal.x) + (v_6.xyz * in_v.normal.y)) + (v_7.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_8;
    tmpvar_2 = worldNormal_1;
    float3 normal_9;
    normal_9 = worldNormal_1;
    float4 tmpvar_10;
    tmpvar_10.w = 1;
    tmpvar_10.xyz = float3(normal_9);
    float3 res_11;
    float3 x_12;
    x_12.x = dot(unity_SHAr, tmpvar_10);
    x_12.y = dot(unity_SHAg, tmpvar_10);
    x_12.z = dot(unity_SHAb, tmpvar_10);
    float3 x1_13;
    float4 tmpvar_14;
    tmpvar_14 = (normal_9.xyzz * normal_9.yzzx);
    x1_13.x = dot(unity_SHBr, tmpvar_14);
    x1_13.y = dot(unity_SHBg, tmpvar_14);
    x1_13.z = dot(unity_SHBb, tmpvar_14);
    res_11 = (x_12 + (x1_13 + (unity_SHC.xyz * ((normal_9.x * normal_9.x) - (normal_9.y * normal_9.y)))));
    res_11 = max(((1.055 * pow(max(res_11, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    out_v.xlv_TEXCOORD3 = tmpvar_3;
    out_v.xlv_TEXCOORD4 = max(float3(0, 0, 0), res_11);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float3 tmpvar_3;
    float3 lightDir_4;
    float4 tmpvar_5;
    tmpvar_5 = in_f.xlv_TEXCOORD3;
    float3 tmpvar_6;
    tmpvar_6 = _WorldSpaceLightPos0.xyz;
    lightDir_4 = tmpvar_6;
    tmpvar_3 = in_f.xlv_TEXCOORD1;
    float3 tmpvar_7;
    float4 tmpvar_8;
    tmpvar_8 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * tmpvar_5);
    tmpvar_7 = (tmpvar_8.xyz * tmpvar_8.w);
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_4;
    float4 c_9;
    float4 c_10;
    float diff_11;
    float tmpvar_12;
    tmpvar_12 = max(0, dot(tmpvar_3, tmpvar_2));
    diff_11 = tmpvar_12;
    c_10.xyz = float3(((tmpvar_7 * tmpvar_1) * diff_11));
    c_10.w = tmpvar_8.w;
    c_9.w = c_10.w;
    c_9.xyz = float3((c_10.xyz + (tmpvar_7 * in_f.xlv_TEXCOORD4)));
    out_f.color = c_9;
    return out_f;
}


ENDCG

}
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Stencil {
   Ref [_Stencil]
   Comp [_StencilComp]
  }
  Blend One One
  GpuProgramID 117651
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
// uniform float4x4 _World2Object;
uniform float4 _Color;
uniform float4 _MainTex_ST;
//uniform float4 _WorldSpaceLightPos0;
uniform float4 _LightColor0;
uniform sampler2D _LightTexture0;
uniform float4x4 unity_WorldToLight;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float3 worldNormal_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    float4 tmpvar_4;
    tmpvar_4 = (in_v.color * _Color);
    tmpvar_3 = tmpvar_4;
    float4 v_5;
    v_5.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_5.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_5.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_5.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).y.x;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).y.x;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).y.x;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).y.x;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(unity_WorldToObject).z.x;
    v_7.y = conv_mxt4x4_1(unity_WorldToObject).z.x;
    v_7.z = conv_mxt4x4_2(unity_WorldToObject).z.x;
    v_7.w = conv_mxt4x4_3(unity_WorldToObject).z.x;
    float3 tmpvar_8;
    tmpvar_8 = normalize((((v_5.xyz * in_v.normal.x) + (v_6.xyz * in_v.normal.y)) + (v_7.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_8;
    tmpvar_2 = worldNormal_1;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    out_v.xlv_TEXCOORD3 = tmpvar_3;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float3 tmpvar_1;
    float3 tmpvar_2;
    float3 tmpvar_3;
    float3 lightDir_4;
    float4 tmpvar_5;
    tmpvar_5 = in_f.xlv_TEXCOORD3;
    float3 tmpvar_6;
    tmpvar_6 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD2));
    lightDir_4 = tmpvar_6;
    tmpvar_3 = in_f.xlv_TEXCOORD1;
    float4 tmpvar_7;
    tmpvar_7 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * tmpvar_5);
    float4 tmpvar_8;
    tmpvar_8.w = 1;
    tmpvar_8.xyz = float3(in_f.xlv_TEXCOORD2);
    float3 tmpvar_9;
    tmpvar_9 = mul(unity_WorldToLight, tmpvar_8).xyz.xyz;
    float tmpvar_10;
    tmpvar_10 = dot(tmpvar_9, tmpvar_9);
    float tmpvar_11;
    tmpvar_11 = tex2D(_LightTexture0, float2(tmpvar_10, tmpvar_10)).w.x;
    tmpvar_1 = _LightColor0.xyz;
    tmpvar_2 = lightDir_4;
    tmpvar_1 = (tmpvar_1 * tmpvar_11);
    float4 c_12;
    float4 c_13;
    float diff_14;
    float tmpvar_15;
    tmpvar_15 = max(0, dot(tmpvar_3, tmpvar_2));
    diff_14 = tmpvar_15;
    c_13.xyz = float3(((tmpvar_7.xyz * tmpvar_7.w) * (tmpvar_1 * diff_14)));
    c_13.w = tmpvar_7.w;
    c_12.w = c_13.w;
    c_12.xyz = float3(c_13.xyz);
    out_f.color = c_12;
    return out_f;
}


ENDCG

}
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Stencil {
   Ref [_Stencil]
   Comp [_StencilComp]
  }
  Blend One OneMinusSrcAlpha
  GpuProgramID 189520
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


#define CODE_BLOCK_VERTEX
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
// uniform float4x4 _World2Object;
uniform float4 _Color;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
};

struct OUT_Data_Vert
{
    float3 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float3 xlv_TEXCOORD0 :TEXCOORD0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float3 worldNormal_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    float4 tmpvar_4;
    tmpvar_4 = (in_v.color * _Color);
    tmpvar_3 = tmpvar_4;
    float4 v_5;
    v_5.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_5.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_5.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_5.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).y.x;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).y.x;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).y.x;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).y.x;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(unity_WorldToObject).z.x;
    v_7.y = conv_mxt4x4_1(unity_WorldToObject).z.x;
    v_7.z = conv_mxt4x4_2(unity_WorldToObject).z.x;
    v_7.w = conv_mxt4x4_3(unity_WorldToObject).z.x;
    float3 tmpvar_8;
    tmpvar_8 = normalize((((v_5.xyz * in_v.normal.x) + (v_6.xyz * in_v.normal.y)) + (v_7.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_8;
    tmpvar_2 = worldNormal_1;
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = tmpvar_2;
    out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    out_v.xlv_TEXCOORD2 = tmpvar_3;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 res_1;
    float3 tmpvar_2;
    tmpvar_2 = in_f.xlv_TEXCOORD0;
    res_1.xyz = float3(((tmpvar_2 * 0.5) + 0.5));
    res_1.w = 0;
    out_f.color = res_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassFinal" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Stencil {
   Ref [_Stencil]
   Comp [_StencilComp]
  }
  Blend One OneMinusSrcAlpha
  GpuProgramID 238287
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


#define CODE_BLOCK_VERTEX
//uniform float4 _ProjectionParams;
//uniform float4 unity_SHAr;
//uniform float4 unity_SHAg;
//uniform float4 unity_SHAb;
//uniform float4 unity_SHBr;
//uniform float4 unity_SHBg;
//uniform float4 unity_SHBb;
//uniform float4 unity_SHC;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
// uniform float4x4 _World2Object;
uniform float4 _Color;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
uniform sampler2D _LightBuffer;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float4 xlv_TEXCOORD4 :TEXCOORD4;
    float3 xlv_TEXCOORD5 :TEXCOORD5;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float4 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD5 :TEXCOORD5;
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
    float4 tmpvar_3;
    float3 tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5 = (in_v.color * _Color);
    tmpvar_2 = tmpvar_5;
    tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
    float4 o_6;
    float4 tmpvar_7;
    tmpvar_7 = (tmpvar_1 * 0.5);
    float2 tmpvar_8;
    tmpvar_8.x = tmpvar_7.x;
    tmpvar_8.y = (tmpvar_7.y * _ProjectionParams.x);
    o_6.xy = float2((tmpvar_8 + tmpvar_7.w));
    o_6.zw = tmpvar_1.zw;
    tmpvar_3.zw = float2(0, 0);
    tmpvar_3.xy = float2(0, 0);
    float4 v_9;
    v_9.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_9.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_9.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_9.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_10;
    v_10.x = conv_mxt4x4_0(unity_WorldToObject).y.x;
    v_10.y = conv_mxt4x4_1(unity_WorldToObject).y.x;
    v_10.z = conv_mxt4x4_2(unity_WorldToObject).y.x;
    v_10.w = conv_mxt4x4_3(unity_WorldToObject).y.x;
    float4 v_11;
    v_11.x = conv_mxt4x4_0(unity_WorldToObject).z.x;
    v_11.y = conv_mxt4x4_1(unity_WorldToObject).z.x;
    v_11.z = conv_mxt4x4_2(unity_WorldToObject).z.x;
    v_11.w = conv_mxt4x4_3(unity_WorldToObject).z.x;
    float4 tmpvar_12;
    tmpvar_12.w = 1;
    tmpvar_12.xyz = float3(normalize((((v_9.xyz * in_v.normal.x) + (v_10.xyz * in_v.normal.y)) + (v_11.xyz * in_v.normal.z))));
    float4 normal_13;
    normal_13 = tmpvar_12;
    float3 res_14;
    float3 x_15;
    x_15.x = dot(unity_SHAr, normal_13);
    x_15.y = dot(unity_SHAg, normal_13);
    x_15.z = dot(unity_SHAb, normal_13);
    float3 x1_16;
    float4 tmpvar_17;
    tmpvar_17 = (normal_13.xyzz * normal_13.yzzx);
    x1_16.x = dot(unity_SHBr, tmpvar_17);
    x1_16.y = dot(unity_SHBg, tmpvar_17);
    x1_16.z = dot(unity_SHBb, tmpvar_17);
    res_14 = (x_15 + (x1_16 + (unity_SHC.xyz * ((normal_13.x * normal_13.x) - (normal_13.y * normal_13.y)))));
    res_14 = max(((1.055 * pow(max(res_14, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    tmpvar_4 = res_14;
    out_v.vertex = tmpvar_1;
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    out_v.xlv_TEXCOORD2 = tmpvar_2;
    out_v.xlv_TEXCOORD3 = o_6;
    out_v.xlv_TEXCOORD4 = tmpvar_3;
    out_v.xlv_TEXCOORD5 = tmpvar_4;
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 tmpvar_1;
    float4 c_2;
    float4 light_3;
    float4 tmpvar_4;
    tmpvar_4 = in_f.xlv_TEXCOORD2;
    float4 tmpvar_5;
    tmpvar_5 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * tmpvar_4);
    float4 tmpvar_6;
    tmpvar_6 = tex2D(_LightBuffer, in_f.xlv_TEXCOORD3);
    light_3 = tmpvar_6;
    light_3 = (-log2(max(light_3, float4(0.001, 0.001, 0.001, 0.001))));
    light_3.xyz = float3((light_3.xyz + in_f.xlv_TEXCOORD5));
    float4 c_7;
    c_7.xyz = float3(((tmpvar_5.xyz * tmpvar_5.w) * light_3.xyz));
    c_7.w = tmpvar_5.w;
    c_2 = c_7;
    tmpvar_1 = c_2;
    out_f.color = tmpvar_1;
    return out_f;
}


ENDCG

}
 Pass {
  Name "DEFERRED"
  Tags { "LIGHTMODE"="Deferred" "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Stencil {
   Ref [_Stencil]
   Comp [_StencilComp]
  }
  Blend One OneMinusSrcAlpha
  GpuProgramID 295862
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
#define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)


#define CODE_BLOCK_VERTEX
//uniform float4 unity_SHAr;
//uniform float4 unity_SHAg;
//uniform float4 unity_SHAb;
//uniform float4 unity_SHBr;
//uniform float4 unity_SHBg;
//uniform float4 unity_SHBb;
//uniform float4 unity_SHC;
//uniform float4x4 UNITY_MATRIX_MVP;
//uniform float4x4 unity_ObjectToWorld;
// uniform float4x4 _World2Object;
uniform float4 _Color;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION;
    float4 color :COLOR;
    float3 normal :NORMAL;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float3 xlv_TEXCOORD2 :TEXCOORD2;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float4 xlv_TEXCOORD4 :TEXCOORD4;
    float3 xlv_TEXCOORD5 :TEXCOORD5;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 xlv_TEXCOORD0 :TEXCOORD0;
    float3 xlv_TEXCOORD1 :TEXCOORD1;
    float4 xlv_TEXCOORD3 :TEXCOORD3;
    float3 xlv_TEXCOORD5 :TEXCOORD5;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
    float4 color1 :SV_Target1;
    float4 color2 :SV_Target2;
    float4 color3 :SV_Target3;
};

OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    float3 worldNormal_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    float4 tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5 = (in_v.color * _Color);
    tmpvar_3 = tmpvar_5;
    float4 v_6;
    v_6.x = conv_mxt4x4_0(unity_WorldToObject).x;
    v_6.y = conv_mxt4x4_1(unity_WorldToObject).x;
    v_6.z = conv_mxt4x4_2(unity_WorldToObject).x;
    v_6.w = conv_mxt4x4_3(unity_WorldToObject).x;
    float4 v_7;
    v_7.x = conv_mxt4x4_0(unity_WorldToObject).y.x;
    v_7.y = conv_mxt4x4_1(unity_WorldToObject).y.x;
    v_7.z = conv_mxt4x4_2(unity_WorldToObject).y.x;
    v_7.w = conv_mxt4x4_3(unity_WorldToObject).y.x;
    float4 v_8;
    v_8.x = conv_mxt4x4_0(unity_WorldToObject).z.x;
    v_8.y = conv_mxt4x4_1(unity_WorldToObject).z.x;
    v_8.z = conv_mxt4x4_2(unity_WorldToObject).z.x;
    v_8.w = conv_mxt4x4_3(unity_WorldToObject).z.x;
    float3 tmpvar_9;
    tmpvar_9 = normalize((((v_6.xyz * in_v.normal.x) + (v_7.xyz * in_v.normal.y)) + (v_8.xyz * in_v.normal.z)));
    worldNormal_1 = tmpvar_9;
    tmpvar_2 = worldNormal_1;
    tmpvar_4.zw = float2(0, 0);
    tmpvar_4.xy = float2(0, 0);
    float3 normal_10;
    normal_10 = worldNormal_1;
    float4 tmpvar_11;
    tmpvar_11.w = 1;
    tmpvar_11.xyz = float3(normal_10);
    float3 res_12;
    float3 x_13;
    x_13.x = dot(unity_SHAr, tmpvar_11);
    x_13.y = dot(unity_SHAg, tmpvar_11);
    x_13.z = dot(unity_SHAb, tmpvar_11);
    float3 x1_14;
    float4 tmpvar_15;
    tmpvar_15 = (normal_10.xyzz * normal_10.yzzx);
    x1_14.x = dot(unity_SHBr, tmpvar_15);
    x1_14.y = dot(unity_SHBg, tmpvar_15);
    x1_14.z = dot(unity_SHBb, tmpvar_15);
    res_12 = (x_13 + (x1_14 + (unity_SHC.xyz * ((normal_10.x * normal_10.x) - (normal_10.y * normal_10.y)))));
    res_12 = max(((1.055 * pow(max(res_12, float3(0, 0, 0)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.xlv_TEXCOORD1 = tmpvar_2;
    out_v.xlv_TEXCOORD2 = mul(unity_ObjectToWorld, in_v.vertex).xyz;
    out_v.xlv_TEXCOORD3 = tmpvar_3;
    out_v.xlv_TEXCOORD4 = tmpvar_4;
    out_v.xlv_TEXCOORD5 = max(float3(0, 0, 0), res_12);
    return out_v;
}

#define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    float4 outEmission_1;
    float3 tmpvar_2;
    float4 tmpvar_3;
    tmpvar_3 = in_f.xlv_TEXCOORD3;
    tmpvar_2 = in_f.xlv_TEXCOORD1;
    float3 tmpvar_4;
    float4 tmpvar_5;
    tmpvar_5 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * tmpvar_3);
    tmpvar_4 = (tmpvar_5.xyz * tmpvar_5.w);
    float4 outDiffuseOcclusion_6;
    float4 outNormal_7;
    float4 emission_8;
    float4 tmpvar_9;
    tmpvar_9.w = 1;
    tmpvar_9.xyz = float3(tmpvar_4);
    outDiffuseOcclusion_6 = tmpvar_9;
    float4 tmpvar_10;
    tmpvar_10.w = 1;
    tmpvar_10.xyz = float3(((tmpvar_2 * 0.5) + 0.5));
    outNormal_7 = tmpvar_10;
    float4 tmpvar_11;
    tmpvar_11.w = 1;
    tmpvar_11.xyz = float3(0, 0, 0);
    emission_8 = tmpvar_11;
    emission_8.xyz = float3((emission_8.xyz + (tmpvar_4 * in_f.xlv_TEXCOORD5)));
    outEmission_1.w = emission_8.w;
    outEmission_1.xyz = float3(exp2((-emission_8.xyz)));
    out_f.color = outDiffuseOcclusion_6;
    out_f.color1 = float4(0, 0, 0, 0);
    out_f.color2 = outNormal_7;
    out_f.color3 = outEmission_1;
    return out_f;
}


ENDCG

}
}
Fallback "Transparent/VertexLit"
}