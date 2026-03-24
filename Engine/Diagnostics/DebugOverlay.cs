/*
File:    DebugOverlay.cs
Author:  BDC
Created: 2026-02-10

Purpose:
Modern debug overlay for showing FPS and diagnostic info.

Notes:
Integrated with unified rendering pipeline.
Uses IRenderContext for consistent text rendering.

*/
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Modern debug overlay for showing FPS and diagnostic info.
    /// </summary>
    public sealed class DebugOverlay
    {
        private readonly FrameStats _stats;
        private readonly IRenderContext _renderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugOverlay"/> class.
        /// </summary>
        /// <param name="stats">The frame statistics to display.</param>
        /// <param name="renderContext">The rendering context for drawing text.</param>
        public DebugOverlay(FrameStats stats, IRenderContext renderContext)
        {
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        }

        /// <summary>
        /// Renders the debug overlay with FPS, frame count, and memory usage.
        /// </summary>
        public void Render()
        {
            // Display FPS
            DrawText($"FPS: {_stats.FramesPerSecond:F1}", 10, 10);

            // Display frame count
            DrawText($"Frames: {_stats.FrameCount}", 10, 30);

            // Display memory usage
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
            _renderContext.DrawText(text, new Vector3(x, y, 0), Color.White, 12.0f);
        }
    }
}


