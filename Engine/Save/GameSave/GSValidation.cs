// =====================================================================================================
//  FILE: GSValidation.cs
//  PATH: Engine/Save/GameSave/GSValidation.cs
//  SUBSYSTEM: GameSave Validation
//
//  ROLE:
//      GSValidation provides deterministic validation routines for GSCore. It ensures that all save
//      data structures contain logically valid values before serialization, deserialization, or
//      application to the live game state.
//
//  RESPONSIBILITIES:
//      - Validate top‑level GSCore fields.
//      - Validate all nested data models (Player, Economy, Waves, Towers, Enemies, GameState,
//        Statistics, Achievements, Unlocks, Metadata).
//      - Guarantee structural integrity before GSCore is used by other GS modules.
//      - Prevent corrupted or incomplete save data from being applied.
//
//  NON-RESPONSIBILITIES:
//      - Capturing game state (handled by GSCapture).
//      - Applying save data (handled by GSApply).
//      - Serializing save data (handled by GSSerial).
//      - Managing GSCore lifecycle or metadata.
//
//  ARCHITECTURAL NOTES:
//      - All methods are static and operate on an existing GSCore instance.
//      - Validation routines must not mutate GSCore; they only inspect.
//      - GSValidation replaces the validation logic previously embedded in SASGameSave.
// =====================================================================================================
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Towers.Save;
using SASZombieAssaultTD.Engine.Waves;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.Save.GameSave
{
    /// <summary>
    /// Static validation routines for GSCore.
    /// </summary>
    public static class GSValidation
    {
        public static bool Validate(GSCore save)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "GameSave.GSValidation", 1, "Validate",
                "Validating GSCore save data.");

            if (save == null)
                return false;

            if (string.IsNullOrEmpty(save.SaveName))
                return false;

            if (save.SaveVersion < 1)
                return false;

            if (save.SaveTime == default)
                return false;

            if (!ValidatePlayer(save.Player))
                return false;

            if (!ValidateEconomy(save.Economy))
                return false;

            if (!ValidateWaves(save.Waves))
                return false;

            if (!ValidateTowers(save.Towers))
                return false;

            if (!ValidateEnemies(save.Enemies))
                return false;

            if (!ValidateGameState(save.GameState))
                return false;

            if (!ValidateStatistics(save.Statistics))
                return false;

            if (!ValidateAchievements(save.Achievements))
                return false;

            if (!ValidateUnlocks(save.Unlocks))
                return false;

            if (!ValidateMetadata(save.Metadata))
                return false;

            return true;
        }

        private static bool ValidatePlayer(PlayerSaveData p)
        {
            if (p == null)
                return false;

            if (p.Level < 1)
                return false;

            if (p.Lives < 0)
                return false;

            if (p.UnlockedTowers == null)
                return false;

            return true;
        }

        private static bool ValidateEconomy(EconomySaveData e)
        {
            if (e == null)
                return false;

            if (e.CurrentCash < 0)
                return false;

            if (e.TotalEarned < 0)
                return false;

            if (e.TowerPurchases == null)
                return false;

            return true;
        }

        private static bool ValidateWaves(WaveSaveData w)
        {
            if (w == null)
                return false;

            if (w.CurrentWave < 0)
                return false;

            if (w.TotalWaves < 0)
                return false;

            if (w.CompletedWaves == null)
                return false;

            return true;
        }

        private static bool ValidateTowers(TowerSaveData t)
        {
            if (t == null)
                return false;

            if (t.TowerTypes == null)
                return false;

            if (t.TowerPositions == null)
                return false;

            if (t.TowerLevels == null)
                return false;

            return true;
        }

        private static bool ValidateEnemies(EnemySaveData e)
        {
            if (e == null)
                return false;

            if (e.TotalSpawned < 0)
                return false;

            if (e.TotalKilled < 0)
                return false;

            if (e.TotalEscaped < 0)
                return false;

            if (e.ActiveEnemies < 0)
                return false;

            return true;
        }

        private static bool ValidateGameState(GameStateSaveData g)
        {
            if (g == null)
                return false;

            if (string.IsNullOrEmpty(g.CurrentState))
                return false;

            if (g.GameSpeed <= 0)
                return false;

            return true;
        }

        private static bool ValidateStatistics(GameStatistics s)
        {
            if (s == null)
                return false;

            if (s.TotalPlayTime < 0)
                return false;

            if (s.SessionsPlayed < 0)
                return false;

            if (s.TotalKills < 0)
                return false;

            if (s.TotalWavesCompleted < 0)
                return false;

            if (s.HighestWave < 0)
                return false;

            return true;
        }

        private static bool ValidateAchievements(AchievementSaveData a)
        {
            if (a == null)
                return false;

            if (a.UnlockedAchievements == null)
                return false;

            if (a.AchievementProgress == null)
                return false;

            if (a.TotalAchievements < 0)
                return false;

            return true;
        }

        private static bool ValidateUnlocks(UnlockSaveData u)
        {
            if (u == null)
                return false;

            if (u.UnlockedTowers == null)
                return false;

            if (u.UnlockedUpgrades == null)
                return false;

            if (u.UnlockedMaps == null)
                return false;

            if (u.UnlockedModes == null)
                return false;

            if (u.TotalUnlocks < 0)
                return false;

            return true;
        }

        private static bool ValidateMetadata(SaveMetadata m)
        {
            if (m == null)
                return false;

            if (m.Tags == null)
                return false;

            if (m.CustomData == null)
                return false;

            return true;
        }
    }
}
