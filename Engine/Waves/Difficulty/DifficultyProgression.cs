// =====================================================================================================
//  FILE: DifficultyProgression.cs
//  PATH: Engine/Waves/Difficulty/DifficultyProgression.cs
//  SUBSYSTEM: Waves Difficulty
//
//  ROLE:
//      Provides deterministic difficulty‑progression logic for wave‑based gameplay.
//      Computes per‑wave scaling values (health, speed, damage, count, pacing, spawn rates) based on
//      difficulty level, wave number, and bounded multiplier rules.
//      Acts as the mathematical backbone for DifficultyScaler and WaveDirectorInitialization.
//
//  RESPONSIBILITIES:
//      - Compute bounded multipliers for enemy attributes across wave progression.
//      - Provide GetMultiplier[...] indexer for deterministic wave‑based multiplier lookup.
//      - Produce DifficultyScale objects for DifficultyScaler.
//      - Enforce clamped, deterministic scaling rules across all difficulty levels.
//      - Maintain pure, stateless progression logic (except for per‑instance wave state).
//
//  NON-RESPONSIBILITIES:
//      - Applying scaling to WaveDefinition instances (DifficultyScaler handles application).
//      - Managing difficulty configuration (DifficultyConfig handles configuration).
//      - Storing or mutating global difficulty state (DifficultyManager handles state).
//      - Executing wave logic, spawning, or runtime behavior.
//
//  ARCHITECTURAL NOTES:
//      - DifficultyProgression is a pure mathematical subsystem: deterministic, testable, and side‑effect free.
//      - DifficultyScaler consumes DifficultyProgression outputs to transform WaveDefinition instances.
//      - WaveDirectorInitialization uses DifficultyProgression to generate default wave scripts.
//      - The GetMultiplier[...] indexer provides a stable, predictable multiplier curve across waves.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Waves.Difficulty
{
    public sealed class DifficultyProgression
    {
        public const int MinimumWave = 1;
        public const int MaximumWave = 100;

        public const float MinimumSpawnRate = 0.05f;
        public const float MaximumSpawnRate = 1f;

        public const float MinimumMultiplier = 0.5f;
        public const float MaximumMultiplier = 2f;

        public const float MinimumPacing = 0.5f;
        public const float MaximumPacing = 2f;

        public const float MinimumHealth = 0.5f;
        public const float MaximumHealth = 2f;

        public const float MinimumSpeed = 0.5f;
        public const float MaximumSpeed = 2f;

        public const float MinimumDamage = 0.5f;
        public const float MaximumDamage = 2f;

        public const float MinimumCount = 0.5f;
        public const float MaximumCount = 2f;

        public enum DifficultyLevel
        {
            Easy,
            Normal,
            Hard,
            Elite,
            Nightmare
        }

        //===============================================================================================
        // LOCAL CLAMP
        //===============================================================================================
        private static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        //===============================================================================================
        // MULTIPLIER INDEXER
        //===============================================================================================
        public class MultiplierIndexerHelper
        {
            internal MultiplierIndexerHelper() { }

            public float this[int waveNumber]
            {
                get
                {
                    float raw = 1.0f + (waveNumber / 5.0f) * 0.1f;
                    return Clamp(raw, MinimumMultiplier, MaximumMultiplier);
                }
            }
        }

        public static readonly MultiplierIndexerHelper GetMultiplier = new MultiplierIndexerHelper();

        //===============================================================================================
        // FLOAT MULTIPLIER CURVE
        //===============================================================================================
        public static float UseMultiplier(int waveNumber)
        {
            if (waveNumber < MinimumWave) waveNumber = MinimumWave;
            if (waveNumber > MaximumWave) waveNumber = MaximumWave;

            float t = (waveNumber - MinimumWave) / (float)(MaximumWave - MinimumWave);
            float value = MinimumMultiplier + t * (MaximumMultiplier - MinimumMultiplier);

            return Clamp(value, MinimumMultiplier, MaximumMultiplier);
        }

        //===============================================================================================
        // INT MULTIPLIER (rounded)
        //===============================================================================================
        internal static int GetMultiplierInt(int waveNumber)
        {
            float f = UseMultiplier(waveNumber);
            int i = (int)System.Math.Round(f);
            return System.Math.Max(1, i);
        }

        //===============================================================================================
        // PROGRESSION STATE
        //===============================================================================================
        public DifficultyLevel Level { get; private set; }
        public float HealthMultiplier { get; private set; }
        public float SpeedMultiplier { get; private set; }
        public float DamageMultiplier { get; private set; }
        public float CountMultiplier { get; private set; }
        public float ChampionSpawnRate { get; private set; }
        public int WaveNumber { get; private set; }

        public DifficultyProgression() { }

        //===============================================================================================
        // SCALE TYPE
        //===============================================================================================
        public sealed class DifficultyScale
        {
            public float EnemyCountMultiplier { get; init; }
            public float SpawnRateMultiplier { get; init; }
            public float PacingMultiplier { get; init; }
        }

        //===============================================================================================
        // SCALING METHODS
        //===============================================================================================
        public static int GetScaledEnemyCount(
            int waveNumber,
            object enemyType,
            object difficulty,
            int baseCount,
            object config,
            float progression)
        {
            return (int)System.Math.Round(baseCount * progression);
        }

        public float GetScaledEnemyCount() => CountMultiplier * WaveNumber;

        //===============================================================================================
        // FACTORY METHOD
        //===============================================================================================
        public static DifficultyProgression ForWave(int waveNumber, DifficultyLevel baseLevel = DifficultyLevel.Normal)
        {
            var p = new DifficultyProgression
            {
                Level = baseLevel,
                WaveNumber = waveNumber
            };

            var baseMultipliers = baseLevel switch
            {
                DifficultyLevel.Easy => new { Health = 0.8f, Speed = 0.9f, Damage = 0.8f, Count = 0.8f, Champion = 0.05f },
                DifficultyLevel.Normal => new { Health = 1.0f, Speed = 1.0f, Damage = 1.0f, Count = 1.0f, Champion = 0.10f },
                DifficultyLevel.Hard => new { Health = 1.3f, Speed = 1.1f, Damage = 1.2f, Count = 1.2f, Champion = 0.15f },
                DifficultyLevel.Elite => new { Health = 1.6f, Speed = 1.2f, Damage = 1.4f, Count = 1.4f, Champion = 0.20f },
                DifficultyLevel.Nightmare => new { Health = 2.0f, Speed = 1.3f, Damage = 1.6f, Count = 1.6f, Champion = 0.25f },
                _ => new { Health = 1.0f, Speed = 1.0f, Damage = 1.0f, Count = 1.0f, Champion = 0.10f }
            };

            float wp = 1.0f + (waveNumber / 5.0f) * 0.1f;

            p.HealthMultiplier = Clamp(baseMultipliers.Health * wp, MinimumHealth, MaximumHealth);
            p.SpeedMultiplier = Clamp(baseMultipliers.Speed * wp, MinimumSpeed, MaximumSpeed);
            p.DamageMultiplier = Clamp(baseMultipliers.Damage * wp, MinimumDamage, MaximumDamage);
            p.CountMultiplier = Clamp(baseMultipliers.Count * wp, MinimumCount, MaximumCount);

            p.ChampionSpawnRate = Clamp(
                baseMultipliers.Champion + (waveNumber * 0.01f),
                MinimumSpawnRate,
                MaximumSpawnRate);

            return p;
        }

        public DifficultyProgression Clone()
        {
            return new DifficultyProgression
            {
                Level = this.Level,
                HealthMultiplier = this.HealthMultiplier,
                SpeedMultiplier = this.SpeedMultiplier,
                DamageMultiplier = this.DamageMultiplier,
                CountMultiplier = this.CountMultiplier,
                ChampionSpawnRate = this.ChampionSpawnRate,
                WaveNumber = this.WaveNumber
            };
        }

        public bool Validate()
        {
            return HealthMultiplier > 0 &&
                   SpeedMultiplier > 0 &&
                   DamageMultiplier > 0 &&
                   CountMultiplier > 0 &&
                   ChampionSpawnRate >= 0 &&
                   ChampionSpawnRate <= 1 &&
                   WaveNumber > 0;
        }

        //===============================================================================================
        // SCALE LOOKUP
        //===============================================================================================
        public DifficultyScale GetScaleForWave(int waveNumber)
        {
            return new DifficultyScale
            {
                EnemyCountMultiplier = CountMultiplier,
                SpawnRateMultiplier = SpeedMultiplier,
                PacingMultiplier = DamageMultiplier
            };
        }

        internal static int HasMultiplier(int waveNumber)
        {
            // If the wave is outside the configured progression range, there is no scaling — return neutral multiplier 1
            if (waveNumber < MinimumWave || waveNumber > MaximumWave)
                return 1;

            // Get the configured integer multiplier for the given wave
            int multiplier = GetMultiplierInt(waveNumber);

            // Ensure a non-zero, positive multiplier; fallback to 1 if configuration yields invalid values
            return multiplier > 0 ? multiplier : 1;
        }
    }
}
