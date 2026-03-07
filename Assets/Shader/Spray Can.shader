Shader "Mobile/SprayPaint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (1,1,0,1)
        _Threshold ("Threshold", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _KeyColor;
            half _Threshold;

            struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float dist = distance(tex.rgb, _KeyColor.rgb);

                float alpha = saturate(dist / _Threshold);

                fixed4 col;
                col.rgb = tex.rgb * i.color.rgb;
                col.a = tex.a * alpha * i.color.a;

                clip(col.a - 0.01);

                return col;
            }
            ENDCG
        }
    }
}
