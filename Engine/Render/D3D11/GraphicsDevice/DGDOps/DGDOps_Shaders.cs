//==========================================================================================
// FILE: DGDOps_Shaders.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Shaders.cs
// SUBSYSTEM: DGD / Shader and Input Layout Management
//
// ROLE:
//     Provides deterministic creation and retrieval of vertex/pixel shaders and input layouts.
//     Wraps D3D11DeviceCore.Device for shader-related resource creation.
//
// RESPONSIBILITIES:
//     - Create vertex and pixel shaders from compiled bytecode.
//     - Create input layouts from vertex shader signatures.
//     - Expose reusable shader and layout instances to DGDOps subsystems.
//
// NON-RESPONSIBILITIES:
//     - Compiling HLSL (handled elsewhere).
//     - Binding shaders to the pipeline.
//     - Managing shader permutations or materials.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic shader and input-layout subsystem.
    /// </summary>
    public sealed class DGDOps_Shaders
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_Shaders(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        // -------------------------------------------------------------------------
        //  PUBLIC: Create Vertex Shader
        // -------------------------------------------------------------------------
        public ID3D11VertexShader CreateVertexShader(byte[] bytecode)
        {
            if (bytecode == null || bytecode.Length == 0)
                throw new ArgumentException("Vertex shader bytecode cannot be null or empty.", nameof(bytecode));

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            return device.CreateVertexShader(bytecode);
        }

        // -------------------------------------------------------------------------
        //  PUBLIC: Create Pixel Shader
        // -------------------------------------------------------------------------
        public ID3D11PixelShader CreatePixelShader(byte[] bytecode)
        {
            if (bytecode == null || bytecode.Length == 0)
                throw new ArgumentException("Pixel shader bytecode cannot be null or empty.", nameof(bytecode));

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            return device.CreatePixelShader(bytecode);
        }

        // -------------------------------------------------------------------------
        //  PUBLIC: Create Input Layout
        // -------------------------------------------------------------------------
        public ID3D11InputLayout CreateInputLayout(InputElementDescription[] elements, byte[] vertexShaderBytecode)
        {
            if (elements == null || elements.Length == 0)
                throw new ArgumentException("Input elements cannot be null or empty.", nameof(elements));

            if (vertexShaderBytecode == null || vertexShaderBytecode.Length == 0)
                throw new ArgumentException("Vertex shader bytecode cannot be null or empty.", nameof(vertexShaderBytecode));

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            return device.CreateInputLayout(elements, vertexShaderBytecode);
        }
    }
}
