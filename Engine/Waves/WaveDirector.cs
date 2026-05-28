/* ====================================================================================================
 *  FILE: WaveDirector.cs
 *  PATH: Engine/Waves/WaveDirector.cs
 *  SUBSYSTEM: Waves
 *  ROLE: Central orchestrator for wave sequencing, spawning, timing, and progression tracking.
 *
 *  RESPONSIBILITIES:
 *      - Manage the full lifecycle of waves (initialization, start, progress, completion).
 *      - Load and execute WaveScript definitions.
 *      - Spawn enemies according to wave patterns and difficulty multipliers.
 *      - Maintain deterministic timing for wave, inter-wave, and spawn cycles.
 *      - Dispatch wave-related events to UI, audio, gameplay, and progression systems.
 *      - Track wave progress, overall progress, and wave statistics.
 *
 *  NON-RESPONSIBILITIES:
 *      - Enemy AI or behavior logic (handled by EnemyManager).
 *      - Difficulty scaling rules (handled by DifficultyManager).
 *      - UI rendering or HUD logic (handled by HUDController).
 *      - Audio playback (handled by ModernPlaySound).
 *      - Progression logic (handled by ProgressionLogic).
 *
 *  DEPENDENCIES:
 *      - WaveScript, WaveSpawnGroup, IWaveSpawnGroup
 *      - EnemyManager
 *      - DifficultyManager
 *      - NavigationGrid
 *      - HUDController
 *      - ModernPlaySound
 *
 *  CALLED BY:
 *      - Game initialization systems
 *      - Gameplay loop (Update)
 *      - External systems requesting wave control (StartWave, StartNextWave)
 *
 *  CALLS INTO:
 *      - EnemyManager (spawn, query active enemies)
 *      - DifficultyManager (difficulty multipliers)
 *      - HUDController (wave notifications)
 *      - ModernPlaySound (audio cues)
 *
 *  ARCHITECTURAL NOTES:
 *      - This class is the authoritative wave controller; no other system may modify wave state.
 *      - All event invocations must use internal invoker methods to preserve encapsulation.
 *      - Must remain deterministic and free of UI or gameplay drift.
 *      - Must not contain serialization, DTOs, or external business logic.
 *
 * ==================================================================================================== */

using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Difficulty;
using SASZombieAssaultTD.Engine.Enemies;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.UI.HUD;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Diagnostics;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Waves;


namespace SASZombieAssaultTD.Engine.Waves
{
    /// <exclude />
    public class WaveDirector
    {
        readonly Dictionary<int, WaveScript> _waveScripts;
        readonly Queue<WaveScript> _upcomingWaves;
        WaveScript _currentWave;
        WaveState _currentState;
        int _currentWaveNumber;
        int _totalWaves;
        bool _isInitialized;
        bool _isPaused;
        bool _isGameComplete;

        // Timing
        float _waveTimer;
        float _interWaveTimer;
        float _spawnTimer;
        float _currentSpawnDelay;

        // Events
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action<Enemy> OnEnemySpawned;
        public event Action OnAllWavesCompleted;
        public event Action OnGameComplete;
        public event Action<float> OnWaveProgressUpdated;

        // Properties
        public int CurrentWave => _currentWaveNumber;
        public int TotalWaves => _totalWaves;
        public WaveState CurrentState => _currentState;
        public bool IsWaveActive => _currentState == WaveState.InProgress;
        public bool IsBetweenWaves => _currentState == WaveState.InterWave;
        public bool IsGameComplete => _isGameComplete;
        public bool IsPaused => _isPaused;
        public float WaveProgress => GetWaveProgress();
        public float OverallProgress => GetOverallProgress();

        // Singleton
        static WaveDirector _instance;
        private object TheContainingType;
        private object TheContainingMember;

        public static WaveDirector Instance => _instance ??= new WaveDirector();

        private WaveDirector()
        {
            _waveScripts = new Dictionary<int, WaveScript>();
            _upcomingWaves = new Queue<WaveScript>();
            _currentState = WaveState.NotStarted;
            _currentWaveNumber = 0;
            _totalWaves = 0;
        }

        /// <summary>
        /// Initialize the wave director.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            System.Diagnostics.Debug.WriteLine("Initializing Wave Director");

            try
            {
                // Load wave scripts
                LoadWaveScripts();

                // Initialize wave queue
                InitializeWaveQueue();

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine($"Wave Director initialized with {_totalWaves} waves");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize Wave Director: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Start the first wave.
        /// </summary>
        public void StartGame()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            System.Diagnostics.Debug.WriteLine("Starting wave game");
            _currentState = WaveState.WaitingToStart;
            StartNextWave();
        }

