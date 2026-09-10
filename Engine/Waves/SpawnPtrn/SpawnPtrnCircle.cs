// =====================================================================================================
//  FILE: SpawnPtrnCircle.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnCircle.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a circular deployment strategy distributing coordinate tables uniformly along
//      radial intervals using polar-to-cartesian coordinate translation routines.
//
//  RESPONSIBILITIES:
//      - Compute equidistant perimeter spawn nodes around an explicitly defined center target vector.
//      - Safely process dynamic radius modifiers during position array generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Persisting configuration states down to disk arrays.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnCircle program.
    /// </summary>
    public struct SpawnPtrnCircleConfig
    {
        public Vector3 CenterPoint { get; set; }
        public float? Radius { get; set; }
        public int EnemyCount { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnCircle execution pipeline.
    /// </summary>
    public class SpawnPtrnCircleValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnCircle
    {
        /// <summary>
        /// Dedicated math path translating radial steps into explicit perimeter positional vectors.
        /// </summary>
        public List<Vector3> ExecuteCircleCalculation(SpawnPtrnCircleConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            float radius = config.Radius ?? 3.0f;

            for (int i = 0; i < config.EnemyCount; i++)
            {
                float angle = (2.0f * MathF.PI * i) / config.EnemyCount;
                float x = config.CenterPoint.X + (MathF.Cos(angle) * radius);
                float y = config.CenterPoint.Y + (MathF.Sin(angle) * radius);

                positions.Add(new Vector3(x, y, config.CenterPoint.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this circular program.
        /// </summary>
        public string GetCircleStrategyDescription()
        {
            return "Spawns units in an evenly distributed perimeter ring formation around a central target location.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this circular script.
        /// </summary>
        public SpawnPtrnCircleValidationResult ValidateCircleParameters(SpawnPtrnCircleConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnCircleValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.Radius.HasValue && config.Radius.Value < 0)
            {
                return new SpawnPtrnCircleValidationResult
                {
                    IsValid = false,
                    Message = "Radius boundary configuration parameter cannot be negative.",
                    Errors = new List<string> { "InvalidRadiusValue" }
                };
            }

            return new SpawnPtrnCircleValidationResult { IsValid = true };
        }
    }
}
