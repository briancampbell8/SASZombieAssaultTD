// =====================================================================================================
//  FILE: GSDataModels.cs
//  PATH: Engine/Save/GameSave/GSDataModels.cs
//  SUBSYSTEM: GameSave Data Models
//
//  ROLE:
//      GSDataModels defines all data‑only structures used by GSCore and the GS subsystem. These classes
//      contain no game logic, no capture logic, no apply logic, and no serialization logic. They are
//      pure containers for save‑state information.
//
//  RESPONSIBILITIES:
//      - Provide strongly‑typed containers for all save data categories.
//      - Support cloning for backup creation.
//      - Remain stable and predictable across engine versions.
//      - Maintain strict separation from GSCore, GSCapture, GSApply, GSValidation, and GSSerial.
//
//  NON-RESPONSIBILITIES:
//      - Capturing game state (handled by GSCapture).
//      - Applying save data (handled by GSApply).
//      - Validating save data (handled by GSValidation).
//      - Serializing save data (handled by GSSerial).
//      - Managing GSCore lifecycle or metadata.
//
//  ARCHITECTURAL NOTES:
//      - All classes are POCOs (Plain Old CLR Objects).
//      - All classes support cloning for backup creation.
//      - GSDataModels replaces the nested classes previously embedded in SASGameSave.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Save.GameSave
{
    // =================================================================================================
    // PLAYER SAVE DATA
    // =================================================================================================
    public class PlayerSaveData
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Lives { get; set; }
        public int MaxLives { get; set; }
        public int Score { get; set; }
        public int Kills { get; set; }

        public List<string> UnlockedTowers { get; set; } = new();

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
                UnlockedTowers = new List<string>(UnlockedTowers)
            };
        }
    }

    // =================================================================================================
    // ECONOMY SAVE DATA
    // =================================================================================================
    public class EconomySaveData
    {
        public int CurrentCash { get; set; }
        public int TotalEarned { get; set; }
        public int TotalSpent { get; set; }

        public Dictionary<string, int> TowerPurchases { get; set; } = new();
        public Dictionary<string, int> UpgradePurchases { get; set; } = new();

        public EconomySaveData Clone()
        {
            return new EconomySaveData
            {
                CurrentCash = CurrentCash,
                TotalEarned = TotalEarned,
                TotalSpent = TotalSpent,
                TowerPurchases = new Dictionary<string, int>(TowerPurchases),
                UpgradePurchases = new Dictionary<string, int>(UpgradePurchases)
            };
        }
    }

    // =================================================================================================
    // ENEMY SAVE DATA
    // =================================================================================================
    public class EnemySaveData
    {
        public int TotalSpawned { get; set; }
        public int TotalKilled { get; set; }
        public int TotalEscaped { get; set; }
        public int ActiveEnemies { get; set; }

        public EnemySaveData Clone()
        {
            return new EnemySaveData
            {
                TotalSpawned = TotalSpawned,
                TotalKilled = TotalKilled,
                TotalEscaped = TotalEscaped,
                ActiveEnemies = ActiveEnemies
            };
        }
    }

    // =================================================================================================
    // GAME STATE SAVE DATA
    // =================================================================================================
    public class GameStateSaveData
    {
        public string CurrentState { get; set; }
        public bool IsPaused { get; set; }
        public float GameSpeed { get; set; }
        public bool AutoSaveEnabled { get; set; }
        public DateTime LastSaveTime { get; set; }

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

    // =================================================================================================
    // GAME STATISTICS
    // =================================================================================================
    public class GameStatistics
    {
        public float TotalPlayTime { get; set; }
        public int SessionsPlayed { get; set; }
        public int BestScore { get; set; }
        public int TotalKills { get; set; }
        public int TotalWavesCompleted { get; set; }
        public TimeSpan FastestWaveCompletion { get; set; }
        public int HighestWave { get; set; }

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

    // =================================================================================================
    // ACHIEVEMENT SAVE DATA
    // =================================================================================================
    public class AchievementSaveData
    {
        public List<string> UnlockedAchievements { get; set; } = new();
        public Dictionary<string, float> AchievementProgress { get; set; } = new();
        public int TotalAchievements { get; set; }
        public float CompletionPercentage { get; set; }

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

    // =================================================================================================
    // UNLOCK SAVE DATA
    // =================================================================================================
    public class UnlockSaveData
    {
        public List<string> UnlockedTowers { get; set; } = new();
        public List<string> UnlockedUpgrades { get; set; } = new();
        public List<string> UnlockedMaps { get; set; } = new();
        public List<string> UnlockedModes { get; set; } = new();
        public int TotalUnlocks { get; set; }

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

    // =================================================================================================
    // SAVE METADATA
    // =================================================================================================
    public class SaveMetadata
    {
        public bool IsBackup { get; set; }
        public string OriginalSaveName { get; set; }
        public DateTime BackupTime { get; set; }

        public List<string> Tags { get; set; } = new();
        public Dictionary<string, string> CustomData { get; set; } = new();
        public string Description { get; internal set; }
        public object Creator { get; internal set; }

        public SaveMetadata Clone()
        {
            return new SaveMetadata
            {
                IsBackup = IsBackup,
                OriginalSaveName = OriginalSaveName,
                BackupTime = BackupTime,
                Tags = new List<string>(Tags),
                CustomData = new Dictionary<string, string>(CustomData)
            };
        }
    }
}
