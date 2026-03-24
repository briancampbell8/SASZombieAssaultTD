/*
File:    Diagnostics.cs
Path:    Engine/GameLoop/Diagnostics.cs
Purpose: P11-09-01 - Contains all performance monitoring for GameLoop.
         Handles frame metrics collection and health reporting.

Role:     Game loop diagnostics specialist.
         - FrameDiagnostics class
         - Performance counters
         - Frame metrics collection
         - Health reporting
         - Diagnostic data structures

Notes:    Contains all diagnostic logic extracted from GameLoop.
         Isolated from main loop logic for clean separation of concerns.
         Provides comprehensive performance monitoring.
*/

using System;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Partial class containing diagnostics logic for GameLoop.
    /// </summary>
    public partial class GameLoop
    {
        /// <summary>
        /// Gets diagnostic information about the game loop.
        /// </summary>
        /// <returns>Game loop diagnostic information.</returns>
        internal GameLoopDiagnostics GetGameLoopDiagnostics()
        {
            return new GameLoopDiagnostics
            {
                State = State,
                IsInitialized = _isInitialized,
                IsRunning = _isRunning,
                FrameCount = _frameCount,
                AverageFrameTime = _averageFrameTime,
                MinFrameTime = _minFrameTime,
                MaxFrameTime = _maxFrameTime,
                FramesPerSecond = FramesPerSecond,
                FrameDiagnostics = _diagnostics.GetDiagnostics(),
                InputDiagnostics = GetInputDiagnostics(),
                TimingInfo = GetTimingInfo()
            };
        }

        /// <summary>
        /// Gets input diagnostics information.
        /// </summary>
        /// <returns>Input diagnostics.</returns>
        private InputDiagnostics GetInputDiagnostics()
        {
            // This would be implemented based on the actual input system
            return new InputDiagnostics
            {
                IsInputActive = _input != null,
                InputEventsProcessed = 0, // Would be tracked by input system
                LastInputTime = 0f, // Would be tracked by input system
                MousePosition = System.Drawing.Point.Empty, // Would come from input system
                KeyStates = new bool[256] // Would come from input system
            };
        }

        /// <summary>
        /// Resets all diagnostic counters.
        /// </summary>
        public void ResetDiagnostics()
        {
            lock (_stateLock)
            {
                ResetTimingStatistics();
                // Reset other diagnostic counters as needed
            }
        }

        /// <summary>
        /// Gets performance metrics for monitoring.
        /// </summary>
        /// <returns>Performance metrics.</returns>
        public GameLoopPerformanceMetrics GetPerformanceMetrics()
        {
            return new GameLoopPerformanceMetrics
            {
                FrameCount = _frameCount,
                AverageFrameTime = _averageFrameTime,
                FramesPerSecond = FramesPerSecond,
                ErrorCount = _diagnostics.ErrorCount,
                MemoryUsage = GC.GetTotalMemory(false),
                Uptime = DateTime.Now - (_lastFrameTime - TimeSpan.FromMilliseconds(_frameCount * _averageFrameTime * 1000))
            };
        }
    }

    /// <summary>
    /// Game loop diagnostic information.
    /// </summary>
    public class GameLoopDiagnostics
    {
        public GameLoopState State { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsRunning { get; set; }
        public int FrameCount { get; set; }
        public float AverageFrameTime { get; set; }
        public float MinFrameTime { get; set; }
        public float MaxFrameTime { get; set; }
        public float FramesPerSecond { get; set; }
        public FrameDiagnostics FrameDiagnostics { get; set; }
        public InputDiagnostics InputDiagnostics { get; set; }
        public TimingInfo TimingInfo { get; set; }
    }

    /// <summary>
    /// Frame diagnostics information.
    /// </summary>
    public class FrameDiagnostics
    {
        public int FrameCount { get; set; }
        public float AverageFrameTime { get; set; }
        public float MinFrameTime { get; set; }
        public float MaxFrameTime { get; set; }
        public float FramesPerSecond { get; set; }
        public int ErrorCount { get; set; }
        public Stopwatch FrameTimer { get; set; }

        public void BeginFrame()
        {
            FrameTimer?.Restart();
        }

        public void EndFrame()
        {
            FrameTimer?.Stop();
        }

        public void RecordFrameError(Exception error)
        {
            ErrorCount++;
        }

        public FrameDiagnostics GetDiagnostics()
        {
            return new FrameDiagnostics
            {
                FrameCount = FrameCount,
                AverageFrameTime = AverageFrameTime,
                MinFrameTime = MinFrameTime,
                MaxFrameTime = MaxFrameTime,
                FramesPerSecond = FramesPerSecond,
                ErrorCount = ErrorCount,
                FrameTimer = FrameTimer
            };
        }
    }

    /// <summary>
    /// Input diagnostics information.
    /// </summary>
    public class InputDiagnostics
    {
        public bool IsInputActive { get; set; }
        public int InputEventsProcessed { get; set; }
        public float LastInputTime { get; set; }
        public System.Drawing.Point MousePosition { get; set; }
        public bool[] KeyStates { get; set; }

        public InputDiagnostics GetDiagnostics()
        {
            return new InputDiagnostics
            {
                IsInputActive = IsInputActive,
                InputEventsProcessed = InputEventsProcessed,
                LastInputTime = LastInputTime,
                MousePosition = MousePosition,
                KeyStates = KeyStates
            };
        }
    }

    /// <summary>
    /// Performance metrics for monitoring.
    /// </summary>
    public class GameLoopPerformanceMetrics
    {
        public int FrameCount { get; set; }
        public float AverageFrameTime { get; set; }
        public float FramesPerSecond { get; set; }
        public int ErrorCount { get; set; }
        public long MemoryUsage { get; set; }
        public TimeSpan Uptime { get; set; }
    }
}
