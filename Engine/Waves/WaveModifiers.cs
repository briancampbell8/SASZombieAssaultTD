// =====================================================================================================
// FILE: WaveModifiers.cs
// PATH: Engine/Waves/WaveModifiers.cs
// SUBSYSTEM: Waves
//
// ROLE:
//     Provides optional per-wave modifier flags and scalar adjustments used by the WaveScript asset.
//     Allows wave designers to toggle special behaviors or apply lightweight stat adjustments that
//     do not belong to the Difficulty subsystem.
//
// RESPONSIBILITIES:
//     - Store deterministic, non-difficulty modifier values for a wave.
//     - Provide boolean toggles for special-case wave behaviors.
//     - Provide scalar multipliers for lightweight adjustments (non-tier difficulty).
//     - Remain passive: no logic, no mutation, no runtime execution.
//
// NON-RESPONSIBILITIES:
//     - Difficulty scaling (handled by DifficultyScaler).
//     - Runtime mutation of WaveScript or WaveDirector state.
//     - Enemy spawning, pacing, or progression logic.
//     - Any form of execution, scheduling, or lifecycle behavior.
//
// ARCHITECTURAL NOTES:
//     - This class is a pure data container.
//     - All values are optional; WaveScript may ignore fields not used by the engine.
//     - Must remain allocation-light and free of side effects.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Passive container for optional per-wave modifier values.
    /// </summary>
    public sealed class WaveModifiers
    {
        // ---------------------------------------------------------------------------------------------
        // OPTIONAL BOOLEAN FLAGS
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// If true, enemies in this wave spawn with increased aggression.
        /// </summary>
        public bool AggressionBoost { get; set; }

        /// <summary>
        /// If true, enemies in this wave spawn with reduced aggression.
        /// </summary>
        public bool CalmWave { get; set; }

        /// <summary>
        /// If true, this wave applies environmental hazards (non-difficulty).
        /// </summary>
        public bool EnableHazards { get; set; }

        /// <summary>
        /// If true, this wave uses special scripted behavior.
        /// </summary>
        public bool SpecialBehavior { get; set; }

        // ---------------------------------------------------------------------------------------------
        // OPTIONAL SCALAR MULTIPLIERS (NON-DIFFICULTY)
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Lightweight enemy health multiplier (not tied to difficulty tiers).
        /// </summary>
        public float HealthScalar { get; set; } = 1f;

        /// <summary>
        /// Lightweight enemy speed multiplier (not tied to difficulty tiers).
        /// </summary>
        public float SpeedScalar { get; set; } = 1f;

        /// <summary>
        /// Lightweight enemy damage multiplier (not tied to difficulty tiers).
        /// </summary>
        public float DamageScalar { get; set; } = 1f;

        /// <summary>
        /// Lightweight spawn rate multiplier (non-difficulty).
        /// </summary>
        public float SpawnRateScalar { get; set; } = 1f;

        // ---------------------------------------------------------------------------------------------
        // OPTIONAL WAVE META
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Optional designer tag for identifying special waves.
        /// </summary>
        public string Tag { get; set; }
    }
}
