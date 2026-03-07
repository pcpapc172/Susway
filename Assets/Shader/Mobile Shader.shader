Shader "Mobile/ColorKey_AlphaBleed"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (1,1,0,1)
        _Threshold ("Key Threshold", Range(0,1)) = 0.15
        _Bleed ("Bleed Sharpness", Range(0.1,10)) = 5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half4 _KeyColor;
            half _Threshold;
            half _Bleed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half smoothMask(half dist)
            {
                half edge = (_Threshold / _Bleed) - _Threshold;
                half t = saturate(dist / edge);
                return t * t * (3.0h - 2.0h * t);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                half dist = distance(tex.rgb, _KeyColor.rgb);
                half mask = smoothMask(dist);

                half alpha = tex.a * mask;

                clip(alpha - 0.01h);

                fixed3 col = lerp(1 - tex.rgb, tex.rgb, 1 - mask);

                fixed4 final;
                final.rgb = col * i.color.rgb;
                final.a = alpha * i.color.a;

                return final;
            }

            ENDCG
        }
    }
}