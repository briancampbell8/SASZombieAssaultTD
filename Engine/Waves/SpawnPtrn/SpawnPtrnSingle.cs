// =====================================================================================================
//  FILE: SpawnPtrnSingle.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnSingle.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a localized focal deployment strategy concentrating unit coordinate nodes on
//      a single absolute position point with tight deterministic micro-offsets.
//
//  RESPONSIBILITIES:
//      - Collapse array coordinate processing down to a single designated vector coordinate spot.
//      - Introduce a local pseudo-random offset matrix to prevent entities from clipping identically.
//      - Provide a dedicated, unique API endpoint for focused individual point node generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Managing wave timing metrics or external ECSEntityCore pooling allocation boundaries.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnSingle program.
    /// </summary>
    public struct SpawnPtrnSingleConfig
    {
        public Vector3 TargetPoint { get; set; }
        public float? MicroVariance { get; set; }
        public int EnemyCount { get; set; }
        public int? Seed { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnSingle execution pipeline.
    /// </summary>
    public class SpawnPtrnSingleValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnSingle
    {
        /// <summary>
        /// Dedicated calculation routing clustering coordinate outputs tightly around a singular point vector.
        /// </summary>
        public List<Vector3> ExecuteSingleCalculation(SpawnPtrnSingleConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            // Extract maximum clustering spread factor using System.Math
            float rawVariance = config.MicroVariance ?? 0.2f;
            float variance = (float)System.Math.Abs(rawVariance);

            // Establish local seeded random state to ensure math remains purely deterministic
            int deterministicSeed = config.Seed ?? (int)(config.TargetPoint.X * 557 + config.TargetPoint.Y * 31);
            Random prng = new Random(deterministicSeed);

            for (int i = 0; i < config.EnemyCount; i++)
            {
                // Scale random dispersal coordinates around target space center lines
                double offsetX = (prng.NextDouble() - 0.5) * variance;
                double offsetY = (prng.NextDouble() - 0.5) * variance;

                float x = config.TargetPoint.X + (float)offsetX;
                float y = config.TargetPoint.Y + (float)offsetY;

                positions.Add(new Vector3(x, y, config.TargetPoint.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this single point program.
        /// </summary>
        public string GetSingleStrategyDescription()
        {
            return "Spawns all units grouped tightly on a single target position point with minor positional distribution offsets.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this single point script.
        /// </summary>
        public SpawnPtrnSingleValidationResult ValidateSingleParameters(SpawnPtrnSingleConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnSingleValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.MicroVariance.HasValue && config.MicroVariance.Value < 0.0f)
            {
                return new SpawnPtrnSingleValidationResult
                {
                    IsValid = false,
                    Message = "Clustering micro-variance configuration parameter cannot be negative.",
                    Errors = new List<string> { "NegativeVarianceValue" }
                };
            }

            return new SpawnPtrnSingleValidationResult { IsValid = true };
        }
    }
}
