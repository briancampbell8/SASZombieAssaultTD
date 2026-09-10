// ====================================================================================================
//  FILE: PlayErrorSound.cs
//  PATH: ./Engine/Audio/
//  MODULE: Audio
//
//  ROLE:
//      Manage audio playback, mixing, or spatial sound behavior.
//
//  RESPONSIBILITIES:
//      - Provide Play() behavior for the Audio subsystem.
//      - Provide PlayInsufficientFunds() behavior for the Audio subsystem.
//      - Provide PlayInvalidPlacement() behavior for the Audio subsystem.
//      - Provide PlayInsufficientResources() behavior for the Audio subsystem.
//      - Provide PlayInvalidAction() behavior for the Audio subsystem.
//      - Provide PlayUpgradeUnavailable() behavior for the Audio subsystem.
//      - Provide PlayTowerLimitReached() behavior for the Audio subsystem.
//      - Provide PlayCooldownNotReady() behavior for the Audio subsystem.
//      - Provide Play() behavior for the Audio subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    PlayErrorSound.cs
Purpose: Plays a standardized error sound for invalid actions.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.ECS
//
{
    ///<summary>
    ///Plays standardized error sounds for invalid actions.
    ///</summary>
    public static class PlayErrorSound
    {
        ///<summary>
        ///Plays a generic error sound.
        ///</summary>
        public static void Play()
        {
            PlaySound.Play("error_generic");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played generic error sound");
        }
        
        ///<summary>
        ///Plays an error sound for insufficient funds.
        ///</summary>
        public static void PlayInsufficientFunds()
        {
            PlaySound.Play("error_insufficient_funds");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played insufficient funds error sound");
        }
        
        ///<summary>
        ///Plays an error sound for invalid placement.
        ///</summary>
        public static void PlayInvalidPlacement()
        {
            PlaySound.Play("error_invalid_placement");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played invalid placement error sound");
        }
        
        ///<summary>
        ///Plays an error sound for insufficient resources.
        ///</summary>
        public static void PlayInsufficientResources()
        {
            PlaySound.Play("error_insufficient_resources");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played insufficient resources error sound");
        }
        
        ///<summary>
        ///Plays an error sound for invalid action.
        ///</summary>
        public static void PlayInvalidAction()
        {
            PlaySound.Play("error_invalid_action");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played invalid action error sound");
        }
        
        ///<summary>
        ///Plays an error sound for upgrade not available.
        ///</summary>
        public static void PlayUpgradeUnavailable()
        {
            PlaySound.Play("error_upgrade_unavailable");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played upgrade unavailable error sound");
        }
        
        ///<summary>
        ///Plays an error sound for tower limit reached.
        ///</summary>
        public static void PlayTowerLimitReached()
        {
            PlaySound.Play("error_tower_limit");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played tower limit error sound");
        }
        
        ///<summary>
        ///Plays an error sound for cooldown not ready.
        ///</summary>
        public static void PlayCooldownNotReady()
        {
            PlaySound.Play("error_cooldown");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, "PlayErrorSound: Played cooldown error sound");
        }
        
        ///<summary>
        ///Plays a specific error sound by name.
        ///</summary>
        ///<param name="errorType">Type of error sound to play.</param>
        public static void Play(string errorType)
        {
            PlaySound.Play($"error_{errorType}");
            DLogger.Log(LogSubsystems.Save, LogEnums.LogLevel.Info, $"PlayErrorSound: Played error sound for '{errorType}'");
        }
    }
}

