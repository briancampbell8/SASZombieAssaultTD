// =====================================================================================================
//  FILE: PathFinderRegion.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderRegion.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic region utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for defining, validating, and iterating rectangular regions.
//
//  RESPONSIBILITIES:
//      - Represent deterministic rectangular regions.
//      - Provide helpers for containment checks and region iteration.
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
//      - Higher-level navigation modules may rely on these helpers for baseline region behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderRegion
    {
        /// <summary>
        /// Creates a deterministic rectangular region.
        /// </summary>
        internal (int x, int y, int w, int h) Create(int x, int y, int w, int h)
        {
            return (x, y, w, h);
        }

        /// <summary>
        /// Returns true if the coordinate lies inside the region.
        /// Deterministic and side-effect free.
        /// </summary>
        internal bool Contains(int rx, int ry, int rw, int rh, int x, int y)
        {
            return x >= rx &&
                   y >= ry &&
                   x < (rx + rw) &&
                   y < (ry + rh);
        }

        /// <summary>
        /// Enumerates all coordinates inside the region deterministically.
        /// </summary>
        internal (int x, int y)[] Enumerate(int rx, int ry, int rw, int rh)
        {
            int count = rw * rh;
            var result = new (int x, int y)[count];

            int index = 0;
            for (int y = ry; y < ry + rh; y++)
            {
                for (int x = rx; x < rx + rw; x++)
                {
                    result[index++] = (x, y);
                }
            }

            return result;
        }
    }
}
