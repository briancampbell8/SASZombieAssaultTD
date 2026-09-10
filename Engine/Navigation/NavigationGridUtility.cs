// ====================================================================================================
//  FILE: NavigationGridUtility.cs
//  PATH: Engine/Navigation/
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Provides deterministic, stateless helper utilities for NavigationGridCore.
//      Converts world-space coordinates to grid-space coordinates, retrieves neighbors,
//      and exposes safe helper operations that do not mutate grid state.
//
//  RESPONSIBILITIES:
//      - Convert world positions to grid positions and vice versa.
//      - Provide deterministic neighbor retrieval for pathfinding.
//      - Provide safe helper methods that operate on NavigationGridCore without storing grid data.
//      - Emit DLogger.Log trace statements for all critical utility operations.
//
//  NON-RESPONSIBILITIES:
//      - Storing walkability or occupancy (handled by NavigationGridCore).
//      - Performing A* pathfinding (handled by AStarPathfinder).
//      - Managing grid dimensions or cell storage (handled by NavigationGridCore).
//      - Maintaining any mutable state.
//
//  ARCHITECTURAL NOTES:
//      - Must remain stateless and deterministic.
//      - Must not allocate persistent data structures.
//      - Must not store references to grid or cell arrays.
//      - All operations must be pure helpers that rely on NavigationGridCore inputs.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    internal static class NavigationGridUtility
    {
        /// <summary>
        /// Converts a world position to a grid position using the provided cell size. Stateless, deterministic, and
        /// does not mutate grid state.
        /// </summary>
        public static Vector3Int WorldToGrid(Vector3 worldPosition, float cellSize)
        {
            int gx = (int)(worldPosition.X / cellSize);
            int gy = (int)(worldPosition.Y / cellSize);

            DLogger.Log(
                $"Navigation.NavigationGridUtility.WorldToGrid: World=({worldPosition.X},{worldPosition.Y},{worldPosition.Z}) " +
                $"CellSize={cellSize} → Grid=({gx},{gy},0)");

            return new Vector3Int(gx, gy, 0);
        }

        /// <summary>
        /// Converts a grid position to a world position using the provided cell size. Stateless, deterministic, and
        /// does not mutate grid state.
        /// </summary>
        public static Vector3 GridToWorld(Vector3Int gridPosition, float cellSize)
        {
            float wx = gridPosition.X * cellSize;
            float wy = gridPosition.Y * cellSize;

            DLogger.Log(
                $"Navigation.NavigationGridUtility.GridToWorld: Grid=({gridPosition.X},{gridPosition.Y},{gridPosition.Z}) " +
                $"CellSize={cellSize} → World=({wx},{wy},0)");

            return new Vector3(wx, wy, 0);
        }

        /// <summary>
        /// Retrieves all valid neighboring cells around a grid position. Deterministic, stateless, and does not mutate
        /// grid state.
        /// </summary>
        public static List<NavigationCellGridCore> GetNeighbors(
            NavigationGridCore grid,
            Vector3Int gridPosition,
            bool allowDiagonal)
        {
            var neighbors = new List<NavigationCellGridCore>();

            int[,] directions = allowDiagonal
                ? new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } }
                : new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 } };

            DLogger.Log(
                $"Navigation.NavigationGridUtility.GetNeighbors: Position=({gridPosition.X},{gridPosition.Y},{gridPosition.Z}) " +
                $"AllowDiagonal={allowDiagonal}");

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int nx = gridPosition.X + directions[i, 0];
                int ny = gridPosition.Y + directions[i, 1];

                if (grid.IsInBounds(nx, ny))
                {
                    var cell = grid.GetCell(nx, ny);

                    if (cell != null)
                    {
                        neighbors.Add(cell);

                        DLogger.Log(
                            $"Navigation.NavigationGridUtility.GetNeighbors: AddedNeighbor=({nx},{ny},0) " +
                            $"Walkable={cell.IsWalkable} MovementCost={cell.MovementCost}");
                    }
                }
            }

            return neighbors;
        }
    }
}
