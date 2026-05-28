/*
Program Name: SASZombieAssaultTD
File Path: Engine\Waves\WaveAudioIntegration.cs
Purpose: P90 Modern Audio Subsystem - Audio integration for wave system.
Features: Wave start, wave complete, and wave announcement audio.
*/

using SASZombieAssaultTD.Engine.Audio;

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Audio integration for the wave system.
    /// P90-08: Wave audio integration with ModernAudioSubsystem
    /// </summary>
    public static class WaveAudioIntegration
    {
        /// <summary>
        /// Plays wave start sound.
        /// </summary>
        public static void PlayWaveStart()
        {
            ModernPlaySound.Play("wave_start");
            System.Diagnostics.Debug.WriteLine("WaveAudioIntegration: Played wave start sound");
        }

        /// <summary>
        /// Plays wave complete sound.
        /// </summary>
        public static void PlayWaveComplete()
        {
            ModernPlaySound.Play("success_wave_complete");
            System.Diagnostics.Debug.WriteLine("WaveAudioIntegration: Played wave complete sound");
        }

        /// <summary>
        /// Plays wave announcement sound.
        /// </summary>
        public static void PlayWaveAnnouncement(int waveNumber)
        {
            ModernPlaySound.Play("wave_start");
            System.Diagnostics.Debug.WriteLine($"WaveAudioIntegration: Played wave {waveNumber} announcement");
        }

        /// <summary>
        /// Plays wave music.
        /// </summary>
        public static void PlayWaveMusic()
        {
            if (ModernPlaySound.GetSubsystem() != null)
            {
                ModernPlaySound.GetSubsystem().PlayMusic("music_wave", true);
                System.Diagnostics.Debug.WriteLine("WaveAudioIntegration: Started wave music");
            }
        }

        /// <summary>
        /// Stops wave music.
        /// </summary>
        public static void StopWaveMusic()
        {
            if (ModernPlaySound.GetSubsystem() != null)
            {
                ModernPlaySound.GetSubsystem().StopMusic();
                System.Diagnostics.Debug.WriteLine("WaveAudioIntegration: Stopped wave music");
            }
        }
    }
}
