// ============================================================================
// File: PipelineState.cs
// Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\D3D11\PipelineState.cs
// Program: PipelineState
// Subsystem: Rendering / D3D11
//
// Purpose:
//     Lightweight D3D11 pipeline state container used by the modern rendering
//     backend. Tracks shader bindings and basic render state flags.
//
// Doctrine:
//     - No System.Diagnostics
//     - Engine.Diagnostics.DebugLogger.Trace() only if used externally (not needed here)
//     - Pure data container, no logic except Clone()
//     - Deterministic, grep‑friendly structure
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    /// <summary>
    /// Minimal D3D11 pipeline state for backend coordination.
    /// Stores shader identifiers and basic render state flags.
    /// </summary>
    public sealed class PipelineState
    {
        /// <summary>
        /// Gets or sets the current vertex shader identifier.
        /// </summary>
        public string VertexShader { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current pixel shader identifier.
        /// </summary>
        public string PixelShader { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether blending is enabled.
        /// </summary>
        public bool BlendEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets whether back‑face culling is enabled.
        /// </summary>
        public bool CullEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether depth testing is enabled.
        /// </summary>
        public bool DepthEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the primitive topology (default: 4 = TriangleList).
        /// </summary>
        public int PrimitiveTopology { get; set; } = 4;

        /// <summary>
        /// Creates a deep copy of this pipeline state.
        /// </summary>
        public PipelineState Clone()
        {
            return new PipelineState
            {
                VertexShader = VertexShader,
                PixelShader = PixelShader,
                BlendEnabled = BlendEnabled,
                CullEnabled = CullEnabled,
                DepthEnabled = DepthEnabled,
                PrimitiveTopology = PrimitiveTopology
            };
        }
    }
}
