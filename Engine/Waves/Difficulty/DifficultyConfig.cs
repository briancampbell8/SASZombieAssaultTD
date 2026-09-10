// =====================================================================================================
// FILE: DifficultyConfig.cs
// PATH: Engine/Waves/Difficulty/DifficultyConfig.cs
// SUBSYSTEM: Waves/Difficulty
//
// ROLE:
//     Provides static difficulty configuration values for each difficulty mode.
//     Acts as a lookup table used by DifficultyScaler.
//
// RESPONSIBILITIES:
//     - Store deterministic scalar values for each difficulty tier using DifficultySettings.
//     - Provide a static Get() method to retrieve the correct DifficultySettings object instance.
//     - Remain immutable and allocation-light.
// =====================================================================================================

using static SASZombieAssaultTD.Engine.Waves.Difficulty.DifficultyEnums;

namespace SASZombieAssaultTD.Engine.Waves.Difficulty
{
    // If DifficultyMode is already recognized from the external namespace, 
    // this local block can be commented out or removed safely.
    public enum DifficultyMode
    {
        Easy,
        Normal,
        Hard,
        Insane,
        Expert
    }

    public sealed class DifficultyConfig
    {
        // ---------------------------------------------------------------------------------------------
        // STATIC PREDEFINED DATA MAPPINGS (RETURNS STANDARDIZED DIFFICULTYSETTINGS)
        // ---------------------------------------------------------------------------------------------

        private static readonly DifficultySettings EasySettings = new DifficultySettings
        {
            Level = DifficultyLevel.Easy,
            HealthMultiplier = 0.75f,
            SpeedMultiplier = 0.90f,
            DamageMultiplier = 0.75f,
            SpawnRateMultiplier = 0.85f
        };

        private static readonly DifficultySettings NormalSettings = new DifficultySettings
        {
            Level = DifficultyLevel.Normal,
            HealthMultiplier = 1.00f,
            SpeedMultiplier = 1.00f,
            DamageMultiplier = 1.00f,
            SpawnRateMultiplier = 1.00f
        };

        private static readonly DifficultySettings HardSettings = new DifficultySettings
        {
            Level = DifficultyLevel.Hard,
            HealthMultiplier = 1.25f,
            SpeedMultiplier = 1.10f,
            DamageMultiplier = 1.25f,
            SpawnRateMultiplier = 1.15f
        };

        private static readonly DifficultySettings InsaneSettings = new DifficultySettings
        {
            Level = DifficultyLevel.Insane,
            HealthMultiplier = 1.50f,
            SpeedMultiplier = 1.20f,
            DamageMultiplier = 1.50f,
            SpawnRateMultiplier = 1.25f
        };

        // ---------------------------------------------------------------------------------------------
        // CONVERTER GET LOOKUP PATHWAY
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Authoritative static factory providing deterministic lookup settings configurations.
        /// </summary>
        public static DifficultySettings Get(DifficultyMode mode)
        {
            switch (mode)
            {
                case DifficultyMode.Easy: return EasySettings;
                case DifficultyMode.Normal: return NormalSettings;
                case DifficultyMode.Hard: return HardSettings;
                case DifficultyMode.Insane: return InsaneSettings;

                default:
                    // Deterministic non-null default layout fallback
                    return NormalSettings;
            }
        }

        public class DifficultySettings
        {
            public DifficultyLevel Level { get; set; }
            public float HealthMultiplier { get; set; }
            public float SpeedMultiplier { get; set; }
            public float DamageMultiplier { get; set; }
            public float SpawnRateMultiplier { get; set; }
            public float EnemyCountMultiplier { get; set; }
            public float WavePacingMultiplier { get; set; } = 1f;

            public DifficultySettings()
            {
                Level = DifficultyLevel.Normal;
                HealthMultiplier = 1.00f;
                SpeedMultiplier = 1.00f;
                DamageMultiplier = 1.00f;
                SpawnRateMultiplier = 1.00f;





            }


        }
    }
}
