//==========================================================================================
// FILE: DGDOps_RenderTargets.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_RenderTargets.cs
// SUBSYSTEM: DGD / Render Target Management
//
// ROLE:
//     Creates GPU-backed render targets using the active D3D11 device.
//     Provides a deterministic path from engine render targets to ID3D11RenderTargetView.
//
// RESPONSIBILITIES:
//     - Create render targets for the engine.
//     - Bridge IRenderTarget to D3D11RenderTarget.
//     - Use D3D11DeviceCore.Device for resource creation.
//
// NON-RESPONSIBILITIES:
//     - Managing swap chains.
//     - Framebuffer upload.
//     - Presentation or resizing.
//==========================================================================================

using System;
using System.Drawing.Imaging;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic render-target creation subsystem.
    /// </summary>
    public sealed class DGDOps_RenderTargets
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_RenderTargets(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Create a GPU-backed render target.
        /// </summary>
        public IRenderTarget CreateRenderTarget(string name, int width, int height, PixelFormat format)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Render target name cannot be null or empty.", nameof(name));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            // D3D11RenderTarget is your existing concrete implementation.
            return new D3D11RenderTarget(
                name,
                width,
                height,
                format,
                device);
        }
    }
}
