Shader "Custom/ARDepthMask"
{
    SubShader
    {
        Tags {
            "RenderType"="Opaque"
            "Queue"="Geometry-10"
        }

        Pass
        {
            // 不輸出顏色（完全透明）
            ColorMask 0

            // 寫入深度
            ZWrite On

            // 正常深度測試
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // ----- URP 6 / Unity 6 必須 include 這些 -----
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            // 頂點著色器
            Varyings vert(Attributes input)
            {
                Varyings o;

                // Unity 6 / URP 新函式：TransformObjectToHClip()
                o.positionHCS = TransformObjectToHClip(input.positionOS.xyz);

                return o;
            }

            // 片段著色器（不輸出任何顏色）
            float4 frag() : SV_Target
            {
                return float4(0, 0, 0, 0);
            }

            ENDHLSL
        }
    }
}
