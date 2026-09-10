struct VSOut {
    float4 position : SV_Position;
    float2 uv       : TEXCOORD0;
};

VSOut main(uint vertexId : SV_VertexID)
{
    VSOut o;

    float2 pos;
    pos.x = (vertexId == 2) ? 3.0 : -1.0;
    pos.y = (vertexId == 1) ? 3.0 : -1.0;

    o.position = float4(pos, 0.0, 1.0);
    o.uv = (pos + 1.0) * 0.5;

    return o;
}
