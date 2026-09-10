// =====================================================================================================
//  FILE: PathFinderOffset.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderOffset.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic offset utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for working with movement offsets and coordinate deltas.
//
//  RESPONSIBILITIES:
//      - Represent and validate movement offsets.
//      - Provide deterministic helpers for offset-based navigation operations.
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
//      - Higher-level navigation modules may rely on these helpers for baseline offset behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderOffset
    {
        /// <summary>
        /// Creates a deterministic offset pair (dx, dy).
        /// Useful for constructing movement deltas in higher-level modules.
        /// </summary>
        internal (int dx, int dy) Create(int dx, int dy)
        {
            return (dx, dy);
        }

        /// <summary>
        /// Determines whether the provided offset is a zero movement delta.
        /// </summary>
        internal bool IsZero(int dx, int dy)
        {
            return dx == 0 && dy == 0;
        }
    }
}
