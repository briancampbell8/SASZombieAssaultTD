// ====================================================================================================
//  FILE: DebugOverlay.cs
//  PATH: Engine/Diagnostics/
//  PROGRAM: DebugOverlay.cs
//  MODULE: Diagnostics Pipeline (Visual Overlay)
//
//  ROLE:
//      Provides an on-screen diagnostics overlay for development builds.
//      Displays real-time engine metrics, debug text, and diagnostic information
//      to assist with engine bring-up, profiling, and troubleshooting.
//
//  RESPONSIBILITIES:
//      - Render lightweight, real-time diagnostic information on screen.
//      - Display subsystem metrics, frame timing, and debug text.
//      - Integrate with the diagnostics pipeline for live feedback.
//      - Remain unobtrusive and low-overhead during development.
//
//  NON-RESPONSIBILITIES:
//      - Performing structured logging (handled by DLogger and Writer).
//      - Managing engine resources, assets, or gameplay logic.
//      - Handling rendering pipelines beyond overlay drawing.
//      - Generating reports, analytics, or long-term storage.
//
//  ARCHITECTURAL NOTES:
//      - DebugOverlay is typically disabled or stripped in release builds.
//      - Should remain lightweight to avoid impacting performance metrics.
//      - Acts as a visual consumer of diagnostics, not a producer.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Modern debug overlay for showing FPS and diagnostic info.
    /// </summary>
    public sealed class DebugOverlay
    {
        private readonly FrameStats _stats;
        private readonly D3D11Adapter_Core _renderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugOverlay"/> class.
        /// </summary>
        /// <param name="stats">The frame statistics to display.</param>
        /// <param name="renderContext">The rendering context for drawing text.</param>
        public DebugOverlay(FrameStats stats, D3D11Adapter_Core renderContext)
        {
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        }

        /// <summary>
        /// Renders the debug overlay with FPS, frame count, and memory usage.
        /// </summary>
        public void Render()
        {
            //Display FPS
            DrawText($"FPS: {_stats.FramesPerSecond:F1}", 10, 10);

            //Display frame count
            DrawText($"Frames: {_stats.FrameCount}", 10, 30);

            //Display memory usage
            var memory = GC.GetTotalMemory(false) / 1024 / 1024;
            DrawText($"Memory: {memory} MB", 10, 50);
        }

        /// <summary>
        /// Draws text on the screen using the render context.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="x">The x-coordinate of the text.</param>
        /// <param name="y">The y-coordinate of the text.</param>
        private void DrawText(string text, float x, float y)
        {
            _renderContext.DrawText(text, new Vector2(x, y), 12.0f, Color.White);
        }
    }
}