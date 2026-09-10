// =====================================================================================================
//  FILE: GameRootExtensions.cs
//  PATH: Engine/GameRoot/GameRootExtensions.cs
//  MODULE: GameRoot
//
//  ROLE:
//      Deterministic, type‑safe extension methods for GameRootMain, enabling lifecycle state
//      inspection, subsystem availability checks, timing diagnostics, and safe status queries.
//
//  RESPONSIBILITIES:
//      - Provide IsInitialized() behavior for the GameRoot subsystem.
//      - Provide IsRunning() behavior for the GameRoot subsystem.
//      - Provide AreSubsystemsReady() behavior for the GameRoot subsystem.
//      - Provide GetLastFrameDelta() behavior for the GameRoot subsystem.
//      - Provide LogTimingDiagnostics() behavior for the GameRoot subsystem.
//      - Provide GetStatusSummary() behavior for the GameRoot subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations directly.
//      - Managing ECS entities or world‑grid occupancy directly.
//      - Allocating subsystems or mutating GameRootMain behavior.
//      - Replacing or overriding GameRootMain’s deterministic lifecycle.
//
//  NOTES:
//      Reflection removed; deterministic public accessors used exclusively.
//      Relocated from Engine/Extensions to Engine/GameRoot.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot
{
    /// <summary>
    /// Deterministic helper extensions for GameRootMain.
    /// </summary>
    public static class GameRootExtensions
    {
        // ----------------------------------------------------------------------------------------------
        //  LIFECYCLE STATE HELPERS
        // ----------------------------------------------------------------------------------------------

        public static bool IsInitialized(this GameRootMain root)
        {
            return root != null && root.Initialized;
        }

        public static bool IsRunning(this GameRootMain root)
        {
            return root != null && root.Running;
        }

        // ----------------------------------------------------------------------------------------------
        //  SUBSYSTEM AVAILABILITY HELPERS
        // ----------------------------------------------------------------------------------------------

        public static bool AreSubsystemsReady(this GameRootMain root)
        {
            if (root == null)
                return false;

            // Deterministic public-accessor check only.
            return
                root.SystemRegistry != null &&
                root.SystemManager != null &&
                root.Initialized && // ensures initialization pipeline executed
                root.Running;        // ensures runtime pipeline active
        }

        // ----------------------------------------------------------------------------------------------
        //  TIMING / FRAME DIAGNOSTICS
        // ----------------------------------------------------------------------------------------------

        public static float GetLastFrameDelta(this GameRootMain root)
        {
            if (root == null)
                return 0f;

            DateTime last = root.LastFrameTime;
            return (float)(DateTime.Now - last).TotalSeconds;
        }

        public static void LogTimingDiagnostics(this GameRootMain root)
        {
            if (root == null)
            {
                DLogger.Log(LogSubsystems.GameRoot, LogLevel.Warning,
                    "[GameRootDebug] No GameRootMain instance.");
                return;
            }

            float delta = root.GetLastFrameDelta();
            bool running = root.IsRunning();
            bool initialized = root.IsInitialized();

            DLogger.Log(
                LogSubsystems.GameRoot,
                LogLevel.Info,
                $"[GameRootDebug] Initialized={initialized}, Running={running}, LastFrameDelta={delta:F4}s"
            );
        }

        // ----------------------------------------------------------------------------------------------
        //  SAFE STATUS SUMMARY
        // ----------------------------------------------------------------------------------------------

        public static string GetStatusSummary(this GameRootMain root)
        {
            if (root == null)
                return "[GameRootDebug] No GameRootMain instance.";

            return
                $"Initialized: {root.IsInitialized()}, " +
                $"Running: {root.IsRunning()}, " +
                $"SubsystemsReady: {root.AreSubsystemsReady()}";
        }
    }
}
