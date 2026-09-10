// =====================================================================================================
//  FILE: PerformanceAnalyzer.cs
//  PATH: Engine/Performance/PerformanceAnalyzer.cs
//  SUBSYSTEM: Performance
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Performance
{
    /// <summary>
    /// Performance analyzer for optimization recommendations.
    /// </summary>
    public class PerformanceAnalyzer
    {
        public PerformanceTrend Trend { get; set; }
        public List<FrameMetrics> RecentFrames { get; set; }

        public PerformanceAnalyzer()
        {
            Trend = new PerformanceTrend();
            RecentFrames = new List<FrameMetrics>();
        }
    }
}
