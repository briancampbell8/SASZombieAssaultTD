// ====================================================================================================
//  FILE: HeartbeatMonitor.cs
//  PATH: Engine/Diagnostics/
//  PROGRAM: HeartbeatMonitor.cs
//  MODULE: Diagnostics Pipeline (Engine Liveness & Health Monitoring)
//
//  ROLE:
//      Monitors engine liveness by tracking heartbeat signals from the main loop.
//      Detects stalls, hangs, and irregular update intervals to support stability
//      diagnostics, watchdog systems, and automated health reporting.
//
//  RESPONSIBILITIES:
//      - Track periodic heartbeat signals from the engine loop.
//      - Detect missed, delayed, or irregular heartbeats indicating potential stalls.
//      - Expose liveness state to DiagnosticsMonitor, DebugOverlay, and tools.
//      - Support engine stability validation and early failure detection.
//
//  NON-RESPONSIBILITIES:
//      - Measuring per-frame performance metrics (handled by FrameStats/FrameDiagnostics).
//      - Writing logs or trace files (handled by Writer and trace sinks).
//      - Rendering diagnostics overlays (handled by DebugOverlay).
//      - Managing engine resources, gameplay logic, or rendering pipelines.
//
//  ARCHITECTURAL NOTES:
//      - Must remain lightweight and non-blocking to avoid affecting engine timing.
//      - Should be deterministic and side-effect-free outside heartbeat tracking.
//      - Designed to integrate with future watchdog, crash detection, and reporting systems.
// ====================================================================================================


using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    ///<summary>
    ///Tracks heartbeat ticks for the engine loop, providing simple timing and count diagnostics.
    ///</summary>
    public sealed class HeartbeatMonitor
    {
        /// Public Properties

        ///<summary>
        ///Total number of heartbeat ticks observed since startup.
        ///</summary>
        public long TickCount { get; private set; }

        ///<summary>
        ///The time (UTC) when the first heartbeat was recorded.
        ///</summary>
        public DateTime? FirstTickUtc { get; private set; }

        ///<summary>
        ///The time (UTC) when the most recent heartbeat was recorded.
        ///</summary>
        public DateTime? LastTickUtc { get; private set; }

        ///<summary>
        ///The duration between the last two ticks, if at least two ticks have occurred.
        ///</summary>
        public TimeSpan? LastDelta { get; private set; }

        ///

        /// Public Methods

        ///<summary>
        ///Records a heartbeat tick at the current UTC time.
        ///</summary>
        public void Tick()
        {
            var now = DateTime.UtcNow;

            if (FirstTickUtc is null)
            {
                FirstTickUtc = now;
            }

            if (LastTickUtc.HasValue)
            {
                LastDelta = now - LastTickUtc.Value;
            }

            LastTickUtc = now;
            TickCount++;
        }

        ///<summary>
        ///Resets all heartbeat counters and timestamps.
        ///</summary>
        public void Reset()
        {
            TickCount = 0;
            FirstTickUtc = null;
            LastTickUtc = null;
            LastDelta = null;
        }

        ///<summary>
        ///Gets the total elapsed time since the first tick.
        ///</summary>
        ///<returns>The total elapsed time as a <see cref="TimeSpan"/>.</returns>
        public TimeSpan? GetElapsedTime()
        {
            return FirstTickUtc.HasValue ? DateTime.UtcNow - FirstTickUtc.Value : null;
        }

        ///
    }
}


