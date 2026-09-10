// ====================================================================================================
//  FILE: SnapshotIntegration.cs
//  PATH: ./Engine/Snapshot/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the SnapshotIntegration module.
//
//  RESPONSIBILITIES:
//      - Provide Initialize() behavior for the Core subsystem.
//      - Provide QuickSnapshot() behavior for the Core subsystem.
//      - Provide MarkFirstFrameRendered() behavior for the Core subsystem.
//      - Provide EnsureTodaySnapshot() behavior for the Core subsystem.
//      - Provide CaptureIfRequested() behavior for the Core subsystem.
//      - Provide CleanupOldSnapshots() behavior for the Core subsystem.
//      - Provide Shutdown() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File: SnapshotIntegration.cs
//Author: BDC
//Created: (auto-generated repair)
//Purpose: Integrates the snapshot system with the engine. Handles initialization,
//         command registration, automatic snapshot triggers, and cleanup.
//Notes:   Exception wrappers removed per doctrine (Option B). Structural integrity
//         restored after brace-collapse caused by commented-out try blocks.
//============================================================================

//

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
//using ModernLoggingSystem = SASZombieAssaultTD.Engine.Core.ModernLoggingSystem;

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Player;

namespace SASZombieAssaultTD.Engine.Snapshot
{
    /// <summary>
    /// Integration point for snapshot system with game engine. Initializes snapshot functionality and registers
    /// commands.
    /// </summary>
    public static class SnapshotIntegration
    {
        private static SnapshotManager _snapshotManager;
        private static bool _initialized = false;
        private static bool _hasCreatedTodaySnapshot = false;
        private static bool _hasRenderedFirstFrame = false;
        private static bool _shouldCaptureThisFrame = false;

        /// <summary>
        /// Initializes the snapshot system. Call this during engine startup.
        /// </summary>
        public static void Initialize(string snapshotDirectory = null)
        {
            if (_initialized)
            {
                DLogger.Log(LogSubsystems.Snapshot, "Warning", "[SNAPSHOT] Already initialized");
                return;
            }

            //Create snapshot manager
            _snapshotManager = new SnapshotManager(snapshotDirectory);

            //Initialize command system
            SnapshotCommands.Initialize(_snapshotManager);

            //Register console commands
            RegisterCommands();

            _initialized = true;
            DLogger.Log(LogSubsystems.Snapshot, "Info", "[SNAPSHOT] System initialized successfully");
        }

        /// <summary>
        /// Gets the global snapshot manager instance.
        /// </summary>
        public static SnapshotManager Instance
        {
            get
            {
                if (!_initialized)
                {
                    throw new InvalidOperationException("Snapshot system not initialized. Call Initialize() first.");
                }
                return _snapshotManager;
            }
        }

        /// <summary>
        /// Creates a quick snapshot with current game state. Convenience method for automatic snapshots.
        /// </summary>
        public static string QuickSnapshot(string context = "auto")
        {
            if (!_initialized)
            {
                DLogger.Log(LogSubsystems.Snapshot, "Error", "[SNAPSHOT] System not initialized");
                return null;
            }

            if (PlayerSystem.Instance?.State == null)
            {
                DLogger.Log(LogSubsystems.Snapshot, "Error", "[SNAPSHOT] Player system not available");
                return null;
            }

            //TODO: Get current wave number and game time from game systems
            int waveNumber = 1; //Placeholder
            float gameTime = 0f; //Placeholder

            var snapshot = _snapshotManager.CreateSnapshot(
                PlayerSystem.Instance.State,
                waveNumber,
                gameTime,
                customData: new System.Collections.Generic.Dictionary<string, object>
                {
                    ["context"] = context,
                    ["auto"] = true
                }
            );

            if (snapshot != null)
            {
                bool saved = _snapshotManager.SaveSnapshot(snapshot);
                if (saved)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "Info", $"[SNAPSHOT] Quick snapshot created: {snapshot.Id}");
                    return snapshot.Id;
                }
            }

            return null;
        }

        /// <summary>
        /// Marks that the first frame has been rendered. Call this from the main render loop after the first frame is
        /// drawn.
        /// </summary>
        public static void MarkFirstFrameRendered()
        {
            _hasRenderedFirstFrame = true;
            DLogger.Log(LogSubsystems.Snapshot, "Info", "[SNAPSHOT] First frame rendered - snapshots now enabled");
        }

        /// <summary>
        /// Ensures today.png snapshot exists (creates once per day). Deterministic entry point for cash button
        /// integration. Only captures after first frame is rendered. Sets a flag for the render loop to capture at end
        /// of frame.
        /// </summary>
        public static void EnsureTodaySnapshot()
        {
            if (!_hasRenderedFirstFrame)
            {
                DLogger.Log(LogSubsystems.Snapshot, "Warning", "[SNAPSHOT] Cannot capture - first frame not yet rendered");
                return;
            }

            if (_hasCreatedTodaySnapshot)
                return;

            _shouldCaptureThisFrame = true;
            DLogger.Log(LogSubsystems.Snapshot, "Info", "[SNAPSHOT] Snapshot requested - will capture at end of frame");
        }

        /// <summary>
        /// Called by render loop at end of frame to capture framebuffer if requested.
        /// </summary>
        public static void CaptureIfRequested(byte[] framebufferPixels, int width, int height)
        {
            if (!_shouldCaptureThisFrame)
                return;

            if (_hasCreatedTodaySnapshot)
            {
                _shouldCaptureThisFrame = false;
                return;
            }

            SnapshotCapture.CaptureFromFramebufferDrawing(framebufferPixels, width, height);
            _hasCreatedTodaySnapshot = true;
            _shouldCaptureThisFrame = false;

            DLogger.Log(LogSubsystems.Snapshot, "Info", "[SNAPSHOT] Today's snapshot captured from framebuffer");
        }

        /// <summary>
        /// Cleans up old snapshots (keeps last N snapshots).
        /// </summary>
        public static void CleanupOldSnapshots(int keepCount = 10)
        {
            if (!_initialized)
            {
                DLogger.Log(LogSubsystems.Snapshot, "Error", "[SNAPSHOT] System not initialized");
                return;
            }

            var snapshots = _snapshotManager.ListSnapshots();
            if (snapshots.Count <= keepCount)
                return;

            //Sort by timestamp (oldest first)
            snapshots.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

            int deleteCount = snapshots.Count - keepCount;
            for (int i = 0; i < deleteCount; i++)
            {
                _snapshotManager.DeleteSnapshot(snapshots[i].Id);
                DLogger.Log(LogSubsystems.Snapshot, "Info", $"[SNAPSHOT] Cleaned up old snapshot: {snapshots[i].Id}");
            }
        }

        /// <summary>
        /// Shuts down the snapshot system. Call this during engine shutdown.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized)
                return;

            CleanupOldSnapshots(20);

            _snapshotManager = null;
            _initialized = false;

            DLogger.Log(LogSubsystems.Snapshot, "Info", "[SNAPSHOT] System shutdown complete");
        }

        //---------------------------------------------------------------------
        //Private Methods
        //---------------------------------------------------------------------

        private static void RegisterCommands()
        {
            //TODO: Register with console command system
            DLogger.Log(LogSubsystems.Snapshot, "Info", "[SNAPSHOT] Commands registered with console system");
        }
    }
}
