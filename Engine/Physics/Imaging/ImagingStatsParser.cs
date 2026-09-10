// ====================================================================================================
//  FILE: ImagingStatsParser.cs
//  PATH: Engine/Physics/Imaging/
//  MODULE: Imaging
//
//  ROLE:
//      Provide lightweight, deterministic formatting helpers for Imaging subsystem statistics.
//      ImagingStatsParser is strictly read‑only and never queries ECS or physics state.
//
//  RESPONSIBILITIES:
//      - Convert SpatialGridStats objects into formatted text.
//      - Provide simple headers and stat blocks for ImagingText / ImagingRenderer.
//      - Remain deterministic and side‑effect free.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Physics.Imaging
{
    /// <summary>
    /// Provides formatted imaging statistics for display.
    /// </summary>
    public sealed class ImagingStatsParser
    {
        private ECSRuntimeCore ecsWorld;

        public ImagingStatsParser() { }

        public ImagingStatsParser(ECSRuntimeCore ecsWorld)
        {
            this.ecsWorld = ecsWorld;
        }

        /// <summary>
        /// Converts SpatialGridStats into a formatted multi‑line string.
        /// </summary>
        public string FormatGridStats(SpatialGridStats stats)
        {
            if (stats == null)
                return "Grid Stats: (null)";

            return
                $"Grid Stats:\n" +
                $"  Cell Size:   {stats.CellSize}\n" +
                $"  Grid Width:  {stats.GridWidth}\n" +
                $"  Grid Height: {stats.GridHeight}\n" +
                $"  Total Cells: {stats.GridWidth * stats.GridHeight}\n";
        }

        /// <summary>
        /// Formats a simple header for imaging stats.
        /// </summary>
        public string FormatHeader() =>
            "=== Imaging Statistics ===";

        /// <summary>
        /// GPU‑safe stats rendering entry point.
        /// </summary>
        public void Render(D3D11Adapter_Core context)
        {
            if (context == null)
                return;

            context.DrawText(
                FormatHeader(),
                10,
                10,
                14f,
                ColorRGBA.White);
        }

        /// <summary>
        /// Internal deterministic render path.
        /// </summary>
        internal void GetRender(D3D11Adapter_Core context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Render(context);
        }

        /// <summary>
        /// Public entry point for callers expecting a lightweight wrapper.
        /// </summary>
        internal void UseRender(D3D11Adapter_Core context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            GetRender(context);
        }
    }
}
