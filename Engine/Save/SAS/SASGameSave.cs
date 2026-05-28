using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.LevelUpControl;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SASZombieAssaultTD.Engine.Save.SAS
{
    /// <summary>
    /// SAS TD game save data container.
    /// Handles serialization and deserialization of game state.
    /// </summary>
    public class SASGameSave
    {
        // Basic game information
        public string SaveName { get; set; }
        public DateTime SaveTime { get; set; }
        public string GameVersion { get; set; }
        public int SaveVersion { get; set; }
        public TimeSpan PlayTime { get; set; }
        public string Difficulty { get; set; }
        public string MapName { get; set; }

        // Player state
        public PlayerSaveData Player { get; set; }
        public EconomySaveData Economy { get; set; }
        public WaveSaveData Waves { get; set; }
        public TowerSaveData Towers { get; set; }
        public EnemySaveData Enemies { get; set; }
        public GameStateSaveData GameState { get; set; }

        // Statistics
        public GameStatistics Statistics { get; set; }
        public AchievementSaveData Achievements { get; set; }
        public UnlockSaveData Unlocks { get; set; }

        // Metadata
        public SaveMetadata Metadata { get; set; }

        public SASGameSave()
        {
            SaveVersion = 1;
            GameVersion = "1.0.0";
            SaveTime = DateTime.Now;
            Player = new PlayerSaveData();
            Economy = new EconomySaveData();
            Waves = new WaveSaveData();
            Towers = new TowerSaveData();
            Enemies = new EnemySaveData();
            GameState = new GameStateSaveData();
            Statistics = new GameStatistics();
            Achievements = new AchievementSaveData();
            Unlocks = new UnlockSaveData();
            Metadata = new SaveMetadata();
        }
        public bool ApplyToCurrentState()
        {
            try
            {
                // Apply player state
                Player?.ApplyToGame();

                // Apply economy state
                Economy?.ApplyToGame();

                // Apply wave state
                Waves?.ApplyToGame();

                Towers.Tower tower = null;
                // Apply tower state
                Towers?.ApplyToGame(tower, tower.Position);

                // Apply enemy state
                Enemies?.ApplyToGame();

                // Apply game state
                GameState?.ApplyToGame();

                // Apply statistics
                Statistics?.ApplyToGame();

                // Apply achievements
                Achievements?.ApplyToGame();

                // Apply unlocks
                Unlocks?.ApplyToGame();

                System.Diagnostics.Debug.WriteLine($"Applied save: {SaveName}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying save data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Create a new save from current game state.
        /// </summary>
        /// <param name="saveName">Name for the save.</param>
        /// <returns>Created save data.</returns>
        public static SASGameSave CreateFromCurrentState(string saveName)
        {
            var difficultyManager = SASZombieAssaultTD.Engine.Difficulty.DifficultyManager.Instance;

            var save = new SASGameSave();
            save.SaveName = saveName;
            save.SaveTime = DateTime.Now;

            save.Difficulty = difficultyManager.CurrentDifficulty
                .Instance()?
                .ToString() ?? "Normal";

            save.MapName = GetCurrentMapName();

            // Capture current game state
            CapturePlayerState(save);
            CaptureEconomyState(save);
            CaptureWaveState(save);
            CaptureTowerState(save);
            CaptureEnemyState(save);
            CaptureGameState(save);
            CaptureStatistics(save);
            CaptureAchievements(save);
            CaptureUnlocks(save);

            return save;
        }

        /// <summary>
        /// Serialize save data to JSON.
        /// </summary>
        /// <returns>JSON string.</returns>
        public string SerializeToJson()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Serialize(this, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error serializing save data: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserialize save data from JSON.
        /// </summary>
        /// <param name="json">JSON string to deserialize.</param>
        /// <returns>Deserialized save data.</returns>
        public static SASGameSave DeserializeFromJson(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var save = JsonSerializer.Deserialize<SASGameSave>(json, options);

                // Validate save data
                if (save != null && save.ValidateSaveData())
                {
                    return save;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deserializing save data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validate save data integrity.
        /// </summary>
        /// <returns>True if save data is valid.</returns>
        public bool ValidateSaveData()
        {
            // Check basic fields
            if (string.IsNullOrEmpty(SaveName))
                return false;

            if (SaveVersion < 1)
                return false;

            if (SaveTime == default)
                return false;

            // Validate nested data
            if (Player?.Validate() == false)
                return false;

            if (Economy?.Validate() == false)
                return false;

            if (Waves?.Validate() == false)
                return false;

            if (Towers?.Validate() == false)
                return false;

            if (Enemies?.Validate() == false)
                return false;

            if (GameState?.Validate() == false)
                return false;

            return true;
        }

        /// <summary>
        /// Get save file size in bytes.
        /// </summary>
        /// <returns>Save file size.</returns>
        public long GetSaveSize()
        {
            var json = SerializeToJson();
            return json?.Length ?? 0;
        }

        /// <summary>
        /// Get save summary for display.
        /// </summary>
        /// <returns>Save summary.</returns>
        public string GetSummary()
        {
            return $"Save: {SaveName}\n" +
                   $"Time: {SaveTime:yyyy-MM-dd HH:mm}\n" +
                   $"Difficulty: {Difficulty}\n" +
                   $"Map: {MapName}\n" +
                   $"Wave: {Waves?.CurrentWave ?? 0}/{Waves?.TotalWaves ?? 0}\n" +
                   $"Cash: ${Economy?.CurrentCash ?? 0}\n" +
                   $"Towers: {Towers?.TowerCount ?? 0}\n" +
                   $"Play Time: {FormatPlayTime(PlayTime)}";
        }

        /// <summary>
        /// Check if save is compatible with current game version.
        /// </summary>
        /// <returns>True if compatible.</returns>
        public bool IsCompatible()
        {
            // Simple version check - in real implementation would be more complex
            return SaveVersion <= 1 && GameVersion == "1.0.0";
        }

        /// <summary>
        /// Apply save data to current game state.
        /// </summary>
        /// <returns>True if successfully applied.</returns>

        /// <summary>
        /// Create a backup of this save.
        /// </summary>
        /// <returns>Backup save data.</returns>
        public SASGameSave CreateBackup()
        {
            var backup = new SASGameSave
            {
                SaveName = $"{SaveName}_Backup",
                SaveTime = DateTime.Now,
                GameVersion = GameVersion,
                SaveVersion = SaveVersion,
                PlayTime = PlayTime,
                Difficulty = Difficulty,
                MapName = MapName,
                Player = Player?.Clone(),
                Economy = Economy?.Clone(),
                Waves = Waves?.Clone(),
                Towers = Towers?.Clone(),
                Enemies = Enemies?.Clone(),
                GameState = GameState?.Clone(),
                Statistics = Statistics?.Clone(),
                Achievements = Achievements?.Clone(),
                Unlocks = Unlocks?.Clone(),
                Metadata = Metadata?.Clone()
            };

            backup.Metadata.IsBackup = true;
            backup.Metadata.OriginalSaveName = SaveName;
            backup.Metadata.BackupTime = DateTime.Now;

            return backup;
        }

        ///  Private Methods

        static void CapturePlayerState(SASGameSave save)
        {
            // Fix PlayerLevel and PlayerLives references
            save.Player = new PlayerSaveData
            {
                Level = PlayerLevel.Instance?.CurrentLevel ?? 1,
                Experience = PlayerLevel.Instance?.CurrentExperience ?? 0,
                Lives = PlayerLives.Instance?.CurrentLives ?? 20,
                MaxLives = PlayerLives.Instance?.MaxLives ?? 20,
                Score = 0,
                Kills = 0
            };
        }

        static void CaptureEconomyState(SASGameSave save)
        {
            // Fix EconomyManager references
            save.Economy = new EconomySaveData
            {
                CurrentCash = EconomyManager.CurrentCash,
                TotalEarned = EconomyManager.TotalEarned,
                TotalSpent = EconomyManager.TotalSpent,
                TowerPurchases = new Dictionary<string, int>(),
                UpgradePurchases = EconomyManager.UpgradePurchases
            };
        }

        static void CaptureWaveState(SASGameSave save)
        {
            // Fix WaveDirector references
            var waveDirector = SASZombieAssaultTD.Engine.Waves.WaveDirector.Instance;

            save.Waves = new WaveSaveData
            {
                CurrentWave = waveDirector?.CurrentWave ?? 0,
                TotalWaves = waveDirector?.TotalWaves ?? 10,
                WaveProgress = waveDirector?.WaveProgress ?? 0f,
                OverallProgress = waveDirector?.OverallProgress ?? 0f,
                IsWaveActive = waveDirector?.IsWaveActive ?? false,
                CompletedWaves = new List<int>() // TODO: WaveDirector doesn't have CompletedWaves property
            };
        }

        static void CaptureTowerState(SASGameSave save)
        {
            // TODO: TowerUpgradeManager class doesn't exist - using placeholder
            save.Towers = new TowerSaveData
            {
                TowerCount = 0, // Placeholder
                TotalValue = 0, // Placeholder
                TowerTypes = new List<string>(), // Placeholder
                TowerPositions = new Dictionary<string, Vector3>(), // Placeholder
                TowerLevels = new Dictionary<string, int>() // Placeholder
            };
        }

        static void CaptureEnemyState(SASGameSave save)
        {
            // Implementation would capture enemy data
            save.Enemies = new EnemySaveData
            {
                TotalSpawned = EnemyManager.Instance?.GetTotalSpawned() ?? 0,
                TotalKilled = EnemyManager.Instance?.GetTotalKilled() ?? 0,
                TotalEscaped = EnemyManager.Instance?.GetTotalEscaped() ?? 0,
                ActiveEnemies = EnemyManager.Instance?.GetActiveEnemyCount() ?? 0
            };
        }

        static void CaptureGameState(SASGameSave save)
        {
            // Implementation would capture game state
            save.GameState = new GameStateSaveData
            {
                CurrentState = "Unknown", // TODO: Implement proper state machine access
                IsPaused = false,
                GameSpeed = 1.0f,
                AutoSaveEnabled = true,
                LastSaveTime = DateTime.Now
            };
        }

        static void CaptureStatistics(SASGameSave save)
        {
            // Implementation would capture statistics
            save.Statistics = new GameStatistics
            {
                TotalPlayTime = 0f,
                SessionsPlayed = 1,
                BestScore = 0,
                TotalKills = 0,
                TotalWavesCompleted = 0,
                FastestWaveCompletion = TimeSpan.Zero,
                HighestWave = 0
            };
        }

        static void CaptureAchievements(SASGameSave save)
        {
            // Implementation would capture achievements
            save.Achievements = new AchievementSaveData
            {
                UnlockedAchievements = new List<string>(),
                AchievementProgress = new Dictionary<string, float>(),
                TotalAchievements = 0,
                CompletionPercentage = 0f
            };
        }

        static void CaptureUnlocks(SASGameSave save)
        {
            // Implementation would capture unlocks
            save.Unlocks = new UnlockSaveData
            {
                UnlockedTowers = new List<string>(),
                UnlockedUpgrades = new List<string>(),
                UnlockedMaps = new List<string>(),
                UnlockedModes = new List<string>(),
                TotalUnlocks = 0
            };
        }

        static string GetCurrentMapName()
        {
            // Implementation would get current map name
            return "DefaultMap";
        }

        static string FormatPlayTime(TimeSpan time)
        {
            if (time.TotalHours > 0)
                return $"{time.Hours}h {time.Minutes}m";
            else if (time.TotalMinutes > 0)
                return $"{time.Minutes}m {time.Seconds}s";
            else
                return $"{time.Seconds}s";
        }

        /// 
    }

    /// <summary>
    /// Save metadata for additional information.
    /// </summary>
    public class SaveMetadata
    {
        public string Creator { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public bool IsBackup { get; set; }
        public string OriginalSaveName { get; set; }
        public DateTime? BackupTime { get; set; }
        public string Checksum { get; set; }
        public Dictionary<string, object> CustomData { get; set; }

        public SaveMetadata()
        {
            Tags = new List<string>();
            CustomData = new Dictionary<string, object>();
        }

        public SaveMetadata Clone()
        {
            return new SaveMetadata
            {
                Creator = Creator,
                Description = Description,
                Tags = new List<string>(Tags),
                IsBackup = IsBackup,
                OriginalSaveName = OriginalSaveName,
                BackupTime = BackupTime,
                Checksum = Checksum,
                CustomData = new Dictionary<string, object>(CustomData)
            };
        }
    }

    /// <summary>
    /// Player save data for SAS TD.
    /// </summary>
    public class PlayerSaveData
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Lives { get; set; }
        public int MaxLives { get; set; }
        public int Score { get; set; }
        public int Kills { get; set; }
        public int CurrentWave { get; set; }
        public float PlayTime { get; set; }
        public List<string> UnlockedTowers { get; set; }
        public Dictionary<string, int> TowerUpgrades { get; set; }

        public PlayerSaveData()
        {
            UnlockedTowers = new List<string>();
            TowerUpgrades = new Dictionary<string, int>();
        }

        public bool Validate() => Level >= 1 && Lives >= 0 && UnlockedTowers != null;

        public void ApplyToGame()
        {
            // Implementation to apply player data to the current game
        }

        public PlayerSaveData Clone()
        {
            return new PlayerSaveData
            {
                Level = Level,
                Experience = Experience,
                Lives = Lives,
                MaxLives = MaxLives,
                Score = Score,
                Kills = Kills,
                CurrentWave = CurrentWave,
                PlayTime = PlayTime,
                UnlockedTowers = new List<string>(UnlockedTowers),
                TowerUpgrades = new Dictionary<string, int>(TowerUpgrades)
            };
        }
    }

    /// <summary>
    /// Economy save data for SAS TD.
    /// </summary>
    public class EconomySaveData
    {
        public int CurrentCash { get; set; }
        public int TotalEarned { get; set; }
        public int TotalSpent { get; set; }
        public Dictionary<string, int> TowerPurchases { get; set; }
        public int UpgradePurchases { get; set; }

        public EconomySaveData()
        {
            CurrentCash = 100;
            TotalEarned = 0;
            TotalSpent = 0;
            TowerPurchases = new Dictionary<string, int>();
            UpgradePurchases = 0;
        }

        public bool Validate() => CurrentCash >= 0 && TotalEarned >= 0 && TowerPurchases != null;

        public void ApplyToGame()
        {
            // Implementation to apply economy data to the current game
        }

        public EconomySaveData Clone()
        {
            return new EconomySaveData
            {
                CurrentCash = CurrentCash,
                TotalEarned = TotalEarned,
                TotalSpent = TotalSpent,
                TowerPurchases = new Dictionary<string, int>(TowerPurchases),
                UpgradePurchases = UpgradePurchases
            };
        }
    }

    /// <summary>
    /// Game statistics for SAS TD.
    /// </summary>
    public class GameStatistics
    {
        public float TotalPlayTime { get; set; }
        public int SessionsPlayed { get; set; }
        public int BestScore { get; set; }
        public int TotalKills { get; set; }
        public int TotalWavesCompleted { get; set; }
        public TimeSpan FastestWaveCompletion { get; set; }
        public int HighestWave { get; set; }

        // Missing properties
        public int WaveReached { get; set; }
        public int EnemiesKilled { get; set; }
        public int TowersBuilt { get; set; }
        public int MoneyEarned { get; set; }
        public int MoneySpent { get; set; }
        public float TimePlayed { get; set; }
        public float Accuracy { get; set; }
        public int TowersLost { get; set; }
        public int LivesLost { get; set; }

        public GameStatistics()
        {
            TotalPlayTime = 0f;
            SessionsPlayed = 0;
            BestScore = 0;
            TotalKills = 0;
            TotalWavesCompleted = 0;
            FastestWaveCompletion = TimeSpan.Zero;
            HighestWave = 0;
        }

        public void ApplyToGame()
        {
            // Implementation to apply statistics to the current game
        }

        public GameStatistics Clone()
        {
            return new GameStatistics
            {
                TotalPlayTime = TotalPlayTime,
                SessionsPlayed = SessionsPlayed,
                BestScore = BestScore,
                TotalKills = TotalKills,
                TotalWavesCompleted = TotalWavesCompleted,
                FastestWaveCompletion = FastestWaveCompletion,
                HighestWave = HighestWave
            };
        }
    }

    /// <summary>
    /// Achievement save data for SAS TD.
    /// </summary>
    public class AchievementSaveData
    {
        public List<string> UnlockedAchievements { get; set; }
        public Dictionary<string, float> AchievementProgress { get; set; }
        public int TotalAchievements { get; set; }
        public float CompletionPercentage { get; set; }

        public AchievementSaveData()
        {
            UnlockedAchievements = new List<string>();
            AchievementProgress = new Dictionary<string, float>();
            TotalAchievements = 0;
            CompletionPercentage = 0f;
        }

        public void ApplyToGame()
        {
            // Implementation to apply achievements to the current game
        }

        public AchievementSaveData Clone()
        {
            return new AchievementSaveData
            {
                UnlockedAchievements = new List<string>(UnlockedAchievements),
                AchievementProgress = new Dictionary<string, float>(AchievementProgress),
                TotalAchievements = TotalAchievements,
                CompletionPercentage = CompletionPercentage
            };
        }
    }

    /// <summary>
    /// Unlock save data for SAS TD.
    /// </summary>
    public class UnlockSaveData
    {
        public List<string> UnlockedTowers { get; set; }
        public List<string> UnlockedUpgrades { get; set; }
        public List<string> UnlockedMaps { get; set; }
        public List<string> UnlockedModes { get; set; }
        public int TotalUnlocks { get; set; }

        public UnlockSaveData()
        {
            UnlockedTowers = new List<string>();
            UnlockedUpgrades = new List<string>();
            UnlockedMaps = new List<string>();
            UnlockedModes = new List<string>();
            TotalUnlocks = 0;
        }

        public void ApplyToGame()
        {
            // Implementation to apply unlocks to the current game
        }

        public UnlockSaveData Clone()
        {
            return new UnlockSaveData
            {
                UnlockedTowers = new List<string>(UnlockedTowers),
                UnlockedUpgrades = new List<string>(UnlockedUpgrades),
                UnlockedMaps = new List<string>(UnlockedMaps),
                UnlockedModes = new List<string>(UnlockedModes),
                TotalUnlocks = TotalUnlocks
            };
        }
    }

    /// <summary>
    /// Wave save data for SAS TD.
    /// </summary>
    public class WaveSaveData
    {
        public int CurrentWave { get; set; }
        public int TotalWaves { get; set; }
        public float WaveProgress { get; set; }
        public float OverallProgress { get; set; }
        public bool IsWaveActive { get; set; }
        public List<int> CompletedWaves { get; set; }

        public WaveSaveData()
        {
            CurrentWave = 0;
            TotalWaves = 0;
            WaveProgress = 0f;
            OverallProgress = 0f;
            IsWaveActive = false;
            CompletedWaves = new List<int>();
        }

        public bool Validate() => CurrentWave >= 0 && TotalWaves >= 0 && CompletedWaves != null;

        public void ApplyToGame()
        {
            // Implementation to apply wave data to the current game
        }

        public WaveSaveData Clone()
        {
            return new WaveSaveData
            {
                CurrentWave = CurrentWave,
                TotalWaves = TotalWaves,
                WaveProgress = WaveProgress,
                OverallProgress = OverallProgress,
                IsWaveActive = IsWaveActive,
                CompletedWaves = new List<int>(CompletedWaves)
            };
        }
    }

    /// <summary>
    /// Game state save data for SAS TD.
    /// </summary>
    public class GameStateSaveData
    {
        public string CurrentState { get; set; }
        public bool IsPaused { get; set; }
        public float GameSpeed { get; set; }
        public bool AutoSaveEnabled { get; set; }
        public DateTime LastSaveTime { get; set; }

        public GameStateSaveData()
        {
            CurrentState = "MainMenu";
            IsPaused = false;
            GameSpeed = 1.0f;
            AutoSaveEnabled = true;
            LastSaveTime = DateTime.Now;
        }

        public bool Validate() => !string.IsNullOrEmpty(CurrentState) && GameSpeed > 0;

        public void ApplyToGame()
        {
            // Implementation to apply game state to the current game
        }

        public GameStateSaveData Clone()
        {
            return new GameStateSaveData
            {
                CurrentState = CurrentState,
                IsPaused = IsPaused,
                GameSpeed = GameSpeed,
                AutoSaveEnabled = AutoSaveEnabled,
                LastSaveTime = LastSaveTime
            };
        }
    }
}