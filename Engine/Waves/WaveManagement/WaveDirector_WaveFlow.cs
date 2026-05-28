/* ====================================================================================================
 *  FILE: WaveDirector_WaveFlow.cs
 *  PATH: Engine/Waves/WaveManagement/WaveDirector_WaveFlow.cs
 *  SUBSYSTEM: Waves
 *  ROLE: Wave lifecycle and flow control for the WaveDirector subsystem.
 *
 *  RESPONSIBILITIES:
 *      - Manage wave lifecycle: start, progress, completion, and game completion.
 *      - Coordinate transitions between wave states.
 *      - Drive per-frame update of the wave system.
 *      - Determine when enemies are cleared and waves can advance.
 *
 *  NON-RESPONSIBILITIES:
 *      - Internal state storage (handled by WaveDirector_Core.cs).
 *      - Wave script loading (handled by WaveDirector_Initialization.cs).
 *      - Enemy spawning (handled by WaveDirector_Spawning.cs).
 *      - Notifications and rewards (handled by WaveDirector_Notifications.cs).
 *      - Stats and progress calculations (handled by WaveDirector_Stats.cs).
 *
 *  ARCHITECTURAL NOTES:
 *      - All public API entry points are exposed via the façade and delegate into these internals.
 *      - This file must not declare public fields or expose internal state directly.
 * ==================================================================================================== */

