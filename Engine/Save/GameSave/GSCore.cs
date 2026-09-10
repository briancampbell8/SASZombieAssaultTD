// =====================================================================================================
//  FILE: GSCore.cs
//  PATH: Engine/Save/GameSave/GSCore.cs
//  SUBSYSTEM: GameSave Core
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.Save;
using SASZombieAssaultTD.Engine.Waves;
using SASZombieAssaultTD.Engine.Waves.Difficulty;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Save.GameSave
{
    // ---------------------------------------------------------------------------------------------
    // Difficulty Manager (non-static class with static Instance)
    // ---------------------------------------------------------------------------------------------
    public sealed class GSCore
    {
        public static GSCore Instance { get; } = new GSCore();

        public int CurrentDifficulty => 1;


        // ---------------------------------------------------------------------------------------------
        // GSCore
        // ---------------------------------------------------------------------------------------------

        private object _saveData;

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
        public void GetSaveSize() { }

        // Statistics
        public GameStatistics Statistics { get; set; }

        public AchievementSaveData Achievements { get; set; }
        public UnlockSaveData Unlocks { get; set; }

        // Metadata
        public SaveMetadata Metadata { get; set; }

        // ---------------------------------------------------------------------------------------------
        // Construction
        // ---------------------------------------------------------------------------------------------
        public GSCore()
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

        // ---------------------------------------------------------------------------------------------
        // Create Save
        // ---------------------------------------------------------------------------------------------
        public static GSCore CreateFromCurrentState(string saveName)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSCore", 1, "Create",
                "Creating GSCore from current game state.");

            var difficultyManager = GSCore.Instance;

            var save = new GSCore
            {
                SaveName = saveName,
                SaveTime = DateTime.Now,
                Difficulty = difficultyManager.CurrentDifficulty.ToString(),
                MapName = GetCurrentMapName()
            };

            GSCapture.CapturePlayerState(save);
            GSCapture.CaptureEconomyState(save);
            GSCapture.CaptureWaveState(save);
            GSCapture.CaptureTowerState(save);
            GSCapture.CaptureEnemyState(save);
            GSCapture.CaptureGameState(save);
            GSCapture.CaptureStatistics(save);
            GSCapture.CaptureAchievements(save);
            GSCapture.CaptureUnlocks(save);

            return save;
        }

        // ---------------------------------------------------------------------------------------------
        // Apply Save
        // ---------------------------------------------------------------------------------------------
        public bool ApplyToCurrentState()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSCore", 2, "Apply",
                "Applying GSCore to current game state.");

            try
            {
                GSApply.ApplyPlayer(this);
                GSApply.ApplyEconomy(this);
                GSApply.ApplyWaves(this);
                GSApply.ApplyTowers(this);
                GSApply.ApplyEnemies(this);
                GSApply.ApplyGameState(this);
                GSApply.ApplyStatistics(this);
                GSApply.ApplyAchievements(this);
                GSApply.ApplyUnlocks(this);

                System.Diagnostics.Debug.WriteLine($"Applied save: {SaveName}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying save data: {ex.Message}");
                return false;
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Validation
        // ---------------------------------------------------------------------------------------------
        public bool ValidateSaveData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSCore", 3, "Validate",
                "Validating GSCore save data.");

            return GSValidation.Validate(this);
        }

        // ---------------------------------------------------------------------------------------------
        // Serialization
        // ---------------------------------------------------------------------------------------------
        public string SerializeToJson()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSCore", 4, "Serialize",
                "Serializing GSCore to JSON.");

            return GSSerial.Serialize(this);
        }

        public static GSCore DeserializeFromJson(string json)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSCore", 5, "Deserialize",
                "Deserializing GSCore from JSON.");

            var save = GSSerial.Deserialize(json);
            if (save != null && save.ValidateSaveData())
                return save;

            return null;
        }

        // ---------------------------------------------------------------------------------------------
        // Backup
        // ---------------------------------------------------------------------------------------------
        public GSCore CreateBackup()
        {
            var backup = new GSCore
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

        // ---------------------------------------------------------------------------------------------
        // Summary
        // ---------------------------------------------------------------------------------------------
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

        // ---------------------------------------------------------------------------------------------
        // Compatibility
        // ---------------------------------------------------------------------------------------------
        public bool IsCompatible()
        {
            return SaveVersion <= 1 && GameVersion == "1.0.0";
        }

        // ---------------------------------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------------------------------
        private static string GetCurrentMapName()
        {
            return "DefaultMap";
        }

        private static string FormatPlayTime(TimeSpan time)
        {
            if (time.TotalHours > 0)
                return $"{time.Hours}h {time.Minutes}m";
            else if (time.TotalMinutes > 0)
                return $"{time.Minutes}m {time.Seconds}s";
            else
                return $"{time.Seconds}s";
        }

        internal DifficultyMode GetCurrentDifficulty()
        {
            // The save stores difficulty as an integer (CurrentDifficulty).
            // Safely cast that integer to the DifficultyMode enum so callers receive the correct enum value.
            // If the integer value is outside the enum range, the cast will still produce a value; callers should handle unknown values if necessary.
            return (DifficultyMode)CurrentDifficulty;
        }
    }
}
