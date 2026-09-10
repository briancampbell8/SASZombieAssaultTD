// =====================================================================================================
//  FILE: PathFinderIndex.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderIndex.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic index utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for converting between linear indices and 2D coordinates.
//
//  RESPONSIBILITIES:
//      - Convert coordinates to deterministic linear indices.
//      - Convert linear indices back into deterministic coordinates.
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
//      - Higher-level navigation modules may rely on these helpers for baseline index behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderIndex
    {
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

        /// <summary>
        /// Returns true if the index is within the valid range for the grid.
        /// Deterministic and side‑effect free.
        /// </summary>
        internal bool IsValid(int index, int width, int height)
        {
            return index >= 0 && index < (width * height);
        }

        /// <summary>
        /// Clamps an index to the nearest valid value within the grid.
        /// </summary>
        internal int Clamp(int index, int width, int height)
        {
            int max = (width * height) - 1;
            if (index < 0) return 0;
            if (index > max) return max;
            return index;
        }
    }
}
