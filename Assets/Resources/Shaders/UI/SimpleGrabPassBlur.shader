// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SimpleGrabPassBlur"
{
    Properties
    {
        _blurriness("Blurriness", Range(0, 30)) = 1
        [HideInInspector] _MainTex("MainTex", 2D) = "white" {}
    }

        HLSLINCLUDE
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        struct a2v { float4 position : POSITION; float2 uv: TEXCOORD0; };
    struct v2f { float4 position : SV_POSITION; float2 uv : TEXCOORD0; };

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;
    float _blurriness;

    static const float BLUR_DISTANCE_MULT = 4;

    v2f fullscreen_VS(a2v IN)
    {
        v2f OUT;
        OUT.position = TransformObjectToHClip(IN.position.xyz);
        OUT.uv = IN.uv;
        return OUT;
    }

#define BLUR_SAMPLE_COORD(COORD, OFS) saturate(IN.uv.COORD + OFS * BLUR_DISTANCE_MULT * _MainTex_TexelSize.COORD * _blurriness)
#define BLUR_SAMPLE_COORDS_X(OFS) float2(BLUR_SAMPLE_COORD(x, OFS), IN.uv.y)
#define BLUR_SAMPLE_COORDS_Y(OFS) float2(IN.uv.x, BLUR_SAMPLE_COORD(y, OFS))
#define BLUR_SAMPLE_ADD(IS_X, WEIGHT, OFS) sum += tex2D(_MainTex, (IS_X ? BLUR_SAMPLE_COORDS_X(OFS) : BLUR_SAMPLE_COORDS_Y(OFS))) * WEIGHT;
#define BLUR_SAMPLE_FUNC(IS_X) \
            float4 sum = float4(0, 0, 0, 0); \
            BLUR_SAMPLE_ADD(IS_X, 0.05, -4.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.09, -3.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.12, -2.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.15, -1.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.18,  0.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.15, +1.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.12, +2.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.09, +3.0) \
            BLUR_SAMPLE_ADD(IS_X, 0.05, +4.0)

    float4 blurHoriz_PS(v2f IN) : SV_Target
    {
        BLUR_SAMPLE_FUNC(true)
        return float4(sum.xyz, 1);
    }

        float4 blurVert_PS(v2f IN) : SV_Target
    {
        BLUR_SAMPLE_FUNC(false)
        //float3 hsv = rgb2hsv(sum.xyz);
        //hsv.y = saturate(hsv.y - DESATURATE_SUB * _blurriness) * lerp(1, DESATURATE_MULT, _blurriness);
        //return float4(hsv2rgb(hsv), 1);
        return float4(sum.xyz, 1);
    }
        ENDHLSL

        SubShader
    {
        Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

            Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }
            ZTest Always
            ZWrite Off
            Cull Off
            HLSLPROGRAM
                #pragma vertex fullscreen_VS
                #pragma fragment blurHoriz_PS
            ENDHLSL
        }

            Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }
            ZTest Always
            ZWrite Off
            Cull Off
            HLSLPROGRAM
                #pragma vertex fullscreen_VS
                #pragma fragment blurVert_PS
            ENDHLSL
        }
    }
}