// =====================================================================================================
//  FILE: NavigationExtensions.cs
//  PATH: Engine/Navigation/NavigationExtensions.cs
//  MODULE: Navigation
//
//  ROLE:
//      Provides deterministic, type‑safe extension methods for NavigationGrid and navigation‑related
//      vector types, enabling grid/world conversion helpers, occupancy checks, bounds validation,
//      neighbor enumeration, and distance utilities.
//
//  RESPONSIBILITIES:
//      - Provide ToWorld() and ToGrid() behavior for the Navigation subsystem.
//      - Provide IsValidCell() and IsCellOccupied() behavior for the Navigation subsystem.
//      - Provide GetOrthogonalNeighbors() and GetAllNeighbors() behavior for the Navigation subsystem.
//      - Provide ManhattanDistance() and EuclideanDistance() behavior for the Navigation subsystem.
//      - Improve readability and maintainability of navigation logic without modifying core systems.
//      - Remain pure: no side effects outside NavigationGrid’s own state fields.
//
//  NON-RESPONSIBILITIES:
//      - Performing pathfinding or GPU operations.
//      - Managing ECS entities or world‑grid occupancy mutation.
//      - Allocating navigation nodes or mutating NavigationGrid behavior.
//      - Replacing or overriding NavigationGrid’s deterministic behavior.
//
//  NOTES:
//      Relocated from Engine/Extensions to Engine/Navigation.
//      Modernized IsCellOccupied() to use correct IsWalkable(x, y) signature.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    /// <summary>
    /// Deterministic helper extensions for NavigationGrid and navigation vectors.
    /// </summary>
    public static class NavigationExtensions
    {
        // ----------------------------------------------------------------------------------------------
        //  GRID / WORLD CONVERSION HELPERS
        // ----------------------------------------------------------------------------------------------

        public static Vector3 ToWorld(this NavigationGrid grid, Vector3Int gridPos)
        {
            if (grid == null)
                return Vector3.Zero;

            return grid.GridToWorld(gridPos);
        }

        public static Vector3Int ToGrid(this NavigationGrid grid, Vector3 worldPos)
        {
            if (grid == null)
                return Vector3Int.Zero;

            return grid.WorldToGrid(worldPos);
        }

        // ----------------------------------------------------------------------------------------------
        //  BOUNDS / OCCUPANCY HELPERS
        // ----------------------------------------------------------------------------------------------

        public static bool IsValidCell(this NavigationGrid grid, Vector3Int gridPos)
        {
            if (grid == null)
                return false;

            return grid.IsInBounds(gridPos.X, gridPos.Y);
        }

        public static bool IsCellOccupied(this NavigationGrid grid, Vector3Int gridPos)
        {
            if (grid == null)
                return false;

            if (!grid.IsInBounds(gridPos.X, gridPos.Y))
                return false;

            return !grid.IsWalkable(gridPos.X, gridPos.Y);
        }

        // ----------------------------------------------------------------------------------------------
        //  NEIGHBOR ENUMERATION
        // ----------------------------------------------------------------------------------------------

        public static IEnumerable<Vector3Int> GetOrthogonalNeighbors(this NavigationGrid grid, Vector3Int cell)
        {
            if (grid == null)
                yield break;

            var candidates = new[]
            {
                new Vector3Int(cell.X + 1, cell.Y, cell.Z),
                new Vector3Int(cell.X - 1, cell.Y, cell.Z),
                new Vector3Int(cell.X, cell.Y + 1, cell.Z),
                new Vector3Int(cell.X, cell.Y - 1, cell.Z)
            };

            foreach (var c in candidates)
            {
                if (grid.IsInBounds(c.X, c.Y))
                    yield return c;
            }
        }

        public static IEnumerable<Vector3Int> GetAllNeighbors(this NavigationGrid grid, Vector3Int cell)
        {
            if (grid == null)
                yield break;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    var n = new Vector3Int(cell.X + dx, cell.Y + dy, cell.Z);
                    if (grid.IsInBounds(n.X, n.Y))
                        yield return n;
                }
            }
        }

        // ----------------------------------------------------------------------------------------------
        //  DISTANCE HELPERS
        // ----------------------------------------------------------------------------------------------

        public static int ManhattanDistance(this Vector3Int a, Vector3Int b)
        {
            return System.Math.Abs(a.X - b.X) +
                   System.Math.Abs(a.Y - b.Y);
        }

        public static float EuclideanDistance(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }
    }
}
