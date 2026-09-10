//==========================================================================================
// FILE: AdapterOps_CreateDevice.cs
// PATH: Engine/Render/Adapter/AdapterOps/AdapterOps_CreateDevice.cs
// SUBSYSTEM: Rendering / D3D11 Device Construction
//
// ROLE:
//     Deterministic creation of the D3D11DeviceCore hardware layer.
//     Supplies the manager with a fully constructed and validated device core.
//
// RESPONSIBILITIES:
//     - Construct D3D11DeviceCore using DXGI factory + window handle.
//     - Validate device + context.
//     - Expose the constructed device core to higher-level systems.
//
// NON-RESPONSIBILITIES:
//     - Enumerating adapters or outputs.
//     - Managing swap chains or pipelines.
//     - Performing any rendering or GPU operations.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic D3D11 device creation subsystem.
    /// </summary>
    public sealed class AdapterOps_CreateDevice
    {
        private readonly IDXGIFactory2 _factory;
        private readonly nint _windowHandle;

        public AdapterOps_CreateDevice(D3D11RenderContext context)
        {
        }

        public AdapterOps_CreateDevice(IDXGIFactory2 factory, nint windowHandle)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _windowHandle = windowHandle == nint.Zero
                ? throw new ArgumentException("Window handle cannot be zero.", nameof(windowHandle))
                : windowHandle;
        }

        /// <summary>
        /// Create the D3D11 device core deterministically.
        /// </summary>
        public D3D11DeviceCore CreateDevice(int width, int height, bool vsync)
        {
            var deviceCore = new D3D11DeviceCore(
                _factory,
                _windowHandle,
                width,
                height,
                vsync);

            ValidateDevice(deviceCore);
            return deviceCore;
        }

        /// <summary>
        /// Validate the constructed device for correctness and stability.
        /// </summary>
        public bool ValidateDevice(D3D11DeviceCore deviceCore)
        {
            if (deviceCore == null)
                throw new ArgumentNullException(nameof(deviceCore));

            if (deviceCore.Device == null)
                throw new InvalidOperationException("D3D11DeviceCore.Device must not be null.");

            if (deviceCore.Context == null)
                throw new InvalidOperationException("D3D11DeviceCore.Context must not be null.");

            if (deviceCore.BackbufferRtv == null)
                throw new InvalidOperationException("Backbuffer RTV must be created.");

            if (deviceCore.SwapChain == null)
                throw new InvalidOperationException("Swap chain must be created.");

            return true;
        }

        internal D3D11DeviceCore CreateDevice()
        {
            // Delegate to the overload that accepts explicit dimensions and vsync setting.
            // Use default values (0,0,false) which let the called overload decide sensible defaults.
            return CreateDevice(0, 0, false);
        }
    }
}
