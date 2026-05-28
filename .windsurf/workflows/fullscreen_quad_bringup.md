---
description: Fullscreen Quad Bring-Up with Real Shaders
---

# Fullscreen Quad Bring-Up Task

## Objective
Re-enable the fullscreen quad with real compiled HLSL bytecode instead of placeholder stubs. This validates the shader pipeline, input layout, device context, adapter, and render target.

## Prerequisites
- Phase 1 (IRenderContext duplication fix) is complete
- Engine builds successfully with 0 errors
- D3D11 device is initialized
- RenderContextD3D11Adapter is functional

## Step 1: Create HLSL Vertex Shader

Create file: `Engine/Rendering/Shaders/FullscreenQuad.vs`

```hlsl
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
```

## Step 2: Create HLSL Pixel Shader

Create file: `Engine/Rendering/Shaders/FullscreenQuad.ps`

```hlsl
// Fullscreen Quad Pixel Shader
// Outputs a solid color (can be modified for texture sampling later)

struct PSInput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

float4 main(PSInput input) : SV_TARGET
{
    // Output a test color (dark blue for visibility)
    return float4(0.1, 0.1, 0.3, 1.0);
}
```

## Step 3: Compile Shaders with DXC

Use DirectX Shader Compiler (dxc) to compile shaders to bytecode:

```powershell
# Compile Vertex Shader
dxc -T vs_5_0 -E main Engine/Rendering/Shaders/FullscreenQuad.vs -Fo Engine/Rendering/Shaders/FullscreenQuad.vs.bin

# Compile Pixel Shader
dxc -T ps_5_0 -E main Engine/Rendering/Shaders/FullscreenQuad.ps -Fo Engine/Rendering/Shaders/FullscreenQuad.ps.bin
```

If dxc is not available, use fxc (older DirectX Shader Compiler):

```powershell
# Compile Vertex Shader
fxc /T vs_5_0 /E main Engine/Rendering/Shaders/FullscreenQuad.vs /Fo Engine/Rendering/Shaders/FullscreenQuad.vs.bin

# Compile Pixel Shader
fxc /T ps_5_0 /E main Engine/Rendering/Shaders/FullscreenQuad.ps /Fo Engine/Rendering/Shaders/FullscreenQuad.ps.bin
```

## Step 4: Convert Bytecode to C# Byte Array

Create a helper script to convert compiled bytecode to C# byte array:

```powershell
# Convert shader bytecode to C# byte array
$vsBytes = [System.IO.File]::ReadAllBytes("Engine/Rendering/Shaders/FullscreenQuad.vs.bin")
$psBytes = [System.IO.File]::ReadAllBytes("Engine/Rendering/Shaders/FullscreenQuad.ps.bin")

$vsArray = "byte[] vsBytecode = { " + ($vsBytes -join ", ") + " };"
$psArray = "byte[] psBytecode = { " + ($psBytes -join ", ") + " };"

$vsArray | Out-File "Engine/Rendering/Shaders/FullscreenQuad.vs.bytes.cs" -Encoding UTF8
$psArray | Out-File "Engine/Rendering/Shaders/FullscreenQuad.ps.bytes.cs" -Encoding UTF8
```

## Step 5: Embed Bytecode in D3D11DeviceCoreQuad

Read `Engine/Rendering/D3D11/D3D11DeviceCoreQuad.cs` and:

1. Replace the placeholder 60-byte stubs with the real compiled bytecode
2. Update the bytecode length constants
3. Ensure the bytecode arrays are static readonly

Example:
```csharp
private static readonly byte[] s_vsBytecode = {
    // Paste compiled VS bytecode here
};

private static readonly byte[] s_psBytecode = {
    // Paste compiled PS bytecode here
};
```

## Step 6: Restore InitializeFullscreenQuad()

In `D3D11DeviceCoreQuad.cs`, restore the `InitializeFullscreenQuad()` method:

