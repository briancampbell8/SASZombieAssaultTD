// ====================================================================================================
//  FILE: AudioIntegration.cs
//  PATH: Engine/Towers/TowerPlacement/AudioIntegration.cs
//  MODULE: Tower System (Audio Integration)
//
//  ROLE:
//      Provides audio cues for tower placement, sell, and upgrade events.
//
//  RESPONSIBILITIES:
//      - Play placement success/deny sounds and upgrade/sell feedback via ModernPlaySound.
//      - Forward positional audio when available using ModernPlaySound.PlayAtPosition.
//
//  NON-RESPONSIBILITIES:
//      - Managing tower placement logic or validation (TowerPlacement subsystem handles that).
//
//  ARCHITECTURAL NOTES:
//      - Lightweight, event-driven helper intended to be safe to call from gameplay code.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.TowerPlacement
{
    ///<summary>
    ///Audio integration for the tower placement system.
    ///P90-10: Tower placement audio integration with ModernAudioSubsystem
    ///</summary>
    public static class AudioIntegration
    {
        ///<summary>
        ///Plays tower placement success sound.
        ///</summary>
        public static void PlayTowerPlacementSuccess(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_place", position);
            ModernPlaySound.Play("success_tower_purchase");
            DLogger.Log($"TowerPlacementAudioIntegration: Played tower placement success at {position}");
        }

        ///<summary>
        ///Plays tower placement deny sound.
        ///</summary>
        public static void PlayTowerPlacementDeny()
        {
            ModernPlaySound.Play("error_invalid_placement");
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerPlacementAudioIntegration: Played tower placement deny");
        }

        ///<summary>
        ///Plays tower sell sound.
        ///</summary>
        public static void PlayTowerSell(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_sell", position);
            DLogger.Log($"TowerPlacementAudioIntegration: Played tower sell at {position}");
        }

        ///<summary>
        ///Plays tower upgrade success sound.
        ///</summary>
        public static void PlayTowerUpgradeSuccess(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_place", position);
            ModernPlaySound.Play("success_upgrade_purchase");
            DLogger.Log($"TowerPlacementAudioIntegration: Played tower upgrade success at {position}");
        }

        ///<summary>
        ///Plays tower upgrade deny sound.
        ///</summary>
        public static void PlayTowerUpgradeDeny()
        {
            ModernPlaySound.Play("error_upgrade_unavailable");
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerPlacementAudioIntegration: Played tower upgrade deny");
        }

        ///<summary>
        ///Plays insufficient funds sound.
        ///</summary>
        public static void PlayInsufficientFunds()
        {
            ModernPlaySound.Play("error_insufficient_funds");
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerPlacementAudioIntegration: Played insufficient funds");
        }

        ///<summary>
        ///Plays tower limit reached sound.
        ///</summary>
        public static void PlayTowerLimitReached()
        {
            ModernPlaySound.Play("error_tower_limit");
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerPlacementAudioIntegration: Played tower limit reached");
        }
    }
}
