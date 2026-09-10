// =====================================================================================================
//  FILE: SpawnPtrnRandom.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnRandom.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a randomized deployment strategy distributing coordinate tables across a 
//      bounded geometric area using a pure, deterministic pseudo‑random distribution system.
//
//  RESPONSIBILITIES:
//      - Compute unpredictable layout nodes safely locked inside rectangular horizontal and vertical margins.
//      - Support local seeding structures to maintain deterministic synchronization across network and save states.
//      - Provide a dedicated, unique API endpoint for bounded randomized node calculations.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Sampling from unpredictable or thread‑volatile hardware clock states directly.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnRandom program.
    /// </summary>
    public struct SpawnPtrnRandomConfig
    {
        public Vector3 CenterPoint { get; set; }
        public float? WidthBounds { get; set; }
        public float? HeightBounds { get; set; }
        public int EnemyCount { get; set; }
        public int? Seed { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnRandom execution pipeline.
    /// </summary>
    public class SpawnPtrnRandomValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnRandom
    {
        /// <summary>
        /// Dedicated math path scattering coordinate offsets uniformly inside a designated layout zone boundary box.
        /// </summary>
        public List<Vector3> ExecuteRandomCalculation(SpawnPtrnRandomConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            // Extract box sizes with deterministic safety limits using System.Math
            float rawWidth = config.WidthBounds ?? 6.0f;
            float rawHeight = config.HeightBounds ?? 6.0f;

            float width = (float)System.Math.Abs(rawWidth);
            float height = (float)System.Math.Abs(rawHeight);

            // Establish local seed tracing from the position matrix coordinates to preserve functional determinism
            int deterministicSeed = config.Seed ?? (int)(config.CenterPoint.X * 733 + config.CenterPoint.Y * 19);
            Random prng = new Random(deterministicSeed);

            for (int i = 0; i < config.EnemyCount; i++)
            {
                // Scale random range from [0, 1] down to [-HalfBounds, +HalfBounds]
                double randomXOffset = (prng.NextDouble() - 0.5) * width;
                double randomYOffset = (prng.NextDouble() - 0.5) * height;

                float x = config.CenterPoint.X + (float)randomXOffset;
                float y = config.CenterPoint.Y + (float)randomYOffset;

                positions.Add(new Vector3(x, y, config.CenterPoint.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this random program.
        /// </summary>
        public string GetRandomStrategyDescription()
        {
            return "Spawns units scattered unpredictably across an isolated rectangular bounding zone mapped around a focal point.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this random script.
        /// </summary>
        public SpawnPtrnRandomValidationResult ValidateRandomParameters(SpawnPtrnRandomConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnRandomValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if ((config.WidthBounds.HasValue && config.WidthBounds.Value < 0f) ||
                (config.HeightBounds.HasValue && config.HeightBounds.Value < 0f))
            {
                return new SpawnPtrnRandomValidationResult
                {
                    IsValid = false,
                    Message = "Rectangular area size boundaries cannot be negative dimensions.",
                    Errors = new List<string> { "NegativeDimensions" }
                };
            }

            return new SpawnPtrnRandomValidationResult { IsValid = true };
        }
    }
}
