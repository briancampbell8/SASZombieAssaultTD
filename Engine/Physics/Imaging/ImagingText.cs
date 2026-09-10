// ====================================================================================================
//  FILE: ImagingText.cs
//  PATH: Engine/Physics/Imaging/
//  MODULE: Imaging
//
//  ROLE:
//      Render text overlays for the Physics Imaging subsystem.
//
//  RESPONSIBILITIES:
//      - Render imaging subsystem status text.
//      - Provide lightweight text overlay support.
//      - Remain strictly read‑only toward ECS and physics state.
//      - Provide text‑level imaging only (no shapes, no grid).
//
//  NON‑RESPONSIBILITIES:
//      - Collision detection.
//      - Physics simulation.
//      - Diagnostics.
//      - Grid rendering (ImagingGrid).
//      - Shape rendering (ImagingShapes).
//      - Stats parsing (ImagingStatsParser).
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Physics.Imaging
{
    /// <summary>
    /// Deterministic text overlay renderer for physics imaging.
    /// </summary>
    public sealed class ImagingText
    {
        private readonly ECSRuntimeCore _ecsWorld;

        public ImagingText(ECSRuntimeCore ecsWorld)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
        }

        /// <summary>
        /// Renders imaging subsystem status text.
        /// </summary>
        public void RenderStats(
            D3D11Adapter_Core context,
            bool showGrid,
            bool showShapes,
            bool showBounds,
            bool showStats)
        {
            if (context == null || !showStats)
                return;

            float x = 10f;
            float y = 10f;
            float lineHeight = 18f;

            // Header
            context.DrawText("Imaging Subsystem", x, y, Color.White);
            y += lineHeight;

            // Flags
            context.DrawText($"Grid:   {(showGrid ? "ON" : "OFF")}", x, y, Color.White);
            y += lineHeight;

            context.DrawText($"Shapes: {(showShapes ? "ON" : "OFF")}", x, y, Color.White);
            y += lineHeight;

            context.DrawText($"Bounds: {(showBounds ? "ON" : "OFF")}", x, y, Color.White);
            y += lineHeight;

            context.DrawText($"Stats:  {(showStats ? "ON" : "OFF")}", x, y, Color.White);
            y += lineHeight;

            // Optional ECS counts (deterministic, no reflection)
            if (_ecsWorld != null)
            {
                var entities = _ecsWorld.Entities;
                var comps = _ecsWorld.Components;
                var systems = _ecsWorld.Systems;

                context.DrawText($"Entities: {entities.Count}", x, y, Color.White);
                y += lineHeight;

                context.DrawText($"Components: {comps.Count}", x, y, Color.White);
                y += lineHeight;

                context.DrawText($"Systems: {systems.Count}", x, y, Color.White);
            }
        }

        internal void GetRenderStats(
    D3D11Adapter_Core context,
    bool showGrid,
    bool showShapes,
    bool showBounds,
    bool showStats)
        {
            // Guard against null context
            if (context == null)
                return;

            try
            {
                // Forward to the concrete renderer that displays the textual stats/flags.
                // UseRenderStats is expected to perform the actual drawing using the
                // provided context and flag values.
                UseRenderStats(context, showGrid, showShapes, showBounds, showStats);
            }
            catch (Exception ex)
            {
                // Avoid throwing from a rendering helper; log to debug output to aid troubleshooting.
                DLogger.Log($"ImagingText.GetRenderStats failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Public wrapper for RenderStats.
        /// </summary>
        internal void UseRenderStats(
            D3D11Adapter_Core context,
            bool showGrid,
            bool showShapes,
            bool showBounds,
            bool showStats)
        {
            RenderStats(context, showGrid, showShapes, showBounds, showStats);
        }
    }
}
