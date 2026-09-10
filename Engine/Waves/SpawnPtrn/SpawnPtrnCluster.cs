// =====================================================================================================
//  FILE: SpawnPtrnCluster.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnCluster.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a clustered group deployment strategy distributing coordinate tables tightly
//      within a radial boundary zone around a central coordinate point using deterministic dispersal math.
//
//  RESPONSIBILITIES:
//      - Compute varied polar coordinate offsets within an explicitly defined max radius margin.
//      - Provide a dedicated, unique API endpoint for clustered matrix generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Accessing global volatile random hardware states directly without parameter constraints.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnCluster program.
    /// </summary>
    public struct SpawnPtrnClusterConfig
    {
        public Vector3 CenterPoint { get; set; }
        public float? Radius { get; set; }
        public int EnemyCount { get; set; }
        public int? Seed { get; set; } // Local seed to maintain pure math determinism
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnCluster execution pipeline.
    /// </summary>
    public class SpawnPtrnClusterValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnCluster
    {
        /// <summary>
        /// Dedicated math path calculating random coordinates bounded within a maximum perimeter displacement.
        /// </summary>
        public List<Vector3> ExecuteClusterCalculation(SpawnPtrnClusterConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            float maxRadius = config.Radius ?? 2.0f;

            // Deterministic pseudo-random seed generator loop to remain free of external dependencies
            int seed = config.Seed ?? (int)(config.CenterPoint.X * 1000 + config.CenterPoint.Y);
            Random prng = new Random(seed);

            for (int i = 0; i < config.EnemyCount; i++)
            {
                // Polar distribution
                double angle = prng.NextDouble() * 2.0 * System.Math.PI;
                double distance = prng.NextDouble() * maxRadius;

                float x = config.CenterPoint.X + (float)(System.Math.Cos(angle) * distance);
                float y = config.CenterPoint.Y + (float)(System.Math.Sin(angle) * distance);

                positions.Add(new Vector3(x, y, config.CenterPoint.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this clustered program.
        /// </summary>
        public string GetClusterStrategyDescription()
        {
            return "Spawns units tightly packed as an offset cluster grouping localized around a central focal spot.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this cluster script.
        /// </summary>
        public SpawnPtrnClusterValidationResult ValidateClusterParameters(SpawnPtrnClusterConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnClusterValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.Radius.HasValue && config.Radius.Value < 0)
            {
                return new SpawnPtrnClusterValidationResult
                {
                    IsValid = false,
                    Message = "Cluster radius parameter threshold bounds cannot be negative.",
                    Errors = new List<string> { "InvalidRadiusValue" }
                };
            }

            return new SpawnPtrnClusterValidationResult { IsValid = true };
        }
    }
}
