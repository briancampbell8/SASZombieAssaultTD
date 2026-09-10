// =====================================================================================================
//  FILE: SpawnPtrnLine.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnLine.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a linear deployment strategy distributing coordinate tables uniformly along
//      a straight vector path extending from an explicitly defined origin.
//
//  RESPONSIBILITIES:
//      - Compute structural point nodes along an inspection vector using step-based spacing metrics.
//      - Standardize direction vectors safely to guarantee uniform node-to-node deployment distances.
//      - Provide a dedicated, unique API endpoint for line-based coordinate calculation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Mutating runtime component values outside this math pipeline.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnLine program.
    /// </summary>
    public struct SpawnPtrnLineConfig
    {
        public Vector3 StartPoint { get; set; }
        public Vector3 Direction { get; set; }
        public float? Spacing { get; set; }
        public int EnemyCount { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnLine execution pipeline.
    /// </summary>
    public class SpawnPtrnLineValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnLine
    {
        /// <summary>
        /// Dedicated math path projecting sequence offsets linearly down an arbitrary vector track.
        /// </summary>
        public List<Vector3> ExecuteLineCalculation(SpawnPtrnLineConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            float spacing = config.Spacing ?? 1.0f;

            // Calculate directional magnitude using explicit System.Math calls
            float magnitude = (float)System.Math.Sqrt((config.Direction.X * config.Direction.X) +
                                                      (config.Direction.Y * config.Direction.Y) +
                                                      (config.Direction.Z * config.Direction.Z));

            // Standardize heading direction or fallback cleanly to a standard horizontal right vector
            Vector3 normalizedDirection = (magnitude > 0.001f)
                ? new Vector3(config.Direction.X / magnitude, config.Direction.Y / magnitude, config.Direction.Z / magnitude)
                : new Vector3(1.0f, 0.0f, 0.0f);

            for (int i = 0; i < config.EnemyCount; i++)
            {
                float totalDistance = i * spacing;
                float x = config.StartPoint.X + (normalizedDirection.X * totalDistance);
                float y = config.StartPoint.Y + (normalizedDirection.Y * totalDistance);
                float z = config.StartPoint.Z + (normalizedDirection.Z * totalDistance);

                positions.Add(new Vector3(x, y, z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this line program.
        /// </summary>
        public string GetLineStrategyDescription()
        {
            return "Spawns units neatly in a single file straight line formation expanding outwards from a given origin pointer.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this line script.
        /// </summary>
        public SpawnPtrnLineValidationResult ValidateLineParameters(SpawnPtrnLineConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnLineValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.Spacing.HasValue && config.Spacing.Value <= 0.0f)
            {
                return new SpawnPtrnLineValidationResult
                {
                    IsValid = false,
                    Message = "Linear alignment spacing parameter must be strictly greater than zero.",
                    Errors = new List<string> { "InvalidSpacingValue" }
                };
            }

            return new SpawnPtrnLineValidationResult { IsValid = true };
        }
    }
}
