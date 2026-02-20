Shader "Unlit/ColorKey_AlphaBleed"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (1,1,0,1) // Yellow
        _Threshold ("Key Threshold", Float) = 0.15
        _Bleed ("Bleed Sharpness", Float) = 5.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _KeyColor;
            float _Threshold;
            float _Bleed;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float dist = distance(col.rgb, _KeyColor.rgb);

                // Soft fade: full alpha if far from key, zero alpha if at key, blend in-between
                float alpha = smoothstep(_Threshold, _Threshold / _Bleed, dist);

                // Optional: boost brightness near the edge (fake dilation)
                col.rgb = lerp(col.rgb, float3(1,1,1), 1 - alpha);

                col.a *= alpha;
                if (col.a < 0.01) discard;

                return col;
            }
            ENDCG
        }
    }
}
