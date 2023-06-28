//UNTIY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

void GrassDisplacement_float(float3 Pos, float3 ParentPos, float ParentSize, UnityTexture2D DisplacementMap, float2 Offset, UnitySamplerState Sampler, out float4 Out) {
    
    float2 pos = float2(Pos.x - (ParentPos.x - ParentSize / 2), Pos.z - (ParentPos.z - ParentSize / 2));

    int s = 1024;
    float x = (pos.x / ParentSize);
    float y = (pos.y / ParentSize);
    if (x / s > 1 || x / s < 0 || y / s > 1 || y / s < 0) {
        Out = float4(1, 1, 1, 1);
        return;
    }

    Out = SAMPLE_TEXTURE2D_LOD(DisplacementMap, Sampler, float2(x, y), 0);
}

#endif