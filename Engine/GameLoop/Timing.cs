/*
File:    Timing.cs
Path:    Engine/GameLoop/Timing.cs
Purpose: P11-09-01 - Contains all timing-related logic for GameLoop.
         Implements the fixed timestep game loop with accumulator.

Role:     Game loop timing specialist.
         - Fixed timestep accumulator logic
         - Delta time calculations
         - Frame pacing and rate limiting
         - Main loop timing coordination
         - Time synchronization

Notes:    Contains all timing logic extracted from GameLoop.
         Implements deterministic physics with fixed timestep.
         Pure timing coordination, no subsystem logic.
*/

using System;
using System.Threading;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Systems
//
{
    ///<summary>
    ///Partial class containing timing logic for GameLoop.
    ///</summary>
    public partial class GameLoop
    {
        //Frame timing
        private DateTime _lastFrameTime;
        private float _frameAccumulator = 0f;
        private const float TargetFrameTime = 1f / 60f; //60 FPS target

        ///<summary>
        ///Gets the target frame time in seconds.
        ///</summary>
        public float TargetFrameTimeSec => TargetFrameTime;

        ///<summary>
        ///Gets the current frame accumulator value.
        ///</summary>
        public float FrameAccumulator => _frameAccumulator;

        ///<summary>
        ///Gets the current frames per second.
        ///</summary>
        public float FramesPerSecond => _averageFrameTime > 0 ? 1f / _averageFrameTime : 0f;

        ///<summary>
        ///Performs the main game loop with fixed timestep.
        ///</summary>
        private void PerformMainLoop()
        {
            _lastFrameTime = DateTime.Now;
            _frameCount = 0;
            _totalFrameTime = 0f;
            _averageFrameTime = 0f;
            _minFrameTime = float.MaxValue;
            _maxFrameTime = float.MinValue;

            try
            {
                while (_isRunning)
                {
                    var currentTime = DateTime.Now;
                    var deltaTime = (float)(currentTime - _lastFrameTime).TotalSeconds;
                    _lastFrameTime = currentTime;

                    //Fixed timestep with accumulator
                    _frameAccumulator += deltaTime;

                    while (_frameAccumulator >= TargetFrameTime)
                    {
                        ProcessFrame(TargetFrameTime);
                        _frameAccumulator -= TargetFrameTime;
                        _frameCount++;
                        _totalFrameTime += TargetFrameTime;

                        //Calculate average frame time
                        _averageFrameTime = _totalFrameTime / _frameCount;

                        //Update min/max frame times
                        UpdateFrameTimeStatistics(deltaTime);
                    }

                    //Frame rate limiting
                    PerformFrameRateLimiting(currentTime);
                }
            }
            finally
            {
                _isRunning = false;
                DLogger.Log($"Game loop completed. Total frames: {_frameCount}, Average frame time: {_averageFrameTime:F4}s, FPS: {FramesPerSecond:F2}");
            }
        }

        ///<summary>
        ///Processes a single frame with update and render phases.
        ///</summary>
        ///<param name="deltaTime">Fixed timestep for this frame.</param>
        private void ProcessFrame(float deltaTime)
        {
            try
            {
                //Start frame diagnostics
                _diagnostics.BeginFrame();

                //Process input first
                _input.ProcessInput();

                //Update game state
                _gameRoot.Update(deltaTime);

                //Render frame
                _gameRoot.Render();

                //End frame diagnostics
                _diagnostics.EndFrame();

                DLogger.Log($"Frame processed in {deltaTime:F4}s");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.GameLoop, LogLevel.Info, "ERROR", $"Frame processing failed: {ex.Message}");
                DLogger.Log(LogSubsystems.GameLoop, LogLevel.Error,
                    ex.ToString(), "Frame processing");
                _diagnostics.RecordFrameError(ex);

                //Decide whether to continue or shutdown based on error severity
                if (IsCriticalError(ex))
                {
                    DLogger.Log(LogSubsystems.GameLoop,
                        LogLevel.Error, "ERROR", "Critical error detected, shutting down game loop");
                    _isRunning = false;
                }
            }
        }

        ///<summary>
        ///Updates frame time statistics.
        ///</summary>
        ///<param name="deltaTime">The current frame time.</param>
        private void UpdateFrameTimeStatistics(float deltaTime)
        {
            if (deltaTime < _minFrameTime)
                _minFrameTime = deltaTime;
            if (deltaTime > _maxFrameTime)
                _maxFrameTime = deltaTime;
        }

        ///<summary>
        ///Performs frame rate limiting to maintain consistent performance.
        ///</summary>
        ///<param name="frameStartTime">The time when the frame started.</param>
        private void PerformFrameRateLimiting(DateTime frameStartTime)
        {
            var frameTime = (float)(DateTime.Now - frameStartTime).TotalSeconds;
            if (frameTime < TargetFrameTime)
            {
                var sleepTime = (int)((TargetFrameTime - frameTime) * 1000);
                Thread.Sleep(sleepTime);
            }
        }

        ///<summary>
        ///Determines if an error is critical enough to shutdown the game loop.
        ///</summary>
        ///<param name="ex">The exception to evaluate.</param>
        ///<returns>True if the error is critical.</returns>
        private bool IsCriticalError(Exception ex)
        {
            //Consider OutOfMemoryException, StackOverflowException, etc. as critical
            return ex is OutOfMemoryException ||
                   ex is StackOverflowException ||
                   ex is AccessViolationException;
        }

        ///<summary>
        ///Resets timing statistics.
        ///</summary>
        private void ResetTimingStatistics()
        {
            _frameCount = 0;
            _totalFrameTime = 0f;
            _averageFrameTime = 0f;
            _minFrameTime = float.MaxValue;
            _maxFrameTime = float.MinValue;
            _frameAccumulator = 0f;
        }

        ///<summary>
        ///Gets detailed timing information.
        ///</summary>
        ///<returns>Timing information.</returns>
        public TimingInfo GetTimingInfo()
        {
            return new TimingInfo
            {
                FrameCount = _frameCount,
                AverageFrameTime = _averageFrameTime,
                MinFrameTime = _minFrameTime,
                MaxFrameTime = _maxFrameTime,
                FramesPerSecond = FramesPerSecond,
                TargetFrameTime = TargetFrameTime,
                FrameAccumulator = _frameAccumulator
            };
        }
    }

    ///<summary>
    ///Timing information for the game loop.
    ///</summary>
    public class TimingInfo
    {
        public int FrameCount { get; set; }
        public float AverageFrameTime { get; set; }
        public float MinFrameTime { get; set; }
        public float MaxFrameTime { get; set; }
        public float FramesPerSecond { get; set; }
        public float TargetFrameTime { get; set; }
        public float FrameAccumulator { get; set; }
    }
}
