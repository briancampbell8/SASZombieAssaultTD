// ====================================================================================================
//  FILE: SnapshotCommands.cs
//  PATH: ./Engine/Snapshot/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the SnapshotCommands module.
//
//  RESPONSIBILITIES:
//      - Provide Initialize() behavior for the Core subsystem.
//      - Provide CaptureScreenshot() behavior for the Core subsystem.
//      - Provide CreateSnapshot() behavior for the Core subsystem.
//      - Provide ListSnapshots() behavior for the Core subsystem.
//      - Provide LoadSnapshot() behavior for the Core subsystem.
//      - Provide DeleteSnapshot() behavior for the Core subsystem.
//      - Provide ValidateSnapshot() behavior for the Core subsystem.
//      - Provide ShowHelp() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File: SnapshotCommands.cs
//Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\Snapshot\SnapshotCommands.cs
//Program: SnapshotCommands
//Subsystem: Snapshot System / Command Routing
//
//Purpose:
//    Provides deterministic, audit‑friendly console commands for snapshot
//    operations. Wraps SnapshotCapture and SnapshotManager functionality.
//
//Doctrine:
//    - No System.Diagnostics
//    - All logging via DLogger.Log()
//    - Deterministic, grep‑friendly trace naming
//    - No fallback logic except explicit exception propagation
//============================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
//
// using SASZombieAssaultTD.Engine.Extensions; // Extensions Removed

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Snapshot
{
    public static class SnapshotCommands
    {
        private static SnapshotManager? _snapshotManager;
        private static object PlayerSystem;

        //--------------------------------------------------------------------
        //INITIALIZATION
        //--------------------------------------------------------------------

        public static void Initialize(SnapshotManager manager)
        {
            _snapshotManager = manager ?? throw new ArgumentNullException(nameof(manager));

            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.Initialize", "Snapshot command system initialized");
        }

        //--------------------------------------------------------------------
        //SCREENSHOT
        //--------------------------------------------------------------------

        public static void CaptureScreenshot(string[] args)
        {
            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CaptureScreenshot.Start", "Begin");

            try
            {
                SnapshotCapture.CaptureScreenshot();
                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CaptureScreenshot.Success", "Screenshot captured");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CaptureScreenshot.Error", ex.Message);
            }
        }

        //--------------------------------------------------------------------
        //CREATE SNAPSHOT
        //--------------------------------------------------------------------

        public static void CreateSnapshot(string[] args)
        {
            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CreateSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CreateSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                string? customName = (args != null && args.Length > 0) ? args[0] : null;

                //Placeholder values — real game systems will supply these
                int waveNumber = 1;
                float gameTime = 0f;

                //TODO: Wire real player state once PlayerSystem API is confirmed.
                var snapshot = _snapshotManager.CreateSnapshot(
                    playerState: null,
                    waveNumber: waveNumber,
                    gameTime: gameTime,
                    customData: null,
                    customId: customName
                );

                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CreateSnapshot.Snapshot", snapshot?.Id ?? "null");

                if (snapshot == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CreateSnapshot.NullSnapshot", "Snapshot creation returned null");
                    return;
                }

                bool saved = _snapshotManager.SaveSnapshot(snapshot);

                if (saved)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CreateSnapshot.Success",
                        $"Saved={snapshot.Id}, Cash={snapshot.PlayerState.Cash}, Lives={snapshot.PlayerState.Lives}, Wave={snapshot.WaveNumber}");
                }
                else
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CreateSnapshot.SaveFailed", snapshot.Id);
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.CreateSnapshot.Error", ex.Message);
            }
        }

        //--------------------------------------------------------------------
        //LIST SNAPSHOTS
        //--------------------------------------------------------------------

        public static void ListSnapshots(string[] args)
        {
            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ListSnapshots.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ListSnapshots.NoManager", "SnapshotManager not initialized");
                    return;
                }

                var snapshots = _snapshotManager.ListSnapshots();

                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ListSnapshots.Count", snapshots.Count.ToString());

                foreach (var s in snapshots)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ListSnapshots.Entry",
                        $"{s.Id} :: Valid={s.IsValid}, Cash={s.Cash}, Lives={s.Lives}, Wave={s.WaveNumber}");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ListSnapshots.Error", ex.Message);
            }
        }

        //--------------------------------------------------------------------
        //LOAD SNAPSHOT
        //--------------------------------------------------------------------

        public static void LoadSnapshot(string[] args)
        {
            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.LoadSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.LoadSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                if (args == null || args.Length == 0)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.LoadSnapshot.Usage", "snapshot_load <id>");
                    return;
                }

                string id = args[0];
                var snapshot = _snapshotManager.LoadSnapshot(id);

                if (snapshot == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.LoadSnapshot.NotFound", id);
                    return;
                }

                bool applied = _snapshotManager.ApplySnapshot(snapshot, out string error);

                if (applied)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.LoadSnapshot.Success",
                        $"Loaded={snapshot.Id}, Cash={snapshot.PlayerState.Cash}, Lives={snapshot.PlayerState.Lives}, Wave={snapshot.WaveNumber}");
                }
                else
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.LoadSnapshot.ApplyFailed", error);
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.LoadSnapshot.Error", ex.Message);
            }
        }

        //--------------------------------------------------------------------
        //DELETE SNAPSHOT
        //--------------------------------------------------------------------

        public static void DeleteSnapshot(string[] args)
        {
            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.DeleteSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.DeleteSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                if (args == null || args.Length == 0)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.DeleteSnapshot.Usage", "snapshot_delete <id>");
                    return;
                }

                string id = args[0];
                bool deleted = _snapshotManager.DeleteSnapshot(id);

                DLogger.Log(LogSubsystems.Snapshot,
                    deleted ? "SnapshotCommands.DeleteSnapshot.Success" : "SnapshotCommands.DeleteSnapshot.Failed",
                    id
                );
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.DeleteSnapshot.Error", ex.Message);
            }
        }

        //--------------------------------------------------------------------
        //VALIDATE SNAPSHOT
        //--------------------------------------------------------------------

        public static void ValidateSnapshot(string[] args)
        {
            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ValidateSnapshot.Start", "Begin");

            try
            {
                if (_snapshotManager == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ValidateSnapshot.NoManager", "SnapshotManager not initialized");
                    return;
                }

                if (args == null || args.Length == 0)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ValidateSnapshot.Usage", "snapshot_validate <id>");
                    return;
                }

                string id = args[0];
                var snapshot = _snapshotManager.LoadSnapshot(id);

                if (snapshot == null)
                {
                    DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ValidateSnapshot.NotFound", id);
                    return;
                }

                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ValidateSnapshot.Result",
                    $"Valid={snapshot.Metadata.IsValid}, Checksum={(string.IsNullOrEmpty(snapshot.Metadata.Checksum) ? "None" : "Present")}");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.ValidateSnapshot.Error", ex.Message);
            }
        }

        //--------------------------------------------------------------------
        //HELP
        //--------------------------------------------------------------------

        public static void ShowHelp(string[] args)
        {
            DLogger.Log(LogSubsystems.Snapshot, "SnapshotCommands.Help", "Displayed");
        }
    }
}

