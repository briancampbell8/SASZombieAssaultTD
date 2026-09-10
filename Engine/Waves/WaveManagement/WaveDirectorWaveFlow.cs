// =====================================================================================================
//  FILE: WaveDirectorWaveFlow.cs
//  PATH: Engine/Waves/WaveManagement/WaveDirectorWaveFlow.cs
//  SUBSYSTEM: Waves WaveManagement
//
//  ROLE:
//      Stateless wave lifecycle and flow coordinator. Drives wave progression, transitions,
//      and completion logic using pure context-driven state evaluation.
//
//  RESPONSIBILITIES:
//      - Coordinate wave start, progression, inter-wave timing, and completion.
//      - Evaluate enemy states to determine wave completion.
//      - Trigger callbacks for wave lifecycle events.
//      - Maintain deterministic, stateless flow logic.
//
//  NON-RESPONSIBILITIES:
//      - Spawning enemies (WaveDirectorSpawning handles spawning).
//      - Difficulty scaling (DifficultyProgression / DifficultyScaler handle scaling).
//      - Script loading (WaveDirectorInitialization handles loading).
//
//  ARCHITECTURAL NOTES:
//      - Pure flow coordinator: no persistent internal fields.
//      - Uses EnemyManagerProvider static class to query active enemies.
//      - WaveDirectorContext provides all runtime state and bindings.
// =====================================================================================================

