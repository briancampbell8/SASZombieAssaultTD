/*
Program Name: SASZombieAssaultTD
File Path: Engine\Save\WaveProgress.cs
Purpose: Wave progress data structure for save/load system.
Features: P100 Integration for wave-specific progress tracking.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Save
{
    /// <summary>
    /// Wave progress information.
    /// P140-04: P100 Integration for wave-specific progress tracking.
    /// </summary>
    public class WaveProgress
    {
        public int WaveNumber { get; set; }
        public bool Completed { get; set; }
        public int EnemiesKilled { get; set; }
        public int ChampionsDefeated { get; set; }
        public float TimeTaken { get; set; }
        public DateTime LastPlayed { get; set; }

        public WaveProgress(int waveNumber)
        {
            WaveNumber = waveNumber;
            Completed = false;
            EnemiesKilled = 0;
            ChampionsDefeated = 0;
            TimeTaken = 0f;
            LastPlayed = DateTime.UtcNow;

            Engine.Diagnostics.DebugLogger.Log("Debug", $"WaveProgress: Created for wave {waveNumber}");
        }

        /// <summary>
        /// Updates the wave progress.
        /// </summary>
        /// <param name="completed">Whether the wave was completed.</param>
        /// <param name="enemiesKilled">Number of enemies killed.</param>
        /// <param name="championsDefeated">Number of champions defeated.</param>
        /// <param name="timeTaken">Time taken to complete the wave.</param>
        public void UpdateProgress(bool completed, int enemiesKilled, int championsDefeated, float timeTaken)
        {
            Completed = completed;
            EnemiesKilled = System.Math.Max(EnemiesKilled, enemiesKilled);
            ChampionsDefeated = System.Math.Max(ChampionsDefeated, championsDefeated);
            TimeTaken = timeTaken > 0 ? timeTaken : TimeTaken;
            LastPlayed = DateTime.UtcNow;

            Engine.Diagnostics.DebugLogger.Log(DiagnosticLevel.Info, "INFO", 
                $"WaveProgress: Updated wave {WaveNumber} - Completed={completed}, EnemiesKilled={EnemiesKilled}, ChampionsDefeated={ChampionsDefeated}");
        }

        /// <summary>
        /// Adds enemies killed to the total.
        /// </summary>
        /// <param name="enemiesKilled">Number of enemies killed to add.</param>
        public void AddEnemiesKilled(int enemiesKilled)
        {
            EnemiesKilled += System.Math.Max(0, enemiesKilled);
        }

        /// <summary>
        /// Adds champions defeated to the total.
        /// </summary>
        /// <param name="championsDefeated">Number of champions defeated to add.</param>
        public void AddChampionsDefeated(int championsDefeated)
        {
            ChampionsDefeated += System.Math.Max(0, championsDefeated);
        }

        /// <summary>
        /// Creates a clone of this wave progress.
        /// </summary>
        /// <returns>A new WaveProgress instance with the same values.</returns>
        public WaveProgress Clone()
        {
            return new WaveProgress(WaveNumber)
            {
                Completed = Completed,
                EnemiesKilled = EnemiesKilled,
                ChampionsDefeated = ChampionsDefeated,
                TimeTaken = TimeTaken,
                LastPlayed = LastPlayed
            };
        }

        /// <summary>
        /// Gets wave progress information as a string.
        /// </summary>
        public override string ToString()
        {
            return $"WaveProgress: Wave={WaveNumber}, Completed={Completed}, " +
                   $"EnemiesKilled={EnemiesKilled}, ChampionsDefeated={ChampionsDefeated}, " +
                   $"TimeTaken={TimeTaken:F1}s";
        }
    }
}
