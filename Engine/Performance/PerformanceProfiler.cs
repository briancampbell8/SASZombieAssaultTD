using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Extensions;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Performance
//
{
    ///<summary>
    ///Performance trend analysis for performance profiling.
    ///</summary>
    public class PerformanceTrend
    {
        public float AverageFPS { get; set; }
        public float MinFPS { get; set; }
        public float MaxFPS { get; set; }
        public List<float> Samples { get; set; }

        public PerformanceTrend()
        {
            AverageFPS = 60f;
            MinFPS = 30f;
            MaxFPS = 120f;
            Samples = new List<float>();
        }
    }

    ///<summary>
    ///Circular buffer for performance metrics.
    ///</summary>
    public class CircularBuffer<T>
    {
        private readonly T[] _buffer;
        private int _head;
        private int _tail;
        private int _count;

        public CircularBuffer(int capacity)
        {
            _buffer = new T[capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public void Add(T item)
        {
            _buffer[_tail] = item;
            _tail = (_tail + 1) % _buffer.Length;
            if (_count < _buffer.Length)
                _count++;
        }

        public T[] GetItems()
        {
            var result = new T[_count];
            for (int i = 0; i < _count; i++)
            {
                result[i] = _buffer[(_head + i) % _buffer.Length];
            }
            return result;
        }
    }

    ///<summary>
    ///Frame metrics for performance tracking.
    ///</summary>
    public class FrameMetrics
    {
        public float FrameTime { get; set; }
        public float FPS { get; set; }
        public long MemoryUsage { get; set; }
        public int DrawCalls { get; set; }

        public FrameMetrics()
        {
            FrameTime = 0f;
            FPS = 60f;
            MemoryUsage = 0;
            DrawCalls = 0;
        }
    }

    ///<summary>
    ///Performance analyzer for optimization recommendations.
    ///</summary>
    public class PerformanceAnalyzer
    {
        public PerformanceTrend Trend { get; set; }
        public List<FrameMetrics> RecentFrames { get; set; }

        public PerformanceAnalyzer()
        {
            Trend = new PerformanceTrend();
            RecentFrames = new List<FrameMetrics>();
        }
    }

    ///<summary>
    ///High-resolution performance profiler for core loop optimization.
    ///P30-01-01: Profile Update() and Render() execution times.
    ///P30-01-02: Add high-resolution performance timers.
    ///</summary>
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

        ///<summary>
        ///Gets whether the profiler is enabled.
        ///</summary>
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        ///<summary>
        ///Gets the target FPS.
        ///</summary>
        public float TargetFPS => _targetFPS;

        ///<summary>
        ///Gets the smoothed delta time.
        ///</summary>
        public float SmoothedDeltaTime => _smoothedDeltaTime;

        ///<summary>
        ///Gets the current FPS.
        ///</summary>
        public float CurrentFPS { get; private set; }

        ///<summary>
        ///Gets the frame time in milliseconds.
        ///</summary>
        public float FrameTimeMs { get; private set; }

        ///<summary>
        ///Gets the CPU usage percentage.
        ///</summary>
        public float CPUUsage => _cpuUsageSample;

        ///<summary>
        ///Event fired when performance metrics are updated.
        ///</summary>
        public event Action<PerformanceProfiler> OnMetricsUpdated;

        ///<summary>
        ///Initializes a new performance profiler.
        ///</summary>
        ///<param name="targetFPS">Target frames per second.</param>
        ///<param name="maxHistorySize">Maximum frame history size.</param>
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
            DLogger.Log(LogSubsystems.Performance,LogLevel.Info, $"PerformanceProfiler: Initialized with target FPS {targetFPS}");
        }

        ///<summary>
        ///Begins profiling a specific operation.
        ///</summary>
        ///<param name="operationName">Name of the operation to profile.</param>
        ///<returns>A profiling session that must be disposed.</returns>
        public ProfilingSession BeginProfile(string operationName)
        {
            if (!_enabled)
                return new ProfilingSession(null, operationName);

            return new ProfilingSession(this, operationName);
        }

        ///<summary>
        ///Records a performance measurement.
        ///</summary>
        ///<param name="operationName">Name of the operation.</param>
        ///<param name="duration">Duration in milliseconds.</param>
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

            //Maintain history size
            while (history.Count > _maxHistorySize)
            {
                history.RemoveAt(0);
            }

            //Update frame time if this is the main frame
            if (operationName == "Frame")
            {
                FrameTimeMs = duration;
                CurrentFPS = 1000f / duration;
                UpdateSmoothedDeltaTime(duration / 1000f);
            }

           DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE", $"PerformanceProfiler: Recorded {operationName}: {duration:F2}ms");
        }

        ///<summary>
        ///Updates the profiler.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update.</param>
        public void Update(float deltaTime)
        {
            if (!_enabled)
                return;

            //Sample CPU usage
            SampleCPUUsage();

            //Update frame skip protection
            UpdateFrameSkipProtection(deltaTime);

            //Fire metrics updated event
            OnMetricsUpdated?.Invoke(this);
        }

        ///<summary>
        ///Gets performance metrics for an operation.
        ///</summary>
        ///<param name="operationName">Name of the operation.</param>
        ///<returns>Performance metrics, or null if not found.</returns>
        public PerformanceMetric GetMetric(string operationName)
        {
            return _metrics.TryGetValue(operationName, out var metric) ? metric : null;
        }

        ///<summary>
        ///Gets frame history for an operation.
        ///</summary>
        ///<param name="operationName">Name of the operation.</param>
        ///<returns>Frame history, or null if not found.</returns>
        public List<float> GetFrameHistory(string operationName)
        {
            return _frameHistory.TryGetValue(operationName, out var history) ? history : null;
        }

        ///<summary>
        ///Gets all performance metrics.
        ///</summary>
        ///<returns>Dictionary of all metrics.</returns>
        public Dictionary<string, PerformanceMetric> GetAllMetrics()
        {
            return new Dictionary<string, PerformanceMetric>(_metrics);
        }

        ///<summary>
        ///Resets all performance metrics.
        ///</summary>
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

            DLogger.Log(LogSubsystems.Performance,LogLevel.Info, "PerformanceProfiler: Reset all metrics");
        }

        ///<summary>
        ///Gets performance summary.
        ///</summary>
        ///<returns>Performance summary as a string.</returns>
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

            //Add top 5 slowest operations
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

        ///<summary>
        ///Sets delta time smoothing factor.
        ///P30-01-04: Add deltaTime smoothing.
        ///</summary>
        ///<param name="smoothingFactor">Smoothing factor (0.0 to 1.0).</param>
        public void SetDeltaTimeSmoothing(float smoothingFactor)
        {
            _deltaTimeSmoothingFactor = System.Math.Clamp(smoothingFactor, 0f, 1f);
            DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "DEBUG", $"PerformanceProfiler: Set delta time smoothing to {_deltaTimeSmoothingFactor:F2}");
        }

        ///<summary>
        ///Gets frame skip protection status.
        ///P30-01-08: Add frame skip protection.
        ///</summary>
        ///<returns>Frame skip protection level.</returns>
        public int GetFrameSkipProtection()
        {
            return _frameSkipProtection;
        }

        ///<summary>
        ///Initializes performance metrics.
        ///</summary>
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

        ///<summary>
        ///Updates smoothed delta time.
        ///</summary>
        ///<param name="deltaTime">Current delta time.</param>
        private void UpdateSmoothedDeltaTime(float deltaTime)
        {
            _smoothedDeltaTime = _smoothedDeltaTime * (1f - _deltaTimeSmoothingFactor) + deltaTime * _deltaTimeSmoothingFactor;
        }

        ///<summary>
        ///Updates frame skip protection.
        ///</summary>
        ///<param name="deltaTime">Current delta time.</param>
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

        ///<summary>
        ///Samples CPU usage.
        ///P30-01-09: Add CPU usage sampling.
        ///</summary>
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

        ///<summary>
        ///Gets profiler information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"PerformanceProfiler: Enabled={_enabled}, FPS={CurrentFPS:F1}, " +
            $"FrameTime={FrameTimeMs:F2}ms, CPU={_cpuUsageSample:F1}%, " +
            $"Metrics={_metrics.Count}";
        }
    }

    ///<summary>
    ///Performance metric data.
    ///</summary>
    public class PerformanceMetric
    {
        public string Name { get; }
        public float MinTime { get; private set; }
        public float MaxTime { get; private set; }
        public float AverageTime { get; private set; }
        public float TotalTime { get; private set; }
        public int SampleCount { get; private set; }

        public PerformanceMetric(string name)
        {
            Name = name;
            MinTime = float.MaxValue;
            MaxTime = 0f;
            AverageTime = 0f;
            TotalTime = 0f;
            SampleCount = 0;
        }

        public void AddMeasurement(float duration)
        {
            TotalTime += duration;
            SampleCount++;
            AverageTime = TotalTime / SampleCount;
            MinTime = System.Math.Min(MinTime, duration);
            MaxTime = System.Math.Max(MaxTime, duration);
        }

        public void Reset()
        {
            MinTime = float.MaxValue;
            MaxTime = 0f;
            AverageTime = 0f;
            TotalTime = 0f;
            SampleCount = 0;
        }

        public override string ToString()
        {
            return $"{Name}: Avg={AverageTime:F2}ms, Min={MinTime:F2}ms, Max={MaxTime:F2}ms, Samples={SampleCount}";
        }
    }

    ///<summary>
    ///Profiling session for measuring operation duration.
    ///</summary>
    public sealed class ProfilingSession : IDisposable
    {
        private readonly PerformanceProfiler _profiler;
        private readonly string _operationName;
        private readonly Stopwatch _timer;

        public ProfilingSession(PerformanceProfiler profiler, string operationName)
        {
            _profiler = profiler;
            _operationName = operationName;
            _timer = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            var elapsed = _timer.ElapsedTicks / (float)Stopwatch.Frequency * 1000f;
            _profiler?.RecordMetric(_operationName, elapsed);
        }
    }

    /// Advanced Performance Monitoring

    ///<summary>
    ///Advanced performance monitoring system with sophisticated analysis and optimization.
    ///</summary>
    public class AdvancedPerformanceMonitoring
    {
        private readonly PerformanceProfiler _profiler;
        private readonly Dictionary<string, PerformanceTrend> _performanceTrends = new();
        private readonly CircularBuffer<FrameMetrics> _frameMetrics = new(300); //5 seconds at 60 FPS
        private readonly PerformanceAnalyzer _analyzer = new();
        private volatile bool _adaptiveOptimizationEnabled = true;
        private volatile float _performanceThreshold = 16.67f; //60 FPS target

        public AdvancedPerformanceMonitoring(PerformanceProfiler profiler)
        {
            _profiler = profiler ?? throw new ArgumentNullException(nameof(profiler));
        }

    }
}
///
