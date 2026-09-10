//==========================================================================================
// FILE: DGDOps_Resize.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Resize.cs
// SUBSYSTEM: DGD / Swap Chain Resize Preparation
//
// ROLE:
//     Provides deterministic pre-resize behavior for the D3D11 pipeline.
//     Clears pipeline state and updates device dimensions before swap-chain buffers are recreated.
//
// RESPONSIBILITIES:
//     - Clear pipeline state to release references to swap-chain buffers.
//     - Update Width/Height in D3D11DeviceCore.
//     - Provide a safe entry point for resize operations.
//
// NON-RESPONSIBILITIES:
//     - Actually resizing the swap chain (handled by DGDOps_SwapChain).
//     - Recreating render targets.
//     - Managing presentation or framebuffer upload.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic swap-chain resize preparation subsystem.
    /// </summary>
    public sealed class DGDOps_Resize
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_Resize(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Prepare the device for a swap-chain resize.
        /// Clears pipeline state and updates dimensions.
        /// </summary>
        public void PrepareResize(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");

            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // Clear pipeline state to release references to swap-chain buffers.
            try
            {
                context.ClearState();
            }
            catch
            {
                // Non-fatal: continue with resize.
            }

            // Update dimensions in the device core.
            _deviceCore.Width = width;
            _deviceCore.Height = height;
        }
    }
}
