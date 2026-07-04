// ====================================================================================================
//  FILE: Timing.cs
//  PATH: Engine/Diagnostics/
//  PROGRAM: Timing.cs
//  MODULE: Diagnostics Pipeline (High‑Precision Timing)
//
//  ROLE:
//      Provides high‑precision timing utilities for the engine, including frame
//      delta measurement, frame pacing support, and real‑time FPS calculation.
//      Serves as a foundational component for profiling, diagnostics overlays,
//      and performance analysis.
//
//  RESPONSIBILITIES:
//      - Measure frame-to-frame delta time using high‑resolution timers.
//      - Calculate real‑time frames per second (FPS).
//      - Provide stable timing data to FrameStats, DebugOverlay, and tools.
//      - Support profiling, performance tuning, and engine stability analysis.
//
//  NON-RESPONSIBILITIES:
//      - Writing logs or trace files (handled by Writer and sinks).
//      - Rendering diagnostics overlays (handled by DebugOverlay).
//      - Managing engine resources, gameplay logic, or rendering pipelines.
//      - Performing long‑term analytics or report generation.
//
//  ARCHITECTURAL NOTES:
//      - Must remain extremely lightweight to avoid impacting frame timing.
//      - Should be deterministic and side‑effect‑free outside timing measurement.
//      - Designed to integrate cleanly with future profiling and diagnostics systems.
// ====================================================================================================

using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    ///<summary>
    ///Tracks frame timing and calculates delta time between frames.
    ///</summary>
    public sealed class FrameTimer
    {
        private readonly Stopwatch _stopwatch = new();
        private double _lastFrameTime;

        ///<summary>
        ///Gets the time elapsed since the last frame, in seconds.
        ///</summary>
        public double DeltaTime { get; private set; }

        ///<summary>
        ///Starts or restarts the frame timer.
        ///</summary>
        public void Start()
        {
            _stopwatch.Restart();
            _lastFrameTime = 0.0;
            DeltaTime = 0.0;
        }

        ///<summary>
        ///Updates the timer and calculates the delta time for the current frame.
        ///</summary>
        public void Tick()
        {
            double now = _stopwatch.Elapsed.TotalSeconds;
            DeltaTime = now - _lastFrameTime;
            _lastFrameTime = now;
        }
    }

    ///<summary>
    ///Calculates frames per second (FPS) based on frame timing.
    ///</summary>
    public sealed class FpsCounter
    {
        private double _accumulatedTime;
        private int _frameCount;
        private double _currentFps;

        ///<summary>
        ///Gets the most recently calculated FPS value.
        ///</summary>
        public double Fps => _currentFps;

        ///<summary>
        ///Updates the FPS counter with the elapsed time for the current frame.
        ///</summary>
        ///<param name="deltaTime">The time elapsed since the last frame, in seconds.</param>
        public void Update(double deltaTime)
        {
            if (deltaTime <= 0) return;

            _accumulatedTime += deltaTime;
            _frameCount++;

            if (_accumulatedTime >= 1.0)
            {
                _currentFps = _frameCount / _accumulatedTime;
                _accumulatedTime = 0.0;
                _frameCount = 0;
            }
        }
    }
}



