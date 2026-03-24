using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Modern logging system with levels, filtering, and thread-safe operation.
    /// Replaces legacy ModernLoggingSystem with complete modern implementation.
    /// </summary>
    public static class ModernLoggingSystem
    {
        private static readonly ConcurrentQueue<LogEntry> _logQueue = new();
        private static LogLevel _minLevel = LogLevel.Debug;
        private static readonly object _lock = new();
        private static bool _initialized = false;

        // Internal monitoring and safety
        private static int _logCount = 0;
        private static readonly int _maxLogsPerSecond = 1000;
        private static readonly int _warningLogsPerSecond = 500; // Early warning threshold
        private static DateTime _lastLogTime = DateTime.Now;
        private static readonly object _monitoringLock = new();
        private static bool _emergencyStop = false;
        private static bool _warningTriggered = false;
        private static string _lastErrorLog = "";
        private static int _duplicateErrorCount = 0;
        private static readonly int _maxDuplicateErrors = 10;
        private static readonly int _warningDuplicateErrors = 5; // Early warning threshold

        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
            Critical = 4
        }

        public struct LogEntry
        {
            public LogLevel Level;
            public string Category;
            public string Message;
            public DateTime Timestamp;
            public Thread Thread;
        }

        /// <summary>
        /// Initialize the logging system.
        /// </summary>
        public static void Initialize()
        {
            lock (_lock)
            {
                if (_initialized) return;

                _initialized = true;
                Log(LogLevel.Info, "SYSTEM", "ModernLoggingSystem initialized");
            }
        }

        /// <summary>
        /// Log a message with specified level and category.
        /// </summary>
        public static void Log(LogLevel level, string category, string message)
        {
            // Internal monitoring - check for emergency stop conditions
            if (_emergencyStop)
            {
                // In emergency stop, only allow critical logs
                if (level < LogLevel.Critical) return;
            }

            // Check for abnormal conditions
            if (!CheckForAbnormalConditions(level, category, message))
            {
                return; // Stop processing due to abnormal condition detected
            }

            if (!_initialized || level < _minLevel) return;

            var entry = new LogEntry
            {
                Level = level,
                Category = category ?? "UNKNOWN",
                Message = message ?? string.Empty,
                Timestamp = DateTime.UtcNow,
                Thread = Thread.CurrentThread
            };

            _logQueue.Enqueue(entry);

            // Update monitoring counters
            lock (_monitoringLock)
            {
                _logCount++;

                // Reset counter every second
                if ((DateTime.Now - _lastLogTime).TotalSeconds >= 1)
                {
                    _logCount = 0;
                    _lastLogTime = DateTime.Now;
                }

                if (_logCount > _maxLogsPerSecond)
                {
                    _emergencyStop = true;
                    Log(LogLevel.Critical, "SYSTEM", "Emergency stop triggered due to excessive logging rate");
                }
                else if (_logCount > _warningLogsPerSecond && !_warningTriggered)
                {
                    _warningTriggered = true;
                    Log(LogLevel.Warning, "SYSTEM", "High logging rate detected");
                }
            }
        }

        /// <summary>
        /// Log debug message.
        /// </summary>
        public static void LogDebug(string message) => Log(LogLevel.Debug, "DEBUG", message);

        /// <summary>
        /// Log info message.
        /// </summary>
        public static void LogInfo(string message) => Log(LogLevel.Info, "INFO", message);

        /// <summary>
        /// Log warning message.
        /// </summary>
        public static void LogWarning(string message) => Log(LogLevel.Warning, "WARN", message);

        /// <summary>
        /// Log error message.
        /// </summary>
        public static void LogError(string message) => Log(LogLevel.Error, "ERROR", message);

        /// <summary>
        /// Log error message with exception context.
        /// Adapts two-parameter error logging calls to the canonical LogError implementation.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="ex">Exception context.</param>
        public static void LogError(string message, Exception ex)
        {
            var fullMessage = string.IsNullOrEmpty(ex?.Message) ? message : $"{message}: {ex.Message}";
            Log(LogLevel.Error, "ERROR", fullMessage);
        }

        /// <summary>
        /// Log critical error message.
        /// </summary>
        public static void LogCritical(string message) => Log(LogLevel.Critical, "CRITICAL", message);

        /// <summary>
        /// Log exception with details.
        /// </summary>
        public static void Exception(Exception ex, string context = "")
        {
            var message = string.IsNullOrEmpty(context)
                ? $"{ex.GetType().Name}: {ex.Message}"
                : $"{context} - {ex.GetType().Name}: {ex.Message}";
            Log(LogLevel.Error, "EXCEPTION", message);
        }

        /// <summary>
        /// Legacy Log method for backward compatibility.
        /// Maps category strings to appropriate LogLevel.
        /// </summary>
        /// <param name="category">Log category (DEBUG, INFO, WARNING, ERROR, CRITICAL)</param>
        /// <param name="message">Log message</param>
        public static void Log(string category, string message)
        {
            var logLevel = category.ToUpper() switch
            {
                "DEBUG" => LogLevel.Debug,
                "INFO" => LogLevel.Info,
                "WARNING" => LogLevel.Warning,
                "WARN" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                "CRITICAL" => LogLevel.Critical,
                _ => LogLevel.Debug
            };

            Log(logLevel, category, message);
        }

        /// <summary>
        /// Get monitoring statistics for the logging system.
        /// </summary>
        /// <returns>Tuple containing monitoring stats</returns>
        public static (bool isEmergencyStop, long logsPerSecond, long duplicateErrors) GetMonitoringStats()
        {
            // Simplified implementation
            return (_emergencyStop, _logCount, _duplicateErrorCount);
        }

        /// <summary>
        /// Reset emergency stop flag.
        /// </summary>
        public static void ResetEmergencyStop()
        {
            // Implementation for emergency stop reset
            _emergencyStop = false;
            _warningTriggered = false;
            _lastErrorLog = "";
            _duplicateErrorCount = 0;
        }

        /// <summary>
        /// Get all log entries.
        /// </summary>
        public static List<LogEntry> GetLogEntries()
        {
            var entries = new List<LogEntry>();
            while (_logQueue.TryDequeue(out var entry))
            {
                entries.Add(entry);
            }
            return entries;
        }

        private static bool CheckForAbnormalConditions(LogLevel level, string category, string message)
        {
            if (level == LogLevel.Error && message == _lastErrorLog)
            {
                _duplicateErrorCount++;

                if (_duplicateErrorCount > _maxDuplicateErrors)
                {
                    _emergencyStop = true;
                    Log(LogLevel.Critical, "SYSTEM", "Emergency stop triggered due to excessive duplicate errors");
                    return false;
                }
                else if (_duplicateErrorCount > _warningDuplicateErrors && !_warningTriggered)
                {
                    _warningTriggered = true;
                    Log(LogLevel.Warning, "SYSTEM", "High duplicate error rate detected");
                }
            }
            else
            {
                _lastErrorLog = message;
                _duplicateErrorCount = 0;
            }

            return true;
        }
    }
}
