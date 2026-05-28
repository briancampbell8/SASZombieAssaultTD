// Fullscreen Quad Vertex Shader
// Renders a single triangle covering the entire screen
// Uses clip-space coordinates to avoid vertex buffer

struct VSInput
{
    uint vertexID : SV_VertexID;
};

struct VSOutput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

VSOutput main(VSInput input)
{
    VSOutput output;
    
    // Generate fullscreen triangle using vertex ID
    // Vertex 0: (-1, -1) - bottom-left
    // Vertex 1: (3, -1)  - bottom-right (extends off-screen)
    // Vertex 2: (-1, 3)  - top-left (extends off-screen)
    
    float2 positions[3] = 
    {
        float2(-1.0, -1.0),
        float2(3.0, -1.0),
        float2(-1.0, 3.0)
    };
    
    float2 uvs[3] = 
    {
        float2(0.0, 1.0),
        float2(2.0, 1.0),
        float2(0.0, -1.0)
    };
    
    output.position = float4(positions[input.vertexID], 0.0, 1.0);
    output.uv = uvs[input.vertexID];
    
    return output;
}
