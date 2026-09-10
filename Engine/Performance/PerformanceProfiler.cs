// =====================================================================================================
// FILE: PerformanceProfiler.cs
// PATH: ./Engine/Performance/
// MODULE: Core
//
// ROLE:
//     Encapsulate core engine behavior for the PerformanceProfiler module.
//
// RESPONSIBILITIES:
//     - Profile performance metrics for core engine operations.
//     - Provide high-resolution performance timers for core engine operations.
//     - Provide GetMetrics() behavior for the Core subsystem.
//     - Provide GetHighResolutionTimer() behavior for the Core subsystem.
//     - Provide Dispose() behavior for the Core subsystem.
//
// NON-RESPONSIBILITIES:
//     - Low-level data persistence or file serialization.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Diagnostics;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Performance
{
    /// <summary>
    /// High-resolution performance profiler for core loop optimization.
    /// </summary>
    public class PerformanceProfiler
    {
        private readonly Dictionary<string, PerformanceMetric> _metrics;
        private readonly Dictionary<string, List<float>> _frameHistory;
        private readonly Stopwatch _highResolutionTimer;
        private readonly float _targetFPS;
        private readonly int _maxHistorySize;
        private bool _enabled;
        private float _deltaTimeSmoothingFactor;
        private float _smoothedDeltaTime;
        private int _frameSkipProtection;
        private float _cpuUsageSample;

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public float TargetFPS => _targetFPS;
        public float SmoothedDeltaTime => _smoothedDeltaTime;
        public float CurrentFPS { get; private set; }
        public float FrameTimeMs { get; private set; }
        public float CPUUsage => _cpuUsageSample;

        public float ManagedMemoryMb { get; internal set; }
        public float CpuUsagePercent { get; internal set; }
        public float GpuUsagePercent { get; internal set; }
        public int EntityCount { get; internal set; }
        public int DrawCallCount { get; internal set; }
        public string PathfindingSummary { get; internal set; }
        public string UiSummary { get; internal set; }
        public float Fps { get; internal set; }

        public event Action<PerformanceProfiler> OnMetricsUpdated;

        public PerformanceProfiler(float targetFPS = 60f, int maxHistorySize = 300)
        {
            _metrics = new Dictionary<string, PerformanceMetric>();
            _frameHistory = new Dictionary<string, List<float>>();
            _highResolutionTimer = new Stopwatch();
            _targetFPS = targetFPS;
            _maxHistorySize = maxHistorySize;
            _enabled = true;
            _deltaTimeSmoothingFactor = 0.1f;
            _smoothedDeltaTime = 1f / targetFPS;
            _frameSkipProtection = 0;
            _cpuUsageSample = 0f;

            InitializeMetrics();
            DLogger.Log(LogSubsystems.Performance, LogEnums.LogLevel.Info, $"PerformanceProfiler: Initialized with target FPS {targetFPS}");
        }

        public ProfilingSession BeginProfile(string operationName)
        {
            if (!_enabled)
                return new ProfilingSession(null, operationName);
            return new ProfilingSession(this, operationName);
        }

        internal void RecordMeasurement(string operationName, float duration)
        {
            if (!_enabled)
                return;

            if (!_metrics.ContainsKey(operationName))
            {
                _metrics[operationName] = new PerformanceMetric(operationName);
                _frameHistory[operationName] = new List<float>();
            }

            var metric = _metrics[operationName];
            var history = _frameHistory[operationName];

            metric.AddMeasurement(duration);
            history.Add(duration);

            while (history.Count > _maxHistorySize)
            {
                history.RemoveAt(0);
            }

            if (operationName == "Frame")
            {
                FrameTimeMs = duration;
                CurrentFPS = 1000f / duration;
                UpdateSmoothedDeltaTime(duration / 1000f);
            }

            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Trace, "TRACE", $"PerformanceProfiler: Recorded {operationName}: {duration:F2}ms");
        }

        public void Update(float deltaTime)
        {
            if (!_enabled)
                return;

            SampleCPUUsage();
            UpdateFrameSkipProtection(deltaTime);
            OnMetricsUpdated?.Invoke(this);
        }

        public PerformanceMetric GetMetric(string operationName)
        {
            return _metrics.TryGetValue(operationName, out var metric) ? metric : null;
        }

        public List<float> GetFrameHistory(string operationName)
        {
            return _frameHistory.TryGetValue(operationName, out var history) ? history : null;
        }

        public Dictionary<string, PerformanceMetric> GetAllMetrics()
        {
            return new Dictionary<string, PerformanceMetric>(_metrics);
        }

        public void Reset()
        {
            foreach (var metric in _metrics.Values)
            {
                metric.Reset();
            }

            foreach (var history in _frameHistory.Values)
            {
                history.Clear();
            }

            _smoothedDeltaTime = 1f / _targetFPS;
            _frameSkipProtection = 0;
            DLogger.Log(LogSubsystems.Performance, LogEnums.LogLevel.Info, "PerformanceProfiler: Reset all metrics");
        }

        public string GetPerformanceSummary()
        {
            var summary = new List<string>
            {
                "Performance Summary",
                $"Target FPS: {_targetFPS}",
                $"Current FPS: {CurrentFPS:F1}",
                $"Frame Time: {FrameTimeMs:F2}ms",
                $"CPU Usage: {_cpuUsageSample:F1}%",
                $"Smoothed Delta Time: {_smoothedDeltaTime:F4}s",
                ""
            };

            var slowestOps = _metrics.Values
                .Where(m => m.SampleCount > 0)
                .OrderByDescending(m => m.AverageTime)
                .Take(5);

            summary.Add("Top 5 Slowest Operations:");
            foreach (var metric in slowestOps)
            {
                summary.Add($"  {metric.Name}: {metric.AverageTime:F2}ms avg ({metric.SampleCount} samples)");
            }

            return string.Join(Environment.NewLine, summary);
        }

        public void SetDeltaTimeSmoothing(float smoothingFactor)
        {
            _deltaTimeSmoothingFactor = System.Math.Clamp(smoothingFactor, 0f, 1f);
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "DEBUG", $"PerformanceProfiler: Set delta time smoothing to {_deltaTimeSmoothingFactor:F2}");
        }

        public int GetFrameSkipProtection()
        {
            return _frameSkipProtection;
        }

        private void InitializeMetrics()
        {
            var coreMetrics = new[]
            {
                "Frame", "Update", "Render", "Input", "Physics", "Audio",
                "UI", "Entities", "Pathfinding", "Rendering", "SpriteBatch"
            };

            foreach (var metricName in coreMetrics)
            {
                _metrics[metricName] = new PerformanceMetric(metricName);
                _frameHistory[metricName] = new List<float>();
            }
        }

        private void UpdateSmoothedDeltaTime(float deltaTime)
        {
            _smoothedDeltaTime = _smoothedDeltaTime * (1f - _deltaTimeSmoothingFactor) + deltaTime * _deltaTimeSmoothingFactor;
        }

        private void UpdateFrameSkipProtection(float deltaTime)
        {
            var targetFrameTime = 1f / _targetFPS;
            if (deltaTime > targetFrameTime * 1.5f)
            {
                _frameSkipProtection++;
            }
            else if (deltaTime < targetFrameTime * 0.8f)
            {
                _frameSkipProtection = System.Math.Max(0, _frameSkipProtection - 1);
            }
        }

        private void SampleCPUUsage()
        {
            try
            {
                using var process = Process.GetCurrentProcess();
                _cpuUsageSample = (float)process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount * 100f / process.WorkingSet64;
            }
            catch
            {
                _cpuUsageSample = 0f;
            }
        }

        public override string ToString()
        {
            return $"PerformanceProfiler: Enabled={_enabled}, FPS={CurrentFPS:F1}, FrameTime={FrameTimeMs:F2}ms, CPU={_cpuUsageSample:F1}%, Metrics={_metrics.Count}";
        }
    }
}
