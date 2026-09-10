// =====================================================================================================
//  FILE: Renderer-GpuTiming.cs
//  PATH: Engine/Render/LegacyRender/Renderer-GpuTiming.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Provides legacy-compatible GPU timing instrumentation for the renderer. This partial tracks
//      per-frame GPU durations, maintains a rolling history buffer, and exposes deterministic timing
//      statistics for diagnostics and performance monitoring.
//
//  RESPONSIBILITIES:
//      - Begin and end GPU timing windows.
//      - Record per-frame GPU durations into a rolling buffer.
//      - Provide average, minimum, and maximum GPU timing statistics.
//      - Emit deterministic diagnostic logs for timing operations.
//
//  NON-RESPONSIBILITIES:
//      - Performing actual GPU timestamp queries (modernization will replace this stub).
//      - Managing frame lifecycle, pacing, or presentation.
//      - Implementing CPU timing or delta-time logic.
//      - Handling initialization or viewport configuration.
//
//  ARCHITECTURAL NOTES:
//      - GPU timing values are legacy stub values until the modern GPU timestamp pipeline is active.
//      - The rolling buffer is capped at 60 entries to maintain stable memory usage.
//      - All timing operations must remain safe even if the renderer is partially initialized.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Begins a GPU timing window. No real GPU timestamp queries occur in the legacy pipeline.
        /// </summary>
        public void BeginGpuTiming()
        {
            if (!_gpuTimingEnabled)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "Renderer: BeginGpuTiming skipped (disabled)");
                return;
            }

            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "Renderer: BeginGpuTiming");
        }

        /// <summary>
        /// Ends a GPU timing window and records a legacy timing value into the rolling buffer.
        /// </summary>
        public void EndGpuTiming()
        {
            if (!_gpuTimingEnabled)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "Renderer: EndGpuTiming skipped (disabled)");
                return;
            }

            // Legacy stub timing value (16,666 microseconds ≈ 60 FPS)
            const long frameTimeMicroseconds = 16666L;

            _gpuFrameTimes.Add(frameTimeMicroseconds);

            // Maintain a rolling buffer of 60 samples
            if (_gpuFrameTimes.Count > 60)
                _gpuFrameTimes.RemoveAt(0);

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Trace,
                $"Renderer: EndGpuTiming — {frameTimeMicroseconds}μs"
            );
        }

        /// <summary>
        /// Returns average, minimum, and maximum GPU timing statistics in milliseconds.
        /// </summary>
        public (float averageMs, float minMs, float maxMs) GetGpuTimingStats()
        {
            if (_gpuFrameTimes.Count == 0)
                return (0f, 0f, 0f);

            long sum = 0;
            long min = long.MaxValue;
            long max = long.MinValue;

            foreach (var time in _gpuFrameTimes)
            {
                sum += time;
                if (time < min) min = time;
                if (time > max) max = time;
            }

            float avgMs = (sum / (float)_gpuFrameTimes.Count) / 1000f;
            float minMs = min / 1000f;
            float maxMs = max / 1000f;

            return (avgMs, minMs, maxMs);
        }
    }
}
