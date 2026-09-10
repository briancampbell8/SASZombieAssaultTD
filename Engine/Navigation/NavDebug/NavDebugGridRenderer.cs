// =====================================================================================================
//  FILE: NavDebugGridRenderer.cs
//  PATH: Engine/Navigation/NavDebug/NavDebugGridRenderer.cs
//  SUBSYSTEM: Navigation.NavDebug
//
//  ROLE:
//      Dedicated renderer for navigation grid visualization.
//      Converts NavigationGrid cell data into renderable rectangles.
//
//  RESPONSIBILITIES:
//      - Render grid cells (walkable / blocked).
//      - Render grid border.
//      - Convert grid coordinates to world coordinates.
//      - Reflect NavigationGrid from the navigation system safely.
//
//  NON-RESPONSIBILITIES:
//      - ECS queries (handled by NavDebugPathRenderer / NavDebugTargetRenderer).
//      - Flow field visualization.
//      - Debug statistics.
//      - Orchestration (NavDebugRenderer handles async scheduling).
//
//  NOTES:
//      - This module replaces the grid portion of the old NavigationDebugRenderer.cs.
//      - All rendering logic is isolated here to preserve subsystem boundaries.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation.NavDebug
{
    /// <summary>
    /// Renders the navigation grid for debug visualization.
    /// </summary>
    public sealed class NavDebugGridRenderer
    {
        /// <summary>
        /// Renders the navigation grid.
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
            {
                // Fallback: initialize a minimal grid to avoid null rendering
                NavigationGrid.Instance.InitializeInstance(10, 10);
                grid = NavigationGrid.Instance;
            }

            float cellSize = grid.CellSize;

            // ------------------------------------------------------------------------------------------------
            // Render each grid cell
            // ------------------------------------------------------------------------------------------------
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var gridPos = new Vector3Int(x, y);
                    var worldPos = grid.GridToWorld(gridPos);
                    bool isWalkable = grid.IsWalkable(gridPos);

                    var color = isWalkable
                        ? Color.FromUint(NavDebugCore.Instance.GridColor)
                        : Color.FromUint(NavDebugCore.Instance.BlockedColor);

                    context.DrawRectangle(
                        (int)worldPos.X,
                        (int)worldPos.Y,
                        (int)cellSize,
                        (int)cellSize,
                        color
                    );
                }
            }

            // ------------------------------------------------------------------------------------------------
            // Render grid border
            // ------------------------------------------------------------------------------------------------
            var origin = grid.WorldOrigin;
            float width = grid.Width * cellSize;
            float height = grid.Height * cellSize;

            context.DrawRectangle(
                (int)origin.X,
                (int)origin.Y,
                (int)width,
                (int)height,
                Color.FromUint(NavDebugCore.Instance.GridBorderColor)
            );
        }
    }
}
