Shader "Subway Surfers/Jetpack Smoke"
{
    Properties
    {
        _MainTex ("Smoke (RGBA)", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Lighting Off
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            SetTexture [_MainTex]
            {
                ConstantColor [_Color]
                combine texture * constant
            }
        }
    }

    Fallback Off
}
