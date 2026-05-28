/* ====================================================================================================
 *  FILE: DebugLogger.cs
 *  PATH: Engine/Diagnostics/DebugLogger.cs
 *  SUBSYSTEM: Diagnostics
 *  ROLE: Central logging façade providing structured log emission, category routing,
 *        and integration with DiagnosticsMonitor for health tracking.
 *
 *  RESPONSIBILITIES:
 *      - Emit structured log entries to the file-based DiagnosticLogger backend.
 *      - Normalize log levels and categories for consistent output.
 *      - Forward all log events to DiagnosticsMonitor for monitoring state updates.
 *      - Provide a unified logging API for all engine subsystems.
 *      - Maintain backward compatibility with legacy logging paths.
 *
 *  NON-RESPONSIBILITIES:
 *      - Monitoring state storage or threshold evaluation (handled by DiagnosticsMonitor).
 *      - File I/O lifecycle management (handled by DiagnosticLogger).
 *      - Subsystem-specific formatting or filtering.
 *
 *  ARCHITECTURAL NOTES:
 *      - All log events must pass through DebugLogger.Log(...) to ensure monitoring integration.
 *      - DiagnosticLogger is the low-level sink; DebugLogger is the public façade.
 *      - LogLevel enum must remain stable and deterministic.
 * ==================================================================================================== */

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    internal static class DiagnosticLogger
    {
        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BDC",
            "DebugLogs",
            "EngineTrace.md"
        );

        private static readonly object _lock = new();

        // Writer is now nullable and guarded
        private static readonly StreamWriter? _writer;

        // Logger health flag
        internal static bool IsHealthy { get; private set; } = true;

        static DiagnosticLogger()
        {
            try
            {
                string? dir = Path.GetDirectoryName(LogPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                _writer = new StreamWriter(LogPath, append: true)
                {
                    AutoFlush = true
                };
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("DiagnosticLogger.Init", $"Failed to initialize DiagnosticLogger: {ex.Message}");

                _writer = null;
                IsHealthy = false;
            }
        }

        public static void Write(string label, string details, string fileName)
        {
            if (_writer == null)
                return;

            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                string entry =
                    $"### {label}\n\n" +
                    $"- **Time:** {timestamp}\n" +
                    $"- **File:** {fileName}\n" +
                    $"- **Details:** {details}\n\n";

                lock (_lock)
                {
                    _writer.Write(entry);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("DiagnosticLogger.Write", $"Failed to write log entry: {ex.Message}");
                IsHealthy = false;
            }
        }

        public static void Shutdown()
        {
            if (_writer == null)
                return;

            try
            {
                lock (_lock)
                {
                    _writer.Dispose();
                }
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("DiagnosticLogger.Shutdown", $"Failed to shutdown DiagnosticLogger: {ex.Message}");
                IsHealthy = false;
            }
        }
    }

    public static class DebugLogger
    {
        private static object TheType;
        private static object TheMember;

        public enum LogLevel { Debug, Info, Warning, Error, Critical }

        // ====================================================================================================
        // CORE LOGGING ENTRY POINT
        // ====================================================================================================
        public static void Log(LogLevel level, string category, string message,
            [CallerFilePath] string filePath = "")
        {
            string fileName = Path.GetFileName(filePath);

            DiagnosticLogger.Write(
                level.ToString().ToUpper(),
                $"{category}: {message}",
                fileName
            );

            DiagnosticsMonitor.RegisterLogEvent(
                ConvertToDiagnosticCategory(level),
                message
            );
        }

        private static DiagnosticCategory ConvertToDiagnosticCategory(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => DiagnosticCategory.Debug,
                LogLevel.Info => DiagnosticCategory.Info,
                LogLevel.Warning => DiagnosticCategory.Warning,
                LogLevel.Error => DiagnosticCategory.Error,
                LogLevel.Critical => DiagnosticCategory.Exception,
                _ => DiagnosticCategory.Debug
            };
        }

        // ====================================================================================================
        // MODERN PUBLIC WRAPPERS
        // ====================================================================================================
        public static void Debug(string category, string message) =>
            Log(LogLevel.Debug, category, message);

        public static void Info(string category, string message) =>
            Log(LogLevel.Info, category, message);

        public static void Warning(string category, string message) =>
            Log(LogLevel.Warning, category, message);

        public static void Error(string category, string message) =>
            Log(LogLevel.Error, category, message);

        public static void Critical(string category, string message) =>
            Log(LogLevel.Critical, category, message);

        public static void Shutdown() => DiagnosticLogger.Shutdown();

        // ====================================================================================================
        // COMPLETE LEGACY COMPATIBILITY LAYER
        // ====================================================================================================

        internal static void LogDebug(string category, string message) =>
            Log(LogLevel.Debug, category, message);

        internal static void LogDebug(string message) =>
            Log(LogLevel.Debug, "DEBUG", message);

        internal static void LogError(string category, Exception ex) =>
            Log(LogLevel.Error, category, $"{ex.GetType().Name}: {ex.Message}");

        internal static void LogError(string message) =>
            Log(LogLevel.Error, "ERROR", message);

        internal static void LogWarning(string message) =>
            Log(LogLevel.Warning, "WARNING", message);

        internal static void LogException(string category, Exception ex) =>
            Log(LogLevel.Critical, category, $"{ex.GetType().Name}: {ex.Message}");

        internal static void Exception(Exception ex, string context) =>
            Log(LogLevel.Critical, "EXCEPTION", $"{context}: {ex.Message}");

        internal static void Log(string category, string message) =>
            Log(LogLevel.Info, category, message);

        internal static void Log(string level, string category, string message)
        {
            if (!Enum.TryParse<LogLevel>(level, true, out var parsed))
                parsed = LogLevel.Info;

            Log(parsed, category, message);
        }

        internal static void Initialize() =>
            Log(LogLevel.Info, "INIT", "Initialization event");

        internal static void LogInfo(string v) =>
            Log(LogLevel.Info, "INFO", v);

        internal static void Trace(string v1, string v2) =>
            Log(LogLevel.Debug, v1, v2);

        internal static void LogError(string v1, string v2)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal static void DebugLog(string v)
        {
            throw new NotImplementedException();
        }
    }
}
