inline float2 blockUV(float blockIndex, float2 baseUV)
{
    float x = (blockIndex + baseUV.x % 1) / 16;
    float y = (floor(blockIndex / 16) + baseUV.y % 1) / 16;
    return float2(x, y);
}

inline float2 damageUV(float damageIndex, float2 baseUV)
{
    float x = ((15 - damageIndex) + baseUV.x % 1) / 16;
    float y = (15 + baseUV.y % 1) / 16;
    return float2(x, y);
}