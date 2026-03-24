using System;
using System.Diagnostics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Performance
{
    /// <summary>
    /// Frame pacing system for maintaining stable frame rates.
    /// P30-01-03: Implement frame pacing (target FPS).
    /// P30-01-10: Verification checklist (stable frame pacing).
    /// </summary>
    public class FramePacer
    {
        private readonly float _targetFPS;
        private readonly float _targetFrameTime;
        private readonly Stopwatch _frameTimer;
        private readonly Stopwatch _sleepTimer;
        private float _lastFrameTime;
        private float _accumulatedTime;
        private int _frameCount;
        private float _averageFPS;
        private bool _enabled;
        private FramePacingMode _pacingMode;
        private float _maxFrameTime;
        private float _minFrameTime;
        private int _droppedFrames;

        /// <summary>
        /// Gets the target FPS.
        /// </summary>
        public float TargetFPS => _targetFPS;

        /// <summary>
        /// Gets the current average FPS.
        /// </summary>
        public float AverageFPS => _averageFPS;

        /// <summary>
        /// Gets the last frame time in milliseconds.
        /// </summary>
        public float LastFrameTime => _lastFrameTime;

        /// <summary>
        /// Gets the number of dropped frames.
        /// </summary>
        public int DroppedFrames => _droppedFrames;

        /// <summary>
        /// Gets the maximum frame time recorded.
        /// </summary>
        public float MaxFrameTime => _maxFrameTime;

        /// <summary>
        /// Gets the minimum frame time recorded.
        /// </summary>
        public float MinFrameTime => _minFrameTime;

        /// <summary>
        /// Gets or sets whether frame pacing is enabled.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Gets or sets the frame pacing mode.
        /// </summary>
        public FramePacingMode PacingMode
        {
            get => _pacingMode;
            set => _pacingMode = value;
        }

        /// <summary>
        /// Event fired when a frame is dropped.
        /// </summary>
        public event Action<int> OnFrameDropped;

        /// <summary>
        /// Event fired when frame pacing statistics are updated.
        /// </summary>
        public event Action<FramePacer> OnStatisticsUpdated;

        /// <summary>
        /// Initializes a new frame pacer.
        /// </summary>
        /// <param name="targetFPS">Target frames per second.</param>
        public FramePacer(float targetFPS = 60f)
        {
            _targetFPS = System.Math.Max(1f, targetFPS);
            _targetFrameTime = 1000f / _targetFPS;
            _frameTimer = new Stopwatch();
            _sleepTimer = new Stopwatch();
            _lastFrameTime = _targetFrameTime;
            _accumulatedTime = 0f;
            _frameCount = 0;
            _averageFPS = _targetFPS;
            _enabled = true;
            _pacingMode = FramePacingMode.Sleep;
            _maxFrameTime = 0f;
            _minFrameTime = float.MaxValue;
            _droppedFrames = 0;

            _frameTimer.Start();
            ModernLoggingSystem.Log("INFO", $"FramePacer: Initialized with target FPS {_targetFPS} (target frame time: {_targetFrameTime:F2}ms)");
        }

        /// <summary>
        /// Begins a new frame.
        /// </summary>
        public void BeginFrame()
        {
            if (!_enabled)
                return;

            _frameTimer.Restart();
        }

        /// <summary>
        /// Ends the current frame and applies pacing.
        /// </summary>
        public void EndFrame()
        {
            if (!_enabled)
                return;

            _frameTimer.Stop();
            _lastFrameTime = (float)_frameTimer.Elapsed.TotalMilliseconds;
            _accumulatedTime += _lastFrameTime;
            _frameCount++;

            // Update statistics
            UpdateStatistics();

            // Apply frame pacing
            ApplyPacing();

            // Reset for next frame
            _frameTimer.Reset();
        }

        /// <summary>
        /// Gets the current FPS.
        /// </summary>
        /// <returns>Current FPS.</returns>
        public float GetCurrentFPS()
        {
            return _frameCount > 0 ? _frameCount * 1000f / _accumulatedTime : _targetFPS;
        }

        /// <summary>
        /// Resets frame pacing statistics.
        /// </summary>
        public void Reset()
        {
            _lastFrameTime = _targetFrameTime;
            _accumulatedTime = 0f;
            _frameCount = 0;
            _averageFPS = _targetFPS;
            _maxFrameTime = 0f;
            _minFrameTime = float.MaxValue;
            _droppedFrames = 0;

            ModernLoggingSystem.Log("INFO", "FramePacer: Reset statistics");
        }

        /// <summary>
        /// Gets frame pacing statistics.
        /// </summary>
        /// <returns>Frame pacing statistics.</returns>
        public FramePacingStatistics GetStatistics()
        {
            return new FramePacingStatistics
            {
                TargetFPS = _targetFPS,
                CurrentFPS = GetCurrentFPS(),
                AverageFPS = _averageFPS,
                LastFrameTime = _lastFrameTime,
                MaxFrameTime = _maxFrameTime,
                MinFrameTime = _minFrameTime == float.MaxValue ? 0f : _minFrameTime,
                DroppedFrames = _droppedFrames,
                TotalFrames = _frameCount,
                PacingMode = _pacingMode
            };
        }

        /// <summary>
        /// Updates frame pacing statistics.
        /// </summary>
        private void UpdateStatistics()
        {
            // Update min/max frame times
            if (_lastFrameTime > _maxFrameTime)
                _maxFrameTime = _lastFrameTime;

            if (_lastFrameTime < _minFrameTime)
                _minFrameTime = _lastFrameTime;

            // Check for dropped frames
            if (_lastFrameTime > _targetFrameTime * 1.5f)
            {
                _droppedFrames++;
                OnFrameDropped?.Invoke(_droppedFrames);
                ModernLoggingSystem.Log("DEBUG", $"FramePacer: Frame dropped (time: {_lastFrameTime:F2}ms, target: {_targetFrameTime:F2}ms)");
            }

            // Update average FPS every 60 frames
            if (_frameCount % 60 == 0)
            {
                _averageFPS = GetCurrentFPS();
                OnStatisticsUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// Applies frame pacing based on the current mode.
        /// </summary>
        private void ApplyPacing()
        {
            if (_pacingMode == FramePacingMode.None)
                return;

            var timeToWait = _targetFrameTime - _lastFrameTime;

            if (timeToWait > 0f)
            {
                switch (_pacingMode)
                {
                    case FramePacingMode.Sleep:
                        ApplySleepPacing(timeToWait);
                        break;
                    case FramePacingMode.BusyWait:
                        ApplyBusyWaitPacing(timeToWait);
                        break;
                    case FramePacingMode.Hybrid:
                        ApplyHybridPacing(timeToWait);
                        break;
                }
            }
        }

        /// <summary>
        /// Applies sleep-based frame pacing.
        /// </summary>
        /// <param name="timeToWait">Time to wait in milliseconds.</param>
        private void ApplySleepPacing(float timeToWait)
        {
            var sleepTime = (int)(timeToWait * 0.9f); // Sleep for 90% of the time
            if (sleepTime > 0)
            {
                System.Threading.Thread.Sleep(sleepTime);
            }

            // Busy wait for the remaining time
            var remainingTime = timeToWait - sleepTime;
            if (remainingTime > 0f)
            {
                var endTime = Stopwatch.GetTimestamp() + (long)(remainingTime * Stopwatch.Frequency / 1000);
                while (Stopwatch.GetTimestamp() < endTime)
                {
                    // Busy wait
                }
            }
        }

        /// <summary>
        /// Applies busy-wait frame pacing.
        /// </summary>
        /// <param name="timeToWait">Time to wait in milliseconds.</param>
        private void ApplyBusyWaitPacing(float timeToWait)
        {
            var endTime = Stopwatch.GetTimestamp() + (long)(timeToWait * Stopwatch.Frequency / 1000);
            while (Stopwatch.GetTimestamp() < endTime)
            {
                // Busy wait
            }
        }

        /// <summary>
        /// Applies hybrid frame pacing.
        /// </summary>
        /// <param name="timeToWait">Time to wait in milliseconds.</param>
        private void ApplyHybridPacing(float timeToWait)
        {
            if (timeToWait > 2f)
            {
                // Sleep for larger waits
                var sleepTime = (int)(timeToWait * 0.8f);
                System.Threading.Thread.Sleep(sleepTime);

                // Busy wait for the remaining time
                var remainingTime = timeToWait - sleepTime;
                if (remainingTime > 0f)
                {
                    var endTime = Stopwatch.GetTimestamp() + (long)(remainingTime * Stopwatch.Frequency / 1000);
                    while (Stopwatch.GetTimestamp() < endTime)
                    {
                        // Busy wait
                    }
                }
            }
            else
            {
                // Busy wait for small waits
                ApplyBusyWaitPacing(timeToWait);
            }
        }

        /// <summary>
        /// Gets frame pacer information as a string.
        /// </summary>
        public override string ToString()
        {
            return $"FramePacer: Target={_targetFPS}fps, Current={GetCurrentFPS():F1}fps, " +
            $"FrameTime={_lastFrameTime:F2}ms, Dropped={_droppedFrames}, " +
            $"Mode={_pacingMode}";
        }
    }

    /// <summary>
    /// Frame pacing modes.
    /// </summary>
    public enum FramePacingMode
    {
        /// <summary>No frame pacing.</summary>
        None,

        /// <summary>Sleep-based pacing.</summary>
        Sleep,

        /// <summary>Busy-wait pacing.</summary>
        BusyWait,

        /// <summary>Hybrid pacing.</summary>
        Hybrid
    }

    /// <summary>
    /// Frame pacing statistics.
    /// </summary>
    public class FramePacingStatistics
    {
        public float TargetFPS { get; set; }
        public float CurrentFPS { get; set; }
        public float AverageFPS { get; set; }
        public float LastFrameTime { get; set; }
        public float MaxFrameTime { get; set; }
        public float MinFrameTime { get; set; }
        public int DroppedFrames { get; set; }
        public int TotalFrames { get; set; }
        public FramePacingMode PacingMode { get; set; }

        public override string ToString()
        {
            return $"FPS: {CurrentFPS:F1}/{TargetFPS} (avg: {AverageFPS:F1}), " +
            $"Frame: {LastFrameTime:F2}ms (min: {MinFrameTime:F2}, max: {MaxFrameTime:F2}), " +
            $"Dropped: {DroppedFrames}/{TotalFrames}";
        }
    }
}




