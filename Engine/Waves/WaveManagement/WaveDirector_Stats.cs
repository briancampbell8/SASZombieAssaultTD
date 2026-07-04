/* ====================================================================================================
 *  FILE: WaveDirector_Stats.cs
 *  PATH: Engine/Waves/WaveManagement/WaveDirector_Stats.cs
 *  SUBSYSTEM: Waves
 *  ROLE: Wave statistics, progress calculations, and enemy count helpers.
 *
 *  RESPONSIBILITIES:
 *      - Provide wave progress (0–1) and overall progress (0–1).
 *      - Provide completed wave count.
 *      - Provide wave statistics for UI or analytics.
 *      - Provide internal enemy count helpers.
 *
 *  NON-RESPONSIBILITIES:
 *      - Wave lifecycle control (handled by WaveDirector_WaveFlow.cs).
 *      - Spawning logic (handled by WaveDirector_Spawning.cs).
 *      - Notifications and rewards (handled by WaveDirector_Notifications.cs).
 *      - Script loading (handled by WaveDirector_Initialization.cs).
 *
 *  ARCHITECTURAL NOTES:
 *      - All stats are internal and deterministic.
 *      - Must not depend on UI or external systems.
 * ==================================================================================================== */

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Enemies;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public partial class WaveDirector
    {
        //===============================================================================================
        // WAVE STATS OBJECT
        //===============================================================================================

        ///<summary>
        ///Represents statistics for a single wave.
        ///</summary>
        public class WaveStats
        {
            public int WaveNumber { get; set; }
            public int TotalEnemies { get; set; }
            public int AliveEnemies { get; set; }
            public int DefeatedEnemies => TotalEnemies - AliveEnemies;
            public float Progress => TotalEnemies == 0 ? 1f : (float)DefeatedEnemies / TotalEnemies;
        }

        //===============================================================================================
        // PUBLIC-FACING INTERNAL STATS ACCESSORS
        //===============================================================================================

        ///<summary>
        ///Returns statistics for the current wave.
        ///</summary>
        internal WaveStats GetWaveStats_Internal()
        {
            if (_currentWave == null)
            {
                return new WaveStats
                {
                    WaveNumber = 0,
                    TotalEnemies = 0,
                    AliveEnemies = 0
                };
            }

            int total = GetTotalEnemiesForWave_Internal(_currentWave);
            int alive = GetAliveEnemiesForWave_Internal(_currentWaveNumber);

            return new WaveStats
            {
                WaveNumber = _currentWaveNumber,
                TotalEnemies = total,
                AliveEnemies = alive
            };
        }

        ///<summary>
        ///Returns the number of completed waves.
        ///</summary>
        internal int GetCompletedWaves_Internal()
        {
            if (_currentWaveNumber == 0)
                return 0;

            if (_currentState == WaveState.Complete)
                return _totalWaves;

            return System.Math.Max(0, _currentWaveNumber - (_currentState == WaveState.InProgress ? 1 : 0));
        }

        ///<summary>
        ///Returns the progress of the current wave (0–1).
        ///</summary>
        internal float GetWaveProgress_Internal()
        {
            var stats = GetWaveStats_Internal();
            return stats.Progress;
        }

        ///<summary>
        ///Returns the overall progress across all waves (0–1).
        ///</summary>
        internal float GetOverallProgress_Internal()
        {
            if (_totalWaves == 0)
                return 0f;

            float completed = GetCompletedWaves_Internal();
            float currentWaveProgress = GetWaveProgress_Internal();

            return (completed + currentWaveProgress) / _totalWaves;
        }

        //===============================================================================================
        // ENEMY COUNT HELPERS
        //===============================================================================================

        ///<summary>
        ///Returns the total number of enemies defined in a wave script.
        ///</summary>
        internal int GetTotalEnemiesForWave_Internal(WaveScript script)
        {
            if (script == null)
                return 0;

            int total = 0;

            foreach (var group in script.SpawnGroups)
                total += ApplyDifficultyMultiplier_Internal(group.Count, script);

            return total;
        }

        ///<summary>
        ///Returns the number of alive enemies belonging to a specific wave.
        ///</summary>
        internal int GetAliveEnemiesForWave_Internal(int waveNumber)
        {
            //TODO: Wire to EnemyManager when available.
            EnemyManager enemyManager = null; //EnemyManager.Instance;

            if (enemyManager == null)
                return 0;

            int alive = 0;

            foreach (var enemy in enemyManager.GetAllEnemies())
            {
                if (enemy.SourceWave == waveNumber && enemy.IsActive)
                    alive++;
            }

            return alive;
        }
    }
}