```csharp
private void InitializeFullscreenQuad()
{
    // Create vertex shader
    var vsResult = _device.CreateVertexShader(
        s_vsBytecode,
        s_vsBytecode.Length,
        null
    );
    
    if (vsResult.Failed)
    {
        Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Error, "ERROR", 
            $"Failed to create fullscreen quad vertex shader: {vsResult.HResult}");
        return;
    }
    
    _fullscreenQuadVS = vsResult.Value;
    
    // Create pixel shader
    var psResult = _device.CreatePixelShader(
        s_psBytecode,
        s_psBytecode.Length,
        null
    );
    
    if (psResult.Failed)
    {
        Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Error, "ERROR", 
            $"Failed to create fullscreen quad pixel shader: {psResult.HResult}");
        return;
    }
    
    _fullscreenQuadPS = psResult.Value;
    
    // Create input layout (empty for fullscreen triangle)
    // No vertex buffer needed - using SV_VertexID
    var inputLayoutDesc = new InputLayoutDesc();
    var layoutResult = _device.CreateInputLayout(
        inputLayoutDesc,
        s_vsBytecode,
        s_vsBytecode.Length
    );
    
    if (layoutResult.Failed)
    {
        Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Error, "ERROR", 
            $"Failed to create fullscreen quad input layout: {layoutResult.HResult}");
        return;
    }
    
    _fullscreenQuadInputLayout = layoutResult.Value;
    
    Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info, "INFO", 
        "Fullscreen quad initialized successfully with real shaders");
}
```

## Step 7: Validate Input Layout

Ensure the input layout is created correctly. Since we're using `SV_VertexID`, the input layout should be empty (no input elements).

## Step 8: Update Render Method

In the render method, ensure the fullscreen quad is drawn:

```csharp
private void RenderFullscreenQuad()
{
    if (_fullscreenQuadVS == null || _fullscreenQuadPS == null)
        return;
    
    _context.VSSetShader(_fullscreenQuadVS);
    _context.PSSetShader(_fullscreenQuadPS);
    _context.IASetInputLayout(_fullscreenQuadInputLayout);
    _context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
    
    // Draw 3 vertices (fullscreen triangle)
    _context.Draw(3, 0);
}
```

## Step 9: Test Shader Creation

Add validation to ensure no `E_INVALIDARG` errors:

```csharp
if (vsResult.HResult == unchecked((int)0x80070057)) // E_INVALIDARG
{
    Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Error, "ERROR", 
        "Invalid argument in vertex shader creation - check bytecode format");
}
```

## Step 10: Build and Test

1. Build the project: `dotnet build`
2. Run the application
3. Verify no shader creation errors in logs
4. Verify the screen renders with the test color (dark blue)
5. Check for any E_INVALIDARG errors

## Success Criteria

- [ ] Shaders compile successfully with dxc/fxc
- [ ] Bytecode is embedded in D3D11DeviceCoreQuad
- [ ] InitializeFullscreenQuad() executes without errors
- [ ] Vertex shader creation succeeds (no E_INVALIDARG)
- [ ] Pixel shader creation succeeds (no E_INVALIDARG)
- [ ] Input layout creation succeeds
- [ ] Fullscreen quad renders with test color
- [ ] No shader-related errors in logs
- [ ] Build succeeds with 0 errors

## Troubleshooting

If shader creation fails with E_INVALIDARG:
- Verify bytecode was compiled with correct shader model (vs_5_0, ps_5_0)
- Verify bytecode file was read correctly (check file size)
- Verify D3D11 device supports SM 5.0
- Check for any bytecode corruption during file read

If screen remains black:
- Verify render target is set correctly
- Verify viewport is configured
- Verify Draw() call is being executed
- Check for any device context errors

## Notes

- Using SV_VertexID eliminates the need for a vertex buffer
- Fullscreen triangle is more efficient than quad (3 vertices vs 4)
- Test color can be changed later to sample from a texture
- This is the foundation for post-processing effects later
