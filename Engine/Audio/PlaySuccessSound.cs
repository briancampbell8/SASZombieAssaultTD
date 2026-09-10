// ====================================================================================================
//  FILE: PlaySuccessSound.cs
//  PATH: ./Engine/Audio/
//  MODULE: Audio
//
//  ROLE:
//      Manage audio playback, mixing, or spatial sound behavior.
//
//  RESPONSIBILITIES:
//      - Provide Play() behavior for the Audio subsystem.
//      - Provide PlayTowerPurchase() behavior for the Audio subsystem.
//      - Provide PlayUpgradePurchase() behavior for the Audio subsystem.
//      - Provide PlayLevelUp() behavior for the Audio subsystem.
//      - Provide PlayAchievementUnlocked() behavior for the Audio subsystem.
//      - Provide PlayWaveCompleted() behavior for the Audio subsystem.
//      - Provide PlayGameCompleted() behavior for the Audio subsystem.
//      - Provide PlayAbilityUnlocked() behavior for the Audio subsystem.
//      - Provide Play() behavior for the Audio subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    PlaySuccessSound.cs
Purpose: Plays a standardized success sound for upgrades and purchases.
*/

using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS
//
{
    /// <summary>
    /// Plays standardized success sounds for upgrades and purchases.
    /// </summary>
    public static class PlaySuccessSound
    {
        /// <summary>
        /// Plays a generic success sound.
        /// </summary>
        public static void Play()
        {
            PlaySound.Play("success_generic");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played generic success sound");
        }

        /// <summary>
        /// Plays a success sound for tower purchase.
        /// </summary>
        public static void PlayTowerPurchase()
        {
            PlaySound.Play("success_tower_purchase");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played tower purchase success sound");
        }

        /// <summary>
        /// Plays a success sound for upgrade purchase.
        /// </summary>
        public static void PlayUpgradePurchase()
        {
            PlaySound.Play("success_upgrade_purchase");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played upgrade purchase success sound");
        }

        /// <summary>
        /// Plays a success sound for level up.
        /// </summary>
        public static void PlayLevelUp()
        {
            PlaySound.Play("success_level_up");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played level up success sound");
        }

        /// <summary>
        /// Plays a success sound for achievement unlocked.
        /// </summary>
        public static void PlayAchievementUnlocked()
        {
            PlaySound.Play("success_achievement");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played achievement success sound");
        }

        /// <summary>
        /// Plays a success sound for wave completed.
        /// </summary>
        public static void PlayWaveCompleted()
        {
            PlaySound.Play("success_wave_complete");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played wave complete success sound");
        }

        /// <summary>
        /// Plays a success sound for game completed.
        /// </summary>
        public static void PlayGameCompleted()
        {
            PlaySound.Play("success_game_complete");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played game complete success sound");
        }

        /// <summary>
        /// Plays a success sound for ability unlocked.
        /// </summary>
        public static void PlayAbilityUnlocked()
        {
            PlaySound.Play("success_ability_unlock");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, "PlaySuccessSound: Played ability unlock success sound");
        }

        /// <summary>
        /// Plays a specific success sound by name.
        /// </summary>
        /// <param name="successType">Type of success sound to play.</param>
        public static void Play(string successType)
        {
            PlaySound.Play($"success_{successType}");
            DLogger.Log(LogSubsystems.Save, LogLevel.Info, $"PlaySuccessSound: Played success sound for '{successType}'");
        }
    }
}
