// ====================================================================================================
// FILE: WaveDirectorInitialization.cs
// PATH: Engine/Waves/WaveManagement/WaveDirectorInitialization.cs
// SUBSYSTEM: Waves
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Save.GameSave;
using SASZombieAssaultTD.Engine.Waves.Difficulty;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public class WaveDirectorInitialization
    {
        private readonly WaveDirectorCore _core = WaveDirectorCore.Instance;

        //===============================================================================================
        // INITIALIZATION ENTRY POINT
        //===============================================================================================

        internal void Initialize()
        {
            if (_core._isInitialized)
                return;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Initializing Wave Director");

            try
            {
                LoadWaveScripts();
                InitializeWaveQueue();

                _core._isInitialized = true;
                DLogger.Log($"Wave Director initialized with {_core._totalWaves} waves");
            }
            catch (Exception ex)
            {
                DLogger.Log($"Failed to initialize Wave Director: {ex.Message}");
                ResetStateOnFailure();
                throw;
            }
        }

        //===============================================================================================
        // LOAD WAVE SCRIPTS
        //===============================================================================================

        internal void LoadWaveScripts()
        {
            try
            {
                var loader = WaveLoader.Instance;
                var scripts = loader.LoadAllWaveScripts();

                _core._waveScripts.Clear();

                foreach (var script in scripts)
                    _core._waveScripts[script.WaveNumber] = script;

                _core._totalWaves = _core._waveScripts.Count;

                DLogger.Log($"Loaded {_core._totalWaves} wave scripts");
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error loading wave scripts: {ex.Message}. Falling back to default generation.");
                CreateDefaultWaveScripts();
            }
        }

        //===============================================================================================
        // INITIALIZE WAVE QUEUE
        //===============================================================================================

        internal void InitializeWaveQueue()
        {
            _core._upcomingWaves.Clear();

            for (int i = 1; i <= _core._totalWaves; i++)
            {
                if (_core._waveScripts.TryGetValue(i, out var script))
                    _core._upcomingWaves.Enqueue(script);
            }
        }

        //===============================================================================================
        // DEFAULT WAVE SCRIPT GENERATION
        //===============================================================================================

        internal void CreateDefaultWaveScripts()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Creating default wave scripts");

            _core._waveScripts.Clear();

            for (int i = 1; i <= 10; i++)
            {
                var waveScript = CreateDefaultWaveScript(i);
                _core._waveScripts[i] = waveScript;
            }

            _core._totalWaves = _core._waveScripts.Count;
        }

        internal WaveScript CreateDefaultWaveScript(int waveNumber)
        {
            // FIX: DifficultyManager is being returned as object — cast it.
            var difficulty = ((GSCore)_core.DifficultyManager).GetCurrentDifficulty();

            var config = DifficultyConfig.Get(difficulty);

            // FIX: Correct indexer syntax
            var progression = DifficultyProgression.GetMultiplier[waveNumber];

            var waveScript = new WaveScript
            {
                WaveNumber = waveNumber,
                InterWaveDelay = 10f,
                Modifiers = new WaveModifiers(),
                Rewards = new WaveRewards(),
                Environment = new WaveEnvironment()
            };

            var enemyCount = 5 + (waveNumber * 2);

            var spawnGroup = new WaveSpawnGroup
            {
                EnemyType = WaveSpawnGroup.ZombieType.Swarm,
                Count = enemyCount,
                SpawnDelay = 0.5f,
                Pattern = SpawnPatternType.Line
            };

            waveScript.SpawnGroups.Add(spawnGroup);

            DifficultyScaler.ApplyScaling(waveScript, config, progression);

            return waveScript;
        }

        //===============================================================================================
        // CLEANUP HELPERS
        //===============================================================================================

        private void ResetStateOnFailure()
        {
            _core._waveScripts.Clear();
            _core._upcomingWaves.Clear();
            _core._totalWaves = 0;
            _core._isInitialized = false;
        }
    }
}
