// =====================================================================================================
//  FILE: GSCapture.cs
//  PATH: Engine/Save/GameSave/GSCapture.cs
//  SUBSYSTEM: GameSave Capture
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Economy;
using SASZombieAssaultTD.Engine.Enemies;
using SASZombieAssaultTD.Engine.LevelUpControl;
using SASZombieAssaultTD.Engine.Towers.Save;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Waves;
using SASZombieAssaultTD.Engine.Waves.WaveManagement;

namespace SASZombieAssaultTD.Engine.Save.GameSave
{
    public static class GSCapture
    {
        // -------------------------------------------------------------------------------------------------
        // PLAYER
        // -------------------------------------------------------------------------------------------------
        public static void CapturePlayerState(GSCore save)
        {
            save.Player = new PlayerSaveData
            {
                Level = PlayerLevel.Instance?.CurrentLevel ?? 1,
                Experience = PlayerLevel.Instance?.CurrentExperience ?? 0,
                Lives = PlayerLives.Instance?.CurrentLives ?? 20,
                MaxLives = PlayerLives.Instance?.MaxLives ?? 20,
                Score = 0,
                Kills = 0,
                UnlockedTowers = new List<string>()
            };
        }

        // -------------------------------------------------------------------------------------------------
        // ECONOMY
        // -------------------------------------------------------------------------------------------------
        public static void CaptureEconomyState(GSCore save)
        {
            save.Economy = new EconomySaveData
            {
                CurrentCash = EconomyManager.CurrentCash,
                TotalEarned = EconomyManager.TotalEarned,
                TotalSpent = EconomyManager.TotalSpent,

                TowerPurchases = new Dictionary<string, int>(),

                UpgradePurchases = new Dictionary<string, int>
                {
                    { "TotalUpgrades", EconomyManager.UpgradePurchases }
                }
            };
        }

        // -------------------------------------------------------------------------------------------------
        // WAVES — Using ONLY real WaveDirectorCore API
        // -------------------------------------------------------------------------------------------------
        public static void CaptureWaveState(GSCore save)
        {
            var wd = WaveDirectorCore.Instance;

            if (wd == null)
            {
                save.Waves = new WaveSaveData
                {
                    CurrentWave = 0,
                    TotalWaves = 0,
                    WaveProgress = 0f,
                    OverallProgress = 0f,
                    IsWaveActive = false,
                    CompletedWaves = new List<int>()
                };
                return;
            }

            save.Waves = new WaveSaveData
            {
                CurrentWave = wd.CurrentWave,
                TotalWaves = wd.TotalWaves,
                WaveProgress = wd.WaveProgress,
                OverallProgress = wd.OverallProgress,

                // WaveState enum is internal → use public bool instead
                IsWaveActive = wd.InProgress,

                CompletedWaves = new List<int>()
            };
        }

        // -------------------------------------------------------------------------------------------------
        // TOWERS (placeholder)
        // -------------------------------------------------------------------------------------------------
        public static void CaptureTowerState(GSCore save)
        {
            save.Towers = new TowerSaveData
            {
                TowerCount = 0,
                TotalValue = 0,
                TowerTypes = new List<string>(),
                TowerPositions = new Dictionary<string, Vector3>(),
                TowerLevels = new Dictionary<string, int>()
            };
        }

        // -------------------------------------------------------------------------------------------------
        // ENEMIES — Cast Instance properly
        // -------------------------------------------------------------------------------------------------
        public static void CaptureEnemyState(GSCore save)
        {
            var em = EnemyManager.Instance as EnemyManager;

            save.Enemies = new EnemySaveData
            {
                TotalSpawned = em?.GetTotalSpawned() ?? 0,
                TotalKilled = em?.GetTotalKilled() ?? 0,
                TotalEscaped = em?.GetTotalEscaped() ?? 0,
                ActiveEnemies = em?.GetActiveEnemyCount() ?? 0
            };
        }

        // -------------------------------------------------------------------------------------------------
        // GAME STATE
        // -------------------------------------------------------------------------------------------------
        public static void CaptureGameState(GSCore save)
        {
            save.GameState = new GameStateSaveData
            {
                CurrentState = "Unknown",
                IsPaused = false,
                GameSpeed = 1.0f,
                AutoSaveEnabled = true,
                LastSaveTime = DateTime.Now
            };
        }

        // -------------------------------------------------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------------------------------------------------
        public static void CaptureStatistics(GSCore save)
        {
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

        // -------------------------------------------------------------------------------------------------
        // ACHIEVEMENTS
        // -------------------------------------------------------------------------------------------------
        public static void CaptureAchievements(GSCore save)
        {
            save.Achievements = new AchievementSaveData
            {
                UnlockedAchievements = new List<string>(),
                AchievementProgress = new Dictionary<string, float>(),
                TotalAchievements = 0,
                CompletionPercentage = 0f
            };
        }

        // -------------------------------------------------------------------------------------------------
        // UNLOCKS
        // -------------------------------------------------------------------------------------------------
        public static void CaptureUnlocks(GSCore save)
        {
            save.Unlocks = new UnlockSaveData
            {
                UnlockedTowers = new List<string>(),
                UnlockedUpgrades = new List<string>(),
                UnlockedMaps = new List<string>(),
                UnlockedModes = new List<string>(),
                TotalUnlocks = 0
            };
        }
    }
}
