// =====================================================================================================
//  FILE: DifficultyProfile.cs
//  PATH: Engine/Waves/Difficulty/DifficultyProfile.cs
//  SUBSYSTEM: Waves/Difficulty Subsystem
//
//  ROLE:
//      Provides a deterministic, serializable container for custom difficulty profiles.
//      Allows future expansion beyond predefined DifficultySettings tiers.
//      Acts as a structured data definition for user-defined or designer-defined difficulty rules.
//
//  RESPONSIBILITIES:
//      - Store custom difficulty multipliers for enemy count, spawn rate, and pacing.
//      - Provide a deterministic configuration surface for advanced difficulty modes.
//      - Serve as an optional extension consumed by DifficultyManager.
//
//  NON-RESPONSIBILITIES:
//      - Managing active difficulty state (DifficultyManager handles state).
//      - Providing static predefined difficulty tiers (DifficultyConfig handles predefined tiers).
//      - Applying scaling to wave definitions (DifficultyScaler handles scaling).
//      - Executing wave logic or spawning behavior.
//
//  ARCHITECTURAL NOTES:
//      - This module is purely a data profile: no logic, no branching, no side effects.
//      - DifficultyManager may load or reference profiles when custom difficulty modes are enabled.
//      - Profiles must remain deterministic and fully serializable.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Waves.Difficulty
{
    public sealed class DifficultyProfile
    {
        public float EnemyCountMultiplier { get; }
        public float SpawnRateMultiplier { get; }
        public float WavePacingMultiplier { get; }

        public DifficultyProfile(
            float enemyCountMultiplier,
            float spawnRateMultiplier,
            float wavePacingMultiplier)
        {
            EnemyCountMultiplier = enemyCountMultiplier;
            SpawnRateMultiplier = spawnRateMultiplier;
            WavePacingMultiplier = wavePacingMultiplier;
        }
    }
}
