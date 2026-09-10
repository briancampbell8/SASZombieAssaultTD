// =====================================================================================================
//  FILE: SpawnPtrnGrid.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnGrid.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a grid matrix deployment strategy distributing coordinate tables uniformly across
//      ordered row and column intersections centered tightly around a core baseline vector.
//
//  RESPONSIBILITIES:
//      - Compute structural grid coordinate nodes using configurable column, row, and spacing parameters.
//      - Symmetrically balance rows and columns around an explicitly defined center target vector.
//      - Provide a dedicated, unique API endpoint for structured matrix array generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Managing active spawner triggers or linking to deleted legacy classes.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnGrid program.
    /// </summary>
    public struct SpawnPtrnGridConfig
    {
        public Vector3 CenterPoint { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public float? Spacing { get; set; }
        public int EnemyCount { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnGrid execution pipeline.
    /// </summary>
    public class SpawnPtrnGridValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnGrid
    {
        /// <summary>
        /// Dedicated math path mapping structured cell nodes dynamically up to the maximum enemy count threshold.
        /// </summary>
        public List<Vector3> ExecuteGridCalculation(SpawnPtrnGridConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            int cols = config.Columns <= 0 ? 3 : config.Columns;
            int rows = config.Rows <= 0 ? 3 : config.Rows;
            float spacing = config.Spacing ?? 1.0f;

            // Compute alignment offsets to center the grid perfectly around the center point using System.Math
            float halfWidth = ((cols - 1) * spacing) / 2.0f;
            float halfHeight = ((rows - 1) * spacing) / 2.0f;

            int index = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (index >= config.EnemyCount)
                        break;

                    // Calculate local offsets relative to center balance metrics
                    float localX = config.CenterPoint.X - halfWidth + (c * spacing);
                    float localY = config.CenterPoint.Y - halfHeight + (r * spacing);

                    positions.Add(new Vector3(localX, localY, config.CenterPoint.Z));
                    index++;
                }

                if (index >= config.EnemyCount)
                    break;
            }

            // Safe fallback container if grid boundaries are smaller than requested deployment count
            while (index < config.EnemyCount)
            {
                positions.Add(config.CenterPoint);
                index++;
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this grid program.
        /// </summary>
        public string GetGridStrategyDescription()
        {
            return "Spawns units inside a symmetrical matrix block arranged evenly by rows and columns around a central spot.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this grid script.
        /// </summary>
        public SpawnPtrnGridValidationResult ValidateGridParameters(SpawnPtrnGridConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnGridValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.Columns <= 0 || config.Rows <= 0)
            {
                return new SpawnPtrnGridValidationResult
                {
                    IsValid = false,
                    Message = "Grid dimensions for columns and rows must be strictly greater than zero.",
                    Errors = new List<string> { "InvalidGridDimensions" }
                };
            }

            if (config.Spacing.HasValue && config.Spacing.Value <= 0.0f)
            {
                return new SpawnPtrnGridValidationResult
                {
                    IsValid = false,
                    Message = "Grid coordinate spacing metric must be greater than zero.",
                    Errors = new List<string> { "InvalidSpacingValue" }
                };
            }

            return new SpawnPtrnGridValidationResult { IsValid = true };
        }
    }
}
