//==========================================================================================
// FILE: DGDOps_Text.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Text.cs
// SUBSYSTEM: DGD / Text Rendering Helpers
//
// ROLE:
//     Provides deterministic GPU-side helpers for text rendering.
//     Defines the subsystem boundary for future glyph batching and font rendering.
//
// RESPONSIBILITIES:
//     - Prepare GPU pipeline for text rendering.
//     - Bind common sampler states.
//     - Serve as the GPU-facing layer for future text rendering logic.
//
// NON-RESPONSIBILITIES:
//     - CPU-side glyph layout or font rasterization.
//     - Vertex buffer creation or management.
//     - Shader compilation or material management.
//     - Render target or swap-chain management.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic GPU-side text rendering subsystem.
    /// </summary>
    public sealed class DGDOps_Text
    {
        private readonly D3D11DeviceCore _deviceCore;
        private readonly DGDOps_Samplers _samplers;

        public DGDOps_Text(D3D11DeviceCore deviceCore, DGDOps_Samplers samplers)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _samplers = samplers ?? throw new ArgumentNullException(nameof(samplers));
        }

        /// <summary>
        /// Prepare the GPU pipeline for text rendering.
        /// </summary>
        public void BeginText()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // Text rendering typically uses linear sampling.
            var sampler = _samplers.GetLinearSampler();
            context.PSSetSampler(0, sampler);
        }

        /// <summary>
        /// End text rendering. Restores minimal pipeline state.
        /// </summary>
        public void EndText()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // Unbind sampler from pixel shader slot 0.
            context.PSSetSampler(0, null);
        }
    }
}
