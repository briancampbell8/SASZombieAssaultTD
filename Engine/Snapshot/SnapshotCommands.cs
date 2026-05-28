// ============================================================================
// File: SnapshotCommands.cs
// Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\Snapshot\SnapshotCommands.cs
// Program: SnapshotCommands
// Subsystem: Snapshot System / Command Routing
//
// Purpose:
//     Provides deterministic, audit‑friendly console commands for snapshot
//     operations. Wraps SnapshotCapture and SnapshotManager functionality.
//
// Doctrine:
//     - No System.Diagnostics
//     - All logging via Engine.Diagnostics.DebugLogger.Trace()
//     - Deterministic, grep‑friendly trace naming
//     - No fallback logic except explicit exception propagation
// ============================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Snapshot
{
    public static class SnapshotCommands
    {
        private static SnapshotManager? _snapshotManager;
        private static object PlayerSystem;

        // --------------------------------------------------------------------
        // INITIALIZATION
        // --------------------------------------------------------------------

        public static void Initialize(SnapshotManager manager)
        {
            _snapshotManager = manager ?? throw new ArgumentNullException(nameof(manager));

            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.Initialize", "Snapshot command system initialized");
        }

        // --------------------------------------------------------------------
        // SCREENSHOT
        // --------------------------------------------------------------------

        public static void CaptureScreenshot(string[] args)
        {
            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CaptureScreenshot.Start", "Begin");

            try
            {
                SnapshotCapture.CaptureScreenshot();
                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CaptureScreenshot.Success", "Screenshot captured");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CaptureScreenshot.Error", ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // CREATE SNAPSHOT
        // --------------------------------------------------------------------

        public static void CreateSnapshot(string[] args)
        {
            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CreateSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CreateSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                string? customName = (args != null && args.Length > 0) ? args[0] : null;

                // Placeholder values — real game systems will supply these
                int waveNumber = 1;
                float gameTime = 0f;

                // TODO: Wire real player state once PlayerSystem API is confirmed.
                var snapshot = _snapshotManager.CreateSnapshot(
                    playerState: null,
                    waveNumber: waveNumber,
                    gameTime: gameTime,
                    customData: null,
                    customId: customName
                );

                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CreateSnapshot.Snapshot", snapshot?.Id ?? "null");

                if (snapshot == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CreateSnapshot.NullSnapshot", "Snapshot creation returned null");
                    return;
                }

                bool saved = _snapshotManager.SaveSnapshot(snapshot);

                if (saved)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CreateSnapshot.Success",
                        $"Saved={snapshot.Id}, Cash={snapshot.PlayerState.Cash}, Lives={snapshot.PlayerState.Lives}, Wave={snapshot.WaveNumber}");
                }
                else
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CreateSnapshot.SaveFailed", snapshot.Id);
                }
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.CreateSnapshot.Error", ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // LIST SNAPSHOTS
        // --------------------------------------------------------------------

        public static void ListSnapshots(string[] args)
        {
            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ListSnapshots.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ListSnapshots.NoManager", "SnapshotManager not initialized");
                    return;
                }

                var snapshots = _snapshotManager.ListSnapshots();

                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ListSnapshots.Count", snapshots.Count.ToString());

                foreach (var s in snapshots)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ListSnapshots.Entry",
                        $"{s.Id} :: Valid={s.IsValid}, Cash={s.Cash}, Lives={s.Lives}, Wave={s.WaveNumber}");
                }
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ListSnapshots.Error", ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // LOAD SNAPSHOT
        // --------------------------------------------------------------------

        public static void LoadSnapshot(string[] args)
        {
            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.LoadSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.LoadSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                if (args == null || args.Length == 0)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.LoadSnapshot.Usage", "snapshot_load <id>");
                    return;
                }

                string id = args[0];
                var snapshot = _snapshotManager.LoadSnapshot(id);

                if (snapshot == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.LoadSnapshot.NotFound", id);
                    return;
                }

                bool applied = _snapshotManager.ApplySnapshot(snapshot, out string error);

                if (applied)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.LoadSnapshot.Success",
                        $"Loaded={snapshot.Id}, Cash={snapshot.PlayerState.Cash}, Lives={snapshot.PlayerState.Lives}, Wave={snapshot.WaveNumber}");
                }
                else
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.LoadSnapshot.ApplyFailed", error);
                }
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.LoadSnapshot.Error", ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // DELETE SNAPSHOT
        // --------------------------------------------------------------------

        public static void DeleteSnapshot(string[] args)
        {
            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.DeleteSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.DeleteSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                if (args == null || args.Length == 0)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.DeleteSnapshot.Usage", "snapshot_delete <id>");
                    return;
                }

                string id = args[0];
                bool deleted = _snapshotManager.DeleteSnapshot(id);

                Engine.Diagnostics.DebugLogger.Trace(
                    deleted ? "SnapshotCommands.DeleteSnapshot.Success" : "SnapshotCommands.DeleteSnapshot.Failed",
                    id
                );
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.DeleteSnapshot.Error", ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // VALIDATE SNAPSHOT
        // --------------------------------------------------------------------

        public static void ValidateSnapshot(string[] args)
        {
            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ValidateSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ValidateSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                if (args == null || args.Length == 0)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ValidateSnapshot.Usage", "snapshot_validate <id>");
                    return;
                }

                string id = args[0];
                var snapshot = _snapshotManager.LoadSnapshot(id);

                if (snapshot == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ValidateSnapshot.NotFound", id);
                    return;
                }

                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ValidateSnapshot.Result",
                    $"Valid={snapshot.Metadata.IsValid}, Checksum={(string.IsNullOrEmpty(snapshot.Metadata.Checksum) ? "None" : "Present")}");
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.ValidateSnapshot.Error", ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // HELP
        // --------------------------------------------------------------------

        public static void ShowHelp(string[] args)
        {
            Engine.Diagnostics.DebugLogger.Trace("SnapshotCommands.Help", "Displayed");
        }
    }
}
