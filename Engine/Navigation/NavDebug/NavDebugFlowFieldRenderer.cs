// =====================================================================================================
//  FILE: NavDebugFlowFieldRenderer.cs
//  PATH: Engine/Navigation/NavDebug/NavDebugFlowFieldRenderer.cs
//  SUBSYSTEM: Navigation.NavDebug
//
//  ROLE:
//      Dedicated renderer for flow field visualization.
//      Samples flow field directions and draws debug indicators.
//
//  RESPONSIBILITIES:
//      - Reflect NavigationGrid from the navigation system.
//      - Sample flow field direction vectors.
//      - Render flow field indicators (arrows, circles, etc.).
//      - Enforce deterministic Option‑B subsystem boundaries.
//
//  NON-RESPONSIBILITIES:
//      - Grid rendering (NavDebugGridRenderer).
//      - Path rendering (NavDebugPathRenderer).
//      - Target rendering (NavDebugTargetRenderer).
//      - Debug statistics (NavDebugStatsRenderer).
//      - Async orchestration (NavDebugRenderer).
//
//  NOTES:
//      - This module replaces the flow field portion of the old NavigationDebugRenderer.cs.
//      - Flow field rendering is intentionally lightweight and optional.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation.NavDebug
{
    /// <summary>
    /// Renders flow field debug visualization.
    /// </summary>
    public sealed class NavDebugFlowFieldRenderer
    {
        /// <summary>
        /// Renders flow field direction indicators.
        /// </summary>
        public void Render(D3D11Adapter_Core context, object? navigationSystem, object navigationSystem1)
        {
            if (context == null || navigationSystem == null)
                return;

            // ------------------------------------------------------------------------------------------------
            // Reflect NavigationGrid from the navigation system
            // ------------------------------------------------------------------------------------------------
            var gridProperty = navigationSystem.GetType().GetProperty("NavigationGrid");
            var grid = gridProperty?.GetValue(navigationSystem) as NavigationGrid;

            if (grid == null)
                return;

            float cellSize = grid.CellSize;

            // ------------------------------------------------------------------------------------------------
            // Sample flow field every other cell (performance optimization)
            // ------------------------------------------------------------------------------------------------
            for (int y = 0; y < grid.Height; y += 2)
            {
                for (int x = 0; x < grid.Width; x += 2)
                {
                    var gridPos = new Vector3Int(x, y);
                    var worldPos = grid.GridToWorld(gridPos);

                    // ------------------------------------------------------------------------------------------------
                    // Placeholder: draw a small circle to indicate flow direction sampling
                    // ------------------------------------------------------------------------------------------------
                    context.DrawCircle(
                        worldPos,
                        2.0f,
                        Color.FromUint(NavDebugCore.Instance.FlowFieldColor)
                    );
                }
            }
        }
    }
}