using System;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Enemies;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public partial class WaveDirector
    {
        // ===============================================================================================
        //  PUBLIC API DELEGATES (CALLED BY FAÇADE)
        // ===============================================================================================

        /// <summary>
        /// Internal implementation for StartGame().
        /// </summary>
        internal void StartGame_Internal()
        {
            if (!_isInitialized)
            {
                Initialize_Internal();
            }

            System.Diagnostics.Debug.WriteLine("Starting wave game");
            _currentState = WaveState.WaitingToStart;
            StartNextWave_Internal();
        }

        /// <summary>
        /// Internal implementation for StartWave(int).
        /// </summary>
        internal async Task<bool> StartWave_Internal(int waveNumber)
        {
            if (!_isInitialized || waveNumber <= 0 || waveNumber > _totalWaves)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid wave number: {waveNumber}");
                return false;
            }

            try
            {
                if (!_waveScripts.TryGetValue(waveNumber, out var waveScript))
                {
                    System.Diagnostics.Debug.WriteLine($"No wave script found for wave {waveNumber}");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Starting wave {waveNumber}");

                _currentWave = waveScript;
                _currentWaveNumber = waveNumber;
                _currentState = WaveState.Starting;
                _waveTimer = 0f;
                _spawnTimer = 0f;

                InvokeWaveStarted(waveNumber);

                await StartWaveSpawning_Internal(waveScript);

                _currentState = WaveState.InProgress;
                System.Diagnostics.Debug.WriteLine($"Wave {waveNumber} started successfully");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to start wave {waveNumber}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Internal implementation for StartNextWave().
        /// </summary>
        internal void StartNextWave_Internal()
        {
            if (_currentState == WaveState.InProgress)
            {
                System.Diagnostics.Debug.WriteLine("Cannot start next wave while current wave is in progress");
                return;
            }

            var nextWaveNumber = _currentWaveNumber + 1;

            if (nextWaveNumber > _totalWaves)
            {
                System.Diagnostics.Debug.WriteLine("All waves completed");
                CompleteGame_Internal();
                return;
            }

            _ = StartWave_Internal(nextWaveNumber);
        }

        /// <summary>
        /// Internal implementation for StartNextWaveEarly().
        /// </summary>
        internal void StartNextWaveEarly_Internal()
        {
            if (_currentState != WaveState.InProgress)
            {
                StartNextWave_Internal();
                return;
            }

            if (AreAllWaveEnemiesDefeated_Internal())
            {
                CompleteCurrentWave_Internal();
                StartNextWave_Internal();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Cannot start next wave early - enemies still active");
            }
        }

        /// <summary>
        /// Internal implementation for Update(float).
        /// </summary>
        internal void Update_Internal(float deltaTime)
        {
            if (!_isInitialized || _isPaused)
                return;

            try
            {
                switch (_currentState)
                {
                    case WaveState.InProgress:
                        UpdateWaveInProgress_Internal(deltaTime);
                        break;

                    case WaveState.InterWave:
                        UpdateInterWave_Internal(deltaTime);
                        break;

                    case WaveState.WaitingToStart:
                        // Waiting for manual or automatic start.
                        break;
                }

                InvokeWaveProgressUpdated(WaveProgress);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating Wave Director: {ex.Message}");
            }
        }

        /// <summary>
        /// Internal implementation for Pause().
        /// </summary>
        internal void Pause_Internal()
        {
            _isPaused = true;
            System.Diagnostics.Debug.WriteLine("Wave Director paused");
        }

        /// <summary>
        /// Internal implementation for Resume().
        /// </summary>
        internal void Resume_Internal()
        {
            _isPaused = false;
            System.Diagnostics.Debug.WriteLine("Wave Director resumed");
        }

        /// <summary>
        /// Internal implementation for Stop().
        /// </summary>
        internal void Stop_Internal()
        {
            _isPaused = true;
            _currentState = WaveState.Stopped;
            System.Diagnostics.Debug.WriteLine("Wave Director stopped");
        }

        /// <summary>
        /// Internal implementation for Reset().
        /// </summary>
        internal void Reset_Internal()
        {
            System.Diagnostics.Debug.WriteLine("Resetting Wave Director");

            _currentWave = null;
            _currentWaveNumber = 0;
            _currentState = WaveState.NotStarted;
            ResetFlags();
            ResetTimers();
            _instance = null;

            _upcomingWaves.Clear();
            InitializeWaveQueue_Internal();

            // TODO: Integrate with EnemyManager when available.
            // EnemyManager.Instance?.ClearAllEnemies();
        }

        // ===============================================================================================
        //  WAVE PROGRESSION
        // ===============================================================================================

        /// <summary>
        /// Updates the current wave while it is in progress.
        /// </summary>
        internal void UpdateWaveInProgress_Internal(float deltaTime)
        {
            _waveTimer += deltaTime;

            if (AreAllWaveEnemiesDefeated_Internal() && _currentWave != null)
            {
                CompleteCurrentWave_Internal();
            }
        }

        /// <summary>
        /// Updates the inter-wave period.
        /// </summary>
        internal void UpdateInterWave_Internal(float deltaTime)
        {
            _interWaveTimer += deltaTime;

            var interWaveDelay = _currentWave?.InterWaveDelay ?? 10f;

            if (_interWaveTimer >= interWaveDelay)
            {
                StartNextWave_Internal();
            }
        }

        /// <summary>
        /// Completes the current wave and transitions to inter-wave state.
        /// </summary>
        internal void CompleteCurrentWave_Internal()
        {
            if (_currentState != WaveState.InProgress)
                return;

            System.Diagnostics.Debug.WriteLine($"Wave {_currentWaveNumber} completed");

            _currentState = WaveState.InterWave;
            _interWaveTimer = 0f;

            ModernPlaySound.Play("wave_complete");

            ShowWaveCompleteNotification_Internal(_currentWave);
            AwardWaveCompletionBonus_Internal();

            InvokeWaveCompleted(_currentWaveNumber);

            if (_currentWaveNumber >= _totalWaves)
            {
                CompleteGame_Internal();
            }
        }

        /// <summary>
        /// Completes the entire game and fires completion events.
        /// </summary>
        internal void CompleteGame_Internal()
        {
            System.Diagnostics.Debug.WriteLine("All waves completed - Game complete!");

            _isGameComplete = true;
            _currentState = WaveState.Complete;

            InvokeAllWavesCompleted();
            InvokeGameComplete();

            ModernPlaySound.Play("victory");
        }

        // ===============================================================================================
        //  ENEMY STATE CHECKS
        // ===============================================================================================

        /// <summary>
        /// Determines whether all enemies from the current wave are defeated.
        /// </summary>
        internal bool AreAllWaveEnemiesDefeated_Internal()
        {
            if (_currentState != WaveState.InProgress)
                return true;

            // TODO: Wire to EnemyManager when available.
            EnemyManager enemyManager = null; // EnemyManager.Instance;

            if (enemyManager == null)
                return true;

            var activeEnemies = enemyManager.GetAllEnemies();

            foreach (var enemy in activeEnemies)
            {
                if (enemy.SourceWave == _currentWaveNumber && enemy.IsActive)
                    return false;
            }

            return true;
        }
    }
}
