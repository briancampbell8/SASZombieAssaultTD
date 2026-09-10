// ====================================================================================================
//  FILE: ModernPlayerStateSystem.cs
//  PATH: Engine/Gameplay/ModernPlayerStateSystem.cs
//  SUBSYSTEM: Gameplay
//
//  PURPOSE:
//      Modern player state management system with persistence, events, and economy integration.
//      Replaces legacy PlayerLives with a complete modern implementation.
//
//  ROLE:
//      - Player lives, cash, score, and wave tracking
//      - Game over, restart, and pause state management
//      - Save/load persistence for player state
//      - Event-based notifications for UI and gameplay systems
//
//  NOTES:
//      - Designed as a modern replacement for legacy PlayerLives
//      - Integrates with GameRoot and other gameplay systems via events
//      - Uses deterministic, structured diagnostics via DLogger
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay
{
    public sealed class ModernPlayerStateSystem : IGameSystem
    {
        private static ModernPlayerStateSystem _instance;
        private static readonly object _lock = new();

        public static ModernPlayerStateSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new ModernPlayerStateSystem();
                    }
                }
                return _instance;
            }
        }

        private ModernPlayerStateSystem()
        {
            _settings = new PlayerSettings();
            ResetToDefaults();
        }

        private int _currentLives;
        private int _maxLives;
        private int _currentCash;
        private int _startingCash;
        private int _score;
        private int _waveNumber;
        private bool _isGameOver;
        private bool _isPaused;

        public event Action<int, int> OnLivesChanged;
        public event Action<int> OnCashChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnWaveChanged;
        public event Action OnGameOver;
        public event Action OnGameRestarted;
        public event Action<bool> OnPauseStateChanged;

        private readonly PlayerSettings _settings;

        // ---------------------------------------------------------------------------------------------
        // Lifecycle
        // ---------------------------------------------------------------------------------------------
        public void Initialize()
        {
            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "PlayerState",
                "ModernPlayerStateSystem initialized");

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "PlayerState",
                $"Player started with {_currentLives}/{_maxLives} lives and ${_currentCash}");
        }

        public void Update(float deltaTime)
        {
            // Reserved for future time-based state updates
        }

        public void ResetToDefaults()
        {
            _currentLives = _settings.StartingLives;
            _maxLives = _settings.MaxLives;
            _currentCash = _settings.StartingCash;
            _startingCash = _settings.StartingCash;
            _score = 0;
            _waveNumber = 1;
            _isGameOver = false;
            _isPaused = false;

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "PlayerState",
                "Player state reset to defaults");
        }

        // ---------------------------------------------------------------------------------------------
        // Lives / Damage / Healing
        // ---------------------------------------------------------------------------------------------
        public void TakeDamage(int damage)
        {
            if (_isGameOver || _isPaused) return;

            var oldLives = _currentLives;
            _currentLives = System.Math.Max(0, _currentLives - damage);

            if (oldLives != _currentLives)
            {
                OnLivesChanged?.Invoke(_currentLives, _maxLives);

                DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Damage",
                    $"Player took {damage} damage. Lives: {_currentLives}/{_maxLives}");

                if (_currentLives <= 0)
                    TriggerGameOver();
            }
        }

        public void Heal(int amount)
        {
            if (_isGameOver || _isPaused) return;

            var oldLives = _currentLives;
            _currentLives = System.Math.Min(_maxLives, _currentLives + amount);

            if (oldLives != _currentLives)
            {
                OnLivesChanged?.Invoke(_currentLives, _maxLives);

                DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Damage",
                    $"Player healed for {amount}. Lives: {_currentLives}/{_maxLives}");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Cash / Economy
        // ---------------------------------------------------------------------------------------------
        public void AddCash(int amount)
        {
            if (_isGameOver || _isPaused) return;

            var oldCash = _currentCash;
            _currentCash += amount;

            if (oldCash != _currentCash)
            {
                OnCashChanged?.Invoke(_currentCash);

                DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Economy",
                    $"Player gained ${amount}. Current: ${_currentCash}");
            }
        }

        public bool SpendCash(int amount)
        {
            if (_isGameOver || _isPaused || _currentCash < amount) return false;

            _currentCash -= amount;
            OnCashChanged?.Invoke(_currentCash);

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Economy",
                $"Player spent ${amount}. Current: ${_currentCash}");

            return true;
        }

        public void Earn(int amount)
        {
            if (amount <= 0) return;

            _currentCash += amount;
            OnCashChanged?.Invoke(_currentCash);

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Debug, "Economy",
                $"Player earned ${amount}. Current: ${_currentCash}");
        }

        public bool Spend(int amount)
        {
            if (amount <= 0 || _currentCash < amount) return false;

            _currentCash -= amount;
            OnCashChanged?.Invoke(_currentCash);

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Debug, "Economy",
                $"Player spent ${amount}. Current: ${_currentCash}");

            return true;
        }

        // ---------------------------------------------------------------------------------------------
        // Score / Waves
        // ---------------------------------------------------------------------------------------------
        public void AddScore(int points)
        {
            if (_isGameOver || _isPaused) return;

            var oldScore = _score;
            _score += points;

            if (oldScore != _score)
            {
                OnScoreChanged?.Invoke(_score);

                DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Score",
                    $"Player gained {points} points. Total: {_score}");
            }
        }

        public void AdvanceWave()
        {
            if (_isGameOver || _isPaused) return;

            _waveNumber++;
            OnWaveChanged?.Invoke(_waveNumber);

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Wave",
                $"Advanced to wave {_waveNumber}");

            var waveBonus = _settings.WaveCompletionBonus * _waveNumber;
            AddCash(waveBonus);
        }

        // ---------------------------------------------------------------------------------------------
        // Game Over / Restart / Pause
        // ---------------------------------------------------------------------------------------------
        public void TriggerGameOver()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            OnGameOver?.Invoke();

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "PlayerState",
                $"Game Over! Final Score: {_score}, Waves Survived: {_waveNumber - 1}");
        }

        public void RestartGame()
        {
            ResetToDefaults();
            OnGameRestarted?.Invoke();

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "PlayerState",
                "Game restarted");
        }

        public void SetPaused(bool paused)
        {
            if (_isGameOver) return;

            _isPaused = paused;
            OnPauseStateChanged?.Invoke(paused);

            DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Pause",
                $"Game {(paused ? "paused" : "resumed")}");
        }

        // ---------------------------------------------------------------------------------------------
        // Persistence
        // ---------------------------------------------------------------------------------------------
        public void SaveState(string saveSlot = "default")
        {
            try
            {
                var saveData = new PlayerSaveData
                {
                    CurrentLives = _currentLives,
                    MaxLives = _maxLives,
                    CurrentCash = _currentCash,
                    Score = _score,
                    WaveNumber = _waveNumber,
                    IsGameOver = _isGameOver,
                    Timestamp = DateTime.UtcNow
                };

                var json = System.Text.Json.JsonSerializer.Serialize(saveData);
                var savePath = GetSavePath(saveSlot);
                File.WriteAllText(savePath, json);

                DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Persistence",
                    $"Player state saved to slot: {saveSlot}");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Error, "Persistence",
                    $"Failed to save player state: {ex.Message}");
            }
        }

        public bool LoadState(string saveSlot = "default")
        {
            try
            {
                var savePath = GetSavePath(saveSlot);
                if (!File.Exists(savePath)) return false;

                var json = File.ReadAllText(savePath);
                var saveData = System.Text.Json.JsonSerializer.Deserialize<PlayerSaveData>(json);

                if (saveData != null)
                {
                    _currentLives = saveData.CurrentLives;
                    _maxLives = saveData.MaxLives;
                    _currentCash = saveData.CurrentCash;
                    _score = saveData.Score;
                    _waveNumber = saveData.WaveNumber;
                    _isGameOver = saveData.IsGameOver;

                    OnLivesChanged?.Invoke(_currentLives, _maxLives);
                    OnCashChanged?.Invoke(_currentCash);
                    OnScoreChanged?.Invoke(_score);
                    OnWaveChanged?.Invoke(_waveNumber);

                    DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Info, "Persistence",
                        $"Player state loaded from slot: {saveSlot}");

                    return true;
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Gameplay, LogEnums.LogLevel.Error, "Persistence",
                    $"Failed to load player state: {ex.Message}");
            }

            return false;
        }

        private string GetSavePath(string saveSlot)
        {
            var saveDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SASZombieAssaultTD",
                "Saves");

            Directory.CreateDirectory(saveDir);
            return Path.Combine(saveDir, $"player_{saveSlot}.json");
        }

        // ---------------------------------------------------------------------------------------------
        // Stats / Properties
        // ---------------------------------------------------------------------------------------------
        public PlayerStats GetStats()
        {
            return new PlayerStats
            {
                CurrentLives = _currentLives,
                MaxLives = _maxLives,
                CurrentCash = _currentCash,
                TotalEarned = _currentCash - _startingCash,
                Score = _score,
                WaveNumber = _waveNumber,
                IsGameOver = _isGameOver,
                IsPaused = _isPaused,
                SurvivalTime = TimeSpan.Zero
            };
        }

        public int CurrentLives => _currentLives;
        public int MaxLives => _maxLives;
        public int CurrentCash => _currentCash;
        public int Score => _score;
        public int WaveNumber => _waveNumber;
        public bool IsGameOver => _isGameOver;
        public bool IsPaused => _isPaused;
        public float LivesPercentage => _maxLives > 0 ? (float)_currentLives / _maxLives : 0f;
    }

    // ====================================================================================================
    // Supporting Data Classes
    // ====================================================================================================

    public class PlayerSettings
    {
        public int StartingLives { get; set; } = 10;
        public int MaxLives { get; set; } = 20;
        public int StartingCash { get; set; } = 1000;
        public int WaveCompletionBonus { get; set; } = 100;
        public bool AllowLivesRegeneration { get; set; } = false;
        public float LivesRegenerationRate { get; set; } = 0.1f;
    }

    public class PlayerSaveData
    {
        public int CurrentLives { get; set; }
        public int MaxLives { get; set; }
        public int CurrentCash { get; set; }

        public int AddedCash { get; set; }
        public int Score { get; set; }
        public int WaveNumber { get; set; }
        public bool IsGameOver { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PlayerStats
    {
        public static PlayerStats Instance => _instance ??= new PlayerStats();
        public static PlayerStats _instance;

        public PlayerStats()
        {
            CurrentLives = 0;
            MaxLives = 0;
            CurrentCash = 0;
            TotalEarned = 0;
            Score = 0;
            WaveNumber = 0;
            IsGameOver = false;
            IsPaused = false;
            SurvivalTime = TimeSpan.Zero;
        }

        public int CurrentLives { get; set; }
        public int MaxLives { get; set; }
        public int CurrentCash { get; set; }
        public int TotalEarned { get; set; }
        public int Score { get; set; }
        public int WaveNumber { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsPaused { get; set; }
        public TimeSpan SurvivalTime { get; set; }
        public int AddedCash { get; internal set; }
    }
}
