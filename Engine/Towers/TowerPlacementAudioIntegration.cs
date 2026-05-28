/*
Program Name: SASZombieAssaultTD
File Path: Engine\Towers\TowerPlacementAudioIntegration.cs
Purpose: P90 Modern Audio Subsystem - Audio integration for tower placement system.
Features: Tower placement, placement deny, tower sell, and tower upgrade audio.
*/

using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Audio integration for the tower placement system.
    /// P90-10: Tower placement audio integration with ModernAudioSubsystem
    /// </summary>
    public static class TowerPlacementAudioIntegration
    {
        /// <summary>
        /// Plays tower placement success sound.
        /// </summary>
        public static void PlayTowerPlacementSuccess(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_place", position);
            ModernPlaySound.Play("success_tower_purchase");
            System.Diagnostics.Debug.WriteLine($"TowerPlacementAudioIntegration: Played tower placement success at {position}");
        }

        /// <summary>
        /// Plays tower placement deny sound.
        /// </summary>
        public static void PlayTowerPlacementDeny()
        {
            ModernPlaySound.Play("error_invalid_placement");
            System.Diagnostics.Debug.WriteLine("TowerPlacementAudioIntegration: Played tower placement deny");
        }

        /// <summary>
        /// Plays tower sell sound.
        /// </summary>
        public static void PlayTowerSell(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_sell", position);
            System.Diagnostics.Debug.WriteLine($"TowerPlacementAudioIntegration: Played tower sell at {position}");
        }

        /// <summary>
        /// Plays tower upgrade success sound.
        /// </summary>
        public static void PlayTowerUpgradeSuccess(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_place", position);
            ModernPlaySound.Play("success_upgrade_purchase");
            System.Diagnostics.Debug.WriteLine($"TowerPlacementAudioIntegration: Played tower upgrade success at {position}");
        }

        /// <summary>
        /// Plays tower upgrade deny sound.
        /// </summary>
        public static void PlayTowerUpgradeDeny()
        {
            ModernPlaySound.Play("error_upgrade_unavailable");
            System.Diagnostics.Debug.WriteLine("TowerPlacementAudioIntegration: Played tower upgrade deny");
        }

        /// <summary>
        /// Plays insufficient funds sound.
        /// </summary>
        public static void PlayInsufficientFunds()
        {
            ModernPlaySound.Play("error_insufficient_funds");
            System.Diagnostics.Debug.WriteLine("TowerPlacementAudioIntegration: Played insufficient funds");
        }

        /// <summary>
        /// Plays tower limit reached sound.
        /// </summary>
        public static void PlayTowerLimitReached()
        {
            ModernPlaySound.Play("error_tower_limit");
            System.Diagnostics.Debug.WriteLine("TowerPlacementAudioIntegration: Played tower limit reached");
        }
    }
}
