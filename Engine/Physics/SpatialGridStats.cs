// ====================================================================================================
//  FILE: SpatialGridStats.cs
//  PATH: Engine/Physics/SpatialGridStats.cs
//  MODULE: Physics
//
//  ROLE:
//      Provide a lightweight, read‑only statistics container for SpatialGrid imaging.
//
//  RESPONSIBILITIES:
//      - Hold immutable grid metrics for imaging modules.
//      - Provide cell size, grid width, grid height, and world bounds metrics.
//      - Serve as a safe, read‑only data object for ImagingGrid and ImagingStatsParser.
//      - Remain strictly read‑only toward ECS and physics state.
//
//  NON-RESPONSIBILITIES:
//      - Performing collision detection.
//      - Performing physics simulation.
//      - Performing diagnostics or debugging in the dictionary sense.
//      - Rendering shapes (delegated to ImagingShapes).
//      - Rendering grid cells (delegated to ImagingGrid).
//      - Rendering text (delegated to ImagingText).
//      - Parsing stats (delegated to ImagingStatsParser).
//
//  NOTES:
//      SpatialGridStats is a simple data container used by the Imaging subsystem. It does not detect or
//      fix errors. It is strictly a read‑only statistics object.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Immutable statistics container for SpatialGrid imaging.
    /// </summary>
    public sealed class SpatialGridStats
    {
        public int GridWidth { get; }
        public int GridHeight { get; }
        public float CellSize { get; }

        public int TotalCells { get; }
        public int OccupiedCells { get; }
        public int TotalEntities { get; }
        public int MinEntitiesPerCell { get; }
        public int MaxEntitiesPerCell { get; }
        public float AverageEntitiesPerCell { get; }

        /// <summary>
        /// Deterministic constructor. All values must be computed externally.
        /// </summary>
        public SpatialGridStats(
            int gridWidth,
            int gridHeight,
            float cellSize,
            int totalCells,
            int occupiedCells,
            int totalEntities,
            int minEntitiesPerCell,
            int maxEntitiesPerCell,
            float averageEntitiesPerCell)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            CellSize = cellSize;

            TotalCells = totalCells;
            OccupiedCells = occupiedCells;
            TotalEntities = totalEntities;
            MinEntitiesPerCell = minEntitiesPerCell;
            MaxEntitiesPerCell = maxEntitiesPerCell;
            AverageEntitiesPerCell = averageEntitiesPerCell;
        }

        /// <summary>
        /// Deterministic factory used by SpatialGridRuntime or ImagingStatsParser.
        /// </summary>
        public static SpatialGridStats FromGrid(
            float cellSize,
            int gridWidth,
            int gridHeight,
            Dictionary<int, List<object>> cellMap,
            int totalEntities)
        {
            int totalCells = gridWidth * gridHeight;

            var counts = cellMap.Values.Select(list => list.Count).ToList();

            int occupied = counts.Count(c => c > 0);
            int min = counts.Count > 0 ? counts.Min() : 0;
            int max = counts.Count > 0 ? counts.Max() : 0;
            float avg = counts.Count > 0 ? (float)counts.Average() : 0f;

            return new SpatialGridStats(
                gridWidth,
                gridHeight,
                cellSize,
                totalCells,
                occupied,
                totalEntities,
                min,
                max,
                avg);
        }
    }
}
