/*
File:    PlayErrorSound.cs
Purpose: Plays a standardized error sound for invalid actions.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Audio
{
    /// <summary>
    /// Plays standardized error sounds for invalid actions.
    /// </summary>
    public static class PlayErrorSound
    {
        /// <summary>
        /// Plays a generic error sound.
        /// </summary>
        public static void Play()
        {
            PlaySound.Play("error_generic");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played generic error sound");
        }
        
        /// <summary>
        /// Plays an error sound for insufficient funds.
        /// </summary>
        public static void PlayInsufficientFunds()
        {
            PlaySound.Play("error_insufficient_funds");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played insufficient funds error sound");
        }
        
        /// <summary>
        /// Plays an error sound for invalid placement.
        /// </summary>
        public static void PlayInvalidPlacement()
        {
            PlaySound.Play("error_invalid_placement");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played invalid placement error sound");
        }
        
        /// <summary>
        /// Plays an error sound for insufficient resources.
        /// </summary>
        public static void PlayInsufficientResources()
        {
            PlaySound.Play("error_insufficient_resources");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played insufficient resources error sound");
        }
        
        /// <summary>
        /// Plays an error sound for invalid action.
        /// </summary>
        public static void PlayInvalidAction()
        {
            PlaySound.Play("error_invalid_action");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played invalid action error sound");
        }
        
        /// <summary>
        /// Plays an error sound for upgrade not available.
        /// </summary>
        public static void PlayUpgradeUnavailable()
        {
            PlaySound.Play("error_upgrade_unavailable");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played upgrade unavailable error sound");
        }
        
        /// <summary>
        /// Plays an error sound for tower limit reached.
        /// </summary>
        public static void PlayTowerLimitReached()
        {
            PlaySound.Play("error_tower_limit");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played tower limit error sound");
        }
        
        /// <summary>
        /// Plays an error sound for cooldown not ready.
        /// </summary>
        public static void PlayCooldownNotReady()
        {
            PlaySound.Play("error_cooldown");
            ModernLoggingSystem.Log("INFO", "PlayErrorSound: Played cooldown error sound");
        }
        
        /// <summary>
        /// Plays a specific error sound by name.
        /// </summary>
        /// <param name="errorType">Type of error sound to play.</param>
        public static void Play(string errorType)
        {
            PlaySound.Play($"error_{errorType}");
            ModernLoggingSystem.Log("INFO", $"PlayErrorSound: Played error sound for '{errorType}'");
        }
    }
}