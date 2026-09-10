//==========================================================================================
// FILE: DGDOps_SwapChain.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_SwapChain.cs
// SUBSYSTEM: DGD / Swap Chain Management
//
// ROLE:
//     Handles swap-chain resizing and backbuffer RTV recreation.
//     Wraps D3D11DeviceCore swap-chain operations deterministically.
//
// RESPONSIBILITIES:
//     - Resize swap chain buffers.
//     - Recreate backbuffer RTV.
//     - Expose safe resize behavior to pipeline manager.
//
// NON-RESPONSIBILITIES:
//     - Creating the device.
//     - Binding pipeline state.
//     - Uploading framebuffer pixels.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    public sealed class DGDOps_SwapChain
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_SwapChain(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Resize the swap chain and recreate the backbuffer RTV.
        /// </summary>
        public void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Invalid resize dimensions.");

            var swapChain = _deviceCore.SwapChain;
            if (swapChain == null)
                throw new InvalidOperationException("Swap chain is not initialized.");

            // Dispose old resources
            _deviceCore.BackbufferRtv?.Dispose();
            _deviceCore.DepthStencilView?.Dispose();
            _deviceCore.DepthStencilTexture?.Dispose();

            // Resize swap chain buffers
            swapChain.ResizeBuffers(
                2,
                (uint)width,
                (uint)height,
                Vortice.DXGI.Format.B8G8R8A8_UNorm,
                Vortice.DXGI.SwapChainFlags.None);

            // Update dimensions
            _deviceCore.Width = width;
            _deviceCore.Height = height;

            // Recreate backbuffer RTV
            using var backbuffer = swapChain.GetBuffer<ID3D11Texture2D>(0);
            _deviceCore.BackbufferRtv = _deviceCore.Device.CreateRenderTargetView(backbuffer);
        }


    }
}
