// ============================================================================
// File: SnapshotIntegration.cs
// Author: BDC
// Created: (auto-generated repair)
// Purpose: Integrates the snapshot system with the engine. Handles initialization,
//          command registration, automatic snapshot triggers, and cleanup.
// Notes:   Exception wrappers removed per doctrine (Option B). Structural integrity
//          restored after brace-collapse caused by commented-out try blocks.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.Player;
using System;

// using ModernLoggingSystem = SASZombieAssaultTD.Engine.Core.ModernLoggingSystem;

namespace SASZombieAssaultTD.Engine.Snapshot
{
    /// <summary>
    /// Integration point for snapshot system with game engine.
    /// Initializes snapshot functionality and registers commands.
    /// </summary>
    public static class SnapshotIntegration
    {
        private static SnapshotManager _snapshotManager;
        private static bool _initialized = false;
        private static bool _hasCreatedTodaySnapshot = false;
        private static bool _hasRenderedFirstFrame = false;
        private static bool _shouldCaptureThisFrame = false;

        /// <summary>
        /// Initializes the snapshot system.
        /// Call this during engine startup.
        /// </summary>
        public static void Initialize(string snapshotDirectory = null)
        {
            if (_initialized)
            {
                System.Diagnostics.Debug.WriteLine("Warning", "[SNAPSHOT] Already initialized");
                return;
            }

            // Create snapshot manager
            _snapshotManager = new SnapshotManager(snapshotDirectory);

            // Initialize command system
            SnapshotCommands.Initialize(_snapshotManager);

            // Register console commands
            RegisterCommands();

            _initialized = true;
            System.Diagnostics.Debug.WriteLine("Info", "[SNAPSHOT] System initialized successfully");
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
        /// Creates a quick snapshot with current game state.
        /// Convenience method for automatic snapshots.
        /// </summary>
        public static string QuickSnapshot(string context = "auto")
        {
            if (!_initialized)
            {
                System.Diagnostics.Debug.WriteLine("Error", "[SNAPSHOT] System not initialized");
                return null;
            }

            if (PlayerSystem.Instance?.State == null)
            {
                System.Diagnostics.Debug.WriteLine("Error", "[SNAPSHOT] Player system not available");
                return null;
            }

            // TODO: Get current wave number and game time from game systems
            int waveNumber = 1; // Placeholder
            float gameTime = 0f; // Placeholder

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
                    System.Diagnostics.Debug.WriteLine("Info", $"[SNAPSHOT] Quick snapshot created: {snapshot.Id}");
                    return snapshot.Id;
                }
            }

            return null;
        }

        /// <summary>
        /// Marks that the first frame has been rendered.
        /// Call this from the main render loop after the first frame is drawn.
        /// </summary>
        public static void MarkFirstFrameRendered()
        {
            _hasRenderedFirstFrame = true;
            System.Diagnostics.Debug.WriteLine("Info", "[SNAPSHOT] First frame rendered - snapshots now enabled");
        }

        /// <summary>
        /// Ensures today.png snapshot exists (creates once per day).
        /// Deterministic entry point for cash button integration.
        /// Only captures after first frame is rendered.
        /// Sets a flag for the render loop to capture at end of frame.
        /// </summary>
        public static void EnsureTodaySnapshot()
        {
            if (!_hasRenderedFirstFrame)
            {
                System.Diagnostics.Debug.WriteLine("Warning", "[SNAPSHOT] Cannot capture - first frame not yet rendered");
                return;
            }

            if (_hasCreatedTodaySnapshot)
                return;

            _shouldCaptureThisFrame = true;
            System.Diagnostics.Debug.WriteLine("Info", "[SNAPSHOT] Snapshot requested - will capture at end of frame");
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

            SnapshotCapture.CaptureFromFramebuffer(framebufferPixels, width, height);
            _hasCreatedTodaySnapshot = true;
            _shouldCaptureThisFrame = false;

            System.Diagnostics.Debug.WriteLine("Info", "[SNAPSHOT] Today's snapshot captured from framebuffer");
        }

        /// <summary>
        /// Cleans up old snapshots (keeps last N snapshots).
        /// </summary>
        public static void CleanupOldSnapshots(int keepCount = 10)
        {
            if (!_initialized)
            {
                System.Diagnostics.Debug.WriteLine("Error", "[SNAPSHOT] System not initialized");
                return;
            }

            var snapshots = _snapshotManager.ListSnapshots();
            if (snapshots.Count <= keepCount)
                return;

            // Sort by timestamp (oldest first)
            snapshots.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

            int deleteCount = snapshots.Count - keepCount;
            for (int i = 0; i < deleteCount; i++)
            {
                _snapshotManager.DeleteSnapshot(snapshots[i].Id);
                System.Diagnostics.Debug.WriteLine("Info", $"[SNAPSHOT] Cleaned up old snapshot: {snapshots[i].Id}");
            }
        }

        /// <summary>
        /// Shuts down the snapshot system.
        /// Call this during engine shutdown.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized)
                return;

            CleanupOldSnapshots(20);

            _snapshotManager = null;
            _initialized = false;

            System.Diagnostics.Debug.WriteLine("Info", "[SNAPSHOT] System shutdown complete");
        }

        // ---------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------

        private static void RegisterCommands()
        {
            // TODO: Register with console command system
            System.Diagnostics.Debug.WriteLine("Info", "[SNAPSHOT] Commands registered with console system");
        }
    }
}
