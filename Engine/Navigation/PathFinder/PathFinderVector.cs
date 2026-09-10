// =====================================================================================================
//  FILE: PathFinderVector.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderVector.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic vector utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for working with coordinate vectors and movement deltas.
//
//  RESPONSIBILITIES:
//      - Represent and manipulate deterministic 2D vectors.
//      - Provide baseline math helpers for vector normalization and magnitude checks.
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
//      - Higher-level navigation modules may rely on these helpers for baseline vector behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderVector
    {
        /// <summary>
        /// Creates a deterministic 2D vector (x, y).
        /// </summary>
        internal (int x, int y) Create(int x, int y)
        {
            return (x, y);
        }

        /// <summary>
        /// Returns true if the vector represents no movement.
        /// </summary>
        internal bool IsZero(int x, int y)
        {
            return x == 0 && y == 0;
        }

        /// <summary>
        /// Computes the Manhattan magnitude of the vector.
        /// Deterministic and side-effect free.
        /// </summary>
        internal int MagnitudeManhattan(int x, int y)
        {
            int ax = x < 0 ? -x : x;
            int ay = y < 0 ? -y : y;
            return ax + ay;
        }
    }
}
