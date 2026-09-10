// =====================================================================================================
//  FILE: AchievementSaveData.cs
//  PATH: Engine/Achievements/AchievementSaveData.cs
//  SUBSYSTEM: Achievements
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Achievements
{
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
            //Implementation to apply achievements to the current game
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

}
