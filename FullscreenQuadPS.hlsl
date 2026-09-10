float4 main(float4 position : SV_Position, float2 uv : TEXCOORD0) : SV_Target
{
    return float4(uv, 0.0, 1.0);
}
