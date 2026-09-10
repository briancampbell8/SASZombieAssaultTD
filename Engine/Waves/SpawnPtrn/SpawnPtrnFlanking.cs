// =====================================================================================================
//  FILE: SpawnPtrnFlanking.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnFlanking.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a flanking deployment strategy distributing coordinate tables across distinct
//      left and right vector wings relative to an explicitly defined directional axis line.
//
//  RESPONSIBILITIES:
//      - Compute offset layout nodes split symmetrically between flanking vectors.
//      - Provide a dedicated, unique API endpoint for flanking coordinate matrix generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Managing enemy state, tracking logic, or referencing deleted legacy classes.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnFlanking program.
    /// </summary>
    public struct SpawnPtrnFlankingConfig
    {
        public Vector3 BaseCenterPoint { get; set; }
        public Vector3 DirectionAxis { get; set; }
        public float? FlankSpread { get; set; }
        public int EnemyCount { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnFlanking execution pipeline.
    /// </summary>
    public class SpawnPtrnFlankingValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnFlanking
    {
        /// <summary>
        /// Dedicated math path splitting unit distributions symmetrically down opposing wing vectors.
        /// </summary>
        public List<Vector3> ExecuteFlankingCalculation(SpawnPtrnFlankingConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            float spread = config.FlankSpread ?? 2.0f;

            // Calculate perpendicular flanking vector using explicit System.Math calls
            float axisLength = (float)System.Math.Sqrt((config.DirectionAxis.X * config.DirectionAxis.X) + (config.DirectionAxis.Y * config.DirectionAxis.Y));
            Vector3 perpVector = (axisLength > 0.001f)
                ? new Vector3(-config.DirectionAxis.Y / axisLength, config.DirectionAxis.X / axisLength, 0f)
                : new Vector3(1f, 0f, 0f); // Default horizontal flanking axis fallback

            int leftSideCount = config.EnemyCount / 2;
            int rightSideCount = config.EnemyCount - leftSideCount;

            // Generate positions for the left wing flank
            for (int i = 0; i < leftSideCount; i++)
            {
                float offsetScale = spread * (1.0f + i);
                float x = config.BaseCenterPoint.X - (perpVector.X * offsetScale);
                float y = config.BaseCenterPoint.Y - (perpVector.Y * offsetScale);
                positions.Add(new Vector3(x, y, config.BaseCenterPoint.Z));
            }

            // Generate positions for the right wing flank
            for (int i = 0; i < rightSideCount; i++)
            {
                float offsetScale = spread * (1.0f + i);
                float x = config.BaseCenterPoint.X + (perpVector.X * offsetScale);
                float y = config.BaseCenterPoint.Y + (perpVector.Y * offsetScale);
                positions.Add(new Vector3(x, y, config.BaseCenterPoint.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this flanking program.
        /// </summary>
        public string GetFlankingStrategyDescription()
        {
            return "Spawns units split across distinct left and right flanking wings projecting outwards from a center direction line.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this flanking script.
        /// </summary>
        public SpawnPtrnFlankingValidationResult ValidateFlankingParameters(SpawnPtrnFlankingConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnFlankingValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if (config.FlankSpread.HasValue && config.FlankSpread.Value <= 0)
            {
                return new SpawnPtrnFlankingValidationResult
                {
                    IsValid = false,
                    Message = "Flanking spread metric must be strictly greater than zero.",
                    Errors = new List<string> { "InvalidSpreadValue" }
                };
            }

            return new SpawnPtrnFlankingValidationResult { IsValid = true };
        }
    }
}
