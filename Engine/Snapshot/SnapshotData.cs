// ====================================================================================================
//  FILE: SnapshotData.cs
//  PATH: ./Engine/Snapshot/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the SnapshotData module.
//
//  RESPONSIBILITIES:
//      - Provide DeepClone() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File: SnapshotData.cs
//Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\Snapshot\SnapshotData.cs
//Program: SnapshotData
//Subsystem: Snapshot System / Data Model
//
//Purpose:
//    Immutable, deterministic snapshot of complete game state.
//    Used by SnapshotManager, SnapshotCapture, and SnapshotCommands.
//
//Doctrine:
//    - Pure data container (no DLogger.Log())
//    - Immutable after construction
//    - Deterministic, grep‑friendly field names
//    - DeepClone() allowed for safe duplication
//============================================================================

//
//
//
using System;   //
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Snapshot
{
    public sealed class SnapshotData
    {
        //IdECSEntityCore
        public string Id { get; }
        public DateTime Timestamp { get; }
        public string GameVersion { get; }

        //Player State
        public PlayerState PlayerState { get; }

        //Game State
        public int WaveNumber { get; }
        public float GameTime { get; }

        //Custom Data
        public Dictionary<string, object> CustomData { get; }

        //Metadata
        public SnapshotMetadata Metadata { get; }

        public SnapshotData(
            string id,
            DateTime timestamp,
            string gameVersion,
            PlayerState playerState,
            int waveNumber,
            float gameTime,
            Dictionary<string, object>? customData = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Timestamp = timestamp;
            GameVersion = gameVersion ?? throw new ArgumentNullException(nameof(gameVersion));
            PlayerState = playerState ?? throw new ArgumentNullException(nameof(playerState));
            WaveNumber = waveNumber;
            GameTime = gameTime;

            CustomData = customData ?? new Dictionary<string, object>();
            Metadata = new SnapshotMetadata();
        }

        public SnapshotData(string v, DateTime timestamp, string gameVersion, SnapshotData snapshotData, int waveNumber, float gameTime, Dictionary<string, object> clonedCustomData)
        {
            Timestamp = timestamp;
            GameVersion = gameVersion;
            WaveNumber = waveNumber;
            GameTime = gameTime;
        }

        public SnapshotData DeepClone()
        {
            var clonedCustomData = new Dictionary<string, object>();

            foreach (var kvp in CustomData)
            {
                if (kvp.Value is ICloneable cloneable)
                    clonedCustomData[kvp.Key] = cloneable.Clone();
                else
                    clonedCustomData[kvp.Key] = kvp.Value;
            }

            return new SnapshotData(
                $"{Id}_clone_{Guid.NewGuid():N}",
                Timestamp,
                GameVersion,
                DeepClone(),

                WaveNumber,
                GameTime,
                clonedCustomData
            );
        }
    }

    public class PlayerState
    {
        internal int Level;
        internal int Experience;
        internal int Cash;
        internal int Lives;
        internal int Score;
        internal int WaveNumber;

        internal PlayerState DeepClone()
        {
            NI.Hit();
            return null;
        }
    }

    //public class PlayerState already defined in Engine.Player.PlayerState
    //{
    //   internal int Level;
    //   internal int Experience;
    //   internal int Cash;
    //   internal int Lives;
    //   internal int Score;
    //   internal int WaveNumber;

    //   internal PlayerState DeepClone()
    //   {
    //       NI.Hit();
    //   }
    //}

    public sealed class SnapshotMetadata
    {
        public string Checksum { get; set; } = string.Empty;
        public int FormatVersion { get; set; } = 1;
        public bool IsValid { get; set; } = true;
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public string CreationContext { get; set; } = "Manual";
    }
}


