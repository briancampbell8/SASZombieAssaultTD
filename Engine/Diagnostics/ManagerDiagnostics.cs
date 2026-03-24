/*
File:    ManagerDiagnostics.cs
Path:    Engine/Diagnostics/ManagerDiagnostics.cs
Purpose:   P11-09-04 - Core manager diagnostics and performance monitoring.
           Provides comprehensive diagnostics for all engine managers.

Role:      Essential manager diagnostics system for performance monitoring.
           - Tracks manager performance metrics and health status
           - Provides real-time diagnostic information for all managers
           - Monitors manager lifecycle and state changes
           - Integrates with GameRoot for comprehensive system visibility
           - Supports diagnostic reporting and analysis

Features:   Real-time manager performance monitoring with detailed metrics.
           Manager health tracking with status and error monitoring.
           Lifecycle monitoring with initialization and shutdown tracking.
           Thread-safe diagnostic operations for concurrent access.
           Integration with GameRoot for complete system visibility.
           Comprehensive diagnostic reporting with analysis capabilities.

Notes:      This system is designed for development and performance monitoring.
           Diagnostic operations have minimal performance impact.
           All diagnostic operations are thread-safe and designed for concurrent access.
           System integrates seamlessly with all engine managers.
           Diagnostic data can be exported for analysis and optimization.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Manager diagnostics and performance monitoring system.
    /// Implements P11-09-04: Manager diagnostics and performance monitoring.
    /// </summary>
    public class ManagerDiagnostics
    {
        #region Private Fields

        private readonly object _diagnosticsLock = new();
        private readonly Dictionary<string, ManagerDiagnosticInfo> _managerInfos = new();
        private readonly Dictionary<string, ManagerPerformanceMetrics> _performanceMetrics = new();
        private bool _isEnabled = false;
        private DateTime _startTime = DateTime.Now;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether manager diagnostics are currently enabled.
        /// </summary>
        public bool IsEnabled => _isEnabled;

        /// <summary>
        /// Gets the number of managers being monitored.
        /// </summary>
        public int ManagerCount => _managerInfos.Count;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the manager diagnostics system.
        /// </summary>
        public void Initialize()
        {
            lock (_diagnosticsLock)
            {
                try
                {
                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo("Initializing ManagerDiagnostics...");

                    _managerInfos.Clear();
                    _performanceMetrics.Clear();

                    _isEnabled = true;
                    _startTime = DateTime.Now;

                    ModernLoggingSystem.LogInfo("ManagerDiagnostics initialized successfully");
                }
                catch (Exception ex)
                {
                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogError($"ManagerDiagnostics initialization failed: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Shuts down the manager diagnostics system.
        /// </summary>
        public void Shutdown()
        {
            lock (_diagnosticsLock)
            {
                try
                {
                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo("Shutting down ManagerDiagnostics...");
                    GenerateFinalReport();

                    _managerInfos.Clear();
                    _performanceMetrics.Clear();

                    _isEnabled = false;

                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo("ManagerDiagnostics shutdown completed");
                }
                catch (Exception ex)
                {
                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogError($"ManagerDiagnostics shutdown failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Registers a manager for diagnostics monitoring.
        /// </summary>
        /// <param name="managerName">The name of the manager.</param>
        /// <param name="managerType">The type of the manager.</param>
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

                ModernLoggingSystem.LogInfo($"Manager registered for diagnostics: {managerName}");
            }
        }

        /// <summary>
        /// Updates manager status.
        /// </summary>
        /// <param name="managerName">The name of the manager.</param>
        /// <param name="status">The new status.</param>
        public void UpdateManagerStatus(string managerName, ManagerStatus status)
        {
            lock (_diagnosticsLock)
            {
                if (!_isEnabled) return;

                if (_managerInfos.TryGetValue(managerName, out var info))
                {
                    info.Status = status;
                    info.LastStatusChange = DateTime.Now;

                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogDebug($"Manager status updated: {managerName} -> {status}");
                }
            }
        }

        /// <summary>
        /// Records manager operation duration.
        /// </summary>
        /// <param name="managerName">The name of the manager.</param>
        /// <param name="operation">The operation type.</param>
        /// <param name="duration">The operation duration in seconds.</param>
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

        /// <summary>
        /// Records manager error.
        /// </summary>
        /// <param name="managerName">The name of the manager.</param>
        /// <param name="error">The error that occurred.</param>
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

                    SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogError($"Manager error recorded: {managerName} - {error.Message}");
                }

                if (_performanceMetrics.TryGetValue(managerName, out var metrics))
                {
                    metrics.ErrorCount++;
                }
            }
        }

        /// <summary>
        /// Gets diagnostic information for a specific manager.
        /// </summary>
        /// <param name="managerName">The name of the manager.</param>
        /// <returns>Manager diagnostic information.</returns>
        public ManagerDiagnosticInfo GetManagerInfo(string managerName)
        {
            lock (_diagnosticsLock)
            {
                return _managerInfos.TryGetValue(managerName, out var info) ? info : null;
            }
        }

        /// <summary>
        /// Gets performance metrics for a specific manager.
        /// </summary>
        /// <param name="managerName">The name of the manager.</param>
        /// <returns>Manager performance metrics.</returns>
        public ManagerPerformanceMetrics GetPerformanceMetrics(string managerName)
        {
            lock (_diagnosticsLock)
            {
                return _performanceMetrics.TryGetValue(managerName, out var metrics) ? metrics : null;
            }
        }

        /// <summary>
        /// Gets comprehensive diagnostics data.
        /// </summary>
        /// <returns>Manager diagnostics data.</returns>
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
                    AverageOperationTime = _performanceMetrics.Values.Average(m => m.AverageOperationTime),
                    TotalOperationTime = _performanceMetrics.Values.Sum(m => m.TotalOperationTime),
                    TotalRunTime = (float)(DateTime.Now - _startTime).TotalSeconds
                };
            }
        }

        #endregion

        #region Private Methods

        private void GenerateFinalReport()
        {
            try
            {
                var diagnostics = GetDiagnostics();

                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo("Manager diagnostics final report:");
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo($"  Total run time: {diagnostics.TotalRunTime:F2}s");
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo($"  Total managers: {diagnostics.ManagerCount}");
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo($"  Active managers: {diagnostics.ActiveManagers}");
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo($"  Inactive managers: {diagnostics.InactiveManagers}");
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo($"  Total errors: {diagnostics.TotalErrors}");
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo($"  Average operation time: {diagnostics.AverageOperationTime * 1000:F2}ms");
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogInfo($"  Total operation time: {diagnostics.TotalOperationTime * 1000:F2}ms");
            }
            catch (Exception ex)
            {
                SASZombieAssaultTD.Engine.Core.ModernLoggingSystem.LogError($"Failed to generate final report: {ex.Message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// Manager diagnostic information.
    /// </summary>
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

    /// <summary>
    /// Manager performance metrics.
    /// </summary>
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

    /// <summary>
    /// Manager diagnostics data container.
    /// </summary>
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

    /// <summary>
    /// Manager status enumeration.
    /// </summary>
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

    /// <summary>
    /// Manager operation enumeration.
    /// </summary>
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

