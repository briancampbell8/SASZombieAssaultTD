// =====================================================================================================
//  FILE: PathFinderBounds.cs
//  PATH: Engine/Navigation/PathFinder/PathFinderBounds.cs
//  SUBSYSTEM: Navigation Subsystem
//
//  ROLE:
//      Provides deterministic boundary utilities for the PathFinder subsystem.
//      Supplies lightweight helpers for validating and clamping coordinates within grid bounds.
//
//  RESPONSIBILITIES:
//      - Validate coordinate positions against grid boundaries.
//      - Clamp coordinates to the nearest valid position.
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
//      - Higher-level navigation modules may rely on these helpers for baseline boundary behavior.
//      - Future expansion should preserve strict determinism and avoid subsystem coupling.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Navigation.PathFinder
{
    internal class PathFinderBounds
    {
        /// <summary>
        /// Returns true if the given coordinates fall within the specified grid dimensions.
        /// Deterministic and side-effect free.
        /// </summary>
        internal bool IsInside(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        /// <summary>
        /// Clamps a coordinate pair to the nearest valid position within the grid.
        /// Ensures deterministic behavior for out-of-range inputs.
        /// </summary>
        internal (int cx, int cy) Clamp(int x, int y, int width, int height)
        {
            int cx = x < 0 ? 0 : (x >= width ? width - 1 : x);
            int cy = y < 0 ? 0 : (y >= height ? height - 1 : y);
            return (cx, cy);
        }

        /// <summary>
        /// Ensures a rectangle defined by (x, y, w, h) fits within the grid bounds.
        /// Returns a deterministically clamped rectangle.
        /// </summary>
        internal (int x, int y, int w, int h) ClampRect(int x, int y, int w, int h, int width, int height)
        {
            int cx = x < 0 ? 0 : (x >= width ? width - 1 : x);
            int cy = y < 0 ? 0 : (y >= height ? height - 1 : y);

            int maxW = width - cx;
            int maxH = height - cy;

            int cw = w > maxW ? maxW : (w < 0 ? 0 : w);
            int ch = h > maxH ? maxH : (h < 0 ? 0 : h);

            return (cx, cy, cw, ch);
        }
    }
}