        /// <summary>
        /// Start a specific wave.
        /// </summary>
        /// <param name="waveNumber">Wave number to start.</param>
        public async Task<bool> StartWave(int waveNumber)
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

                // Set current wave
                _currentWave = waveScript;
                _currentWaveNumber = waveNumber;
                _currentState = WaveState.Starting;
                _waveTimer = 0f;
                _spawnTimer = 0f;

                // Trigger wave started event
                OnWaveStarted?.Invoke(waveNumber);

                // Start spawning enemies
                await StartWaveSpawning(waveScript);

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
        /// Starts spawning enemies for the current wave.
        /// </summary>
        async Task StartWaveSpawning(WaveScript waveScript)
        {
            // Placeholder implementation
            await Task.Delay(1000);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "WaveDirector: Started wave spawning");
        }

        /// <summary>
        /// Start a specific wave (synchronous version).
        /// </summary>
        /// <param name="waveNumber">Wave number to start.</param>
        public void StartWaveSync(int waveNumber) => StartWave(waveNumber).GetAwaiter().GetResult();

        /// <summary>
        /// Start the next wave in sequence.
        /// </summary>
        public void StartNextWave()
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
                CompleteGame();
                return;
            }

            _ = StartWave(nextWaveNumber);
        }

        /// <summary>
        /// Start next wave early (if current wave is complete).
        /// </summary>
        public void StartNextWaveEarly()
        {
            if (_currentState != WaveState.InProgress)
            {
                StartNextWave();
                return;
            }

            // Check if all enemies from current wave are defeated
            if (AreAllWaveEnemiesDefeated())
            {
                CompleteCurrentWave();
                StartNextWave();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Cannot start next wave early - enemies still active");
            }
        }

        /// <summary>
        /// Update wave director.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>
        public void Update(float deltaTime)
        {
            if (!_isInitialized || _isPaused) return;

            try
            {
                switch (_currentState)
                {
                    case WaveState.InProgress:
                        UpdateWaveInProgress(deltaTime);
                        break;

                    case WaveState.InterWave:
                        UpdateInterWave(deltaTime);
                        break;

                    case WaveState.WaitingToStart:
                        // Waiting for manual start
                        break;
                }

                // Update progress
                OnWaveProgressUpdated?.Invoke(WaveProgress);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating Wave Director: {ex.Message}");
            }
        }

        /// <summary>
        /// Pause wave director.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
            System.Diagnostics.Debug.WriteLine("Wave Director paused");
        }

        /// <summary>
        /// Resume wave director.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
            System.Diagnostics.Debug.WriteLine("Wave Director resumed");
        }

        /// <summary>
        /// Stop wave director.
        /// </summary>
        public void Stop()
        {
            _isPaused = true;
            _currentState = WaveState.Stopped;
            System.Diagnostics.Debug.WriteLine("Wave Director stopped");
        }

        /// <summary>
        /// Reset wave director to initial state.
        /// </summary>
        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine("Resetting Wave Director");

            _currentWave = null;
            _currentWaveNumber = 0;
            _currentState = WaveState.NotStarted;
            _isPaused = false;
            _isGameComplete = false;
            _waveTimer = 0f;
            _interWaveTimer = 0f;
            _spawnTimer = 0f;
            _instance = null;

            // Clear wave queue and rebuild
            _upcomingWaves.Clear();
            InitializeWaveQueue();

            // Clear all enemies
            var value = WaveDirector._instance?.ClearAllEnemies();
        }

        object ClearAllEnemies()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if all enemies from current wave are defeated.
        /// </summary>
        public bool AreAllWaveEnemiesDefeated()
        {
            if (_currentState != WaveState.InProgress)
                return true;

            var enemyManager = WaveDirector.Instance;

            if (enemyManager == null)
                return true;
            // Check if any enemies from current wave are still active
            var activeEnemies = enemyManager.GetActiveEnemies();

            // Use standard looping or cast elements explicitly to your Enemy class
            foreach (var obj in activeEnemies)
            {
                if (obj is Engine.Enemies.Enemy enemy) // Casts object to concrete Enemy type safely
                {
                    if (enemy.SourceWave == _currentWaveNumber && enemy.IsActive)
                        return false; // still an active enemy from this wave
                }
            }

            return true; // no active enemies from this wave


        }

        private IEnumerable<object> GetActiveEnemies()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get wave statistics.
        /// </summary>
        public WaveStats GetWaveStats()
        {
            return new WaveStats
            {
                CurrentWave = _currentWaveNumber,
                TotalWaves = _totalWaves,
                EnemiesInWave = _currentWave?.GetTotalEnemyCount() ?? 0,
                EnemiesSpawned = GetEnemiesSpawnedInWave(),
                EnemiesDefeated = GetEnemiesDefeatedInWave(),
                WaveProgress = WaveProgress,
                OverallProgress = OverallProgress,
                TimeInWave = _waveTimer,
                State = _currentState
            };
        }

        /// <summary>
        /// Gets the number of completed waves.
        /// </summary>
        /// <returns>Number of waves that have been completed.</returns>
        public int GetCompletedWaves()
        {
            return _currentWaveNumber > 0 && _currentState != WaveState.InProgress
                ? _currentWaveNumber
                : System.Math.Max(0, _currentWaveNumber - 1);
        }

        /// <summary>
        /// Load wave scripts from data.
        /// </summary>
        void LoadWaveScripts()
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
                // Create default wave scripts
                CreateDefaultWaveScripts();
            }
        }

        /// <summary>
        /// Initialize wave queue.
        /// </summary>
        void InitializeWaveQueue()
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

        /// <summary>
        /// Execute a wave script.
        /// </summary>
        async Task ExecuteWaveScript(WaveScript waveScript)
        {
            System.Diagnostics.Debug.WriteLine($"Executing wave script for wave {waveScript.WaveNumber}");

            foreach (var spawnGroup in waveScript.SpawnGroups)
            {
                if (_currentState != WaveState.InProgress)
                    break;

                await ExecuteSpawnGroup(spawnGroup);

                // Wait between spawn groups
                if (spawnGroup.DelayAfterGroup > 0)
                {
                    await Task.Delay((int)(spawnGroup.DelayAfterGroup * 1000));
                }
            }
        }

        /// <summary>
        /// Execute a spawn group.
        /// </summary>
        async Task ExecuteSpawnGroup(IWaveSpawnGroup spawnGroup)
        {
            System.Diagnostics.Debug.WriteLine($"Executing spawn group: {spawnGroup.GetType().Name}");

            for (int i = 0; i < spawnGroup.Count; i++)
            {
                if (_currentState != WaveState.InProgress)
                    break;

                // Spawn enemy
                var enemy = SpawnEnemy(SASZombieAssaultTD.Engine.Enemies.ZombieType.Basic);

                if (enemy != null)
                {
                    OnEnemySpawned?.Invoke(enemy);
                    spawnGroup.OnEnemySpawnedCallback(enemy);
                }

                // Wait between spawns
                await Task.Delay(1000); // 1 second default delay
            }
        }

        /// <summary>
        /// Spawn a single enemy.
        /// </summary>
        Enemy SpawnEnemy(SASZombieAssaultTD.Engine.Enemies.ZombieType zombieType)
        {
            try
            {
                // TODO: EnemyManager doesn't have static Instance - need to inject or use different pattern
                EnemyManager enemyManager = null; // EnemyManager.Instance;

                if (enemyManager == null)
                    return null;

                // Get spawn position (use default pattern)
                var spawnPosition = GetSpawnPosition(SpawnPatternType.Line);

                // Create enemy
                var enemy = enemyManager.SpawnEnemy((SASZombieAssaultTD.Engine.Enemies.EnemyType)zombieType, spawnPosition);

                if (enemy == null)
                    return null;

                // Apply basic wave modifications
                ApplyWaveModifications(enemy);

                // Set source wave
                enemy.SourceWave = _currentWaveNumber;

                // Play spawn sound
                ModernPlaySound.PlayAtPosition("enemy_spawn", spawnPosition);

                return enemy;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error spawning enemy: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get spawn position based on pattern.
        /// </summary>
        Vector3 GetSpawnPosition(SpawnPatternType pattern)
        {
            var spawnPoints = NavigationGrid.Instance?.GetSpawnPoints();

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                return new Vector3(0, 0, 0);
            }

            return pattern switch
            {
                SpawnPatternType.Line => (Vector3)spawnPoints[new System.Random().Next(0, spawnPoints.Count)],
                SpawnPatternType.Circle => (Vector3)spawnPoints[0], // Simplified
                SpawnPatternType.Wave => (Vector3)spawnPoints[0], // Simplified
                SpawnPatternType.Random => (Vector3)spawnPoints[new System.Random().Next(0, spawnPoints.Count)],
                _ => (Vector3)spawnPoints[0]
            };
        }

        /// <summary>
        /// Get cluster spawn position.
        /// </summary>
        Vector3 GetClusterSpawnPosition(List<Vector3> spawnPoints)
        {
            // Pick a random spawn point and cluster around it
            var random = new System.Random();
            var basePoint = spawnPoints[random.Next(0, spawnPoints.Count)];

            var offset = new Vector3(
                (float)(random.NextDouble() * 2 - 1),
                (float)(random.NextDouble() * 2 - 1),
                0
            );

            return basePoint + offset;
        }

        /// <summary>
        /// Get spread spawn position.
        /// </summary>
        Vector3 GetSpreadSpawnPosition(List<Vector3> spawnPoints)
        {
            // Distribute across all spawn points
            var random = new System.Random();
            var index = random.Next(0, spawnPoints.Count);
            return spawnPoints[index];
        }

        /// <summary>
        /// Apply wave modifications to enemy.
        /// </summary>
        void ApplyWaveModifications(Enemy enemy)
        {
            if (_currentWave == null)
                return;

            // Apply difficulty multiplier
            var difficultyMultiplier = _currentWave.DifficultyMultiplier.GetMultiplier();

            // Scale health
            enemy.MaxHealth = (int)(enemy.MaxHealth * difficultyMultiplier);
            enemy.Health = enemy.MaxHealth;

            // Scale damage
            enemy.Damage = (int)(enemy.Damage * difficultyMultiplier);
        }

        /// <summary>
        /// Apply difficulty multiplier to wave script.
        /// </summary>
        void ApplyDifficultyMultiplier(WaveScript waveScript)
        {
            var difficulty = DifficultyManager.Instance != null ? (DifficultyMode)DifficultyManager.Instance.CurrentDifficulty :
                DifficultyMode.Normal;
            var waveDifficulty = new DifficultyMultiplier();
            waveDifficulty.CurrentDifficulty = difficulty;
            waveScript.DifficultyMultiplier = waveDifficulty;
        }

        /// <summary>
        /// Update wave in progress.
        /// </summary>
        void UpdateWaveInProgress(float deltaTime)
        {
            _waveTimer += deltaTime;

            // Check if wave is complete
            if (AreAllWaveEnemiesDefeated() && _currentWave != null)
            {
                CompleteCurrentWave();
            }
        }

        /// <summary>
        /// Update inter-wave period.
        /// </summary>
        void UpdateInterWave(float deltaTime)
        {
            _interWaveTimer += deltaTime;

            var interWaveDelay = _currentWave?.InterWaveDelay ?? 10f;

            if (_interWaveTimer >= interWaveDelay)
            {
                StartNextWave();
            }
        }

        /// <summary>
        /// Complete current wave.
        /// </summary>
        void CompleteCurrentWave()
        {
            if (_currentState != WaveState.InProgress)
                return;

            System.Diagnostics.Debug.WriteLine($"Wave {_currentWaveNumber} completed");

            _currentState = WaveState.InterWave;
            _interWaveTimer = 0f;

            // Play wave complete sound
            ModernPlaySound.Play("wave_complete");

            // Show wave complete notification
            ShowWaveCompleteNotification(_currentWave);

            // Award wave completion bonus
            AwardWaveCompletionBonus();

            // Notify wave completed
            OnWaveCompleted?.Invoke(_currentWaveNumber);

            // Check if all waves are complete
            if (_currentWaveNumber >= _totalWaves)
            {
                CompleteGame();
            }
        }

        /// <summary>
        /// Complete the entire game.
        /// </summary>
        void CompleteGame()
        {
            System.Diagnostics.Debug.WriteLine("All waves completed - Game complete!");

            _isGameComplete = true;
            _currentState = WaveState.Complete;

            // Notify game complete
            OnAllWavesCompleted?.Invoke();
            OnGameComplete?.Invoke();

            // Play victory sound
            ModernPlaySound.Play("victory");
        }

        /// <summary>
        /// Get wave progress (0-1).
        /// </summary>
        float GetWaveProgress()
        {
            if (_currentState != WaveState.InProgress || _currentWave == null)
                return 0f;

            var totalEnemies = _currentWave.GetTotalEnemyCount();
            var defeatedEnemies = GetEnemiesDefeatedInWave();

            return totalEnemies > 0 ? (float)defeatedEnemies / totalEnemies : 0f;
        }

        /// <summary>
        /// Get overall progress (0-1).
        /// </summary>
        float GetOverallProgress()
        {
            if (_totalWaves == 0)
                return 0f;

            var waveProgress = GetWaveProgress();
            var completedWaves = _currentWaveNumber - 1;

            return (completedWaves + waveProgress) / _totalWaves;
        }

        /// <summary>
        /// Get enemies spawned in current wave.
        /// </summary>
        int GetEnemiesSpawnedInWave()
        {
            // TODO: EnemyManager doesn't have static Instance - need to inject or use different pattern
            EnemyManager enemyManager = null; // EnemyManager.Instance;

            if (enemyManager == null)
                return 0;

            var count = 0;
            var allEnemies = enemyManager.GetAllEnemies();

            foreach (var enemy in allEnemies)
            {
                NotImplementedGuard.Hit("NOT_IMPLEMENTED");
                // TODO: check if this is correct
                // TODO: enemy.SourceWave is object type, need cast
                // if (enemy.SourceWave == _currentWaveNumber)
                // {
                //     count++;
                // }
            }

            return count;
        }

        /// <summary>
        /// Get enemies defeated in current wave.
        /// </summary>
        int GetEnemiesDefeatedInWave()
        {
            // TODO: EnemyManager doesn't have static Instance - need to inject or use different pattern
            EnemyManager enemyManager = null; // EnemyManager.Instance;

            if (enemyManager == null)
                return 0;

            var count = 0;
            var allEnemies = enemyManager.GetAllEnemies();

            foreach (var enemy in allEnemies)
            {
                // TODO: enemy.SourceWave is object type, need cast
                // if (enemy.SourceWave == _currentWaveNumber && !enemy.IsActive)
                // {
                //     count++;
                // }
            }

            return count;
        }

        /// <summary>
        /// Show wave notification.
        /// </summary>
        void ShowWaveNotification(WaveScript waveScript)
        {
            var notification = new WaveNotification
            {
                WaveNumber = waveScript.WaveNumber,
                Title = $"Wave {waveScript.WaveNumber}",
                Description = GetWaveDescription(waveScript),
                Duration = 3f
            };

            HUDController.Instance?.ShowWaveNotification(notification);
        }

        /// <summary>
        /// Show wave complete notification.
        /// </summary>
        void ShowWaveCompleteNotification(WaveScript waveScript)
        {
            var notification = new WaveNotification
            {
                WaveNumber = waveScript.WaveNumber,
                Title = "Wave Complete!",
                Description = $"Wave {waveScript.WaveNumber} defeated",
                Duration = 2f
            };

            HUDController.Instance?.ShowWaveNotification(notification);
        }

        /// <summary>
        /// Get wave description.
        /// </summary>
        string GetWaveDescription(WaveScript waveScript)
        {
            var description = $"Enemies: {waveScript.GetTotalEnemyCount()}\n";

            foreach (var group in waveScript.SpawnGroups)
                description += $"{group.EnemyType} x{group.Count}\n";

            return description.Trim();
        }

        /// <summary>
        /// Award wave completion bonus.
        /// </summary>
        void AwardWaveCompletionBonus()
        {
            var bonus = _currentWaveNumber * 100; // $100 per wave

            var economy = ModernPlayerStateSystem.Instance;
            economy?.Earn(bonus);

            System.Diagnostics.Debug.WriteLine($"Awarded wave completion bonus: ${bonus}");
        }

        /// <summary>
        /// Create default wave scripts if loading fails.
        /// </summary>
        void CreateDefaultWaveScripts()
        {
            System.Diagnostics.Debug.WriteLine("Creating default wave scripts");

            // Create 10 default waves
            for (int i = 1; i <= 10; i++)
            {
                var waveScript = CreateDefaultWaveScript(i);
                _waveScripts[i] = waveScript;
            }

            _totalWaves = _waveScripts.Count;
        }

        WaveScript CreateDefaultWaveScript(int waveNumber)
        {
            var waveScript = new WaveScript
            {
                WaveNumber = waveNumber,
                InterWaveDelay = 10f,
                DifficultyMultiplier = new DifficultyMultiplier(DifficultyMultiplier.Instance.GetMultiplier(DifficultyMode.Normal))
            };

            // Add spawn groups based on wave number
            var enemyCount = 5 + (waveNumber * 2); // Increase enemies per wave

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

    /// <summary>
    /// Wave states.
    /// </summary>
    public enum WaveState
    {
        NotStarted,
        WaitingToStart,
        Starting,
        InProgress,
        InterWave,
        Complete,
        Stopped
    }

    /// <summary>
    /// Wave statistics.
    /// </summary>
    public class WaveStats
    {
        public int CurrentWave { get; set; }
        public int TotalWaves { get; set; }
        public int EnemiesInWave { get; set; }
        public int EnemiesSpawned { get; set; }
        public int EnemiesDefeated { get; set; }
        public float WaveProgress { get; set; }
        public float OverallProgress { get; set; }
        public float TimeInWave { get; set; }
        public WaveState State { get; set; }
    }

    /// <summary>
    /// Wave notification for UI.
    /// </summary>
    public class WaveNotification
    {
        public int WaveNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public float Duration { get; set; }
    }
}