using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Frame diagnostics system for FPS counting and performance monitoring.
    /// Implements P11-09-06: Add frame diagnostics without affecting frame pacing.
    /// </summary>
    public class FrameDiagnostics
    {
        ///  Public Properties

        /// <summary>
        /// Gets the current frames per second.
        /// </summary>
        public float CurrentFPS { get; private set; }

        /// <summary>
        /// Gets the average FPS over the measurement period.
        /// </summary>
        public float AverageFPS { get; private set; }

        /// <summary>
        /// Gets the minimum FPS recorded.
        /// </summary>
        public float MinFPS { get; private set; }

        /// <summary>
        /// Gets the maximum FPS recorded.
        /// </summary>
        public float MaxFPS { get; private set; }

        /// <summary>
        /// Gets the average frame time in milliseconds.
        /// </summary>
        public float AverageFrameTimeMs { get; private set; }

        /// <summary>
        /// Gets the total number of frames processed.
        /// </summary>
        public long TotalFrames { get; private set; }

        /// <summary>
        /// Gets the total elapsed time in seconds.
        /// </summary>
        public float TotalTime { get; private set; }

        /// 

        ///  Private Fields

        private float _fpsAccumulator;
        private int _fpsFrameCount;
        private float _totalFrameTime;
        private int _frameTimeCount;
        private readonly List<float> _frameTimeHistory;
        private readonly Queue<float> _recentFrameTimes;

        private const int HistorySize = 1000;
        private const int RecentFrameCount = 60; // Last 60 frames for rolling average

        /// 

        ///  Constructor

        /// <summary>
        /// Initializes a new instance of the FrameDiagnostics class.
        /// </summary>
        public FrameDiagnostics()
        {
            _frameTimeHistory = new List<float>(HistorySize);
            _recentFrameTimes = new Queue<float>(RecentFrameCount);
            Reset();
        }

        /// 

        ///  Public Methods

        /// <summary>
        /// Updates frame metrics based on the current frame.
        /// </summary>
        /// <param name="frameTime">The time taken to process the current frame in seconds.</param>
        /// <param name="deltaTime">The delta time for the current frame in seconds.</param>
        public void UpdateFrameMetrics(double frameTime, float deltaTime)
        {
            TotalFrames++;
            TotalTime += deltaTime;

            float frameTimeMs = (float)(frameTime * 1000.0);

            UpdateFrameTimeStatistics(frameTimeMs);
            UpdateFPSMetrics(deltaTime);
        }

        /// <summary>
        /// Gets the rolling average frame time over the last N frames.
        /// </summary>
        /// <param name="frameCount">Number of recent frames to average.</param>
        /// <returns>The average frame time in milliseconds.</returns>
        public float GetRollingAverageFrameTime(int frameCount = RecentFrameCount)
        {
            if (_recentFrameTimes.Count == 0)
                return 0f;

            float sum = 0f;
            int count = System.Math.Min(frameCount, _recentFrameTimes.Count);
            var recentTimes = _recentFrameTimes.ToArray();

            for (int i = System.Math.Max(0, recentTimes.Length - count); i < recentTimes.Length; i++)
            {
                sum += recentTimes[i];
            }

            return sum / count;
        }

        /// <summary>
        /// Gets performance statistics as a formatted string.
        /// </summary>
        /// <returns>Formatted performance statistics.</returns>
        public string GetPerformanceStats()
        {
            float rollingAvg = GetRollingAverageFrameTime();
            return $"FPS: {CurrentFPS:F1} (Avg: {AverageFPS:F1}, Min: {MinFPS:F1}, Max: {MaxFPS:F1}) | " +
                   $"Frame Time: {AverageFrameTimeMs:F2}ms (Rolling: {rollingAvg:F2}ms) | " +
                   $"Frames: {TotalFrames} | Time: {TotalTime:F1}s";
        }

        /// <summary>
        /// Gets detailed performance data for debugging.
        /// </summary>
        /// <returns>Detailed performance information.</returns>
        public string GetDetailedStats()
        {
            float rollingAvg = GetRollingAverageFrameTime();
            float variance = CalculateFrameTimeVariance();

            return "=== Frame Diagnostics ===\n" +
                   $"Current FPS: {CurrentFPS:F2}\n" +
                   $"Average FPS: {AverageFPS:F2}\n" +
                   $"Min/Max FPS: {MinFPS:F2}/{MaxFPS:F2}\n" +
                   $"Average Frame Time: {AverageFrameTimeMs:F3}ms\n" +
                   $"Rolling Frame Time (60): {rollingAvg:F3}ms\n" +
                   $"Frame Time Variance: {variance:F3}ms\n" +
                   $"Total Frames: {TotalFrames:N0}\n" +
                   $"Total Time: {TotalTime:F1}s\n" +
                   $"History Size: {_frameTimeHistory.Count}/{HistorySize}";
        }

        /// <summary>
        /// Resets all diagnostics to initial state.
        /// </summary>
        public void Reset()
        {
            CurrentFPS = 0f;
            AverageFPS = 0f;
            MinFPS = 0f;
            MaxFPS = 0f;
            AverageFrameTimeMs = 0f;
            TotalFrames = 0;
            TotalTime = 0f;

            _fpsAccumulator = 0f;
            _fpsFrameCount = 0;
            _totalFrameTime = 0f;
            _frameTimeCount = 0;

            _frameTimeHistory.Clear();
            _recentFrameTimes.Clear();

            DLogger.Log("INFO", "FrameDiagnostics reset");
        }

        /// <summary>
        /// Gets frame time history for analysis.
        /// </summary>
        /// <returns>Copy of frame time history in milliseconds.</returns>
        public float[] GetFrameTimeHistory()
        {
            return _frameTimeHistory.ToArray();
        }

        /// <summary>
        /// Checks if performance is within acceptable thresholds.
        /// </summary>
        /// <param name="targetFPS">Target FPS threshold (default: 60).</param>
        /// <param name="maxFrameTimeMs">Maximum acceptable frame time in milliseconds (default: 16.67ms for 60 FPS).</param>
        /// <returns>True if performance is acceptable, false otherwise.</returns>
        public bool IsPerformanceAcceptable(float targetFPS = 60f, float maxFrameTimeMs = 16.67f)
        {
            return CurrentFPS >= targetFPS * 0.9f && AverageFrameTimeMs <= maxFrameTimeMs * 1.1f;
        }

        /// 

        ///  Private Methods

        private void UpdateFrameTimeStatistics(float frameTimeMs)
        {
            _totalFrameTime += frameTimeMs;
            _frameTimeCount++;
            AverageFrameTimeMs = _totalFrameTime / _frameTimeCount;

            // Maintain frame time history
            _frameTimeHistory.Add(frameTimeMs);
            if (_frameTimeHistory.Count > HistorySize)
            {
                _frameTimeHistory.RemoveAt(0);
            }

            // Maintain recent frame times for rolling average
            _recentFrameTimes.Enqueue(frameTimeMs);
            if (_recentFrameTimes.Count > RecentFrameCount)
            {
                _recentFrameTimes.Dequeue();
            }
        }

        private void UpdateFPSMetrics(float deltaTime)
        {
            _fpsAccumulator += deltaTime;
            _fpsFrameCount++;

            // Update FPS metrics every second
            if (_fpsAccumulator >= 1.0f)
            {
                CurrentFPS = _fpsFrameCount / _fpsAccumulator;
                AverageFPS = TotalFrames / TotalTime;

                // Update min/max FPS
                if (CurrentFPS < MinFPS || MinFPS == 0)
                    MinFPS = CurrentFPS;
                if (CurrentFPS > MaxFPS)
                    MaxFPS = CurrentFPS;

                // Reset accumulators
                _fpsAccumulator = 0f;
                _fpsFrameCount = 0;
            }
        }

        private float CalculateFrameTimeVariance()
        {
            if (_recentFrameTimes.Count < 2)
                return 0f;

            float sum = 0f;
            float count = _recentFrameTimes.Count;

            foreach (float frameTime in _recentFrameTimes)
            {
                sum += frameTime;
            }

            float mean = sum / count;
            float variance = 0f;

            foreach (float frameTime in _recentFrameTimes)
            {
                float diff = frameTime - mean;
                variance += diff * diff;
            }

            return variance / count;
        }

        /// 
    }
}




