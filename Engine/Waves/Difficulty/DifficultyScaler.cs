// =====================================================================================================
//  FILE: DifficultyScaler.cs
//  PATH: Engine/Waves/Difficulty/DifficultyScaler.cs
//  SUBSYSTEM: Waves/Difficulty Subsystem
//
//  ROLE:
//      Applies deterministic difficulty scaling rules to wave definitions.
//      Consumes DifficultySettings and produces scaled wave data without side effects.
//      Acts as the pure functional transformation layer between DifficultyManager and WaveManagement.
//
//  RESPONSIBILITIES:
//      - Apply enemy count multipliers.
//      - Apply spawn rate multipliers.
//      - Apply wave pacing multipliers.
//      - Produce a new, scaled WaveDefinition instance deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Storing difficulty state (DifficultyManager handles state).
//      - Providing difficulty configuration (DifficultyConfig handles configuration).
//      - Executing wave logic or spawning behavior.
//      - Performing runtime progression calculations.
//
//  ARCHITECTURAL NOTES:
//      - This module is stateless and purely functional.
//      - WaveManagement calls this module through IDifficultyProvider.
//      - All scaling rules must remain deterministic and testable.
// =====================================================================================================

using static SASZombieAssaultTD.Engine.Waves.Difficulty.DifficultyConfig;

namespace SASZombieAssaultTD.Engine.Waves.Difficulty
{
    public static class DifficultyScaler
    {
        public static WaveDefinition ApplyScaling(WaveDefinition wave, DifficultySettings settings)
        {
            var scaled = wave.Clone();

            scaled.EnemyCount = (int)(scaled.EnemyCount * settings.EnemyCountMultiplier);
            scaled.SpawnRate = scaled.SpawnRate * settings.SpawnRateMultiplier;
            scaled.Pacing = scaled.Pacing * settings.WavePacingMultiplier;

            return scaled;
        }

        internal static void ApplyScaling(WaveScript waveScript, DifficultySettings config, object progression)
        {
            if (waveScript == null || config == null)
                return;

            // WaveScript.Definition must be WaveDefinition, not object
            if (waveScript.Definition is not WaveDefinition definition)
                return;

            // Apply global difficulty multipliers
            definition.EnemyCount = (int)(definition.EnemyCount * config.EnemyCountMultiplier);
            definition.SpawnRate = definition.SpawnRate * config.SpawnRateMultiplier;
            definition.Pacing = definition.Pacing * config.WavePacingMultiplier;

            // Apply progression scaling if provided
            if (progression is DifficultyProgression prog)
            {
                // Your engine uses WaveNumber, not WaveIndex
                int waveNumber = waveScript.WaveNumber;

                // Your engine uses GetScaleForWave(), not GetWaveScale()
                var scale = prog.GetScaleForWave(waveNumber);

                definition.EnemyCount = (int)(definition.EnemyCount * scale.EnemyCountMultiplier);
                definition.SpawnRate = definition.SpawnRate * scale.SpawnRateMultiplier;
                definition.Pacing = definition.Pacing * scale.PacingMultiplier;
            }
        }
    }

    public class WaveDefinition
    {
        public int EnemyCount { get; set; }
        public float SpawnRate { get; set; }
        public float Pacing { get; set; }

        public WaveDefinition Clone()
        {
            return new WaveDefinition
            {
                EnemyCount = this.EnemyCount,
                SpawnRate = this.SpawnRate,
                Pacing = this.Pacing
            };
        }
    }
}
