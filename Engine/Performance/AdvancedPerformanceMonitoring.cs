// =====================================================================================================
//  FILE: AdvancedPerformanceMonitoring.cs
//  PATH: Engine/Performance/AdvancedPerformanceMonitoring.cs
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

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Performance
{
    public class AdvancedPerformanceMonitoring
    {
        private readonly PerformanceProfiler _profiler;
        private readonly Dictionary<string, PerformanceTrend> _performanceTrends = new();
        private readonly CircularBuffer<FrameMetrics> _frameMetrics = new(300); //5 seconds at 60 FPS
        private readonly PerformanceAnalyzer _analyzer = new();
        private volatile bool _adaptiveOptimizationEnabled = true;
        private volatile float _performanceThreshold = 16.67f; //60 FPS target

        public AdvancedPerformanceMonitoring(PerformanceProfiler profiler) => _profiler = profiler ?? throw new ArgumentNullException(nameof(profiler));
    }
}
