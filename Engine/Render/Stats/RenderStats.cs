// ====================================================================================================
//  FILE: RenderStats.cs
//  PATH: Engine\Render\Stats\RenderStats.cs
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide Reset() behavior for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Render.Stats
{
    /// <summary>
    /// Per-frame rendering statistics for the rendering subsystem.
    /// Backend-agnostic; consumed by UI, diagnostics, and frame pacing logic.
    /// </summary>
    public sealed class RenderStats
    {
        //--------------------------------------------------------------------
        // Core metrics
        //--------------------------------------------------------------------

        /// <summary>
        /// Number of draw calls executed this frame.
        /// </summary>
        public int DrawCalls { get; set; }

        /// <summary>
        /// Number of state changes (shaders, blend, rasterizer, etc.).
        /// </summary>
        public int StateChanges { get; set; }

        /// <summary>
        /// Number of frames rendered since startup.
        /// </summary>
        public int FramesRendered { get; set; }

        //--------------------------------------------------------------------
        // Timing metrics
        //--------------------------------------------------------------------

        /// <summary>
        /// CPU frame time in milliseconds.
        /// </summary>
        public double CpuFrameTimeMs { get; set; }

        /// <summary>
        /// GPU frame time in milliseconds.
        /// </summary>
        public double GpuFrameTimeMs { get; set; }

        //--------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------

        public RenderStats() => Reset();

        //--------------------------------------------------------------------
        // Reset
        //--------------------------------------------------------------------

        /// <summary>
        /// Resets all counters to zero.
        /// </summary>
        public void Reset()
        {
            DrawCalls = 0;
            StateChanges = 0;
            FramesRendered = 0;
            CpuFrameTimeMs = 0.0;
            GpuFrameTimeMs = 0.0;
        }
    }
}
