//============================================================================
//File: LastBuildTrigger.cs
//Program: LastBuildTrigger
//Subsystem: Snapshot System / Daily Finalization
//
//Purpose:
//    Provides a manual trigger for marking the last build of the day.
//    Integrates with SnapshotCapture to finalize today's snapshot,
//    timestamp it, and optionally clean up old snapshots.
//
//Doctrine:
//    - No System.Diagnostics
//    - All logging via DLogger.Log()
//    - Deterministic, grep‑friendly trace naming
//    - No fallback logic except explicit exception propagation
//============================================================================

using System;
//

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Snapshot
{
    public static class LastBuildTrigger
    {
        //private static object SnapshotCapture; //Forward declaration

        //=====================================================================
        //MAIN ENTRY POINT
        //=====================================================================

        ///<summary>
        ///Marks the current build as the last build of the day.
        ///Finalizes the daily snapshot and optionally cleans up old snapshots.
        ///</summary>
        public static void MarkLastBuild(bool cleanupOldSnapshots = true, int keepCount = 30)
        {
            DLogger.Log("LastBuildTrigger.MarkLastBuild.Start",
                $"cleanup={cleanupOldSnapshots}, keep={keepCount}");

            try
            {
                DLogger.Log("LastBuildTrigger.MarkLastBuild.Header",
                    $"Triggered at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                //Finalize the daily snapshot
                //object value = SnapshotCapture.FinalizeDailySnapshot(); NOTE: This method does not return a value, so we just call it directly.
                SnapshotCapture.FinalizeDailySnapshot();

                //Optional cleanup
                if (cleanupOldSnapshots)
                {
                    SnapshotCapture.CleanupOldSnapshots(keepCount);
                }

                DLogger.Log("LastBuildTrigger.MarkLastBuild.Success",
                    "Daily snapshot finalized successfully");

                //Show final status
                SnapshotCapture.StatusCommand(null);

                DLogger.Log("LastBuildTrigger.MarkLastBuild.Complete", "OK");
            }
            catch (Exception ex)
            {
                DLogger.Log("LastBuildTrigger.MarkLastBuild.Error", ex.Message);
                throw;
            }
        }

        //=====================================================================
        //QUICK FINALIZE
        //=====================================================================

        ///<summary>
        ///Finalizes the daily snapshot without cleanup.
        ///</summary>
        public static void QuickFinalize()
        {
            DLogger.Log("LastBuildTrigger.QuickFinalize.Start", "No cleanup");
            MarkLastBuild(cleanupOldSnapshots: false);
        }

        //=====================================================================
        //END-OF-DAY HEURISTIC
        //=====================================================================

        ///<summary>
        ///Returns true if the current time suggests "end of day".
        ///</summary>
        public static bool IsEndOfDay()
        {
            var now = DateTime.Now;
            bool result = now.Hour >= 18 || now.Hour <= 2;

            DLogger.Log("LastBuildTrigger.IsEndOfDay.Check",
                $"Hour={now.Hour}, Result={result}");

            return result;
        }

        //=====================================================================
        //CONTEXT REPORTING
        //=====================================================================

        ///<summary>
        ///Displays snapshot and build context information.
        ///</summary>
        public static void ShowBuildContext()
        {
            var now = DateTime.Now;
            var snapshotInfo = SnapshotCapture.GetSnapshotInfo();

            DLogger.Log("LastBuildTrigger.Context.Header", "=== BUILD CONTEXT ===");
            DLogger.Log("LastBuildTrigger.Context.Time", now.ToString("yyyy-MM-dd HH:mm:ss"));
            DLogger.Log("LastBuildTrigger.Context.DayOfWeek", now.DayOfWeek.ToString());
            DLogger.Log("LastBuildTrigger.Context.Hour", now.Hour.ToString());
            DLogger.Log("LastBuildTrigger.Context.EndOfDay", IsEndOfDay().ToString());

            if (snapshotInfo.Exists)
            {
                DLogger.Log("LastBuildTrigger.Context.Snapshot.Exists",
                    $"Size={snapshotInfo.SizeBytes:N0}, Created={snapshotInfo.CreatedTime:HH:mm:ss}, Age={(now - snapshotInfo.CreatedTime).TotalMinutes:F1}m");
            }
            else
            {
                DLogger.Log("LastBuildTrigger.Context.Snapshot.None", "No current snapshot found");
            }

            var finalized = SnapshotCapture.ListFinalizedSnapshots();
            DLogger.Log("LastBuildTrigger.Context.FinalizedCount", finalized.Length.ToString());

            DLogger.Log("LastBuildTrigger.Context.Footer", "=== END CONTEXT ===");
        }
    }
}
