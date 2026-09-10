//==========================================================================================
// FILE: DGDOps_Shapes.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Shapes.cs
// SUBSYSTEM: DGD / Shape Rendering Helpers
//
// ROLE:
//     Provides deterministic GPU-side helpers for drawing primitive shapes.
//     Defines the subsystem boundary for future shape batching and draw calls.
//
// RESPONSIBILITIES:
//     - Prepare GPU pipeline for shape rendering.
//     - Bind common sampler and pipeline state.
//     - Serve as the GPU-facing layer for future shape rendering logic.
//
// NON-RESPONSIBILITIES:
//     - CPU-side shape batching.
//     - Vertex buffer creation or management.
//     - Shader compilation or material management.
//     - Render target or swap-chain management.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic GPU-side shape rendering subsystem.
    /// </summary>
    public sealed class DGDOps_Shapes
    {
        private readonly D3D11DeviceCore _deviceCore;
        private readonly DGDOps_Samplers _samplers;

        public DGDOps_Shapes(D3D11DeviceCore deviceCore, DGDOps_Samplers samplers)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _samplers = samplers ?? throw new ArgumentNullException(nameof(samplers));
        }

        /// <summary>
        /// Prepare the GPU pipeline for shape rendering.
        /// </summary>
        public void BeginShapes()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // Bind linear sampler to pixel shader slot 0.
            var sampler = _samplers.GetLinearSampler();
            context.PSSetSampler(0, sampler);
        }

        /// <summary>
        /// End shape rendering. Restores minimal pipeline state.
        /// </summary>
        public void EndShapes()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // Unbind sampler from pixel shader slot 0.
            context.PSSetSampler(0, null);
        }
    }
}
