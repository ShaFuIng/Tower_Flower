Shader "Custom/SemiTransparentOcclusion"
{
    Properties
    {
        _Color("Tint Color", Color) = (1, 0.5, 0, 0.4)
    }

    SubShader
    {
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Transparent+10"
        }

        // 🔥 半透明渲染 Blend（畫顏色）
        Blend SrcAlpha OneMinusSrcAlpha

        // 🔥 同時寫入深度（可以遮擋 AR 物件）
        ZWrite On

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Color;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                return _Color;
            }

            ENDHLSL
        }
    }
}
