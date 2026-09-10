// ====================================================================================================
//  FILE: PlayerEvents.cs
//  PATH: ./Engine/Player/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the PlayerEvents module.
//
//  RESPONSIBILITIES:
//      - Provide TriggerEnemyKilled() behavior for the Core subsystem.
//      - Provide TriggerWaveCompleted() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//File:    PlayerEvents.cs
//Purpose: Central event system for player-related gameplay events.
//Features: Static events for enemy kills and wave completions.
//Integration: Used by WaveDirector and EnemySystem to notify PlayerSystem.
//Architecture: Static event pattern for cross-system communication.
//

//

using SASZombieAssaultTD.Engine.Enemies;
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Player
{
    ///<summary>
    ///Central event system for player-related gameplay events.
    ///Provides static events for cross-system communication between
    ///WaveDirector, EnemySystem, and PlayerSystem.
    ///</summary>
    public static class PlayerEvents
    {
        ///<summary>
        ///Event raised when an enemy is killed.
        ///Provides the enemy death event data for reward processing.
        ///</summary>
        public static event Action<EnemyDeathEvent> EnemyKilled;

        ///<summary>
        ///Event raised when a wave is completed.
        ///Provides the wave number for reward processing.
        ///</summary>
        public static event Action<int> WaveCompleted;

        ///<summary>
        ///Triggers the EnemyKilled event.
        ///Called by EnemySystem when an enemy dies.
        ///</summary>
        ///<param name="eventData">The enemy death event data.</param>
        public static void TriggerEnemyKilled(EnemyDeathEvent eventData)
        {
            EnemyKilled?.Invoke(eventData);
        }

        ///<summary>
        ///Triggers the WaveCompleted event.
        ///Called by WaveDirector when a wave is completed.
        ///</summary>
        ///<param name="waveNumber">The completed wave number.</param>
        public static void TriggerWaveCompleted(int waveNumber)
        {
            WaveCompleted?.Invoke(waveNumber);
        }

        ///<summary>
        ///Static event hook for cash changes.
        ///UI systems subscribe to this for real-time cash display updates.
        ///</summary>
        ///
        public static event Action<int> OnCashChanged
        {
            add => ((dynamic)PlayerSystem.Instance).OnCashChanged += value;
            remove => ((dynamic)PlayerSystem.Instance).OnCashChanged -= value;
        }

        ///<summary>
        ///Static event hook for experience changes.
        ///UI systems subscribe to this for real-time XP display updates.
        ///</summary>
        public static event Action<int> OnExperienceChanged
        {
            add => ((dynamic)PlayerSystem.Instance).OnExperienceChanged += value;
            remove => ((dynamic)PlayerSystem.Instance).OnExperienceChanged -= value;
        }

        ///<summary>
        ///Static event hook for level changes.
        ///UI systems subscribe to this for real-time level display updates.
        ///</summary>
        public static event Action<int> OnLevelChanged
        {
            add => ((dynamic)PlayerSystem.Instance).OnLevelChanged += value;
            remove => ((dynamic)PlayerSystem.Instance).OnLevelChanged -= value;
        }

        ///<summary>
        ///Static event hook for score changes.
        ///UI systems subscribe to this for real-time score display updates.
        ///</summary>
        public static event Action<int> OnScoreChanged
        {
            add => ((dynamic)PlayerSystem.Instance).OnScoreChanged += value;
            remove => ((dynamic)PlayerSystem.Instance).OnScoreChanged -= value;
        }
    }
}


