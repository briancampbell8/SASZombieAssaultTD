// =====================================================================================================
//  FILE: PathFinderGrid.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderGrid.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic, lightweight grid utilities for the PathFinder subsystem.
//      Supplies foundational helpers for grid validation and coordinate normalization.
//
//  RESPONSIBILITIES:
//      - Validate grid boundaries and coordinate positions.
//      - Normalize coordinates for deterministic navigation operations.
//      - Serve as a minimal utility module for higher-level PathFinder components.
//
//  NON-RESPONSIBILITIES:
//      - Executing full pathfinding algorithms.
//      - Managing grid cell data, node structures, or navigation maps.
//      - Performing optimization, caching, or heuristic calculations.
//      - Interacting with engine lifecycle or rendering systems.
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally minimal and dependency-free.
//      - Higher-level navigation modules may rely on these helpers for baseline behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderGrid
    {
        /// <summary>
        /// Validates whether the given coordinates fall within the specified grid dimensions.
        /// Deterministic and side-effect free.
        /// </summary>
        internal bool IsValidCell(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        /// <summary>
        /// Clamps a coordinate pair to the nearest valid position within the grid.
        /// Ensures deterministic behavior for out-of-range inputs.
        /// </summary>
        internal (int x, int y) ClampToGrid(int x, int y, int width, int height)
        {
            int cx = x < 0 ? 0 : (x >= width ? width - 1 : x);
            int cy = y < 0 ? 0 : (y >= height ? height - 1 : y);
            return (cx, cy);
        }
    }
}
