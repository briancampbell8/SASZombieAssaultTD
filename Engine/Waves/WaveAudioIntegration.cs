// ====================================================================================================
//  FILE: WaveAudioIntegration.cs
//  PATH: Engine/Waves/
//  MODULE: Wave System (Audio Integration)
//
//  ROLE:
//      Provides simple audio hooks for common wave lifecycle events.
//
//  RESPONSIBILITIES:
//      - Play sound cues for wave start, wave complete, and wave announcements.
//      - Use ModernPlaySound for playback and emit light diagnostics.
//
//  NON-RESPONSIBILITIES:
//      - Managing complex audio mixing or music transitions.
//
//  ARCHITECTURAL NOTES:
//      - Intentionally minimal and synchronous; should be called from gameplay event handlers.
// ====================================================================================================

using SASZombieAssaultTD.Engine.ECS;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves
{
    ///<summary>
    ///Audio integration for the wave system.
    ///P90-08: Wave audio integration with ModernAudioSubsystem
    ///</summary>
    public static class WaveAudioIntegration
    {
        ///<summary>
        ///Plays wave start sound.
        ///</summary>
        public static void PlayWaveStart()
        {
            ModernPlaySound.Play("wave_start");
            DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveAudioIntegration: Played wave start sound");
        }

        ///<summary>
        ///Plays wave complete sound.
        ///</summary>
        public static void PlayWaveComplete()
        {
            ModernPlaySound.Play("success_wave_complete");
            DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveAudioIntegration: Played wave complete sound");
        }

        ///<summary>
        ///Plays wave announcement sound.
        ///</summary>
        public static void PlayWaveAnnouncement(int waveNumber)
        {
            ModernPlaySound.Play("wave_start");
            DLogger.Log($"WaveAudioIntegration: Played wave {waveNumber} announcement");
        }

        ///<summary>
        ///Plays wave music.
        ///</summary>
        public static void PlayWaveMusic()
        {
            if (ModernPlaySound.GetSubsystem() != null)
            {
                ModernPlaySound.GetSubsystem().PlayMusic("music_wave", true);
                DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveAudioIntegration: Started wave music");
            }
        }

        ///<summary>
        ///Stops wave music.
        ///</summary>
        public static void StopWaveMusic()
        {
            if (ModernPlaySound.GetSubsystem() != null)
            {
                ModernPlaySound.GetSubsystem().StopMusic();
                DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveAudioIntegration: Stopped wave music");
            }
        }
    }
}
