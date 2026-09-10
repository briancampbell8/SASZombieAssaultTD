// =====================================================================================================
//  FILE: PathFinderMask.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderMask.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic mask utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for representing and evaluating cell mask states.
//
//  RESPONSIBILITIES:
//      - Represent deterministic cell mask values.
//      - Provide helpers for checking walkability and blocked states.
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
//      - Higher-level navigation modules may rely on these helpers for baseline mask behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderMask
    {
        /// <summary>
        /// Creates a deterministic mask value.
        /// Common usage: 0 = walkable, 1 = blocked, other values = custom flags.
        /// </summary>
        internal int Create(int value)
        {
            return value;
        }

        /// <summary>
        /// Returns true if the mask value represents a blocked cell.
        /// Deterministic and side-effect free.
        /// </summary>
        internal bool IsBlocked(int mask)
        {
            return mask != 0;
        }

        /// <summary>
        /// Returns true if the mask value represents a walkable cell.
        /// Deterministic and side-effect free.
        /// </summary>
        internal bool IsWalkable(int mask)
        {
            return mask == 0;
        }

        /// <summary>
        /// Applies a mask override deterministically.
        /// </summary>
        internal int Apply(int originalMask, int overrideMask)
        {
            return overrideMask;
        }
    }
}
