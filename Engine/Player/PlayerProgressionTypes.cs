// ============================================================================
// File: PlayerProgressionTypes.cs
// FilePath: Engine/Player/PlayerProgressionTypes.cs
// Purpose: Defines supporting types required by PlayerProgressionController.
// These types provide data containers and event dispatch mechanisms used by
// the progression subsystem. No game logic is implemented in this file.
// Integration: PlayerProgressionController constructs, reads, and writes these
// types. PlayerSystem dispatches events to external listeners.
// Data Flow: 
//   - PlayerProgressionData: persisted progression values.
//   - PlayerState: runtime-only state referenced by progression logic.
//   - LevelUp: payload for level-up notifications.
//   - PlayerSystem: event dispatcher for progression-related events.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Player
{
    // ------------------------------------------------------------------------
    // Type: PlayerProgressionData
    // Purpose: Holds persistent progression values for a player.
    // Integration: Used exclusively by PlayerProgressionController.
    // Data Flow: Controller reads/writes fields; save/load system serializes it.
    // ------------------------------------------------------------------------
    public class PlayerProgressionData
    {
        public int CurrentLevel { get; set; } = 1;                     // Level value
        public int CurrentExperience { get; set; } = 0;                // XP at current level
        public HashSet<string> UnlockedTowers { get; set; }            // Tower unlock set
            = new HashSet<string>();                                   // Initialized container
    }

    // ------------------------------------------------------------------------
    // Type: PlayerState
    // Purpose: Represents runtime player state used by progression logic.
    // Integration: Passed into PlayerProgressionController constructor.
    // Data Flow: Controller reads state values when awarding XP.
    // ------------------------------------------------------------------------
    //public class PlayerState Allows for flexible extension of player state without modifying core progression logic.
    //{
    //    // Extended fields added by gameplay systems as needed.
    //    internal object Lives;
    //    internal object MaxLives;
    //    internal object WaveNumber;
    //    internal bool IsGameOver;
    //    internal bool IsPaused;
    //    internal object Cash;
    //    internal object Score;
    //    internal object Progression;

    //    internal void Validate()
    //    {
    //        NI.Hit();
    //    }
    //}

    // ------------------------------------------------------------------------
    // Type: LevelUp
    // Purpose: Payload for level-up events dispatched by PlayerSystem.
    // Integration: Created by PlayerProgressionController.LevelUp().
    // Data Flow: PlayerSystem receives this event and notifies listeners.
    // ------------------------------------------------------------------------
    public class LevelUp
    {
        public int NewLevel { get; set; }          // Resulting level after increment
        public int Experience { get; set; }        // XP carried into new level
    }

    // ------------------------------------------------------------------------
    // Type: PlayerSystem
    // Purpose: Central dispatcher for player-related events and global player state.
    // Integration: PlayerProgressionController triggers events through this type.
    // Data Flow: Receives LevelUp and Unlock events; forwards to listeners.
    // ------------------------------------------------------------------------
    public sealed class PlayerSystem
    {
        private static readonly PlayerSystem _instance = new PlayerSystem(); // Singleton instance
        private object Progression;
        internal object State;
        internal object Cash;
        internal object Score;
        internal object Lives;
        internal object Economy;

        public static PlayerSystem Instance => _instance;                    // Global accessor

        // --- Added State Properties to resolve SaveLoadController compilation errors ---
        //public PlayerState State { get; set; } = new PlayerState(); already used by PlayerProgressionController,
        // so we can't add it here without causing a circular dependency. Instead, we can add the individual properties directly to PlayerSystem.
        public int MaxLives { get; set; }
        public int WaveNumber { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsPaused { get; set; }
        // -------------------------------------------------------------------------------

        private PlayerSystem() { }                                           // Private constructor

        // Dispatches a level-up event to listeners.
        public void TriggerLevelUp(LevelUp evt)
        {
            Debug.WriteLine(
                $"PlayerSystem: LevelUp triggered - Level {evt.NewLevel}, XP {evt.Experience}",
                "Info");
        }

        // Dispatches a tower-unlock event to listeners.
        public void TriggerUnlock(string towerId)
        {
            Debug.WriteLine(
                $"PlayerSystem: Tower unlocked - {towerId}",
                "Info");
        }

        // Validates the restored state.
        public void Validate()
        {
            Debug.WriteLine("PlayerSystem: Validating restored state...", "Info");
            // Add custom validation logic here if needed
        }

        internal void RestoreCash(object cash)
        {
            NI.Hit();
        }

        internal void RestoreScore(object score)
        {
            NI.Hit();
        }

        // Change from 'private' to 'internal' to match your other methods
        internal void RestoreExperience(int experience)
        {
            // Option A: If PlayerSystem has a direct Progression property
            if (PlayerSystem.Instance.Progression != null)
            {
                PlayerSystem.Instance.Progression = experience;
            }
        }



        internal void RestoreLevel(object currentLevel)
        {
            NI.Hit();
        }

        internal void RestoreUnlockedTowers(List<string> list)
        {
            NI.Hit();
        }

        internal void RestoreExperience(object experience)
        {
            NI.Hit();
        }
    }

}

