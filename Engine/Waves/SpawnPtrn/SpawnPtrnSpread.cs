// =====================================================================================================
//  FILE: SpawnPtrnSpread.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnSpread.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a uniform spread deployment strategy distributing coordinate tables across
//      multiple targeted anchor nodes using explicit step-based scatter dimensions.
//
//  RESPONSIBILITIES:
//      - Distribute calculated unit positions evenly across a collection of provided anchor point vectors.
//      - Introduce a local pseudo-random scatter padding value to decouple identical spawn footprints.
//      - Provide a dedicated, unique API endpoint for spread-based array coordinate generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Accessing thread-volatile platform clock elements directly without seeding variables.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnSpread program.
    /// </summary>
    public struct SpawnPtrnSpreadConfig
    {
        public List<Vector3> AnchorPoints { get; set; }
        public float? SpreadRadius { get; set; }
        public int EnemyCount { get; set; }
        public int? Seed { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnSpread execution pipeline.
    /// </summary>
    public class SpawnPtrnSpreadValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnSpread
    {
        /// <summary>
        /// Dedicated math path cyclical iterating across input anchors while padding local coordinates with spread math.
        /// </summary>
        public List<Vector3> ExecuteSpreadCalculation(SpawnPtrnSpreadConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            // Fallback cleanly to a core point vector if no anchors are provided
            var anchors = config.AnchorPoints;
            if (anchors == null || anchors.Count == 0)
            {
                anchors = new List<Vector3> { Vector3.Zero };
            }

            // Extract scattering margin constraints with safe absolute limits using System.Math
            float rawRadius = config.SpreadRadius ?? 0.5f;
            float radius = (float)System.Math.Abs(rawRadius);

            // Maintain pure functional determinism by driving lookups via local seeded indices
            int localSeed = config.Seed ?? (int)(anchors[0].X * 911 + anchors[0].Y * 13);
            Random prng = new Random(localSeed);

            for (int i = 0; i < config.EnemyCount; i++)
            {
                // Select active target anchor point cyclically
                Vector3 currentAnchor = anchors[i % anchors.Count];

                // Scale random spatial noise displacement vectors inside the spread radius limits
                double scatterX = (prng.NextDouble() - 0.5) * radius;
                double scatterY = (prng.NextDouble() - 0.5) * radius;

                float x = currentAnchor.X + (float)scatterX;
                float y = currentAnchor.Y + (float)scatterY;

                positions.Add(new Vector3(x, y, currentAnchor.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this spread program.
        /// </summary>
        public string GetSpreadStrategyDescription()
        {
            return "Spawns units distributed evenly across all validated anchor node paths with an absolute spatial padding factor.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this spread script.
        /// </summary>
        public SpawnPtrnSpreadValidationResult ValidateSpreadParameters(SpawnPtrnSpreadConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnSpreadValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.SpreadRadius.HasValue && config.SpreadRadius.Value < 0.0f)
            {
                return new SpawnPtrnSpreadValidationResult
                {
                    IsValid = false,
                    Message = "Spread radius dispersion parameter cannot be a negative value.",
                    Errors = new List<string> { "NegativeRadiusValue" }
                };
            }

            return new SpawnPtrnSpreadValidationResult { IsValid = true };
        }
    }
}
