/*
File:    ModernPlaySound.cs
Folder:  Engine/Audio/
Purpose:  P90 Modern Audio Subsystem - Modern audio playback helper.
Features: Integration with ModernAudioSubsystem for actual audio playback.
*/

using System;
//
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Audio
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
            System.Diagnostics.Debug.WriteLine("ModernPlaySound: Initialized with ModernAudioSubsystem");
        }

        ///<summary>
        ///Plays a sound by name.
        ///</summary>
        public static void Play(string soundName)
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log("WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.PlaySound(soundName);
            DLogger.Log(LogSubsystems.Audio,LogLevel.Info, $"ModernPlaySound: Playing sound '{soundName}'");
        }
        
        ///<summary>
        ///Plays a sound with volume control.
        ///</summary>
        public static void Play(string soundName, float volume)
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log("WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.PlaySound(soundName, default, volume);
            DLogger.Log(LogSubsystems.Audio,LogLevel.Info, $"ModernPlaySound: Playing sound '{soundName}' at volume {volume}");
        }
        
        ///<summary>
        ///Plays a sound at a specific position.
        ///</summary>
        public static void PlayAtPosition(string soundName, Vector3 position)
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log("WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.PlaySound(soundName, position, 1.0f);
            DLogger.Log(LogSubsystems.Audio,LogLevel.Info, $"ModernPlaySound: Playing sound '{soundName}' at position {position}");
        }
        
        ///<summary>
        ///Stops all currently playing sounds.
        ///</summary>
        public static void StopAll()
        {
            if (_audioSubsystem == null)
            {
                DLogger.Log("WARN", "ModernPlaySound: Audio subsystem not initialized");
                return;
            }

            _audioSubsystem.StopAllSounds();
            DLogger.Log(LogSubsystems.Audio,LogLevel.Info, "ModernPlaySound: Stopped all sounds");
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
