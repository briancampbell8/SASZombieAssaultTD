// =====================================================================================================
//  FILE: PathFinderStep.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderStep.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic step utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for representing movement steps and transitions.
//
//  RESPONSIBILITIES:
//      - Represent deterministic movement steps.
//      - Provide helpers for step comparison and delta extraction.
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
//      - Higher-level navigation modules may rely on these helpers for baseline step behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderStep
    {
        /// <summary>
        /// Creates a deterministic movement step from (x1, y1) to (x2, y2).
        /// </summary>
        internal (int x1, int y1, int x2, int y2) Create(int x1, int y1, int x2, int y2)
        {
            return (x1, y1, x2, y2);
        }

        /// <summary>
        /// Returns the deterministic delta between two coordinates.
        /// </summary>
        internal (int dx, int dy) Delta(int x1, int y1, int x2, int y2)
        {
            return (x2 - x1, y2 - y1);
        }

        /// <summary>
        /// Returns true if the step represents no movement.
        /// </summary>
        internal bool IsStationary(int x1, int y1, int x2, int y2)
        {
            return x1 == x2 && y1 == y2;
        }

        /// <summary>
        /// Returns true if two steps are identical.
        /// Deterministic and side‑effect free.
        /// </summary>
        internal bool Equals(
            int x1a, int y1a, int x2a, int y2a,
            int x1b, int y1b, int x2b, int y2b)
        {
            return x1a == x1b &&
                   y1a == y1b &&
                   x2a == x2b &&
                   y2a == y2b;
        }
    }
}
