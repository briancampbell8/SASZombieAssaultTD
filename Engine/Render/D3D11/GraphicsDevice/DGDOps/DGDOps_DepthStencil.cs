//==========================================================================================
// FILE: DGDOps_DepthStencil.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_DepthStencil.cs
// SUBSYSTEM: DGD / Depth-Stencil Buffer Management
//
// ROLE:
//     Creates and manages depth-stencil textures and views.
//     Recreates depth buffers on resize.
//
// RESPONSIBILITIES:
//     - Create depth-stencil texture.
//     - Create depth-stencil view (DSV).
//     - Recreate depth buffer when swap-chain is resized.
//     - Store DSV and texture in D3D11DeviceCore.
//
// NON-RESPONSIBILITIES:
//     - Binding DSV to the pipeline.
//     - Managing render targets or swap-chain buffers.
//     - Performing depth testing logic.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic depth-stencil buffer creation subsystem.
    /// </summary>
    public sealed class DGDOps_DepthStencil
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_DepthStencil(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Create or recreate the depth-stencil buffer and view.
        /// </summary>
        public void CreateDepthStencil()
        {
            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            int width = _deviceCore.Width;
            int height = _deviceCore.Height;

            if (width <= 0 || height <= 0)
                throw new InvalidOperationException("DeviceCore dimensions are invalid for depth-stencil creation.");

            // Dispose old resources
            _deviceCore.DepthStencilView?.Dispose();
            _deviceCore.DepthStencilTexture?.Dispose();

            // Create depth-stencil texture
            var texDesc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            var depthTexture = device.CreateTexture2D(texDesc);
            if (depthTexture == null)
                throw new InvalidOperationException("Failed to create depth-stencil texture.");

            _deviceCore.DepthStencilTexture = depthTexture;

            // Create depth-stencil view
            var dsv = device.CreateDepthStencilView(depthTexture);
            if (dsv == null)
                throw new InvalidOperationException("Failed to create depth-stencil view.");

            _deviceCore.DepthStencilView = dsv;
        }
    }
}
