using SASZombieAssaultTD.Engine.Economy;
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
    /// 
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
        /// <summary>
        /// Applies the loaded save data to the current game state.
        /// Iterates through all save data modules and calls their respective `ApplyToGame` methods.
        /// </summary>
        /// <returns>True if the save data was successfully applied; otherwise, false.</returns>
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
                // TODO: The 'tower' parameter is initialized to null, leading to a NullReferenceException if dereferenced.
                // Revisit the signature of Towers.ApplyToGame or how a specific tower instance should be passed/handled.
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

                Console.WriteLine($"Applied save: {SaveName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying save data: {ex.Message}");
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

            // Safely retrieves current difficulty or defaults to "Normal" if unavailable.
            save.Difficulty = difficultyManager.CurrentDifficulty

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
                    WriteIndented = true, // Makes the JSON output human-readable with indentation.
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Converts C# PascalCase properties to camelCase in JSON.
                };

                return JsonSerializer.Serialize(this, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serializing save data: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserialize save data from JSON.
        /// </summary>
        /// <param name="json">JSON string to deserialize.</param>
        /// <returns>Deserialized save data, or null if deserialization fails or the data is invalid.</returns>
        public static SASGameSave DeserializeFromJson(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Allows deserialization from JSON with different casing (e.g., camelCase to PascalCase).
                };

                var save = JsonSerializer.Deserialize<SASGameSave>(json, options);

                // Validate save data after deserialization to ensure integrity.
                if (save != null && save.ValidateSaveData())
                {
                    return save;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing save data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validate save data integrity.
        /// Performs basic checks on core fields and calls validation on all nested save data objects.
        /// </summary>
        /// <returns>True if save data is valid.</returns>
        public bool ValidateSaveData()
        {
            // Check basic fields
            if (string.IsNullOrEmpty(SaveName))
                return false;

            if (SaveVersion < 1) return false;

            if (SaveTime == default) return false;

            // Validate nested data
            if (Player?.Validate() == false)
                return false;

            if (Economy?.Validate() == false) return false;

            if (Waves?.Validate() == false) return false;

            if (Towers?.Validate() == false) return false;

            if (Enemies?.Validate() == false) return false;

            if (GameState?.Validate() == false) return false;

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
        /// Performs a deep clone of all nested save data objects to ensure the backup is an independent copy.
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
                // Clones all nested mutable objects to ensure the backup is independent.
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

        #region Private Methods

        /// <summary>
        /// Captures the current player state from game singletons and populates the save object.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CapturePlayerState(SASGameSave save)
        {
            // Fix PlayerLevel and PlayerLives references
            // Safely retrieves current player data from singleton instances or defaults to initial values.
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

        /// <summary>
        /// Captures the current economy state from the EconomyManager singleton and populates the save object.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureEconomyState(SASGameSave save)
        {
            // Fix EconomyManager references
            // Safely retrieves current economy data from the singleton instance or defaults to initial values.
            save.Economy = new EconomySaveData
            {
                CurrentCash = EconomyManager.CurrentCash,
                TotalEarned = EconomyManager.TotalEarned,
                TotalSpent = EconomyManager.TotalSpent,
                TowerPurchases = new Dictionary<string, int>(), // TODO: EconomyManager should expose actual tower purchase data.
                UpgradePurchases = EconomyManager.UpgradePurchases
            };
        }

        /// <summary>
        /// Captures the current wave state from the WaveDirector singleton and populates the save object.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureWaveState(SASGameSave save)
        {
            // Fix WaveDirector references
            var waveDirector = SASZombieAssaultTD.Engine.Waves.WaveDirector.Instance;

            // Safely retrieves current wave data from the singleton instance or defaults to initial values.
            save.Waves = new WaveSaveData
            {
                CurrentWave = waveDirector?.CurrentWave ?? 0,
                TotalWaves = waveDirector?.TotalWaves ?? 10,
                WaveProgress = waveDirector?.WaveProgress ?? 0f,
                OverallProgress = waveDirector?.OverallProgress ?? 0f,
                IsWaveActive = waveDirector?.IsWaveActive ?? false,
                CompletedWaves = new List<int>() // TODO: WaveDirector doesn't have CompletedWaves property; needs to be implemented.
            };
        }

        /// <summary>
        /// Captures the current tower state. This is currently a placeholder implementation.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureTowerState(SASGameSave save)
        {
            // TODO: TowerUpgradeManager class doesn't exist - using placeholder
            // This method requires implementation to capture actual tower data from the game world.
            save.Towers = new TowerSaveData
            {
                TowerCount = 0, // Placeholder
                TotalValue = 0, // Placeholder
                TowerTypes = new List<string>(), // Placeholder
                TowerPositions = new Dictionary<string, Vector3>(), // Placeholder
                TowerLevels = new Dictionary<string, int>() // Placeholder
            };
        }

        /// <summary>
        /// Captures the current enemy statistics from the EnemyManager singleton and populates the save object.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureEnemyState(SASGameSave save)
        {
            // Implementation would capture enemy data
            // Safely retrieves enemy statistics from the singleton instance or defaults to initial values.
            save.Enemies = new EnemySaveData
            {
                TotalSpawned = EnemyManager.Instance?.GetTotalSpawned() ?? 0,
                TotalKilled = EnemyManager.Instance?.GetTotalKilled() ?? 0,
                TotalEscaped = EnemyManager.Instance?.GetTotalEscaped() ?? 0,
                ActiveEnemies = EnemyManager.Instance?.GetActiveEnemyCount() ?? 0
            };
        }

        /// <summary>
        /// Captures the current general game state. This is currently a placeholder implementation.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureGameState(SASGameSave save)
        {
            // Implementation would capture game state
            // This method requires implementation to capture actual game state data (e.g., from a game state machine).
            save.GameState = new GameStateSaveData
            {
                CurrentState = "Unknown", // TODO: Implement proper state machine access to get the current game state.
                IsPaused = false,
                GameSpeed = 1.0f,
                AutoSaveEnabled = true,
                LastSaveTime = DateTime.Now
            };
        }

        /// <summary>
        /// Captures the current overall game statistics. This is currently a placeholder implementation.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureStatistics(SASGameSave save)
        {
            // Implementation would capture statistics
            // This method requires implementation to capture actual game statistics from a statistics manager.
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

        /// <summary>
        /// Captures the current achievement state. This is currently a placeholder implementation.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureAchievements(SASGameSave save)
        {
            // Implementation would capture achievements
            // This method requires implementation to capture actual achievement data from an achievement system.
            save.Achievements = new AchievementSaveData
            {
                UnlockedAchievements = new List<string>(),
                AchievementProgress = new Dictionary<string, float>(),
                TotalAchievements = 0,
                CompletionPercentage = 0f
            };
        }

        /// <summary>
        /// Captures the current unlock state. This is currently a placeholder implementation.
        /// </summary>
        /// <param name="save">The save data object to populate.</param>
        static void CaptureUnlocks(SASGameSave save)
        {
            // Implementation would capture unlocks
            // This method requires implementation to capture actual unlock data from an unlock system.
            save.Unlocks = new UnlockSaveData
            {
                UnlockedTowers = new List<string>(),
                UnlockedUpgrades = new List<string>(),
                UnlockedMaps = new List<string>(),
                UnlockedModes = new List<string>(),
                TotalUnlocks = 0
            };
        }

        /// <summary>
        /// Retrieves the name of the currently active map. This is currently a placeholder.
        /// </summary>
        /// <returns>The current map name.</returns>
        static string GetCurrentMapName()
        {
            // Implementation would get current map name
            return "DefaultMap";
        }

        /// <summary>
        /// Formats a TimeSpan into a human-readable string (e.g., "1h 30m", "5m 15s", "45s").
        /// Uses pattern matching for clear time unit selection.
        /// </summary>
        /// <param name="time">The TimeSpan to format.</param>
        /// <returns>A formatted string representing the play time.</returns>
        static string FormatPlayTime(TimeSpan time)
        {
            return time.TotalHours switch
            {
                > 0 => $"{time.Hours}h {time.Minutes}m",
                _ => time.TotalMinutes switch
                {
                    > 0 => $"{time.Minutes}m {time.Seconds}s",
                    _ => $"{time.Seconds}s"
                }
            };
        }

        #endregion
    }

    /// <summary>
    /// Save metadata for additional information about a saved game.
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

        /// <summary>
        /// Creates a deep clone of the SaveMetadata object.
        /// Ensures new lists and dictionaries are created to prevent shared references.
        /// </summary>
        /// <returns>A new SaveMetadata instance with copied data.</returns>
        public SaveMetadata Clone()
        {
            return new SaveMetadata
            {
                Creator = Creator,
                Description = Description,
                Tags = new List<string>(Tags), // Deep copy
                IsBackup = IsBackup,
                OriginalSaveName = OriginalSaveName,
                BackupTime = BackupTime,
                Checksum = Checksum,
                CustomData = new Dictionary<string, object>(CustomData) // Deep copy
            };
        }
    }

    /// <summary>
    /// Player save data for SAS TD.
    /// Contains stats and progress specific to the player.
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

        /// <summary>
        /// Validates the player save data for basic integrity.
        /// </summary>
        /// <returns>True if the data is considered valid.</returns>
        public bool Validate() => Level >= 1 && Lives >= 0 && UnlockedTowers != null;

        public void ApplyToGame()
        {
            // Implementation to apply player data to the current game
        }

        /// <summary>
        /// Creates a deep clone of the PlayerSaveData object.
        /// Ensures new lists and dictionaries are created to prevent shared references.
        /// </summary>
        /// <returns>A new PlayerSaveData instance with copied data.</returns>
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
                UnlockedTowers = new List<string>(UnlockedTowers), // Deep copy
                TowerUpgrades = new Dictionary<string, int>(TowerUpgrades) // Deep copy
            };
        }
    }

    /// <summary>
    /// Economy save data for SAS TD.
    /// Stores cash, earnings, spending, and purchase history.
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

        /// <summary>
        /// Validates the economy save data for basic integrity.
        /// </summary>
        /// <returns>True if the data is considered valid.</returns>
        public bool Validate() => CurrentCash >= 0 && TotalEarned >= 0 && TowerPurchases != null;

        public void ApplyToGame()
        {
            // Implementation to apply economy data to the current game
        }

        /// <summary>
        /// Creates a deep clone of the EconomySaveData object.
        /// Ensures new lists and dictionaries are created to prevent shared references.
        /// </summary>
        /// <returns>A new EconomySaveData instance with copied data.</returns>
        public EconomySaveData Clone()
        {
            return new EconomySaveData
            {
                CurrentCash = CurrentCash,
                TotalEarned = TotalEarned,
                TotalSpent = TotalSpent,
                TowerPurchases = new Dictionary<string, int>(TowerPurchases), // Deep copy
                UpgradePurchases = UpgradePurchases
            };
        }
    }

    /// <summary>
    /// Game statistics for SAS TD.
    /// Records various metrics about the player's performance across sessions.
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

        // Additional statistics properties that might be needed in a complete implementation.
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

        /// <summary>
        /// Creates a shallow clone of the GameStatistics object.
        /// As all properties are value types or immutable, a shallow copy is sufficient.
        /// </summary>
        /// <returns>A new GameStatistics instance with copied data.</returns>
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
    /// Tracks unlocked achievements and progress towards others.
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

        /// <summary>
        /// Creates a deep clone of the AchievementSaveData object.
        /// Ensures new lists and dictionaries are created to prevent shared references.
        /// </summary>
        /// <returns>A new AchievementSaveData instance with copied data.</returns>
        public AchievementSaveData Clone()
        {
            return new AchievementSaveData
            {
                UnlockedAchievements = new List<string>(UnlockedAchievements), // Deep copy
                AchievementProgress = new Dictionary<string, float>(AchievementProgress), // Deep copy
                TotalAchievements = TotalAchievements,
                CompletionPercentage = CompletionPercentage
            };
        }
    }

    /// <summary>
    /// Unlock save data for SAS TD.
    /// Tracks which game elements (towers, upgrades, maps, modes) the player has unlocked.
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

        /// <summary>
        /// Creates a deep clone of the UnlockSaveData object.
        /// Ensures new lists are created to prevent shared references.
        /// </summary>
        /// <returns>A new UnlockSaveData instance with copied data.</returns>
        public UnlockSaveData Clone()
        {
            return new UnlockSaveData
            {
                UnlockedTowers = new List<string>(UnlockedTowers), // Deep copy
                UnlockedUpgrades = new List<string>(UnlockedUpgrades), // Deep copy
                UnlockedMaps = new List<string>(UnlockedMaps), // Deep copy
                UnlockedModes = new List<string>(UnlockedModes), // Deep copy
                TotalUnlocks = TotalUnlocks
            };
        }
    }

    /// <summary>
    /// Wave save data for SAS TD.
    /// Stores the current state of the game's wave progression.
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

        /// <summary>
        /// Validates the wave save data for basic integrity.
        /// </summary>
        /// <returns>True if the data is considered valid.</returns>
        public bool Validate() => CurrentWave >= 0 && TotalWaves >= 0 && CompletedWaves != null;

        public void ApplyToGame()
        {
            // Implementation to apply wave data to the current game
        }

        /// <summary>
        /// Creates a deep clone of the WaveSaveData object.
        /// Ensures a new list is created to prevent shared references.
        /// </summary>
        /// <returns>A new WaveSaveData instance with copied data.</returns>
        public WaveSaveData Clone()
        {
            return new WaveSaveData
            {
                CurrentWave = CurrentWave,
                TotalWaves = TotalWaves,
                WaveProgress = WaveProgress,
                OverallProgress = OverallProgress,
                IsWaveActive = IsWaveActive,
                CompletedWaves = new List<int>(CompletedWaves) // Deep copy
            };
        }
    }

    /// <summary>
    /// Game state save data for SAS TD.
    /// Stores critical real-time game state information like pause status and speed.
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

        /// <summary>
        /// Validates the game state save data for basic integrity.
        /// </summary>
        /// <returns>True if the data is considered valid.</returns>
        public bool Validate() => !string.IsNullOrEmpty(CurrentState) && GameSpeed > 0;

        public void ApplyToGame()
        {
            // Implementation to apply game state to the current game
        }

        /// <summary>
        /// Creates a shallow clone of the GameStateSaveData object.
        /// As all properties are value types or immutable, a shallow copy is sufficient.
        /// </summary>
        /// <returns>A new GameStateSaveData instance with copied data.</returns>
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