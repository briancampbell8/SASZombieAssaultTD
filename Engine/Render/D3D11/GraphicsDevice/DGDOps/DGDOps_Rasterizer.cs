//==========================================================================================
// FILE: DGDOps_Rasterizer.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Rasterizer.cs
// SUBSYSTEM: DGD / Rasterizer State Management
//
// ROLE:
//     Creates and manages rasterizer states (solid, wireframe, culling modes).
//     Provides deterministic GPU-side helpers for all draw pipelines.
//
// RESPONSIBILITIES:
//     - Create rasterizer states using D3D11DeviceCore.Device.
//     - Expose reusable rasterizer-state instances.
//     - Bind/unbind rasterizer states to the pipeline.
//
// NON-RESPONSIBILITIES:
//     - Managing shaders or blend states.
//     - Creating render targets or swap-chain buffers.
//     - Performing draw calls.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic rasterizer-state subsystem.
    /// </summary>
    public sealed class DGDOps_Rasterizer
    {
        private readonly D3D11DeviceCore _deviceCore;

        private ID3D11RasterizerState _solidCullBack;
        private ID3D11RasterizerState _solidCullNone;
        private ID3D11RasterizerState _wireframe;

        public DGDOps_Rasterizer(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        // -------------------------------------------------------------------------
        //  SOLID (Cull Back)
        // -------------------------------------------------------------------------
        public ID3D11RasterizerState GetSolidCullBack()
        {
            if (_solidCullBack != null)
                return _solidCullBack;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new RasterizerDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                FrontCounterClockwise = false,
                DepthClipEnable = true,
                ScissorEnable = false,
                MultisampleEnable = false,
                AntialiasedLineEnable = false
            };

            _solidCullBack = device.CreateRasterizerState(desc);
            return _solidCullBack;
        }

        // -------------------------------------------------------------------------
        //  SOLID (Cull None)
        // -------------------------------------------------------------------------
        public ID3D11RasterizerState GetSolidCullNone()
        {
            if (_solidCullNone != null)
                return _solidCullNone;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new RasterizerDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                FrontCounterClockwise = false,
                DepthClipEnable = true,
                ScissorEnable = false,
                MultisampleEnable = false,
                AntialiasedLineEnable = false
            };

            _solidCullNone = device.CreateRasterizerState(desc);
            return _solidCullNone;
        }

        // -------------------------------------------------------------------------
        //  WIREFRAME
        // -------------------------------------------------------------------------
        public ID3D11RasterizerState GetWireframe()
        {
            if (_wireframe != null)
                return _wireframe;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new RasterizerDescription
            {
                FillMode = FillMode.Wireframe,
                CullMode = CullMode.None,
                FrontCounterClockwise = false,
                DepthClipEnable = true,
                ScissorEnable = false,
                MultisampleEnable = false,
                AntialiasedLineEnable = false
            };

            _wireframe = device.CreateRasterizerState(desc);
            return _wireframe;
        }

        // -------------------------------------------------------------------------
        //  PIPELINE BINDING HELPERS
        // -------------------------------------------------------------------------
        public void Bind(ID3D11RasterizerState state)
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            context.RSSetState(state);
        }

        public void Unbind()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            context.RSSetState(null);
        }
    }
}
