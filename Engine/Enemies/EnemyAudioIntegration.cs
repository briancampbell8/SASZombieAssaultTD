/*
Program Name: SASZombieAssaultTD
File Path: Engine\Enemies\EnemyAudioIntegration.cs
Purpose: P90 Modern Audio Subsystem - Audio integration for enemy system.
Features: Enemy death, spawn, and attack audio.
*/

using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Audio integration for the enemy system.
    /// P90-09: Enemy audio integration with ModernAudioSubsystem
    /// </summary>
    public static class EnemyAudioIntegration
    {
        /// <summary>
        /// Plays enemy death sound.
        /// </summary>
        public static void PlayEnemyDeath(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("enemy_death", position);
            System.Diagnostics.Debug.WriteLine($"EnemyAudioIntegration: Played enemy death sound at {position}");
        }

        /// <summary>
        /// Plays enemy spawn sound.
        /// </summary>
        public static void PlayEnemySpawn(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("wave_start", position);
            System.Diagnostics.Debug.WriteLine($"EnemyAudioIntegration: Played enemy spawn sound at {position}");
        }

        /// <summary>
        /// Plays enemy attack sound.
        /// </summary>
        public static void PlayEnemyAttack(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_fire", position);
            System.Diagnostics.Debug.WriteLine($"EnemyAudioIntegration: Played enemy attack sound at {position}");
        }

        /// <summary>
        /// Plays enemy hit sound.
        /// </summary>
        public static void PlayEnemyHit(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_fire", position);
            System.Diagnostics.Debug.WriteLine($"EnemyAudioIntegration: Played enemy hit sound at {position}");
        }
    }
}
