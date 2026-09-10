// ====================================================================================================
//  FILE: ImagingGrid.cs
//  PATH: Engine/Physics/Imaging/
//  MODULE: Imaging
//
//  ROLE:
//      Render spatial grid cell imaging for the Physics Imaging subsystem.
//
//  RESPONSIBILITIES:
//      - Render spatial grid cells.
//      - Render grid occupancy visualization.
//      - Render world bounds visualization.
//      - Provide lightweight grid iteration for imaging only.
//      - Remain strictly read‑only toward ECS and physics state.
//
//  NON-RESPONSIBILITIES:
//      - Performing collision detection.
//      - Performing physics simulation.
//      - Performing diagnostics or debugging in the dictionary sense.
//      - Rendering shapes (delegated to ImagingShapes).
//      - Rendering text (delegated to ImagingText).
//      - Parsing stats (delegated to ImagingStatsParser).
//
//  NOTES:
//      ImagingGrid produces visual representations of spatial grid state. It does not detect or fix
//      errors. It is strictly a visualization module.
// ====================================================================================================

using System.Linq;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Physics.Imaging
{
    /// <summary>
    /// Renders spatial grid visualization for physics imaging.
    /// </summary>
    public sealed class ImagingGrid
    {
        private readonly ECSRuntimeCore _ecsWorld;
        private readonly SpatialGridRuntime grid;
        // removed _ECSEntityCoreCore — spatial queries are provided by the spatial grid itself

        public ImagingGrid(ECSRuntimeCore ecsWorld)
        {
            _ecsWorld = ecsWorld;
        }

        /// <summary>
        /// Renders spatial grid cells and world bounds.
        /// </summary>
        public void Render(D3D11Adapter_Core context, uint gridColor)
        {



            if (grid == null)
                return;

            // Acquire grid stats (read‑only)
            var stats = grid.GetStatsObject();
            if (stats == null)
                return;

            var worldMin = grid.WorldMin;
            var cellSize = stats.CellSize;

            // Render each grid cell
            for (int x = 0; x < stats.GridWidth; x++)
            {
                for (int y = 0; y < stats.GridHeight; y++)
                {

                    var cellMin = worldMin +
                        new Vector3(x: x * cellSize,
                        y: y * cellSize,
                        z: 0);

                    var cellMax = cellMin + new Vector3(cellSize, cellSize, 0);

                    // Occupancy check (read‑only)
                    // Use the spatial grid's query method instead of a non-existent method on ECSEntityCore
                    bool isOccupied = grid.GetEntitiesInArea(cellMin, cellMax).Any();


                    uint color = isOccupied ? 0x60606060 : gridColor;

                    var rect = Rectangle.FromPositionAndSize(
                        cellMin.X,
                        cellMin.Y,
                        cellSize,
                        cellSize);
                    context.FillRectangle(rect, Color.FromArgb(0, 0, 0, 0));

                    //context.FillRectangle(rect, Color.FromUint(color));  FILLTEST1
                }
            }

            // Render world bounds
            var boundsRect = Rectangle.FromPositionAndSize(
                worldMin.X,
                worldMin.Y,
                stats.GridWidth * cellSize,
                stats.GridHeight * cellSize);

            context.DrawRectangle(boundsRect, Color.FromUint(0xFFFFFFFF), 2.0f);
        }
    }
}