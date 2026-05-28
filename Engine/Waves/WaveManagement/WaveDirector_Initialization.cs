/* ====================================================================================================
 *  FILE: WaveDirector_Initialization.cs
 *  PATH: Engine/Waves/WaveManagement/WaveDirector_Initialization.cs
 *  SUBSYSTEM: Waves
 *  ROLE: Initialization and wave script loading for the WaveDirector subsystem.
 *
 *  RESPONSIBILITIES:
 *      - Load wave scripts from WaveLoader.
 *      - Initialize the wave queue in deterministic order.
 *      - Create fallback/default wave scripts if loading fails.
 *      - Provide internal initialization entry point for the façade.
 *
 *  NON-RESPONSIBILITIES:
 *      - Wave lifecycle control (handled by WaveDirector_WaveFlow.cs).
 *      - Enemy spawning (handled by WaveDirector_Spawning.cs).
 *      - Notifications and rewards (handled by WaveDirector_Notifications.cs).
 *      - Stats and progress calculations (handled by WaveDirector_Stats.cs).
 *
 *  ARCHITECTURAL NOTES:
 *      - Must remain deterministic and free of gameplay logic.
 *      - Must not expose public fields or modify external systems.
 * ==================================================================================================== */

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public partial class WaveDirector
    {
        // ===============================================================================================
        //  INITIALIZATION ENTRY POINT
        // ===============================================================================================

        /// <summary>
        /// Internal implementation for Initialize().
        /// Loads wave scripts and prepares the wave queue.
        /// </summary>
        internal void Initialize_Internal()
        {
            if (_isInitialized)
                return;

            System.Diagnostics.Debug.WriteLine("Initializing Wave Director");

            try
            {
                LoadWaveScripts_Internal();
                InitializeWaveQueue_Internal();

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine($"Wave Director initialized with {_totalWaves} waves");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize Wave Director: {ex.Message}");
                throw;
            }
        }

        // ===============================================================================================
        //  LOAD WAVE SCRIPTS
        // ===============================================================================================

        /// <summary>
        /// Loads all wave scripts using WaveLoader.
        /// Falls back to default scripts if loading fails.
        /// </summary>
        internal void LoadWaveScripts_Internal()
        {
            try
            {
                WaveLoader loader = null;
                var scripts = loader.LoadAllWaveScripts();

                _waveScripts.Clear();

                foreach (var script in scripts)
                    _waveScripts[script.WaveNumber] = script;

                _totalWaves = _waveScripts.Count;

                System.Diagnostics.Debug.WriteLine($"Loaded {_totalWaves} wave scripts");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading wave scripts: {ex.Message}");
                CreateDefaultWaveScripts_Internal();
            }
        }

        // ===============================================================================================
        //  INITIALIZE WAVE QUEUE
        // ===============================================================================================

        /// <summary>
        /// Builds the wave queue in ascending wave order.
        /// </summary>
        internal void InitializeWaveQueue_Internal()
        {
            _upcomingWaves.Clear();

            for (int i = 1; i <= _totalWaves; i++)
            {
                if (_waveScripts.TryGetValue(i, out var script))
                {
                    _upcomingWaves.Enqueue(script);
                }
            }
        }

        // ===============================================================================================
        //  DEFAULT WAVE SCRIPT GENERATION
        // ===============================================================================================

        /// <summary>
        /// Creates a full set of default wave scripts when loading fails.
        /// </summary>
        internal void CreateDefaultWaveScripts_Internal()
        {
            System.Diagnostics.Debug.WriteLine("Creating default wave scripts");

            _waveScripts.Clear();

            for (int i = 1; i <= 10; i++)
            {
                var waveScript = CreateDefaultWaveScript_Internal(i);
                _waveScripts[i] = waveScript;
            }

            _totalWaves = _waveScripts.Count;
        }

        /// <summary>
        /// Creates a single default wave script with simple scaling.
        /// </summary>
        internal WaveScript CreateDefaultWaveScript_Internal(int waveNumber)
        {
            var waveScript = new WaveScript
            {
                WaveNumber = waveNumber,
                InterWaveDelay = 10f,
                DifficultyMultiplier = new DifficultyMultiplier(
                    DifficultyMultiplier.Instance.GetMultiplier(DifficultyMode.Normal))
            };

            // Basic enemy scaling: 5 + (2 × waveNumber)
            var enemyCount = 5 + (waveNumber * 2);

            var spawnGroup = new WaveSpawnGroup
            {
                EnemyType = (WaveSpawnGroup.ZombieType)SASZombieAssaultTD.Engine.Enemies.ZombieType.Swarm,
                Count = enemyCount,
                SpawnDelay = 0.5f,
                Pattern = SpawnPatternType.Line
            };

            waveScript.SpawnGroups.Add(spawnGroup);

            return waveScript;
        }
    }
}
