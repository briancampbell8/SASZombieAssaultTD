using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.State
{
    /// <summary>
    /// Performance profiler for state machine operations.
    /// P20-02-Enhancement: Real-time performance monitoring and analysis.
    /// </summary>
    public class StateMachineProfiler
    {
        private readonly Dictionary<GameStateType, StatePerformanceMetrics> _stateMetrics;
        private readonly Dictionary<string, OperationMetrics> _operationMetrics;
        private readonly object _profilerLock = new object();
        private bool _profilingEnabled = true;
        
        /// <summary>
        /// Enables or disables profiling.
        /// </summary>
        public bool ProfilingEnabled
        {
            get => _profilingEnabled;
            set => _profilingEnabled = value;
        }
        
        /// <summary>
        /// Initializes a new state machine profiler.
        /// </summary>
        public StateMachineProfiler()
        {
            _stateMetrics = new Dictionary<GameStateType, StatePerformanceMetrics>();
            _operationMetrics = new Dictionary<string, OperationMetrics>();
        }
        
        /// <summary>
        /// Starts profiling a state operation.
        /// </summary>
        /// <param name="stateType">The state type.</param>
        /// <param name="operation">The operation being performed.</param>
        /// <returns>A profiling session that must be disposed when the operation completes.</returns>
        public StateProfilingSession StartProfiling(GameStateType stateType, string operation)
        {
            if (!_profilingEnabled)
            return new StateProfilingSession(null, null, null);
            
            return new StateProfilingSession(this, stateType, operation);
        }
        
        /// <summary>
        /// Records a completed operation.
        /// </summary>
        /// <param name="stateType">The state type.</param>
        /// <param name="operation">The operation name.</param>
        /// <param name="duration">The operation duration.</param>
        internal void RecordOperation(GameStateType stateType, string operation, TimeSpan duration)
        {
            if (!_profilingEnabled)
            return;
            
            lock (_profilerLock)
            {
                // Update state-specific metrics
                if (!_stateMetrics.ContainsKey(stateType))
                {
                    _stateMetrics[stateType] = new StatePerformanceMetrics { StateType = stateType };
                }
                
                var stateMetric = _stateMetrics[stateType];
                stateMetric.RecordOperation(operation, duration);
                
                // Update operation-specific metrics
                var operationKey = $"{stateType}_{operation}";
                if (!_operationMetrics.ContainsKey(operationKey))
                {
                    _operationMetrics[operationKey] = new OperationMetrics
                    {
                        StateType = stateType,
                        Operation = operation
                    };
                }
                
                var operationMetric = _operationMetrics[operationKey];
                operationMetric.RecordExecution(duration);
            }
        }
        
        /// <summary>
        /// Gets performance metrics for a specific state.
        /// </summary>
        /// <param name="stateType">The state type.</param>
        /// <returns>Performance metrics for the state.</returns>
        public StatePerformanceMetrics GetStateMetrics(GameStateType stateType)
        {
            lock (_profilerLock)
            {
                return _stateMetrics.TryGetValue(stateType, out var metrics) ? metrics.Clone() : new StatePerformanceMetrics { StateType = stateType };
            }
        }
        
        /// <summary>
        /// Gets performance metrics for a specific operation.
        /// </summary>
        /// <param name="stateType">The state type.</param>
        /// <param name="operation">The operation name.</param>
        /// <returns>Performance metrics for the operation.</returns>
        public OperationMetrics GetOperationMetrics(GameStateType stateType, string operation)
        {
            lock (_profilerLock)
            {
                var operationKey = $"{stateType}_{operation}";
                return _operationMetrics.TryGetValue(operationKey, out var metrics) ? metrics.Clone() : new OperationMetrics { StateType = stateType, Operation = operation };
            }
        }
        
        /// <summary>
        /// Gets all state performance metrics.
        /// </summary>
        /// <returns>Dictionary of state types and their metrics.</returns>
        public Dictionary<GameStateType, StatePerformanceMetrics> GetAllStateMetrics()
        {
            lock (_profilerLock)
            {
                return _stateMetrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
            }
        }
        
        /// <summary>
        /// Gets all operation performance metrics.
        /// </summary>
        /// <returns>Dictionary of operation keys and their metrics.</returns>
        public Dictionary<string, OperationMetrics> GetAllOperationMetrics()
        {
            lock (_profilerLock)
            {
                return _operationMetrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone());
            }
        }
        
        /// <summary>
        /// Generates a performance report.
        /// </summary>
        /// <returns>Performance report as a string.</returns>
        public string GeneratePerformanceReport()
        {
            lock (_profilerLock)
            {
                var report = new List<string>
                {
                    "State Machine Performance Report",
                    $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                    $"Profiling Enabled: {_profilingEnabled}",
                    ""
                };
                
                // State performance summary
                report.Add("=== State Performance Summary ===");
                foreach (var kvp in _stateMetrics.OrderByDescending(x => x.Value.TotalExecutionTime))
                {
                    var metrics = kvp.Value;
                    report.Add($"{kvp.Key}:");
                    report.Add($"  Total Time: {metrics.TotalExecutionTime.TotalMilliseconds:F2}ms");
                    report.Add($"  Call Count: {metrics.CallCount}");
                    report.Add($"  Avg Time: {metrics.AverageExecutionTime.TotalMilliseconds:F2}ms");
                    report.Add($"  Max Time: {metrics.MaxExecutionTime.TotalMilliseconds:F2}ms");
                    report.Add("");
                }
                
                // Slowest operations
                report.Add("=== Slowest Operations ===");
                var slowestOps = _operationMetrics.OrderByDescending(x => x.Value.AverageExecutionTime).Take(10);
                foreach (var kvp in slowestOps)
                {
                    var metrics = kvp.Value;
                    report.Add($"{metrics.StateType}.{metrics.Operation}: {metrics.AverageExecutionTime.TotalMilliseconds:F2}ms avg ({metrics.ExecutionCount} calls)");
                }
                report.Add("");
                
                // Most frequent operations
                report.Add("=== Most Frequent Operations ===");
                var frequentOps = _operationMetrics.OrderByDescending(x => x.Value.ExecutionCount).Take(10);
                foreach (var kvp in frequentOps)
                {
                    var metrics = kvp.Value;
                    report.Add($"{metrics.StateType}.{metrics.Operation}: {metrics.ExecutionCount} calls ({metrics.AverageExecutionTime.TotalMilliseconds:F2}ms avg)");
                }
                
                return string.Join(Environment.NewLine, report);
            }
        }
        
        /// <summary>
        /// Clears all performance metrics.
        /// </summary>
        public void ClearMetrics()
        {
            lock (_profilerLock)
            {
                _stateMetrics.Clear();
                _operationMetrics.Clear();
                Engine.Diagnostics.DebugLogger.LogDebug("INFO", "StateMachineProfiler: All performance metrics cleared");
            }
        }
        
        /// <summary>
        /// Identifies performance issues based on thresholds.
        /// </summary>
        /// <param name="slowOperationThresholdMs">Threshold for slow operations in milliseconds.</param>
        /// <param name="frequentOperationThreshold">Threshold for frequent operations.</param>
        /// <returns>List of performance issues.</returns>
        public List<string> IdentifyPerformanceIssues(double slowOperationThresholdMs = 16.0, int frequentOperationThreshold = 1000)
        {
            lock (_profilerLock)
            {
                var issues = new List<string>();
                
                // Check for slow operations
                var slowOps = _operationMetrics.Where(x => x.Value.AverageExecutionTime.TotalMilliseconds > slowOperationThresholdMs);
                foreach (var kvp in slowOps)
                {
                    var metrics = kvp.Value;
                    issues.Add($"Slow operation: {metrics.StateType}.{metrics.Operation} averages {metrics.AverageExecutionTime.TotalMilliseconds:F2}ms (threshold: {slowOperationThresholdMs}ms)");
                }
                
                // Check for very frequent operations
                var frequentOps = _operationMetrics.Where(x => x.Value.ExecutionCount > frequentOperationThreshold);
                foreach (var kvp in frequentOps)
                {
                    var metrics = kvp.Value;
                    issues.Add($"Frequent operation: {metrics.StateType}.{metrics.Operation} called {metrics.ExecutionCount} times (threshold: {frequentOperationThreshold})");
                }
                
                // Check for states with high total execution time
                var heavyStates = _stateMetrics.Where(x => x.Value.TotalExecutionTime.TotalMilliseconds > 1000.0);
                foreach (var kvp in heavyStates)
                {
                    var metrics = kvp.Value;
                    issues.Add($"Heavy state: {metrics.StateType} total execution time {metrics.TotalExecutionTime.TotalMilliseconds:F2}ms");
                }
                
                return issues;
            }
        }
    }
    
    /// <summary>
    /// Represents a profiling session for a state operation.
    /// </summary>
    public sealed class StateProfilingSession : IDisposable
    {
        private readonly StateMachineProfiler _profiler;
        private readonly GameStateType _stateType;
        private readonly string _operation;
        private readonly Stopwatch _stopwatch;
        
        /// <summary>
        /// Initializes a new profiling session.
        /// </summary>
        /// <param name="profiler">The profiler.</param>
        /// <param name="stateType">The state type.</param>
        /// <param name="operation">The operation name.</param>
        internal StateProfilingSession(StateMachineProfiler profiler, GameStateType? stateType, string operation)
        {
            _profiler = profiler;
            _stateType = stateType ?? GameStateType.Boot; // Default value
            _operation = operation ?? "Unknown";
            _stopwatch = Stopwatch.StartNew();
        }
        
        /// <summary>
        /// Ends the profiling session and records the metrics.
        /// </summary>
        public void Dispose()
        {
            _stopwatch.Stop();
            _profiler?.RecordOperation(_stateType, _operation, _stopwatch.Elapsed);
        }
    }
    
    /// <summary>
    /// Performance metrics for a specific state.
    /// </summary>
    public class StatePerformanceMetrics
    {
        /// <summary>
        /// The state type.
        /// </summary>
        public GameStateType StateType { get; set; }
        
        /// <summary>
        /// Total execution time for all operations.
        /// </summary>
        public TimeSpan TotalExecutionTime { get; private set; }
        
        /// <summary>
        /// Total number of operation calls.
        /// </summary>
        public int CallCount { get; private set; }
        
        /// <summary>
        /// Average execution time per operation.
        /// </summary>
        public TimeSpan AverageExecutionTime => CallCount > 0 ? TimeSpan.FromTicks(TotalExecutionTime.Ticks / CallCount) : TimeSpan.Zero;
        
        /// <summary>
        /// Maximum execution time recorded.
        /// </summary>
        public TimeSpan MaxExecutionTime { get; private set; }
        
        /// <summary>
        /// Minimum execution time recorded.
        /// </summary>
        public TimeSpan MinExecutionTime { get; private set; } = TimeSpan.MaxValue;
        
        /// <summary>
        /// Records an operation execution.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <param name="duration">The operation duration.</param>
        public void RecordOperation(string operation, TimeSpan duration)
        {
            TotalExecutionTime = TotalExecutionTime.Add(duration);
            CallCount++;
            
            if (duration > MaxExecutionTime)
            MaxExecutionTime = duration;
            
            if (duration < MinExecutionTime)
            MinExecutionTime = duration;
        }
        
        /// <summary>
        /// Creates a clone of this metrics object.
        /// </summary>
        /// <returns>A cloned copy of the metrics.</returns>
        public StatePerformanceMetrics Clone()
        {
            return new StatePerformanceMetrics
            {
                StateType = this.StateType,
                TotalExecutionTime = this.TotalExecutionTime,
                CallCount = this.CallCount,
                MaxExecutionTime = this.MaxExecutionTime,
                MinExecutionTime = this.MinExecutionTime
            };
        }
    }
    
    /// <summary>
    /// Performance metrics for a specific operation.
    /// </summary>
    public class OperationMetrics
    {
        /// <summary>
        /// The state type.
        /// </summary>
        public GameStateType StateType { get; set; }
        
        /// <summary>
        /// The operation name.
        /// </summary>
        public string Operation { get; set; }
        
        /// <summary>
        /// Total execution time.
        /// </summary>
        public TimeSpan TotalExecutionTime { get; private set; }
        
        /// <summary>
        /// Number of executions.
        /// </summary>
        public int ExecutionCount { get; private set; }
        
        /// <summary>
        /// Average execution time.
        /// </summary>
        public TimeSpan AverageExecutionTime => ExecutionCount > 0 ? TimeSpan.FromTicks(TotalExecutionTime.Ticks / ExecutionCount) : TimeSpan.Zero;
        
        /// <summary>
        /// Maximum execution time.
        /// </summary>
        public TimeSpan MaxExecutionTime { get; private set; }
        
        /// <summary>
        /// Minimum execution time.
        /// </summary>
        public TimeSpan MinExecutionTime { get; private set; } = TimeSpan.MaxValue;
        
        /// <summary>
        /// Records an execution.
        /// </summary>
        /// <param name="duration">The execution duration.</param>
        public void RecordExecution(TimeSpan duration)
        {
            TotalExecutionTime = TotalExecutionTime.Add(duration);
            ExecutionCount++;
            
            if (duration > MaxExecutionTime)
            MaxExecutionTime = duration;
            
            if (duration < MinExecutionTime)
            MinExecutionTime = duration;
        }
        
        /// <summary>
        /// Creates a clone of this metrics object.
        /// </summary>
        /// <returns>A cloned copy of the metrics.</returns>
        public OperationMetrics Clone()
        {
            return new OperationMetrics
            {
                StateType = this.StateType,
                Operation = this.Operation,
                TotalExecutionTime = this.TotalExecutionTime,
                ExecutionCount = this.ExecutionCount,
                MaxExecutionTime = this.MaxExecutionTime,
                MinExecutionTime = this.MinExecutionTime
            };
        }
    }
}




