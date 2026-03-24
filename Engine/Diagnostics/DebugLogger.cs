/*
File:    ModernLoggingSystem.cs
Author:  BDC
Created: 2026-02-10

Purpose:
File-based diagnostic logger writing labeled entries for tracing.

Notes:
Holds a StreamWriter open for the process lifetime to avoid
open/write/close per call. Directory is created once in the
static constructor. Thread-safe via lock.

*/
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// File-based diagnostic logger writing labeled entries for tracing.
    /// </summary>
    public static class DiagnosticLogger
    {
        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BDC",
            "DebugLogs",
            "EngineTrace.md"
        );

        private static readonly StreamWriter _writer;
        private static readonly object _lock = new();

        // Recognized log categories
        public const string Phase5 = "Phase5";

        static DiagnosticLogger()
        {
            try
            {
                string? dir = Path.GetDirectoryName(LogPath);

                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                _writer = new StreamWriter(LogPath, append: true) { AutoFlush = true };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to initialize DiagnosticLogger: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Logs a diagnostic entry with a label and details.
        /// </summary>
        /// <param name="label">The label for the log entry.</param>
        /// <param name="details">The details of the log entry.</param>
        /// <param name="filePath">The source file path of the caller (auto-populated).</param>
        public static void Log(
            string label,
            string details,
            [CallerFilePath] string filePath = ""
        )
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
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
                Console.Error.WriteLine($"Failed to log entry: {ex.Message}");
            }
        }

        /// <summary>
        /// Flushes and closes the underlying stream. Call once at engine shutdown.
        /// </summary>
        public static void Shutdown()
        {
            try
            {
                lock (_lock)
                {
                    _writer?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to shutdown DiagnosticLogger: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// DebugLogger alias for DiagnosticLogger for backward compatibility.
    /// </summary>
    public static class DebugLogger
    {
        public const string Phase5 = DiagnosticLogger.Phase5;

        public static void Log(string label, string details, [CallerFilePath] string filePath = "")
        {
            DiagnosticLogger.Log(label, details, filePath);
        }

        public static void LogInfo(string message) => DiagnosticLogger.Log("INFO", message);
        public static void LogError(string message) => DiagnosticLogger.Log("ERROR", message);
        public static void LogDebug(string message) => DiagnosticLogger.Log("DEBUG", message);
        public static void LogWarning(string message) => DiagnosticLogger.Log("WARNING", message);

        public static void Shutdown() => DiagnosticLogger.Shutdown();
    }
}
