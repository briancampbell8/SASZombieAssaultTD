// =====================================================================================================
//  FILE: WaveStatMultiplier.cs
//  PATH: Engine/Waves/WaveStatMultiplier.cs
//  SUBSYSTEM: Waves Module
//
//  ROLE:
//      Provides deterministic, wave-local stat multipliers used to adjust enemy attributes
//      for a specific wave. This class is purely a data container and does not perform
//      any scaling logic.
//
//  RESPONSIBILITIES:
//      - Store per-wave stat multipliers (health, speed, damage, etc.).
//      - Provide a clean, deterministic structure for WaveScript and WaveDirector.
//      - Support cloning for safe reuse and modification.
//
//  NON-RESPONSIBILITIES:
//      - Applying global difficulty scaling (handled by DifficultyScaler).
//      - Managing difficulty tiers or progression (handled by DifficultyManager and
//        DifficultyProgression).
//      - Executing wave logic or spawning behavior.
//
//  ARCHITECTURAL NOTES:
//      - This class is part of the WaveDirector definition layer.
//      - Must remain deterministic and free of external dependencies.
//      - Contains no runtime logic; only stat multiplier data.
//
//  CHANGE LOG:
//      P11-08-06: Initial creation of WaveStatMultiplier to replace DifficultyMultiplier
//                 and remove difficulty subsystem contamination from WaveScript.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Waves
{
    public sealed class WaveStatMultiplier
    {
        public float HealthMultiplier { get; set; } = 1.0f;
        public float SpeedMultiplier { get; set; } = 1.0f;
        public float DamageMultiplier { get; set; } = 1.0f;

        public WaveStatMultiplier Clone()
        {
            return new WaveStatMultiplier
            {
                HealthMultiplier = this.HealthMultiplier,
                SpeedMultiplier = this.SpeedMultiplier,
                DamageMultiplier = this.DamageMultiplier
            };
        }
    }
}
