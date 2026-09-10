// =====================================================================================================
//  FILE: SpawnPtrnWave.cs
//  PATH: Engine/Waves/SpawnPtrn/SpawnPtrnWave.cs
//  SUBSYSTEM: Waves SpawnPtrn
//
//  ROLE:
//      Implements a sinusoidal wave deployment strategy distributing coordinate tables progressively
//      along an advancement axis modulated by mathematical amplitude and frequency constants.
//
//  RESPONSIBILITIES:
//      - Compute incremental wave positions using frequency factors relative to an explicit origin.
//      - Modulate perpendicular track offsets using standard deterministic sinusoidal math properties.
//      - Provide a dedicated, unique API endpoint for wave-based coordinate collection generation.
//
//  NON-RESPONSIBILITIES:
//      - Directly handling pathfinding nodes or route validation calculations.
//      - Managing countdown clocks or checking runtime system wave definitions.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves.SpawnPtrn
{
    /// <summary>
    /// Dedicated parameter layout required strictly by the SpawnPtrnWave program.
    /// </summary>
    public struct SpawnPtrnWaveConfig
    {
        public Vector3 StartOrigin { get; set; }
        public Vector3 AdvanceDirection { get; set; }
        public float? Amplitude { get; set; }
        public float? Frequency { get; set; }
        public float? Spacing { get; set; }
        public int EnemyCount { get; set; }
    }

    /// <summary>
    /// Unique validation response wrapper specific to the SpawnPtrnWave execution pipeline.
    /// </summary>
    public class SpawnPtrnWaveValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class SpawnPtrnWave
    {
        /// <summary>
        /// Dedicated math path projecting sequence steps linearly while modulating the perpendicular vector via System.Math.Sin properties.
        /// </summary>
        public List<Vector3> ExecuteWaveCalculation(SpawnPtrnWaveConfig config)
        {
            var positions = new List<Vector3>();
            if (config.EnemyCount <= 0) return positions;

            float amplitude = config.Amplitude ?? 2.0f;
            float frequency = config.Frequency ?? 0.5f;
            float spacing = config.Spacing ?? 1.0f;

            // Compute alignment headers using explicit System.Math calls
            float axisLength = (float)System.Math.Sqrt((config.AdvanceDirection.X * config.AdvanceDirection.X) +
                                                      (config.AdvanceDirection.Y * config.AdvanceDirection.Y));

            Vector3 forward = (axisLength > 0.001f)
                ? new Vector3(config.AdvanceDirection.X / axisLength, config.AdvanceDirection.Y / axisLength, 0f)
                : new Vector3(1f, 0f, 0f); // Default straight horizontal advance progression track

            // Derive orthogonal perpendicular axis for sinusoidal height offset projection
            Vector3 orthogonal = new Vector3(-forward.Y, forward.X, 0f);

            for (int i = 0; i < config.EnemyCount; i++)
            {
                float linearDistance = i * spacing;

                // Explicitly leveraging standard System.Math over structural MathF expressions
                double sineInput = i * frequency;
                float currentWaveOffset = (float)(System.Math.Sin(sineInput) * amplitude);

                // Combine forward step components with modulated sideward wave amplitudes
                float x = config.StartOrigin.X + (forward.X * linearDistance) + (orthogonal.X * currentWaveOffset);
                float y = config.StartOrigin.Y + (forward.Y * linearDistance) + (orthogonal.Y * currentWaveOffset);

                positions.Add(new Vector3(x, y, config.StartOrigin.Z));
            }

            return positions;
        }

        /// <summary>
        /// Retrieves descriptive metadata unique to this wave pattern program.
        /// </summary>
        public string GetWaveStrategyDescription()
        {
            return "Spawns units undulating along a standard sinusoidal wave track tracking outward down an advance line vector.";
        }

        /// <summary>
        /// Safety parameter validation rule block bound entirely to this wave script.
        /// </summary>
        public SpawnPtrnWaveValidationResult ValidateWaveParameters(SpawnPtrnWaveConfig config)
        {
            if (config.EnemyCount <= 0)
            {
                return new SpawnPtrnWaveValidationResult
                {
                    IsValid = false,
                    Message = "Enemy count must be greater than 0.",
                    Errors = new List<string> { "InvalidEnemyCount" }
                };
            }

            if ((config.Amplitude.HasValue && config.Amplitude.Value < 0f) ||
                (config.Frequency.HasValue && config.Frequency.Value <= 0f))
            {
                return new SpawnPtrnWaveValidationResult
                {
                    IsValid = false,
                    Message = "Amplitude constraints cannot be negative, and frequency values must be strictly greater than zero.",
                    Errors = new List<string> { "InvalidWaveModulationFactors" }
                };
            }

            return new SpawnPtrnWaveValidationResult { IsValid = true };
        }
    }
}
