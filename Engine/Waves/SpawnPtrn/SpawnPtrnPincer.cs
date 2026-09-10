// =====================================================================================================
//  FILE: SpawnPtrnPincer.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnPincer.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a dual-converging pincer deployment strategy distributing coordinate tables 
//      alternatingly across two extreme outer locations advancing toward a target sector.
//
//  RESPONSIBILITIES:
//      - Compute structural point nodes alternating between a designated Left flanking origin and a Right flanking origin.
//      - Provide step-based inline displacement offsets along a specified advancement vector.
//      - Provide a dedicated, unique API endpoint for pincer-based coordinate matrix generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Modifying component properties or state contexts outside this mathematical scope.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnPincer program.
    /// </summary>
    public struct SpawnPtrnPincerConfig
    {
        public Vector3 LeftFlankOrigin { get; set; }
        public Vector3 RightFlankOrigin { get; set; }
        public Vector3 TargetDirection { get; set; }
        public float? AdvanceSpacing { get; set; }
        public int EnemyCount { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnPincer execution pipeline.
    /// </summary>
    public class SpawnPtrnPincerValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnPincer
    {
        /// <summary>
        /// Dedicated math path alternating distribution counts symmetrically across two opposite converging origin nodes.
        /// </summary>
        public List<Vector3> ExecutePincerCalculation(SpawnPtrnPincerConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            float spacing = config.AdvanceSpacing ?? 1.0f;

            // Normalize tracking direction vector using System.Math calls
            float magnitude = (float)System.Math.Sqrt((config.TargetDirection.X * config.TargetDirection.X) +
                                                      (config.TargetDirection.Y * config.TargetDirection.Y) +
                                                      (config.TargetDirection.Z * config.TargetDirection.Z));

            Vector3 advanceDir = (magnitude > 0.001f)
                ? new Vector3(config.TargetDirection.X / magnitude, config.TargetDirection.Y / magnitude, config.TargetDirection.Z / magnitude)
                : new Vector3(0.0f, -1.0f, 0.0f); // Default top-to-bottom tactical progression track

            for (int i = 0; i < config.EnemyCount; i++)
            {
                // Alternating choice logic splits numbers between structural sides
                bool isLeftFlank = (i % 2 == 0);
                Vector3 baselineOrigin = isLeftFlank ? config.LeftFlankOrigin : config.RightFlankOrigin;

                // Stagger deployment steps linearly along the advance heading lines based on row depth indices
                int rowDepth = i / 2;
                float depthOffset = rowDepth * spacing;

                float x = baselineOrigin.X + (advanceDir.X * depthOffset);
                float y = baselineOrigin.Y + (advanceDir.Y * depthOffset);
                float z = baselineOrigin.Z + (advanceDir.Z * depthOffset);

                positions.Add(new Vector3(x, y, z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this pincer program.
        /// </summary>
        public string GetPincerStrategyDescription()
        {
            return "Spawns units alternatingly down left and right flanking parameters converging inwards on a target zone.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this pincer script.
        /// </summary>
        public SpawnPtrnPincerValidationResult ValidatePincerParameters(SpawnPtrnPincerConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnPincerValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.AdvanceSpacing.HasValue && config.AdvanceSpacing.Value < 0.0f)
            {
                return new SpawnPtrnPincerValidationResult
                {
                    IsValid = false,
                    Message = "Pincer advance spacing sequence metric cannot be negative.",
                    Errors = new List<string> { "InvalidSpacingValue" }
                };
            }

            return new SpawnPtrnPincerValidationResult { IsValid = true };
        }
    }
}
