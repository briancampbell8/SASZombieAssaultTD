// ====================================================================================================
//  FILE: EnemyAudioIntegration.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EnemyAudioIntegration module.
//
//  RESPONSIBILITIES:
//      - Provide PlayEnemyDeath() behavior for the Core subsystem.
//      - Provide PlayEnemySpawn() behavior for the Core subsystem.
//      - Provide PlayEnemyAttack() behavior for the Core subsystem.
//      - Provide PlayEnemyHit() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
Program Name: SASZombieAssaultTD
File Path: Engine\Enemies\EnemyAudioIntegration.cs
Purpose: P90 Modern Audio Subsystem - Audio integration for enemy system.
Features: Enemy death, spawn, and attack audio.
*/

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Audio integration for the enemy system.
    ///P90-09: Enemy audio integration with ModernAudioSubsystem
    ///</summary>
    public static class EnemyAudioIntegration
    {
        ///<summary>
        ///Plays enemy death sound.
        ///</summary>
        public static void PlayEnemyDeath(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("enemy_death", position);
            DLogger.Log($"EnemyAudioIntegration: Played enemy death sound at {position}");
        }

        ///<summary>
        ///Plays enemy spawn sound.
        ///</summary>
        public static void PlayEnemySpawn(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("wave_start", position);
            DLogger.Log($"EnemyAudioIntegration: Played enemy spawn sound at {position}");
        }

        ///<summary>
        ///Plays enemy attack sound.
        ///</summary>
        public static void PlayEnemyAttack(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_fire", position);
            DLogger.Log($"EnemyAudioIntegration: Played enemy attack sound at {position}");
        }

        ///<summary>
        ///Plays enemy hit sound.
        ///</summary>
        public static void PlayEnemyHit(Vector3 position)
        {
            ModernPlaySound.PlayAtPosition("tower_fire", position);
            DLogger.Log($"EnemyAudioIntegration: Played enemy hit sound at {position}");
        }
    }
}

