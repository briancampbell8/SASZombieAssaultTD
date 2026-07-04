// ====================================================================================================
//  FILE: FrameStats.cs
//  PATH: Engine/Diagnostics/
//  PROGRAM: FrameStats.cs
//  MODULE: Diagnostics Pipeline (Frame-Level Metrics)
//
//  ROLE:
//      Collects and exposes per-frame diagnostic metrics for the engine.
//      Tracks timing, subsystem performance, and frame-level statistics to support
//      profiling, debugging, and real-time diagnostics overlays.
// 
//
//  RESPONSIBILITIES: 
//      - Measure and record per-frame timing and performance metrics.
//      - Track subsystem execution costs and diagnostic counters.
//      - Provide real-time metrics to DebugOverlay, DiagnosticsMonitor, and tools.
//      - Support profiling, performance tuning, and engine stability analysis.
//     
//
//  NON-RESPONSIBILITIES: 
//      - Writing logs or trace files (handled by Writer and trace sinks).
//      - Rendering diagnostics overlays (handled by DebugOverlay).
//      - Managing engine resources, gameplay logic, or rendering pipelines.
//      - Performing long-term analytics or report generation.
// 
//
//  ARCHITECTURAL NOTES:
//      - Must remain lightweight to avoid impacting frame timing.
//      - Should be deterministic and side-effect-free outside metric collection.
//      - Designed to integrate with future profiling and reporting systems.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    ///<summary>
    ///Tracks frame timing and statistics for diagnostics.
    ///Example usage:
    ///  var stats = new FrameStats();
    ///  stats.OnFrame(deltaSeconds);
    ///  float fps = stats.FramesPerSecond;
    ///</summary>
    public sealed class FrameStats
    {
        /// Public Properties

        ///<summary>
        ///Total number of frames rendered since startup.
        ///</summary>
        public long FrameCount { get; private set; }

        ///<summary>
        ///The most recently computed frames-per-second value.
        ///</summary>
        public float FramesPerSecond { get; private set; }

        ///

        /// Private Fields

        private float _accumulatedTime;
        private int _framesThisSecond;

        ///

        /// Public Methods

        ///<summary>
        ///Call once per frame with the elapsed time (in seconds) since the previous frame.
        ///</summary>
        ///<param name="deltaSeconds">The time elapsed since the last frame, in seconds.</param>
        public void OnFrame(float deltaSeconds)
        {
            if (deltaSeconds <= 0)
                return;

            FrameCount++;
            _framesThisSecond++;
            _accumulatedTime += deltaSeconds;

            if (_accumulatedTime >= 1.0f)
            {
                FramesPerSecond = _framesThisSecond / _accumulatedTime;
                ResetFrameAccumulator();
            }
        }

        ///<summary>
        ///Resets all frame statistics.
        ///</summary>
        public void Reset()
        {
            FrameCount = 0;
            FramesPerSecond = 0f;
            ResetFrameAccumulator();
        }

        ///

        /// Private Methods

        ///<summary>
        ///Resets the frame accumulator used for FPS calculation.
        ///</summary>
        private void ResetFrameAccumulator()
        {
            _accumulatedTime = 0f;
            _framesThisSecond = 0;
        }

        ///
    }
}


