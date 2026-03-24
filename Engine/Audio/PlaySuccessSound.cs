/*
File:    PlaySuccessSound.cs
Purpose: Plays a standardized success sound for upgrades and purchases.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Audio
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
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played generic success sound");
        }
        
        /// <summary>
        /// Plays a success sound for tower purchase.
        /// </summary>
        public static void PlayTowerPurchase()
        {
            PlaySound.Play("success_tower_purchase");
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played tower purchase success sound");
        }
        
        /// <summary>
        /// Plays a success sound for upgrade purchase.
        /// </summary>
        public static void PlayUpgradePurchase()
        {
            PlaySound.Play("success_upgrade_purchase");
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played upgrade purchase success sound");
        }
        
        /// <summary>
        /// Plays a success sound for level up.
        /// </summary>
        public static void PlayLevelUp()
        {
            PlaySound.Play("success_level_up");
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played level up success sound");
        }
        
        /// <summary>
        /// Plays a success sound for achievement unlocked.
        /// </summary>
        public static void PlayAchievementUnlocked()
        {
            PlaySound.Play("success_achievement");
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played achievement success sound");
        }
        
        /// <summary>
        /// Plays a success sound for wave completed.
        /// </summary>
        public static void PlayWaveCompleted()
        {
            PlaySound.Play("success_wave_complete");
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played wave complete success sound");
        }
        
        /// <summary>
        /// Plays a success sound for game completed.
        /// </summary>
        public static void PlayGameCompleted()
        {
            PlaySound.Play("success_game_complete");
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played game complete success sound");
        }
        
        /// <summary>
        /// Plays a success sound for ability unlocked.
        /// </summary>
        public static void PlayAbilityUnlocked()
        {
            PlaySound.Play("success_ability_unlock");
            ModernLoggingSystem.Log("INFO", "PlaySuccessSound: Played ability unlock success sound");
        }
        
        /// <summary>
        /// Plays a specific success sound by name.
        /// </summary>
        /// <param name="successType">Type of success sound to play.</param>
        public static void Play(string successType)
        {
            PlaySound.Play($"success_{successType}");
            ModernLoggingSystem.Log("INFO", $"PlaySuccessSound: Played success sound for '{successType}'");
        }
    }
}