// ====================================================================================================
//  FILE: PlayerStateData.cs
//  PATH: ./Engine/Player/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the PlayerStateData module.
//
//  RESPONSIBILITIES:
//      - Provide DeepClone() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        PlayerStateData.cs
//File Path:   E:\SASZombieAssaultTD\Engine\Player
//Program:     PlayerStateData
//Author:      BDC
//Created:     2026-05-19
//Purpose:     Serializable DTO for PlayerState used by SnapshotData.
//Notes:       Pure data. Immutable structure. No runtime dependencies.
//             Used exclusively for save/load, snapshot, and persistence.
//============================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Snapshot
{
    ///<summary>
    ///Serializable snapshot of the player's state.
    ///Pure data container with no runtime logic.
    ///</summary>
    [Serializable]
    public sealed class PlayerStateData
    {
        //--------------------------------------------------------------------
        // Core Player State Fields
        //--------------------------------------------------------------------

        public int Level { get; set; }
        public int Experience { get; set; }
        public int Cash { get; set; }
        public int Lives { get; set; }
        public int Score { get; set; }
        public int WaveNumber { get; set; }

        //--------------------------------------------------------------------
        // Constructors
        //--------------------------------------------------------------------

        ///<summary>
        ///Default constructor for serializers.
        ///</summary>
        public PlayerStateData() { }

        ///<summary>
        ///Creates a snapshot from a live PlayerState instance.
        ///</summary>
        public PlayerStateData(PlayerState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            Level = state.Level;
            Experience = state.Experience;
            Cash = state.Cash;
            Lives = state.Lives;
            Score = state.Score;
            WaveNumber = state.WaveNumber;
        }

        //--------------------------------------------------------------------
        // Deep Clone
        //--------------------------------------------------------------------

        ///<summary>
        ///Creates a deep copy of this PlayerStateData instance.
        ///</summary>
        public PlayerStateData DeepClone()
        {
            return new PlayerStateData
            {
                Level = Level,
                Experience = Experience,
                Cash = Cash,
                Lives = Lives,
                Score = Score,
                WaveNumber = WaveNumber
            };
        }

        //--------------------------------------------------------------------
        // Diagnostics
        //--------------------------------------------------------------------

        public override string ToString()
        {
            return $"PlayerStateData(Level={Level}, XP={Experience}, Cash={Cash}, Lives={Lives}, Score={Score}, Wave={WaveNumber})";
        }
    }
}

