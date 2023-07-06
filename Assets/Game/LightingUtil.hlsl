#ifndef CUSTOM_LIGHTINGUTIL_INCLUDED
#define CUSTOM_LIGHTINGUTIL_INCLUDED

void CalculateMainLight_float(float3 WorldPos, out float3 Direction, out float3 Color) {
#if SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
#else
    //Light mainLight = GetMainLight(0);
    Direction = _MainLightPosition.xyz;
    Color = _MainLightColor.rgb;
#endif
}

#endif