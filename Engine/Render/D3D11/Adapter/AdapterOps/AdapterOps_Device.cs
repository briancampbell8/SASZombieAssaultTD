//==========================================================================================
// FILE: AdapterOps_Device.cs
// PATH: Engine/Render/Adapter/AdapterOps_Device.cs
// SUBSYSTEM: Rendering / D3D11 Active Device Management
//
// ROLE:
// Provides deterministic management of the active D3D11 device and immediate
// context. Acts as the stable interface for all subsystems requiring access
// to the GPU device, ensuring consistent lifetime, state, and retrieval.
//
// RESPONSIBILITIES:
//  - Store and expose the active D3D11 device and immediate context.
//  - Provide deterministic lifetime management for the device core.
//  - Validate device availability before subsystem operations.
//  - Supply structured device-state information to higher-level systems.
//  - Support adapter manager orchestration with stable device access.
//
// NON-RESPONSIBILITIES:
//  - Creating the D3D11 device (handled by AdapterOps_CreateDevice).
//  - Managing swap chains, pipelines, or frame lifecycle.
//  - Allocating GPU resources or pipeline states.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    /// <summary>
    /// Deterministic active D3D11 device management subsystem.
    /// </summary>
    public sealed class AdapterOps_Device
    {
        private readonly D3D11RenderContext _context;
        private D3D11DeviceCore _activeDevice;
        private bool _disposed;

        public AdapterOps_Device(D3D11RenderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Set the active D3D11 device.
        public void SetActiveDevice(D3D11DeviceCore deviceCore)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AdapterOps_Device));

            _activeDevice = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        // Retrieve the active D3D11 device.
        public D3D11DeviceCore GetActiveDevice()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AdapterOps_Device));

            return _activeDevice;
        }

        // Retrieve the active immediate context.
        public ID3D11DeviceContext GetImmediateContext()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AdapterOps_Device));

            return _activeDevice?.Context;
        }

        // Validate that a device is currently active.
        public bool ValidateActiveDevice()
        {
            if (_disposed)
                return false;

            return _activeDevice != null &&
                   _activeDevice.Device != null &&
                   _activeDevice.Context != null;
        }

        // Retrieve structured device-state information.
        public object GetDeviceStateInfo()
        {
            var deviceCore = _activeDevice;

            var renderContextInfo = new
            {
                Width = _context?.Width ?? 0,
                Height = _context?.Height ?? 0,
                AspectRatio = _context?.AspectRatio ?? 0.0f
            };

            var activeDeviceInfo = new
            {
                Present = deviceCore != null,
                DevicePresent = deviceCore?.Device != null,
                ContextPresent = deviceCore?.Context != null,
                SwapChainPresent = deviceCore?.SwapChain != null,
                BackbufferRtvPresent = deviceCore?.BackbufferRtv != null,
                Width = deviceCore?.Width ?? renderContextInfo.Width,
                Height = deviceCore?.Height ?? renderContextInfo.Height
            };

            return new
            {
                RenderContext = renderContextInfo,
                ActiveDevice = activeDeviceInfo,
                Disposed = _disposed
            };
        }

        public void Dispose()
        {
            _disposed = true;
            _activeDevice = null;
        }
    }
}
