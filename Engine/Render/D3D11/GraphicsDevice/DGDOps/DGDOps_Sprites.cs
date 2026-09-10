//==========================================================================================
// FILE: DGDOps_Sprites.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Sprites.cs
// SUBSYSTEM: DGD / Sprite Rendering Helpers
//
// ROLE:
//     Provides deterministic GPU-side helpers for sprite rendering.
//     Defines the subsystem boundary for future sprite batching and draw calls.
//
// RESPONSIBILITIES:
//     - Expose a clean entry point for sprite rendering operations.
//     - Provide access to device, context, and common sampler states.
//     - Serve as the GPU-facing layer for a future sprite batching system.
//
// NON-RESPONSIBILITIES:
//     - CPU-side sprite batching.
//     - Texture atlas management.
//     - Creating shaders or input layouts.
//     - Managing render targets or swap chains.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic GPU-side sprite rendering subsystem.
    /// </summary>
    public sealed class DGDOps_Sprites
    {
        private readonly D3D11DeviceCore _deviceCore;
        private readonly DGDOps_Samplers _samplers;

        public DGDOps_Sprites(D3D11DeviceCore deviceCore, DGDOps_Samplers samplers)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            _samplers = samplers ?? throw new ArgumentNullException(nameof(samplers));
        }

        /// <summary>
        /// Prepare the GPU pipeline for sprite rendering.
        /// This binds common sampler states and ensures the pipeline is ready for draw calls.
        /// </summary>
        public void BeginSprites()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // Bind linear sampler to pixel shader slot 0.
            var sampler = _samplers.GetLinearSampler();
            context.PSSetSampler(0, sampler);
        }

        /// <summary>
        /// End sprite rendering. This does not flush or present; it simply
        /// restores minimal pipeline state to avoid leaking sprite-specific bindings.
        /// </summary>
        public void EndSprites()
        {
            var context = _deviceCore.Context;
            if (context == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            // Unbind sampler from pixel shader slot 0.
            context.PSSetSampler(0, null);
        }
    }
}
