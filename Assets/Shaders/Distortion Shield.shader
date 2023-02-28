Shader "Custom/Distortion Shield" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0.0, 1.0)) = 0.1
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _Distortion;
            float4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 distortedUv = i.uv + (_Distortion * 0.5 - _Distortion * i.uv.yx) * (col.a * 2 - 1);
                col = tex2D(_MainTex, distortedUv);
                col.a *= i.color.a;
                return col * i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}