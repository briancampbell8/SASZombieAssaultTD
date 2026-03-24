using System;
using System.IO;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Gameplay
{
    /// <summary>
    /// Modern player state management system with persistence, events, and economy integration.
    /// Replaces legacy PlayerLives with complete modern implementation.
    /// </summary>
    public sealed class ModernPlayerStateSystem : IGameSystem
    {
        private static ModernPlayerStateSystem _instance;
        private static readonly object _lock = new object();

        public static ModernPlayerStateSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ModernPlayerStateSystem();
                        }
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

        // Events
        public event Action<int, int> OnLivesChanged;
        public event Action<int> OnCashChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnWaveChanged;
        public event Action OnGameOver;
        public event Action OnGameRestarted;
        public event Action<bool> OnPauseStateChanged;

        // Game settings
        private readonly PlayerSettings _settings;

        /// <summary>
        /// Initialize the player state system.
        /// </summary>
        public void Initialize()
        {
            ModernLoggingSystem.LogInfo("ModernPlayerStateSystem initialized");
            ModernLoggingSystem.LogInfo($"Player started with {_currentLives}/{_maxLives} lives and ${_currentCash}");
        }

        /// <summary>
        /// Update the player state system.
        /// </summary>
        public void Update(float deltaTime)
        {
            // Handle any time-based player state updates
            // (e.g., regeneration, timed bonuses, etc.)
        }

        /// <summary>
        /// Reset player to default state.
        /// </summary>
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

            ModernLoggingSystem.LogInfo("Player state reset to defaults");
        }

        /// <summary>
        /// Take damage from player.
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (_isGameOver || _isPaused) return;

            var oldLives = _currentLives;
            _currentLives = System.Math.Max(0, _currentLives - damage);

            if (oldLives != _currentLives)
            {
                OnLivesChanged?.Invoke(_currentLives, _maxLives);
                ModernLoggingSystem.LogInfo($"Player took {damage} damage. Lives: {_currentLives}/{_maxLives}");

                if (_currentLives <= 0)
                {
                    TriggerGameOver();
                }
            }
        }

        /// <summary>
        /// Heal player by specified amount.
        /// </summary>
        public void Heal(int amount)
        {
            if (_isGameOver || _isPaused) return;

            var oldLives = _currentLives;
            _currentLives = System.Math.Min(_maxLives, _currentLives + amount);

            if (oldLives != _currentLives)
            {
                OnLivesChanged?.Invoke(_currentLives, _maxLives);
                ModernLoggingSystem.LogInfo($"Player healed for {amount}. Lives: {_currentLives}/{_maxLives}");
            }
        }

        /// <summary>
        /// Add cash to player.
        /// </summary>
        public void AddCash(int amount)
        {
            if (_isGameOver || _isPaused) return;

            var oldCash = _currentCash;
            _currentCash += amount;

            if (oldCash != _currentCash)
            {
                OnCashChanged?.Invoke(_currentCash);
                ModernLoggingSystem.LogInfo($"Player gained ${amount}. Current: ${_currentCash}");
            }
        }

        /// <summary>
        /// Spend cash if available.
        /// </summary>
        public bool SpendCash(int amount)
        {
            if (_isGameOver || _isPaused || _currentCash < amount) return false;

            var oldCash = _currentCash;
            _currentCash -= amount;

            OnCashChanged?.Invoke(_currentCash);
            ModernLoggingSystem.LogInfo($"Player spent ${amount}. Current: ${_currentCash}");

            return true;
        }

        /// <summary>
        /// Add score to player.
        /// </summary>
        public void AddScore(int points)
        {
            if (_isGameOver || _isPaused) return;

            var oldScore = _score;
            _score += points;

            if (oldScore != _score)
            {
                OnScoreChanged?.Invoke(_score);
                ModernLoggingSystem.LogDebug($"Player gained {points} points. Total: {_score}");
            }
        }

        /// <summary>
        /// Advance to next wave.
        /// </summary>
        public void AdvanceWave()
        {
            if (_isGameOver || _isPaused) return;

            _waveNumber++;
            OnWaveChanged?.Invoke(_waveNumber);
            ModernLoggingSystem.LogInfo($"Advanced to wave {_waveNumber}");

            // Give wave completion bonus
            var waveBonus = _settings.WaveCompletionBonus * _waveNumber;
            AddCash(waveBonus);
        }

        /// <summary>
        /// Set game over state.
        /// </summary>
        public void TriggerGameOver()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            OnGameOver?.Invoke();
            ModernLoggingSystem.LogInfo($"Game Over! Final Score: {_score}, Waves Survived: {_waveNumber - 1}");
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
        public void RestartGame()
        {
            ResetToDefaults();
            OnGameRestarted?.Invoke();
            ModernLoggingSystem.LogInfo("Game restarted");
        }

        /// <summary>
        /// Set pause state.
        /// </summary>
        public void SetPaused(bool paused)
        {
            if (_isGameOver) return;

            _isPaused = paused;
            OnPauseStateChanged?.Invoke(paused);
            ModernLoggingSystem.LogInfo($"Game {(paused ? "paused" : "resumed")}");
        }

        /// <summary>
        /// Save player state.
        /// </summary>
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

                ModernLoggingSystem.LogInfo($"Player state saved to slot: {saveSlot}");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.LogError($"Failed to save player state: {ex.Message}");
            }
        }

        /// <summary>
        /// Load player state.
        /// </summary>
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

                    // Trigger events for loaded state
                    OnLivesChanged?.Invoke(_currentLives, _maxLives);
                    OnCashChanged?.Invoke(_currentCash);
                    OnScoreChanged?.Invoke(_score);
                    OnWaveChanged?.Invoke(_waveNumber);

                    ModernLoggingSystem.LogInfo($"Player state loaded from slot: {saveSlot}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.LogError($"Failed to load player state: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Get player statistics.
        /// </summary>
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
                SurvivalTime = DateTime.UtcNow - DateTime.UtcNow // Would track actual survival time
            };
        }

        /// <summary>
        /// Earn cash for the player.
        /// </summary>
        /// <param name="amount">Amount of cash to earn.</param>
        public void Earn(int amount)
        {
            if (amount <= 0) return;

            _currentCash += amount;
            OnCashChanged?.Invoke(_currentCash);
        }

        /// <summary>
        /// Spend cash from the player.
        /// </summary>
        /// <param name="amount">Amount to spend.</param>
        /// <returns>True if player had enough cash.</returns>
        public bool Spend(int amount)
        {
            if (amount <= 0) return false;
            if (_currentCash < amount) return false;

            _currentCash -= amount;
            OnCashChanged?.Invoke(_currentCash);
            return true;
        }

        private string GetSavePath(string saveSlot)
        {
            var saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SASZombieAssaultTD", "Saves");
            Directory.CreateDirectory(saveDir);
            return Path.Combine(saveDir, $"player_{saveSlot}.json");
        }

        // Properties
        public int CurrentLives => _currentLives;
        public int MaxLives => _maxLives;
        public int CurrentCash => _currentCash;
        public int Score => _score;
        public int WaveNumber => _waveNumber;
        public bool IsGameOver => _isGameOver;
        public bool IsPaused => _isPaused;
        public float LivesPercentage => _maxLives > 0 ? (float)_currentLives / _maxLives : 0f;
    }

    /// <summary>
    /// Player settings configuration.
    /// </summary>
    public class PlayerSettings
    {
        public int StartingLives { get; set; } = 10;
        public int MaxLives { get; set; } = 20;
        public int StartingCash { get; set; } = 1000;
        public int WaveCompletionBonus { get; set; } = 100;
        public bool AllowLivesRegeneration { get; set; } = false;
        public float LivesRegenerationRate { get; set; } = 0.1f; // Lives per second
    }

    /// <summary>
    /// Player save data structure.
    /// </summary>
    public class PlayerSaveData
    {
        public int CurrentLives { get; set; }
        public int MaxLives { get; set; }
        public int CurrentCash { get; set; }
        public int Score { get; set; }
        public int WaveNumber { get; set; }
        public bool IsGameOver { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Player statistics.
    /// </summary>
    public class PlayerStats
    {
        public int CurrentLives { get; set; }
        public int MaxLives { get; set; }
        public int CurrentCash { get; set; }
        public int TotalEarned { get; set; }
        public int Score { get; set; }
        public int WaveNumber { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsPaused { get; set; }
        public TimeSpan SurvivalTime { get; set; }
    }
}
