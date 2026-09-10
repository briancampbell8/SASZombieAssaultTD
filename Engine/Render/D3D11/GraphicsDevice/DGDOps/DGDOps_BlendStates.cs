//==========================================================================================
// FILE: DGDOps_BlendStates.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_BlendStates.cs
// SUBSYSTEM: DGD / Blend State Management
//
// ROLE:
//     Creates and manages common blend states (opaque, alpha blend, additive).
//     Provides deterministic GPU-side helpers for sprite, UI, and text rendering.
//
// RESPONSIBILITIES:
//     - Create blend states using D3D11DeviceCore.Device.
//     - Expose reusable blend-state instances.
//     - Bind/unbind blend states to the pipeline.
//
// NON-RESPONSIBILITIES:
//     - Managing shaders or samplers.
//     - Creating render targets or swap-chain buffers.
//     - Performing draw calls.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic blend-state subsystem.
    /// </summary>
    public sealed class DGDOps_BlendStates
    {
        private readonly D3D11DeviceCore _deviceCore;

        private ID3D11BlendState _opaque;
        private ID3D11BlendState _alphaBlend;
        private ID3D11BlendState _additive;

        public DGDOps_BlendStates(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        // -------------------------------------------------------------------------
        //  OPAQUE BLEND STATE (No blending)
        // -------------------------------------------------------------------------
        public ID3D11BlendState GetOpaque()
        {
            if (_opaque != null)
                return _opaque;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new BlendDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };

            desc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                BlendEnable = false,
                RenderTargetWriteMask = ColorWriteEnable.All
            };

            _opaque = device.CreateBlendState(desc);
            return _opaque;
        }

        // -------------------------------------------------------------------------
        //  ALPHA BLEND STATE (Standard transparency)
        // -------------------------------------------------------------------------
        public ID3D11BlendState GetAlphaBlend()
        {
            if (_alphaBlend != null)
                return _alphaBlend;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new BlendDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };

            desc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                BlendEnable = true,
                SourceBlend = Blend.One,
                DestinationBlend = Blend.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceBlendAlpha = Blend.One,
                DestinationBlendAlpha = Blend.InverseSourceAlpha,
                RenderTargetWriteMask = ColorWriteEnable.All
            };

            _alphaBlend = device.CreateBlendState(desc);
            return _alphaBlend;
        }

        // -------------------------------------------------------------------------
        //  ADDITIVE BLEND STATE (Glow, light effects)
        // -------------------------------------------------------------------------
        public ID3D11BlendState GetAdditive()
        {
            if (_additive != null)
                return _additive;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new BlendDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };

            desc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                BlendEnable = true,
                SourceBlend = Blend.One,
                DestinationBlend = Blend.One,
                BlendOperation = BlendOperation.Add,
                SourceBlendAlpha = Blend.One,
                DestinationBlendAlpha = Blend.One,
                BlendOperationAlpha = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteEnable.All
            };

            _additive = device.CreateBlendState(desc);
            return _additive;
        }

        // -------------------------------------------------------------------------
        //  PIPELINE BINDING HELPERS
        // -------------------------------------------------------------------------
        public void Bind(ID3D11BlendState blendState)
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            context.OMSetBlendState(blendState);
        }

        public void Unbind()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            context.OMSetBlendState(null);
        }
    }
}
