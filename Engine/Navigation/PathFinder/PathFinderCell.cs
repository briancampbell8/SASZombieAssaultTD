// =====================================================================================================
//  FILE: PathFinderCell.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderCell.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Represents a deterministic, lightweight cell utility module for the PathFinder subsystem.
//      Provides foundational helpers for cell validation and basic cell-state operations.
//
//  RESPONSIBILITIES:
//      - Validate cell coordinates within a navigation grid.
//      - Provide deterministic cell-state helpers for higher-level navigation modules.
//      - Serve as a minimal, dependency-free utility for PathFinder components.
//
//  NON-RESPONSIBILITIES:
//      - Managing full grid structures or node graphs.
//      - Executing pathfinding algorithms or heuristics.
//      - Handling rendering, update loops, or engine lifecycle operations.
//      - Performing caching, smoothing, or optimization logic.
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally minimal and deterministic.
//      - Higher-level navigation modules may rely on these helpers for baseline cell behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderCell
    {
        /// <summary>
        /// Determines whether the given cell coordinates fall within the specified grid dimensions.
        /// Deterministic and side-effect free.
        /// </summary>
        internal bool IsValid(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        /// <summary>
        /// Converts a cell coordinate pair into a deterministic linear index.
        /// Useful for compact cell referencing in higher-level modules.
        /// </summary>
        internal int ToIndex(int x, int y, int width)
        {
            return (y * width) + x;
        }
    }
}
