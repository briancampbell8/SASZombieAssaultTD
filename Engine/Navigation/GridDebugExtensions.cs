// =====================================================================================================
//  FILE: GridDebugExtensions.cs
//  PATH: Engine/Navigation/GridDebugExtensions.cs
//  MODULE: Navigation / Debugging
//
//  ROLE:
//      Provide deterministic, type‑safe debugging helpers for NavigationGrid, enabling ASCII grid
//      visualization, occupancy inspection, bounds validation, and coordinate conversion diagnostics.
//
//  RESPONSIBILITIES:
//      - Provide ToAsciiMap() behavior for the Navigation subsystem.
//      - Provide LogAsciiMap() behavior for the Navigation subsystem.
//      - Provide GetOccupiedCells() behavior for the Navigation subsystem.
//      - Provide GetFreeCells() behavior for the Navigation subsystem.
//      - Provide LogCoordinateConversion() behavior for the Navigation subsystem.
//      - Provide LogCellInfo() behavior for the Navigation subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations directly.
//      - Managing ECS entities or world‑grid occupancy mutation.
//      - Allocating grid cells or mutating NavigationGrid behavior.
//      - Replacing or overriding NavigationGrid’s deterministic behavior.
//
//  NOTES:
//      Relocated from Engine/Extensions to Engine/Navigation.
//      Fixed CS1955: replaced invalid IsOccupied(x, y) calls with deterministic IsWalkable() logic.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Text;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Deterministic debugging helpers for NavigationGrid.
    /// </summary>
    public static class GridDebugExtensions
    {
        // ----------------------------------------------------------------------------------------------
        //  ASCII GRID VISUALIZATION
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Generates an ASCII visualization of the grid occupancy.
        /// </summary>
        public static string ToAsciiMap(this NavigationGrid grid)
        {
            if (grid == null)
                return "[GridDebug] No grid available.";

            var sb = new StringBuilder();

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    bool occupied = !grid.IsWalkable(x, y);
                    sb.Append(occupied ? "#" : ".");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Logs the ASCII grid map to the diagnostics subsystem.
        /// </summary>
        public static void LogAsciiMap(this NavigationGrid grid)
        {
            string map = grid.ToAsciiMap();
            DLogger.Log(LogSubsystems.Navigation, LogEnums.LogLevel.Info, map);
        }

        // ----------------------------------------------------------------------------------------------
        //  OCCUPANCY DEBUGGING
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns all occupied grid cells.
        /// </summary>
        public static IEnumerable<Vector3Int> GetOccupiedCells(this NavigationGrid grid)
        {
            if (grid == null)
                yield break;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (!grid.IsWalkable(x, y))
                        yield return new Vector3Int(x, y, 0);
                }
            }
        }

        /// <summary>
        /// Returns all free grid cells.
        /// </summary>
        public static IEnumerable<Vector3Int> GetFreeCells(this NavigationGrid grid)
        {
            if (grid == null)
                yield break;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.IsWalkable(x, y))
                        yield return new Vector3Int(x, y, 0);
                }
            }
        }

        // ----------------------------------------------------------------------------------------------
        //  COORDINATE DEBUGGING
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Logs the world and grid coordinates for a given world position.
        /// </summary>
        public static void LogCoordinateConversion(this NavigationGrid grid, Vector3 worldPos)
        {
            if (grid == null)
            {
                DLogger.Log(LogSubsystems.Navigation, LogEnums.LogLevel.Warning, "[GridDebug] No grid available.");
                return;
            }

            Vector3Int gridPos = grid.WorldToGrid(worldPos);

            DLogger.Log(
                LogSubsystems.Navigation,
                LogEnums.LogLevel.Info,
                $"[GridDebug] World {worldPos.X},{worldPos.Y} → Grid {gridPos.X},{gridPos.Y}"
            );
        }

        /// <summary>
        /// Logs whether a grid cell is valid and occupied.
        /// </summary>
        public static void LogCellInfo(this NavigationGrid grid, Vector3Int cell)
        {
            if (grid == null)
            {
                DLogger.Log(LogSubsystems.Navigation, LogEnums.LogLevel.Warning, "[GridDebug] No grid available.");
                return;
            }

            bool inBounds = grid.IsInBounds(cell.X, cell.Y);
            bool occupied = inBounds && !grid.IsWalkable(cell.X, cell.Y);

            DLogger.Log(
                LogSubsystems.Navigation,
                LogEnums.LogLevel.Info,
                $"[GridDebug] Cell {cell.X},{cell.Y} → InBounds={inBounds}, Occupied={occupied}"
            );
        }
    }
}
