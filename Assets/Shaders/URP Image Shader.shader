Shader "Custom/UI/ImageGradient" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientTex ("Gradient", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 1)) = 0
        _Alpha ("Alpha", Range(0, 1)) = 1
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _GradientTex;
            float4 _EmissionColor;
            float _EmissionStrength;
            float _Alpha;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 mainColor = tex2D(_MainTex, i.uv);
                float4 gradientColor = tex2D(_GradientTex, i.uv);
                fixed4 emissionColor = _EmissionColor.rgb * _EmissionStrength;

                // Multiply the main color with the gradient color
                fixed4 finalColor = mainColor * gradientColor;

                // Add the emission color
                finalColor += emissionColor;

                // Apply the alpha value
                finalColor.a *= _Alpha;

                return finalColor;
            }
            ENDCG
        }
    }
}
