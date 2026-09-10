//==========================================================================================
// FILE: DGDOps_DeviceContext.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_DeviceContext.cs
// SUBSYSTEM: DGD / Device Context Access
//
// ROLE:
//     Provides deterministic access to the active ID3D11DeviceContext.
//     Wraps D3D11DeviceCore and exposes the GPU command interface.
//
// RESPONSIBILITIES:
//     - Retrieve the active device context.
//     - Validate context availability.
//     - Expose context for rendering subsystems (DGDOps_*).
//
// NON-RESPONSIBILITIES:
//     - Creating the device.
//     - Managing swap chains.
//     - Performing rendering operations.
//     - Uploading framebuffer data.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic device-context access subsystem.
    /// </summary>
    public sealed class DGDOps_DeviceContext
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_DeviceContext(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Retrieve the active ID3D11DeviceContext.
        /// </summary>
        public ID3D11DeviceContext GetContext()
        {
            var ctx = _deviceCore.Context;

            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            return ctx;
        }

        /// <summary>
        /// Try-get variant for subsystems that prefer graceful fallback.
        /// </summary>
        public ID3D11DeviceContext TryGetContext()
        {
            return _deviceCore.Context;
        }
    }
}
