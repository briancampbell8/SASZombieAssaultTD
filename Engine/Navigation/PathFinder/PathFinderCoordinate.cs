// =====================================================================================================
//  FILE: PathFinderCoordinate.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderCoordinate.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic coordinate utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for converting, normalizing, and comparing grid coordinates.
//
//  RESPONSIBILITIES:
//      - Represent and manipulate deterministic 2D coordinates.
//      - Provide baseline helpers for coordinate comparison and normalization.
//      - Serve as a minimal utility module for higher-level PathFinder components.
//
//  NON-RESPONSIBILITIES:
//      - Executing pathfinding algorithms or heuristics.
//      - Managing grid structures, nodes, or cell data.
//      - Handling rendering, update loops, or engine lifecycle operations.
//      - Performing optimization, caching, or smoothing logic.
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally minimal and dependency-free.
//      - Higher-level navigation modules may rely on these helpers for baseline coordinate behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderCoordinate
    {
        /// <summary>
        /// Creates a deterministic coordinate pair (x, y).
        /// </summary>
        internal (int x, int y) Create(int x, int y)
        {
            return (x, y);
        }

        /// <summary>
        /// Returns true if both coordinates match exactly.
        /// Deterministic and side-effect free.
        /// </summary>
        internal bool Equals(int x1, int y1, int x2, int y2)
        {
            return x1 == x2 && y1 == y2;
        }

        /// <summary>
        /// Normalizes a coordinate by clamping negative values to zero.
        /// Does not enforce upper bounds; use PathFinderBounds for full clamping.
        /// </summary>
        internal (int nx, int ny) NormalizeNonNegative(int x, int y)
        {
            int nx = x < 0 ? 0 : x;
            int ny = y < 0 ? 0 : y;
            return (nx, ny);
        }

        /// <summary>
        /// Converts a coordinate pair into a deterministic linear index.
        /// </summary>
        internal int ToIndex(int x, int y, int width)
        {
            return (y * width) + x;
        }

        /// <summary>
        /// Converts a deterministic linear index back into (x, y) coordinates.
        /// </summary>
        internal (int x, int y) FromIndex(int index, int width)
        {
            int y = index / width;
            int x = index % width;
            return (x, y);
        }
    }
}
