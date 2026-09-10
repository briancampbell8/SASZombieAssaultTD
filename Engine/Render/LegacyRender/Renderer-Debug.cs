// =====================================================================================================
//  FILE: Renderer-Debug.cs
//  PATH: Engine/Render/LegacyRender/Renderer-Debug.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Provides renderer-specific diagnostic utilities, internal state inspection helpers,
//      and lightweight instrumentation for development builds. This partial exposes safe,
//      non-invasive debug functionality without affecting production rendering behavior.
//
//  RESPONSIBILITIES:
//      - Expose deterministic debug information about renderer internals.
//      - Provide safe inspection helpers for development and testing.
//      - Offer lightweight instrumentation hooks for profiling and diagnostics.
//      - Maintain strict isolation from operational rendering logic.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations.
//      - Managing initialization, frame lifecycle, or viewport configuration.
//      - Implementing color grading, screenshots, or timing logic.
//      - Introducing dependencies that affect release builds.
//
//  ARCHITECTURAL NOTES:
//      - All debug helpers must remain optional and safe to call at any engine stage.
//      - This partial may expand as modernization progresses, but must never interfere
//        with deterministic rendering behavior.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Returns a compact diagnostic snapshot of internal renderer state.
        /// Safe for debug logging at any stage of the engine lifecycle.
        /// </summary>
        public string GetDebugState()
        {
            return $"[Renderer Debug] Init={_isInitialized}, " +
                   $"Viewport={_viewportSize.X}x{_viewportSize.Y}, " +
                   $"Scale={_renderScale:F2}, " +
                   $"VSync={_vsyncEnabled}, " +
                   $"ColorGrade={_colorGradingEnabled}, " +
                   $"GPU={_gpuContext}";
        }

        /// <summary>
        /// Emits a detailed renderer state dump to the diagnostic logger.
        /// </summary>
        public void DumpRendererState()
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                $"Renderer State Dump:\n" +
                $"  Initialized:        {_isInitialized}\n" +
                $"  Viewport:           {_viewportSize.X}x{_viewportSize.Y}\n" +
                $"  Clear Color:        {_clearColor}\n" +
                $"  VSync Enabled:      {_vsyncEnabled}\n" +
                $"  Render Scale:       {_renderScale:F2}\n" +
                $"  Color Grading:      {_colorGradingEnabled}\n" +
                $"  Tint:               {_colorGradeTint}\n" +
                $"  Contrast:           {_colorGradeContrast:F2}\n" +
                $"  Brightness:         {_colorGradeBrightness:F2}\n" +
                $"  GPU Context:        {_gpuContext}"
            );
        }

        /// <summary>
        /// Emits a single-line debug heartbeat for continuous monitoring.
        /// Useful for verifying renderer activity during long-running sessions.
        /// </summary>
        public void DebugHeartbeat()
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer Heartbeat: Init={_isInitialized}, VP={_viewportSize.X}x{_viewportSize.Y}, GPU={_gpuContext}"
            );
        }

        internal void DrawSprite(string sprite, Vector3 previewPos, Vector3 previewSize, Color color)
        {
            // Forward to the overload that accepts an explicit opacity value.
            // Use full opacity for the simple DrawSprite call.
            DrawSprite(sprite, previewPos, previewSize, color, 1f);
        }
    }
}
