//==========================================================================================
// FILE: DGDOps_Initialize.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Initialize.cs
// SUBSYSTEM: DGD / Device Initialization
//
// ROLE:
//     Performs deterministic initialization of the D3D11 device core.
//     Sets width/height and marks the device as initialized.
//
// RESPONSIBILITIES:
//     - Validate initialization parameters.
//     - Store width and height in D3D11DeviceCore.
//     - Mark the device as initialized.
//
// NON-RESPONSIBILITIES:
//     - Creating the device or swap chain.
//     - Managing render targets.
//     - Performing framebuffer uploads.
//     - Presenting frames.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic initialization subsystem for D3D11DeviceCore.
    /// </summary>
    public sealed class DGDOps_Initialize
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_Initialize(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Initialize the device core with window handle and dimensions.
        /// </summary>
        public void Initialize(nint windowHandle, int width, int height)
        {
            if (windowHandle == nint.Zero)
                throw new ArgumentException("Window handle cannot be zero.", nameof(windowHandle));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");

            // Store dimensions
            _deviceCore.Width = width;
            _deviceCore.Height = height;

            // Mark initialized
            _deviceCore.IsInitialized = true;
        }
    }
}
