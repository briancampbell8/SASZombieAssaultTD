// ====================================================================================================
//  FILE: EngineTraceWriter.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics Pipeline (Primary Markdown Sink)
//
//  ROLE:
//      Writes fully formatted Markdown diagnostic entries to EngineTrace.md. This file serves as the
//      authoritative, chronological, unfiltered diagnostic trace for the entire engine. ALL
//      diagnostic messages—across all subsystems, categories, and severity levels (Info → Exception)
//      MUST be written here without exception.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class EngineTraceWriter
    {
        private static readonly string TracePath;

        static EngineTraceWriter()
        {
            try
            {
                // Resolve runtime directory (bin/Debug/net8.0-windows/)
                string runtimeDir = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location
                )!;

                // Resolve project root (three levels up)
                string projectRoot = Path.GetFullPath(
                    Path.Combine(runtimeDir, @"..\..\..")
                );

                // Resolve Engine/Reporting/Logs directory
                string logsDir = Path.Combine(projectRoot, "Engine", "Reporting", "Logs");

                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);

                // Final trace file path
                TracePath = Path.Combine(logsDir, "EngineTrace.md");
            }
            catch
            {
                // Absolute fallback — MUST NOT throw
                TracePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "EngineTrace.md"
                );
            }
        }

        /// <summary>
        /// Writes a diagnostic entry to EngineTrace.md deterministically.
        /// </summary>
        public static void Write(DiagnosticEntry entry)
        {
            if (entry == null)
                return;

            try
            {
                using var writer = new StreamWriter(TracePath, append: true);

                writer.WriteLine($"Subsystem: {entry.Subsystem}");
                writer.WriteLine($"Level: {entry.Level}");
                writer.WriteLine($"Category: {entry.Category}");
                writer.WriteLine($"Priority: {entry.Priority}");
                writer.WriteLine($"Message: {entry.Message}");
                writer.WriteLine($"Timestamp: {entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}");
                writer.WriteLine($"CorrelationId: {entry.CorrelationId}");
                writer.WriteLine($"RequestId: {entry.RequestId}");
                writer.WriteLine($"Operation: {entry.Operation}");
                writer.WriteLine($"PatternTag: {entry.PatternTag}");
                writer.WriteLine($"OverloadArgs: {entry.OverloadArgs}");
                writer.WriteLine("------------------------------------------------------------");
                writer.WriteLine();
            }
            catch
            {
                // Diagnostics MUST NEVER throw.
                // If writing fails, swallow silently.
            }
        }
    }
}
