// ====================================================================================================
//  FILE: DifficultyScaling.cs
//  PATH: Engine/Waves/
//  MODULE: Wave System (Difficulty Scaling)
//
//  ROLE:
//      Encapsulates difficulty progression logic and multipliers for waves and enemies.
//
//  RESPONSIBILITIES:
//      - Provide DifficultyLevel enum and maintain current scaling factors.
//      - Compute health/armor/damage multipliers based on wave progression.
//
//  NON-RESPONSIBILITIES:
//      - Directly spawning enemies (WaveDirector handles spawning).
//
//  ARCHITECTURAL NOTES:
//      - Keep calculations deterministic and testable.
// ====================================================================================================

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Waves
{
    ///<summary>
    ///Difficulty scaling system for wave progression.
    ///P100-01: Difficulty scaling implementation
    ///</summary>
    public class DifficultyScaling
    {
        ///<summary>
        ///Difficulty levels available in the game.
        ///</summary>
        public enum DifficultyLevel
        {
            Easy,
            Normal,
            Hard,
            Elite,
            Nightmare
        }

        ///<summary>
        ///Current difficulty level.
        ///</summary>
        public DifficultyLevel Level { get; private set; }

        ///<summary>
        ///Health multiplier for enemies.
        ///</summary>
        public float HealthMultiplier { get; private set; }

        ///<summary>
        ///Speed multiplier for enemies.
        ///</summary>
        public float SpeedMultiplier { get; private set; }

        ///<summary>
        ///Damage multiplier for enemies.
        ///</summary>
        public float DamageMultiplier { get; private set; }

        ///<summary>
        ///Enemy count multiplier.
        ///</summary>
        public float CountMultiplier { get; private set; }

        ///<summary>
        ///Champion spawn rate (0.0 to 1.0).
        ///</summary>
        public float ChampionSpawnRate { get; private set; }

        ///<summary>
        ///Wave number this scaling is for.
        ///</summary>
        public int WaveNumber { get; private set; }

        private DifficultyScaling()
        {
        }

        ///<summary>
        ///Get difficulty scaling for a specific wave and base difficulty.
        ///</summary>
        ///<param name="waveNumber">Current wave number.</param>
        ///<param name="baseLevel">Base difficulty level.</param>
        ///<returns>Difficulty scaling configuration.</returns>
        public static DifficultyScaling GetForWave(int waveNumber, DifficultyLevel baseLevel = DifficultyLevel.Normal)
        {
            var scaling = new DifficultyScaling
            {
                Level = baseLevel,
                WaveNumber = waveNumber
            };

            //Base multipliers per difficulty level
            var baseMultipliers = baseLevel switch
            {
                DifficultyLevel.Easy => new { Health = 0.8f, Speed = 0.9f, Damage = 0.8f, Count = 0.8f, Champion = 0.05f },
                DifficultyLevel.Normal => new { Health = 1.0f, Speed = 1.0f, Damage = 1.0f, Count = 1.0f, Champion = 0.1f },
                DifficultyLevel.Hard => new { Health = 1.3f, Speed = 1.1f, Damage = 1.2f, Count = 1.2f, Champion = 0.15f },
                DifficultyLevel.Elite => new { Health = 1.6f, Speed = 1.2f, Damage = 1.4f, Count = 1.4f, Champion = 0.2f },
                DifficultyLevel.Nightmare => new { Health = 2.0f, Speed = 1.3f, Damage = 1.6f, Count = 1.6f, Champion = 0.25f },
                _ => new { Health = 1.0f, Speed = 1.0f, Damage = 1.0f, Count = 1.0f, Champion = 0.1f }
            };

            //Wave-based progression (increases every 5 waves)
            var waveProgression = 1.0f + (waveNumber / 5.0f) * 0.1f;

            scaling.HealthMultiplier = baseMultipliers.Health * waveProgression;
            scaling.SpeedMultiplier = baseMultipliers.Speed * waveProgression;
            scaling.DamageMultiplier = baseMultipliers.Damage * waveProgression;
            scaling.CountMultiplier = baseMultipliers.Count * waveProgression;
            scaling.ChampionSpawnRate = System.Math.Clamp(baseMultipliers.Champion + (waveNumber * 0.01f), 0f, 0.5f);

            return scaling;
        }

        ///<summary>
        ///Get current difficulty scaling (singleton pattern).
        ///</summary>
        ///<returns>Current difficulty scaling.</returns>
        public static DifficultyScaling GetCurrent()
        {
            //This would typically be set by the game state
            //For now, return a default Normal difficulty for wave 1
            return GetForWave(1, DifficultyLevel.Normal);
        }

        ///<summary>
        ///Get difficulty level as string.
        ///</summary>
        ///<returns>Difficulty level name.</returns>
        public string GetLevelName()
        {
            return Level.ToString();
        }

        ///<summary>
        ///Clone this difficulty scaling.
        ///</summary>
        ///<returns>Cloned difficulty scaling.</returns>
        public DifficultyScaling Clone()
        {
            return new DifficultyScaling
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

        ///<summary>
        ///Validate difficulty scaling configuration.
        ///</summary>
        ///<returns>True if valid.</returns>
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
    }
}
