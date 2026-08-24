Shader "Hidden/Outline"
{
    Properties
    {
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        ZTest Always
        ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert            
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"     

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 screenPos : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            TEXTURE2D(_IdTex);
            SAMPLER(sampler_IdTex);

            float4 _OutlineColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                float4 positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                float4 clipVertex = positionCS / positionCS.w;
                OUT.screenPos = ComputeScreenPos(clipVertex).xy;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color;
                half4 screenTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.screenPos);
                half4 IdTex = SAMPLE_TEXTURE2D(_IdTex, sampler_IdTex, IN.screenPos);
                half4 IdTexRight = SAMPLE_TEXTURE2D(_IdTex, sampler_IdTex, IN.screenPos + float2(_MainTex_TexelSize.x, 0));
                half4 IdTexUp = SAMPLE_TEXTURE2D(_IdTex, sampler_IdTex, IN.screenPos + float2(0, _MainTex_TexelSize.y));
                half4 IdTexLeft = SAMPLE_TEXTURE2D(_IdTex, sampler_IdTex, IN.screenPos + float2(-_MainTex_TexelSize.x, 0));
                half4 IdTexDown = SAMPLE_TEXTURE2D(_IdTex, sampler_IdTex, IN.screenPos + float2(0, -_MainTex_TexelSize.y));
                bool upHit = IdTex < IdTexUp;
                bool rightHit = IdTex < IdTexRight;
                bool leftHit = IdTex < IdTexLeft;
                bool downHit = IdTex < IdTexDown;
                half outline = upHit | downHit | leftHit | rightHit;
                color = screenTex * (1-outline) + _OutlineColor * outline;
                return color;
            }
            ENDHLSL
        }
    }
}