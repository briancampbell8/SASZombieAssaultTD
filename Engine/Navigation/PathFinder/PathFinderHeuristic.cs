// =====================================================================================================
//  FILE: PathFinderHeuristic.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderHeuristic.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic heuristic utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for computing simple, deterministic distance estimates.
//
//  RESPONSIBILITIES:
//      - Provide deterministic heuristic calculations.
//      - Support higher-level navigation modules with baseline cost estimates.
//      - Serve as a minimal utility module without algorithmic decision-making.
//
//  NON-RESPONSIBILITIES:
//      - Executing pathfinding algorithms or heuristics selection logic.
//      - Managing grid structures, nodes, or cell data.
//      - Handling rendering, update loops, or engine lifecycle operations.
//      - Performing optimization, caching, or smoothing logic.
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally minimal and dependency-free.
//      - Heuristic functions must remain deterministic and side‑effect free.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderHeuristic
    {
        /// <summary>
        /// Computes the Manhattan distance between two coordinates.
        /// Deterministic and side‑effect free.
        /// </summary>
        internal int Manhattan(int x1, int y1, int x2, int y2)
        {
            int dx = x1 > x2 ? x1 - x2 : x2 - x1;
            int dy = y1 > y2 ? y1 - y2 : y2 - y1;
            return dx + dy;
        }

        /// <summary>
        /// Computes the Chebyshev distance between two coordinates.
        /// Deterministic and side‑effect free.
        /// </summary>
        internal int Chebyshev(int x1, int y1, int x2, int y2)
        {
            int dx = x1 > x2 ? x1 - x2 : x2 - x1;
            int dy = y1 > y2 ? y1 - y2 : y2 - y1;
            return dx > dy ? dx : dy;
        }

        /// <summary>
        /// Computes the Euclidean distance squared.
        /// Deterministic and avoids floating‑point operations.
        /// </summary>
        internal int EuclideanSquared(int x1, int y1, int x2, int y2)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            return (dx * dx) + (dy * dy);
        }
    }
}
