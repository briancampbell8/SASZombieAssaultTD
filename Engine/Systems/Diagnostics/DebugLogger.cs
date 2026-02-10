/*
    File:    DebugLogger.cs
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

namespace SASZombieAssaultTD.Engine.Systems.Diagnostics
{
    public static class DebugLogger
    {
        private static readonly string LogPath =
            @"E:\BDC\DebugLogs\EngineTrace.md";

        private static readonly StreamWriter _writer;
        private static readonly object _lock = new object();

        // Recognized log categories
        public const string Phase5 = "Phase5";

        static DebugLogger()
        {
            string? dir = Path.GetDirectoryName(LogPath);

            if (dir != null)
            {
                Directory.CreateDirectory(dir);
            }

            _writer = new StreamWriter(LogPath, append: true) { AutoFlush = true };
        }

        public static void Log(
            string label,
            string details,
            [CallerFilePath] string filePath = ""
        )
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

        /// <summary>
        /// Flushes and closes the underlying stream. Call once at engine shutdown.
        /// </summary>
        public static void Shutdown()
        {
            lock (_lock)
            {
                _writer.Flush();
                _writer.Dispose();
            }
        }
    }
}