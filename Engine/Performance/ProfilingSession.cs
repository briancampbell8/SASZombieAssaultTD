// =====================================================================================================
//  FILE: ProfilingSession.cs
//  PATH: Engine/Performance/ProfilingSession.cs
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

namespace SASZombieAssaultTD.Engine.Performance
{
    public sealed class ProfilingSession : IDisposable
    {
        private readonly PerformanceProfiler _profiler;
        private readonly string _operationName;
        private readonly System.Diagnostics.Stopwatch _timer;

        public ProfilingSession(PerformanceProfiler profiler, string operationName)
        {
            _profiler = profiler;
            _operationName = operationName;
            _timer = System.Diagnostics.Stopwatch.StartNew();
        }

        /// <summary>
        /// Captures current execution tracking ticks and calculates absolute millisecond conversions.
        /// </summary>
        /// <summary>
        /// Captures current execution tracking ticks and calculates absolute millisecond conversions.
        /// </summary>
        public void Dispose()
        {
            _timer.Stop();

            // Standard deterministic C# math scale translation converting raw ticks into millisecond floats
            float elapsedMs = (_timer.ElapsedTicks / (float)System.Diagnostics.Stopwatch.Frequency) * 1000f;

            // 🟢 Swapped RecordMetric to match your actual method name: RecordMeasurement
            _profiler?.RecordMeasurement(_operationName, elapsedMs);
        }

    }

}
