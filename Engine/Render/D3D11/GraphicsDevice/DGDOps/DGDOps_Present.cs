//==========================================================================================
// FILE: DGDOps_Present.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Present.cs
// SUBSYSTEM: DGD / Frame Submission & Presentation
//
// ROLE:
//     Flushes GPU commands and presents the swap chain.
//     Provides a deterministic end-of-frame submission path.
//
// RESPONSIBILITIES:
//     - Flush the ID3D11DeviceContext.
//     - Present the active swap chain.
//     - Expose a safe "Present" entry point for the engine.
//
// NON-RESPONSIBILITIES:
//     - Creating the device or swap chain.
//     - Managing render targets.
//     - Performing framebuffer uploads.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic frame submission and presentation subsystem.
    /// </summary>
    public sealed class DGDOps_Present
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_Present(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Flush GPU commands and present the swap chain.
        /// </summary>
        public void Present()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            var swapChain = _deviceCore.SwapChain;
            if (swapChain == null)
                throw new InvalidOperationException("Swap chain is not initialized.");

            // Ensure all queued GPU work is submitted.
            context.Flush();

            // Present with vsync enabled (sync interval = 1).
            swapChain.Present(1, 0);
        }

        /// <summary>
        /// Flush GPU commands without presenting (useful for offscreen rendering).
        /// </summary>
        public void Flush()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            context.Flush();
        }
    }
}
