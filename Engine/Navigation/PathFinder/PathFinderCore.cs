// =====================================================================================================
//  FILE: PathFinderCore.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderCore.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides foundational deterministic utility functions for the Navigation subsystem.
//      Supplies lightweight, dependency-free helpers used by higher-level pathfinding modules.
//
//  RESPONSIBILITIES:
//      - Perform basic coordinate and grid-boundary validation.
//      - Provide deterministic distance calculations for navigation operations.
//      - Serve as a stable core utility module for other PathFinder components.
//
//  NON-RESPONSIBILITIES:
//      - Executing full pathfinding algorithms.
//      - Managing navigation grids, nodes, or cell structures.
//      - Handling caching, smoothing, or optimization logic.
//      - Interacting with engine lifecycle or rendering systems.
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally minimal and deterministic.
//      - Higher-level navigation modules (e.g., PathFinderHeap, PathFinderOptimizations)
//        may rely on these helpers for baseline behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderCore
    {
        /// <summary>
        /// Determines whether the provided coordinates fall within the bounds
        /// of a navigation grid defined by width and height.
        /// </summary>
        internal bool IsWithinBounds(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        /// <summary>
        /// Computes the Manhattan distance between two grid coordinates.
        /// Deterministic and side-effect free.
        /// </summary>
        internal int ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2;
            int dy = y1 - y2;
            return (dx < 0 ? -dx : dx) + (dy < 0 ? -dy : dy);
        }
    }
}
