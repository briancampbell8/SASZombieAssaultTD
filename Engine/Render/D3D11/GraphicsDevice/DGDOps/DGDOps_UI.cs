//==========================================================================================
// FILE: DGDOps_UI.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_UI.cs
// SUBSYSTEM: DGD / UI Rendering Helpers
//
// ROLE:
//     Provides deterministic GPU-side helpers for UI rendering.
//     Defines the subsystem boundary for future UI batching and draw calls.
//
// RESPONSIBILITIES:
//     - Prepare GPU pipeline for UI rendering.
//     - Bind common sampler and pipeline state.
//     - Serve as the GPU-facing layer for future UI rendering logic.
//
// NON-RESPONSIBILITIES:
//     - CPU-side UI batching or layout.
//     - Vertex buffer creation or management.
//     - Shader compilation or material management.
//     - Render target or swap-chain management.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic GPU-side UI rendering subsystem.
    /// </summary>
    public sealed class DGDOps_UI
    {
        private readonly D3D11DeviceCore _deviceCore;
        private readonly DGDOps_Samplers _samplers;
        private readonly DGDOps_BlendStates _blendStates;
        private readonly DGDOps_Rasterizer _rasterizer;

        public DGDOps_UI(
            D3D11DeviceCore deviceCore,
            DGDOps_Samplers samplers,
            DGDOps_BlendStates blendStates,
            DGDOps_Rasterizer rasterizer)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _samplers = samplers ?? throw new ArgumentNullException(nameof(samplers));
            _blendStates = blendStates ?? throw new ArgumentNullException(nameof(blendStates));
            _rasterizer = rasterizer ?? throw new ArgumentNullException(nameof(rasterizer));
        }

        /// <summary>
        /// Prepare the GPU pipeline for UI rendering.
        /// </summary>
        public void BeginUI()
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // UI uses linear sampling.
            ctx.PSSetSampler(0, _samplers.GetLinearSampler());

            // UI uses alpha blending.
            ctx.OMSetBlendState(_blendStates.GetAlphaBlend());

            // UI uses solid rasterization with no culling.
            ctx.RSSetState(_rasterizer.GetSolidCullNone());
        }

        /// <summary>
        /// End UI rendering. Restores minimal pipeline state.
        /// </summary>
        public void EndUI()
        {
            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            ctx.PSSetSampler(0, null);
            ctx.OMSetBlendState(null);
            ctx.RSSetState(null);
        }
    }
}