using System;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Enemies;   // Needed for IEnemyManager
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public class WaveDirectorWorkflow
    {
        // Removed incorrect property:
        // public object EnemyManagerProvider { get; private set; }

        // ===============================================================================================
        // GAME START
        // ===============================================================================================
        internal void StartGame_Internal(WaveDirectorContext context)
        {
            if (!context.IsInitialized)
                context.OnInitializeInternal?.Invoke();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Starting wave game");

            context.CurrentState = WaveState.WaitingToStart;
            StartNextWave_Internal(context);
        }

        // ===============================================================================================
        // START WAVE
        // ===============================================================================================
        internal async Task<bool> StartWave_Internal(WaveDirectorContext context, int waveNumber)
        {
            if (!context.IsInitialized || waveNumber <= 0 || waveNumber > context.TotalWaves)
            {
                DLogger.Log($"Invalid wave number: {waveNumber}");
                return false;
            }

            try
            {
                if (!context.WaveScripts.TryGetValue(waveNumber, out var waveScript))
                {
                    DLogger.Log($"No wave script found for wave {waveNumber}");
                    return false;
                }

                DLogger.Log($"Starting wave {waveNumber}");

                context.CurrentWave = waveScript;
                context.CurrentWaveNumber = waveNumber;
                context.CurrentState = WaveState.Starting;
                context.WaveTimer = 0f;
                context.SpawnTimer = 0f;

                context.OnInvokeWaveStarted?.Invoke(waveNumber);

                if (context.OnStartWaveSpawningInternal != null)
                    await context.OnStartWaveSpawningInternal(waveScript);

                context.CurrentState = WaveState.InProgress;

                DLogger.Log($"Wave {waveNumber} started successfully");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log($"Failed to start wave {waveNumber}: {ex.Message}");
                return false;
            }
        }

        // ===============================================================================================
        // START NEXT WAVE
        // ===============================================================================================
        internal void StartNextWave_Internal(WaveDirectorContext context)
        {
            if (context.CurrentState == WaveState.InProgress)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Cannot start next wave while current wave is in progress");
                return;
            }

            var nextWaveNumber = context.CurrentWaveNumber + 1;

            if (nextWaveNumber > context.TotalWaves)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "All waves completed");
                CompleteGame_Internal(context);
                return;
            }

            _ = StartWave_Internal(context, nextWaveNumber);
        }

        // ===============================================================================================
        // EARLY START
        // ===============================================================================================
        internal void StartNextWaveEarly_Internal(WaveDirectorContext context)
        {
            if (context.CurrentState != WaveState.InProgress)
            {
                StartNextWave_Internal(context);
                return;
            }

            if (AreAllWaveEnemiesDefeated_Internal(context))
            {
                CompleteCurrentWave_Internal(context);
                StartNextWave_Internal(context);
            }
            else
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Cannot start next wave early - enemies still active");
            }
        }

        // ===============================================================================================
        // UPDATE
        // ===============================================================================================
        internal void Update_Internal(WaveDirectorContext context, float deltaTime)
        {
            if (!context.IsInitialized || context.IsPaused)
                return;

            try
            {
                UpdateState_Internal(context, deltaTime);
                context.OnInvokeWaveProgressUpdated?.Invoke();
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error updating Wave Director: {ex.Message}");
            }
        }

        private void UpdateState_Internal(WaveDirectorContext context, float deltaTime)
        {
            switch (context.CurrentState)
            {
                case WaveState.Starting:
                    context.OnUpdateWaveStartingInternal?.Invoke();
                    break;

                case WaveState.InProgress:
                    UpdateWaveInProgress_Internal(context, deltaTime);
                    break;

                case WaveState.InterWave:
                    UpdateInterWave_Internal(context, deltaTime);
                    break;

                case WaveState.WaitingToStart:
                    context.OnUpdateWaitingToStartInternal?.Invoke();
                    break;

                case WaveState.Complete:
                    context.OnUpdateCompleteGameInternal?.Invoke();
                    break;
            }
        }

        // ===============================================================================================
        // PAUSE / RESUME / STOP / RESET
        // ===============================================================================================
        internal void Pause_Internal(WaveDirectorContext context)
        {
            context.IsPaused = true;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Wave Director paused");
        }

        internal void Resume_Internal(WaveDirectorContext context)
        {
            context.IsPaused = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Wave Director resumed");
        }

        internal void Stop_Internal(WaveDirectorContext context)
        {
            context.IsPaused = true;
            context.CurrentState = WaveState.Stopped;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Wave Director stopped");
        }

        internal void Reset_Internal(WaveDirectorContext context)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Resetting Wave Director");

            context.CurrentWave = null;
            context.CurrentWaveNumber = 0;
            context.CurrentState = WaveState.NotStarted;

            context.OnResetFlags?.Invoke();
            context.OnResetTimers?.Invoke();

            context.UpcomingWaves.Clear();
            context.OnInitializeWaveQueueInternal?.Invoke();

            // FIX: Cast to IEnemyManager
            var enemyService = EnemyManagerProvider.GetActiveManager() as IEnemyManager;
            enemyService?.ClearAllEnemies();
        }

        // ===============================================================================================
        // WAVE PROGRESS
        // ===============================================================================================
        internal void UpdateWaveInProgress_Internal(WaveDirectorContext context, float deltaTime)
        {
            context.WaveTimer += deltaTime;

            if (AreAllWaveEnemiesDefeated_Internal(context) && context.CurrentWave != null)
                CompleteCurrentWave_Internal(context);
        }

        internal void UpdateInterWave_Internal(WaveDirectorContext context, float deltaTime)
        {
            context.InterWaveTimer += deltaTime;

            float interWaveDelay = context.GetInterWaveDelay();

            if (context.InterWaveTimer >= interWaveDelay)
                StartNextWave_Internal(context);
        }

        // ===============================================================================================
        // WAVE COMPLETE
        // ===============================================================================================
        internal void CompleteCurrentWave_Internal(WaveDirectorContext context)
        {
            if (context.CurrentState != WaveState.InProgress)
                return;

            DLogger.Log($"Wave {context.CurrentWaveNumber} completed");

            context.CurrentState = WaveState.InterWave;
            context.InterWaveTimer = 0f;

            ModernPlaySound.Play("wave_complete");
            context.OnInvokeWaveCompleted?.Invoke(context.CurrentWaveNumber);

            if (context.CurrentWaveNumber >= context.TotalWaves)
                CompleteGame_Internal(context);
        }

        internal void CompleteGame_Internal(WaveDirectorContext context)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "All waves completed - Game complete!");

            context.IsGameComplete = true;
            context.CurrentState = WaveState.Complete;

            context.OnInvokeAllWavesCompleted?.Invoke();
            context.OnInvokeGameComplete?.Invoke();

            ModernPlaySound.Play("victory");
        }

        // ===============================================================================================
        // ENEMY CHECK
        // ===============================================================================================
        internal bool AreAllWaveEnemiesDefeated_Internal(WaveDirectorContext context)
        {
            if (context.CurrentState != WaveState.InProgress)
                return true;

            // FIX: Cast to IEnemyManager
            var enemyManager = EnemyManagerProvider.GetActiveManager() as IEnemyManager;
            if (enemyManager == null)
                return true;

            var activeEnemies = enemyManager.GetAllEnemies();
            if (activeEnemies == null)
                return true;

            foreach (var enemy in activeEnemies)
            {
                if (enemy.SourceWave == context.CurrentWaveNumber && enemy.IsActive)
                    return false;
            }

            return true;
        }
    }
}
