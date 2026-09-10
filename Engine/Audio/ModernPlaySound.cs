// ====================================================================================================
//  FILE: ModernPlaySound.cs
//  PATH: ./Engine/Audio/
//  MODULE: Audio
//
//  ROLE:
//      Manage audio playback, mixing, or spatial sound behavior.
//
//  RESPONSIBILITIES:
//      - Provide Initialize() behavior for the Audio subsystem.
//      - Provide Play() behavior for the Audio subsystem.
//      - Provide Play() behavior for the Audio subsystem.
//      - Provide PlayAtPosition() behavior for the Audio subsystem.
//      - Provide StopAll() behavior for the Audio subsystem.
//      - Provide GetRegistry() behavior for the Audio subsystem.
//      - Provide GetSubsystem() behavior for the Audio subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    ModernPlaySound.cs
Folder:  Engine/Audio/
Purpose:  P90 Modern Audio Subsystem - Modern audio playback helper.
Features: Integration with ModernAudioSubsystem for actual audio playback.
*/

//
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.ECS
{
    ///<summary>
    ///Modern audio playback helper for UI and gameplay events.
    ///P90-06: ModernPlaySound integration with ModernAudioSubsystem
    ///</summary>
    public static class ModernPlaySound
    {
        private static ModernAudioSubsystem _audioSubsystem;
        private static AudioRegistry _registry;

        ///<summary>
        ///Initializes the ModernPlaySound helper with an audio subsystem.
        ///</summary>
        public static void Initialize(ModernAudioSubsystem audioSubsystem)
        {
            _audioSubsystem = audioSubsystem;
            _registry = new AudioRegistry();
            _registry.LoadDefaults();
            DLogger.Log(
                LogSubsystems.Audio,
                "ModernPlaySound: Initialized with ModernAudioSubsystem");
        }

        ///<summary>
        ///Plays a sound by name.
        ///</summary>
        public static void Play(string soundName)
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log(LogSubsystems.Audio, "WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.PlaySound(soundName);
            DLogger.Log(LogSubsystems.Audio, LogEnums.LogLevel.Info, $"ModernPlaySound: Playing sound '{soundName}'");
        }

        ///<summary>
        ///Plays a sound with volume control.
        ///</summary>
        public static void Play(string soundName, float volume)
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log(LogSubsystems.Audio, "WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.PlaySound(soundName, default, volume);
            DLogger.Log(LogSubsystems.Audio, LogEnums.LogLevel.Info, $"ModernPlaySound: Playing sound '{soundName}' at volume {volume}");
        }

        ///<summary>
        ///Plays a sound at a specific position.
        ///</summary>
        public static void PlayAtPosition(string soundName, Vector3 position)
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log(LogSubsystems.Audio, "WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.PlaySound(soundName, position, 1.0f);
            DLogger.Log(LogSubsystems.Audio, LogEnums.LogLevel.Info, $"ModernPlaySound: Playing sound '{soundName}' at position {position}");
        }

        ///<summary>
        ///Stops all currently playing sounds.
        ///</summary>
        public static void StopAll()
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log(LogSubsystems.Audio, "WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.StopAllSounds();
            DLogger.Log(LogSubsystems.Audio, LogEnums.LogLevel.Info, "ModernPlaySound: Stopped all sounds");
        }

        ///<summary>
        ///Gets the audio registry.
        ///</summary>
        public static AudioRegistry GetRegistry()
        {
            return _registry;
        }

        ///<summary>
        ///Gets the audio subsystem instance.
        ///</summary>
        public static ModernAudioSubsystem GetSubsystem()
        {
            return _audioSubsystem;
        }
    }
}

