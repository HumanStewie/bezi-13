#ifndef GETLIGHT_INCLUDED
#define GETLIGHT_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
    #if (SHADERPASS != SHADERPASS_FORWARD)
        #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
    #endif
#endif

void GetLight_float(float3 worldPosition, out float3 direction, out float attenuation)
{
    #ifdef UNIVERSAL_LIGHTING_INCLUDED
    VertexPositionInputs vertexInput = (VertexPositionInputs)0;
    vertexInput.positionWS = worldPosition;
    float4 shadowCoord = GetShadowCoord(vertexInput);
    Light l = GetMainLight(shadowCoord);
    direction = l.direction;
    attenuation = l.shadowAttenuation * l.color.r;
    #else
    direction = float3(0.707, 0.707, 0);
    attenuation = 1;
    #endif
}
#endif