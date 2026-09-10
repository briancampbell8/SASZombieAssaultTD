// =====================================================================================================
//  FILE: SpawnPtrnSpiral.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnSpiral.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a spiral deployment strategy distributing coordinate tables progressively along
//      an advancing radius using polar‑to‑cartesian coordinate scaling routines.
//
//  RESPONSIBILITIES:
//      - Compute incremental spiral spawn nodes around an explicitly defined center target vector.
//      - Safely process dynamic starting and ending radius boundaries during position array generation.
//      - Provide a dedicated, unique API endpoint for spiral matrix generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Persisting configuration states down to disk arrays.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnSpiral program.
    /// </summary>
    public struct SpawnPtrnSpiralConfig
    {
        public Vector3 CenterPoint { get; set; }
        public float? RadiusStart { get; set; }
        public float? RadiusEnd { get; set; }
        public int EnemyCount { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnSpiral execution pipeline.
    /// </summary>
    public class SpawnPtrnSpiralValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnSpiral
    {
        /// <summary>
        /// Dedicated math path looping polar angles progressively over an advancing radius modifier.
        /// </summary>
        public List<Vector3> ExecuteSpiralCalculation(SpawnPtrnSpiralConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            float radiusStart = config.RadiusStart ?? 1.0f;
            float radiusEnd = config.RadiusEnd ?? 5.0f;

            for (int i = 0; i < config.EnemyCount; i++)
            {
                float progress = (config.EnemyCount - 1 == 0) ? 0.0f : (float)i / (config.EnemyCount - 1);
                float currentRadius = radiusStart + (radiusEnd - radiusStart) * progress;

                // Progressively advance the angle for the spiral layout effect
                double angle = i * 0.5;

                // Explicitly leveraging standard System.Math over structural MathF variations
                float x = config.CenterPoint.X + (float)(System.Math.Cos(angle) * currentRadius);
                float y = config.CenterPoint.Y + (float)(System.Math.Sin(angle) * currentRadius);

                positions.Add(new Vector3(x, y, config.CenterPoint.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this spiral program.
        /// </summary>
        public string GetSpiralStrategyDescription()
        {
            return "Spawns units in a widening spiral pattern progressing outwards from a central target location.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this spiral script.
        /// </summary>
        public SpawnPtrnSpiralValidationResult ValidateSpiralParameters(SpawnPtrnSpiralConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnSpiralValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            bool validRanges = (!config.RadiusStart.HasValue || config.RadiusStart.Value >= 0) &&
                               (!config.RadiusEnd.HasValue || config.RadiusEnd.Value >= 0);

            if (!validRanges)
            {
                return new SpawnPtrnSpiralValidationResult
                {
                    IsValid = false,
                    Message = "Radius boundaries configuration parameters cannot be negative.",
                    Errors = new List<string> { "NegativeRadiusValue" }
                };
            }

            if (config.RadiusStart.HasValue && config.RadiusEnd.HasValue && config.RadiusStart.Value >= config.RadiusEnd.Value)
            {
                return new SpawnPtrnSpiralValidationResult
                {
                    IsValid = false,
                    Message = "Starting radius must be strictly less than ending radius.",
                    Errors = new List<string> { "InvalidRadiusRange" }
                };
            }

            return new SpawnPtrnSpiralValidationResult { IsValid = true };
        }
    }
}
