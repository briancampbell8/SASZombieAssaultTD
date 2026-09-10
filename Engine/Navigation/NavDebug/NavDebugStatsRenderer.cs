// =====================================================================================================
//  FILE: NavDebugStatsRenderer.cs
//  PATH: Engine/Navigation/NavDebug/NavDebugStatsRenderer.cs
//  SUBSYSTEM: Navigation.NavDebug
//
//  ROLE:
//      Dedicated renderer for navigation debug statistics.
//      Displays grid size, agent counts, path metrics, and active debug flags.
//
//  RESPONSIBILITIES:
//      - Collect debug statistics from NavDebugCore and navigation system.
//      - Render formatted text lines to the D3D11Adapter_Core.
//      - Enforce deterministic Option‑B subsystem boundaries.
//
//  NON-RESPONSIBILITIES:
//      - Grid rendering (NavDebugGridRenderer).
//      - Path rendering (NavDebugPathRenderer).
//      - Target rendering (NavDebugTargetRenderer).
//      - Flow field rendering (NavDebugFlowFieldRenderer).
//      - Async orchestration (NavDebugRenderer).
//
//  NOTES:
//      - This module replaces the statistics portion of the old NavigationDebugRenderer.cs.
//      - All rendering logic is isolated here to preserve subsystem boundaries.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation.NavDebug
{
    /// <summary>
    /// Renders navigation debug statistics.
    /// </summary>
    public sealed class NavDebugStatsRenderer
    {
        /// <summary>
        /// Renders debug statistics text.
        /// </summary>
        public void Render(D3D11Adapter_Core context, object? navigationSystem, object navigationSystem1)
        {
            if (context == null)
                return;

            var core = NavDebugCore.Instance;

            // ------------------------------------------------------------------------------------------------
            // Collect statistics
            // ------------------------------------------------------------------------------------------------
            string gridSize = GetGridSize(navigationSystem);
            int agentsProcessed = GetAgentsProcessed(navigationSystem);
            int pathsRequested = GetPathsRequested(navigationSystem);
            int pathsCompleted = GetPathsCompleted(navigationSystem);

            // ------------------------------------------------------------------------------------------------
            // Build stats list
            // ------------------------------------------------------------------------------------------------
            var stats = new List<string>
            {
                "Navigation Debug Stats:",
                $"Grid: {gridSize}",
                $"Agents Processed: {agentsProcessed}",
                $"Paths Requested: {pathsRequested}",
                $"Paths Completed: {pathsCompleted}",
                $"Show Grid: {core.ShowGrid}",
                $"Show Paths: {core.ShowPaths}",
                $"Show Targets: {core.ShowTargets}",
                $"Show Flow Fields: {core.ShowFlowFields}",
                $"Show Stats: {core.ShowStats}"
            };

            // ------------------------------------------------------------------------------------------------
            // Render text lines
            // ------------------------------------------------------------------------------------------------
            int y = 10;

            foreach (string line in stats)
            {
                context.DrawText(
                    line,
                    new Vector3(10, y, 0),
                    Color.White
                );

                y += 15;
            }
        }

        // =================================================================================================
        //  Helper Methods
        // =================================================================================================

        private string GetGridSize(object? navigationSystem)
        {
            if (navigationSystem == null)
                return "0x0";

            try
            {
                dynamic nav = navigationSystem;
                var grid = nav.NavigationGrid;

                if (grid == null)
                    return "0x0";

                return $"{grid.Width} x {grid.Height}";
            }
            catch
            {
                return "0x0";
            }
        }

        private int GetAgentsProcessed(object? navigationSystem)
        {
            if (navigationSystem == null)
                return 0;

            try
            {
                dynamic nav = navigationSystem;
                return nav.AgentsProcessed ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private int GetPathsRequested(object? navigationSystem)
        {
            if (navigationSystem == null)
                return 0;

            try
            {
                dynamic nav = navigationSystem;
                return nav.PathsRequested ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private int GetPathsCompleted(object? navigationSystem)
        {
            if (navigationSystem == null)
                return 0;

            try
            {
                dynamic nav = navigationSystem;
                return nav.PathsCompleted ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        internal void Render(
    D3D11Adapter_Core context,
    IECSRuntimeCore ecsWorld,
    object navigationSystem)
        {
            // Validate required inputs
            if (context == null || ecsWorld == null)
                return;

            try
            {
                // Gather stats using existing helper methods on this class
                string gridSize = GetGridSize(navigationSystem);
                int agentsProcessed = GetAgentsProcessed(navigationSystem);
                int pathsRequested = GetPathsRequested(navigationSystem);
                int pathsCompleted = GetPathsCompleted(navigationSystem);

                // Compose a readable multi-line summary. Use Environment.NewLine for line breaks as requested.
                var sb = new System.Text.StringBuilder();
                sb.Append("Nav Debug Stats").Append(Environment.NewLine);
                sb.Append("--------------").Append(Environment.NewLine);
                sb.Append("Grid Size: ").Append(gridSize).Append(Environment.NewLine);
                sb.Append("Agents Processed: ").Append(agentsProcessed).Append(Environment.NewLine);
                sb.Append("Paths Requested: ").Append(pathsRequested).Append(Environment.NewLine);
                sb.Append("Paths Completed: ").Append(pathsCompleted).Append(Environment.NewLine);

                // This method does not assume any specific on-screen font/rendering API is available here.
                // Fallback to writing the stats to console so the data is available without introducing
                DLogger.Log($"NavDebugStatsRenderer.Render: {sb.ToString()}");
            }
            catch (Exception ex)
            {
                // Defensive logging; do not throw from a rendering helper.
                DLogger.Log($"NavDebugStatsRenderer.Render: {ex.Message}"); DLogger.Log(ex);
            }
        }
    }
}
