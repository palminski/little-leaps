Shader "UI/ScanlineOverlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Line Color", Color) = (0,0,0,1)
        _LinePeriod ("Line Period (game pixels)", Float) = 2
        _LineThickness ("Line Thickness (game pixels)", Float) = 1
        _LineOpacity ("Line Opacity", Range(0,1)) = 0.9
        _LineSoftness ("Line Edge Softness (game pixels)", Range(0,1)) = 0.5
        _PixelScale ("Screen Pixels Per Game Pixel", Float) = 1

    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType" = "Transparent" "IgnoreProjector"="True" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata {float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f {float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 _Color;
            float _LinePeriod;
            float _LineThickness;
            float _LineOpacity;
            float _LineSoftness;
            float _PixelScale;

            half4 frag(v2f i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float screenPixelY = i.uv.y * _ScreenParams.y;
                float gamePixelRow = floor(screenPixelY / max(_PixelScale, 1));

                float halfPeriod = _LinePeriod * 0.5;
                float centered = fmod(gamePixelRow + halfPeriod, _LinePeriod) - halfPeriod;
                float distFromLineCenter = abs(centered);

                float halfThickness = _LineThickness * 0.5;
                float lineMask = 1.0 - smoothstep(halfThickness - _LineSoftness, halfThickness + _LineSoftness, distFromLineCenter);

                return half4(_Color.rgb, lineMask * _LineOpacity * tex.a);
            }
            ENDHLSL
        }
    }
}
