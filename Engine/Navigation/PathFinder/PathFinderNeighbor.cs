// =====================================================================================================
//  FILE: PathFinderNeighbor.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderNeighbor.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic neighbor enumeration utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for retrieving and validating adjacent grid cells.
//
//  RESPONSIBILITIES:
//      - Enumerate neighboring cell coordinates deterministically.
//      - Validate neighbor positions against grid boundaries.
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
//      - Higher-level navigation modules may rely on these helpers for baseline neighbor behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderNeighbor
    {
        /// <summary>
        /// Returns the four cardinal neighbors of a cell as deterministic (nx, ny) pairs.
        /// </summary>
        internal (int nx, int ny)[] GetCardinalNeighbors(int x, int y)
        {
            return new[]
            {
                (x,     y - 1), // Up
                (x + 1, y    ), // Right
                (x,     y + 1), // Down
                (x - 1, y    )  // Left
            };
        }

        /// <summary>
        /// Filters the provided neighbor list to only those within the grid boundaries.
        /// Deterministic and side-effect free.
        /// </summary>
        internal (int nx, int ny)[] FilterValidNeighbors(
            (int nx, int ny)[] neighbors,
            int width,
            int height)
        {
            int count = 0;

            // First pass: count valid neighbors
            foreach (var n in neighbors)
            {
                if (n.nx >= 0 && n.nx < width &&
                    n.ny >= 0 && n.ny < height)
                {
                    count++;
                }
            }

            // Second pass: collect valid neighbors
            var result = new (int nx, int ny)[count];
            int index = 0;

            foreach (var n in neighbors)
            {
                if (n.nx >= 0 && n.nx < width &&
                    n.ny >= 0 && n.ny < height)
                {
                    result[index++] = n;
                }
            }

            return result;
        }
    }
}
