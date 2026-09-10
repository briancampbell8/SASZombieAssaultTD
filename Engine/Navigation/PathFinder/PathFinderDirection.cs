// =====================================================================================================
//  FILE: PathFinderDirection.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderDirection.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic direction utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for working with directional vectors and movement offsets.
//
//  RESPONSIBILITIES:
//      - Represent and validate directional movement offsets.
//      - Provide deterministic helpers for direction-based navigation operations.
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
//      - Higher-level navigation modules may rely on these helpers for baseline direction behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderDirection
    {
        /// <summary>
        /// Returns the four cardinal movement offsets as deterministic (dx, dy) pairs.
        /// </summary>
        internal (int dx, int dy)[] GetCardinalOffsets()
        {
            return new[]
            {
                ( 0, -1), // Up
                ( 1,  0), // Right
                ( 0,  1), // Down
                (-1,  0)  // Left
            };
        }

        /// <summary>
        /// Determines whether the provided offset represents a valid cardinal direction.
        /// </summary>
        internal bool IsCardinal(int dx, int dy)
        {
            return (dx == 0 && (dy == -1 || dy == 1)) ||
                   (dy == 0 && (dx == -1 || dx == 1));
        }
    }
}
