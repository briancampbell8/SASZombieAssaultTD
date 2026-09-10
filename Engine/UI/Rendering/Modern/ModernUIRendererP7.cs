// =====================================================================================================
//  FILE: ModernUIRendererP7.cs
//  PATH: Engine/UI/Rendering/Modern/ModernUIRendererP7.cs
//  SUBSYSTEM: Modern UI Rendering — Performance Subsystem (Partial)
//
//  ROLE:
//      Monitors render performance metrics and adjusts ModernUIRenderer quality profiles
//      using hysteresis-based decisions to avoid frame-to-frame thrashing.
//
//  RESPONSIBILITIES:
//      - Evaluate FPS, GPU load, and CPU load against target thresholds.
//      - Track consecutive breached/headroom frames over a fixed evaluation window.
//      - Decide when to increase or decrease quality levels.
//      - Apply quality profile changes deterministically.
//      - Keep logging off the hot path except on state transitions.
//
//  NON-RESPONSIBILITIES:
//      - Actual rendering or command construction.
//      - Batching, sorting, or command ordering.
//      - GPU resource ownership or lifetime management.
//      - UI element layout or culling logic.
//
//  ARCHITECTURAL NOTES:
//      - Uses a hysteresis window to prevent jittery quality changes.
//      - This partial MUST own all fields it uses (P7_* naming).
//      - No cross-partial field access is permitted.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Diagnostics.Performance;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        // --------------------------------------------------------------------
        // P7 OWNED FIELDS — NO CROSS-PARTIAL DEPENDENCIES
        // --------------------------------------------------------------------
        private int P7_currentQualityLevel = 3;
        private int P7_consecutiveBreachedFrames;
        private int P7_consecutiveHeadroomFrames;

        private const int P7_EvaluationWindowFrameCount = 60;

        private RenderPerformanceMonitor P7_performanceMonitor;

        // --------------------------------------------------------------------
        // BINDING API
        // --------------------------------------------------------------------
        internal void P7SetPerformanceMonitor(RenderPerformanceMonitor monitor)
        {
            P7_performanceMonitor = monitor;
        }

        // --------------------------------------------------------------------
        // QUALITY ADJUSTMENT ENTRY POINT
        // --------------------------------------------------------------------
        private void P7AdjustQualityIfNeeded()
        {
            try
            {
                RenderPerformanceStats stats = P7_performanceMonitor.GetStats();

                QualityDecision decision = P7EvaluatePerformance(stats);

                if (decision != QualityDecision.None)
                    P7ApplyQualitySettings(decision);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    $"[ModernUIRendererP7] AdjustQualityIfNeeded execution failed: {ex.Message}");
                throw;
            }
        }

        // --------------------------------------------------------------------
        // PERFORMANCE EVALUATION
        // --------------------------------------------------------------------
        private QualityDecision P7EvaluatePerformance(RenderPerformanceStats stats)
        {
            try
            {
                if (stats.Fps < 50.0 || stats.GpuLoad > 90.0f || stats.CpuLoad > 90.0f)
                {
                    P7_consecutiveHeadroomFrames = 0;
                    P7_consecutiveBreachedFrames++;

                    if (P7_consecutiveBreachedFrames >= P7_EvaluationWindowFrameCount &&
                        P7_currentQualityLevel > 0)
                    {
                        P7_consecutiveBreachedFrames = 0;
                        return QualityDecision.Decrease;
                    }

                    return QualityDecision.None;
                }

                if (stats.Fps > 110.0 && stats.GpuLoad < 60.0f && stats.CpuLoad < 60.0f)
                {
                    P7_consecutiveBreachedFrames = 0;
                    P7_consecutiveHeadroomFrames++;

                    if (P7_consecutiveHeadroomFrames >= P7_EvaluationWindowFrameCount &&
                        P7_currentQualityLevel < 5)
                    {
                        P7_consecutiveHeadroomFrames = 0;
                        return QualityDecision.Increase;
                    }

                    return QualityDecision.None;
                }

                P7_consecutiveBreachedFrames = 0;
                P7_consecutiveHeadroomFrames = 0;
                return QualityDecision.None;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error",
                    $"[ModernUIRendererP7] EvaluatePerformance failed: {ex.Message}");
                throw;
            }
        }

        // --------------------------------------------------------------------
        // APPLY QUALITY SETTINGS
        // --------------------------------------------------------------------
        private void P7ApplyQualitySettings(QualityDecision decision)
        {
            int oldLevel = P7_currentQualityLevel;

            switch (decision)
            {
                case QualityDecision.Decrease:
                    P7_currentQualityLevel = System.Math.Max(0, P7_currentQualityLevel - 1);
                    break;

                case QualityDecision.Increase:
                    P7_currentQualityLevel = System.Math.Min(5, P7_currentQualityLevel + 1);
                    break;

                case QualityDecision.None:
                default:
                    return;
            }

            if (oldLevel != P7_currentQualityLevel)
            {
                DLogger.Log(
                    LogSubsystems.ResourcesPipeline,
                    "Diagnostics",
                    $"[ModernUIRendererP7] Subsystem profile shifted from Level {oldLevel} → {P7_currentQualityLevel}. Trigger action: {decision}"
                );
            }
        }


    }
}
