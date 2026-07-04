// ====================================================================================================
//  FILE: ManagerDiagnostics.cs
//  PATH: Engine/Diagnostics/ManagerDiagnostics.cs
//  SUBSYSTEM: Diagnostics
//
//  PURPOSE:
//      P11-09-04 — Core manager diagnostics and performance monitoring.
//      Provides comprehensive diagnostics for all engine managers.
//
//  ROLE:
//      - Tracks manager performance metrics and health status
//      - Provides real-time diagnostic information for all managers
//      - Monitors manager lifecycle and state changes
//      - Integrates with GameRoot for comprehensive system visibility
//      - Supports diagnostic reporting and analysis
//
//  FEATURES:
//      - Real-time manager performance monitoring with detailed metrics
//      - Manager health tracking with status and error monitoring
//      - Lifecycle monitoring with initialization and shutdown tracking
//      - Thread-safe diagnostic operations for concurrent access
//      - Integration with GameRoot for complete system visibility
//      - Comprehensive diagnostic reporting with analysis capabilities
//
//  NOTES:
//      - Designed for development and performance monitoring
//      - Diagnostic operations have minimal performance impact
//      - All diagnostic operations are thread-safe
//      - Integrates seamlessly with all engine managers
//      - Diagnostic data can be exported for analysis and optimization
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public class ManagerDiagnostics
    {
        // ---------------------------------------------------------------------------------------------
        // Private Fields
        // ---------------------------------------------------------------------------------------------
        private readonly object _diagnosticsLock = new();
        private readonly Dictionary<string, ManagerDiagnosticInfo> _managerInfos = new();
        private readonly Dictionary<string, ManagerPerformanceMetrics> _performanceMetrics = new();
        private bool _isEnabled = false;
        private DateTime _startTime = DateTime.Now;

        // ---------------------------------------------------------------------------------------------
        // Public Properties
        // ---------------------------------------------------------------------------------------------
        public bool IsEnabled => _isEnabled;
        public int ManagerCount => _managerInfos.Count;

        // ---------------------------------------------------------------------------------------------
        // Public Methods
        // ---------------------------------------------------------------------------------------------
        public void Initialize()
        {
            lock (_diagnosticsLock)
            {
                try
                {
                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Info,
                        "ManagerDiagnostics",
                        "Initializing ManagerDiagnostics...");

                    _managerInfos.Clear();
                    _performanceMetrics.Clear();

                    _isEnabled = true;
                    _startTime = DateTime.Now;

                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Info,
                        "ManagerDiagnostics",
                        "ManagerDiagnostics initialized successfully");
                }
                catch (Exception ex)
                {
                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Error,
                        "ManagerDiagnostics",
                        $"ManagerDiagnostics initialization failed: {ex.Message}");
                    throw;
                }
            }
        }

        public void Shutdown()
        {
            lock (_diagnosticsLock)
            {
                try
                {
                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Info,
                        "ManagerDiagnostics",
                        "Shutting down ManagerDiagnostics...");

                    GenerateFinalReport();

                    _managerInfos.Clear();
                    _performanceMetrics.Clear();

                    _isEnabled = false;

                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Info,
                        "ManagerDiagnostics",
                        "ManagerDiagnostics shutdown completed");
                }
                catch (Exception ex)
                {
                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Error,
                        "ManagerDiagnostics",
                        $"ManagerDiagnostics shutdown failed: {ex.Message}");
                }
            }
        }

        public void RegisterManager(string managerName, Type managerType)
        {
            lock (_diagnosticsLock)
            {
                if (!_isEnabled) return;

                _managerInfos[managerName] = new ManagerDiagnosticInfo
                {
                    ManagerName = managerName,
                    ManagerType = managerType,
                    RegistrationTime = DateTime.Now,
                    Status = ManagerStatus.Registered
                };

                _performanceMetrics[managerName] = new ManagerPerformanceMetrics
                {
                    ManagerName = managerName,
                    StartTime = DateTime.Now
                };

                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"Manager registered for diagnostics: {managerName}");
            }
        }

        public void UpdateManagerStatus(string managerName, ManagerStatus status)
        {
            lock (_diagnosticsLock)
            {
                if (!_isEnabled) return;

                if (_managerInfos.TryGetValue(managerName, out var info))
                {
                    info.Status = status;
                    info.LastStatusChange = DateTime.Now;

                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Debug,
                        "ManagerDiagnostics",
                        $"Manager status updated: {managerName} -> {status}");
                }
            }
        }

        public void RecordOperationDuration(string managerName, ManagerOperation operation, float duration)
        {
            lock (_diagnosticsLock)
            {
                if (!_isEnabled) return;

                if (_performanceMetrics.TryGetValue(managerName, out var metrics))
                {
                    metrics.RecordOperation(operation, duration);
                }
            }
        }

        public void RecordError(string managerName, Exception error)
        {
            lock (_diagnosticsLock)
            {
                if (!_isEnabled) return;

                if (_managerInfos.TryGetValue(managerName, out var info))
                {
                    info.ErrorCount++;
                    info.LastError = error;
                    info.LastErrorTime = DateTime.Now;

                    DLogger.Log(
                        LogSubsystems.Diagnostics,
                        LogLevel.Error,
                        "ManagerDiagnostics",
                        $"Manager error recorded: {managerName} - {error.Message}");
                }

                if (_performanceMetrics.TryGetValue(managerName, out var metrics))
                {
                    metrics.ErrorCount++;
                }
            }
        }

        public ManagerDiagnosticInfo GetManagerInfo(string managerName)
        {
            lock (_diagnosticsLock)
            {
                return _managerInfos.TryGetValue(managerName, out var info) ? info : null;
            }
        }

        public ManagerPerformanceMetrics GetPerformanceMetrics(string managerName)
        {
            lock (_diagnosticsLock)
            {
                return _performanceMetrics.TryGetValue(managerName, out var metrics) ? metrics : null;
            }
        }

        public ManagerDiagnosticsData GetDiagnostics()
        {
            lock (_diagnosticsLock)
            {
                return new ManagerDiagnosticsData
                {
                    IsEnabled = _isEnabled,
                    StartTime = _startTime,
                    ManagerCount = _managerInfos.Count,
                    TotalErrors = _managerInfos.Values.Sum(m => m.ErrorCount),
                    ActiveManagers = _managerInfos.Values.Count(m => m.Status == ManagerStatus.Running),
                    InactiveManagers = _managerInfos.Values.Count(m => m.Status != ManagerStatus.Running),
                    AverageOperationTime = _performanceMetrics.Values.Any()
                        ? _performanceMetrics.Values.Average(m => m.AverageOperationTime)
                        : 0f,
                    TotalOperationTime = _performanceMetrics.Values.Sum(m => m.TotalOperationTime),
                    TotalRunTime = (float)(DateTime.Now - _startTime).TotalSeconds
                };
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------------------------------------------
        private void GenerateFinalReport()
        {
            try
            {
                var diagnostics = GetDiagnostics();

                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    "Manager diagnostics final report:");

                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"  Total run time: {diagnostics.TotalRunTime:F2}s");
                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"  Total managers: {diagnostics.ManagerCount}");
                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"  Active managers: {diagnostics.ActiveManagers}");
                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"  Inactive managers: {diagnostics.InactiveManagers}");
                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"  Total errors: {diagnostics.TotalErrors}");
                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"  Average operation time: {diagnostics.AverageOperationTime * 1000:F2}ms");
                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Info,
                    "ManagerDiagnostics",
                    $"  Total operation time: {diagnostics.TotalOperationTime * 1000:F2}ms");
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Diagnostics,
                    LogLevel.Error,
                    "ManagerDiagnostics",
                    $"Failed to generate final report: {ex.Message}");
            }
        }
    }

    public class ManagerDiagnosticInfo
    {
        public string ManagerName { get; set; }
        public Type ManagerType { get; set; }
        public ManagerStatus Status { get; set; }
        public DateTime RegistrationTime { get; set; }
        public DateTime LastStatusChange { get; set; }
        public int ErrorCount { get; set; }
        public Exception LastError { get; set; }
        public DateTime LastErrorTime { get; set; }
        public float Uptime => (float)(DateTime.Now - RegistrationTime).TotalSeconds;
    }

    public class ManagerPerformanceMetrics
    {
        public string ManagerName { get; set; }
        public DateTime StartTime { get; set; }
        public int TotalOperations { get; set; }
        public float TotalOperationTime { get; set; }
        public float AverageOperationTime => TotalOperations > 0 ? TotalOperationTime / TotalOperations : 0f;
        public float MinOperationTime { get; set; } = float.MaxValue;
        public float MaxOperationTime { get; set; }
        public int ErrorCount { get; set; }
        public Dictionary<ManagerOperation, float> OperationTimes { get; set; } = new();

        public void RecordOperation(ManagerOperation operation, float duration)
        {
            TotalOperations++;
            TotalOperationTime += duration;
            MinOperationTime = System.Math.Min(MinOperationTime, duration);
            MaxOperationTime = System.Math.Max(MaxOperationTime, duration);

            if (!OperationTimes.ContainsKey(operation))
            {
                OperationTimes[operation] = 0f;
            }

            OperationTimes[operation] += duration;
        }
    }

    public class ManagerDiagnosticsData
    {
        public bool IsEnabled { get; set; }
        public DateTime StartTime { get; set; }
        public int ManagerCount { get; set; }
        public int TotalErrors { get; set; }
        public int ActiveManagers { get; set; }
        public int InactiveManagers { get; set; }
        public float AverageOperationTime { get; set; }
        public float TotalOperationTime { get; set; }
        public float TotalRunTime { get; set; }
    }

    public enum ManagerStatus
    {
        Registered,
        Initializing,
        Initialized,
        Running,
        Paused,
        ShuttingDown,
        Shutdown,
        Error
    }

    public enum ManagerOperation
    {
        Initialize,
        Shutdown,
        Update,
        Render,
        ProcessInput,
        HandleEvent,
        Custom
    }
}
