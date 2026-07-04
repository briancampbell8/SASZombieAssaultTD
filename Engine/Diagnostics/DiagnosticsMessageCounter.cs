// ====================================================================================================
//  FILE: DiagnosticsMessageCounter.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics Pipeline (Console Summary Output)
//
//  ROLE:
//      Provides a deterministic, non-blocking, non-throwing summary of diagnostic activity for the
//      current engine run. After EngineTrace.md and DiagnosticsReport.md have been written, this
//      module counts the number of diagnostic entries in each file and displays the results in the
//      PowerShell console. This serves as a lightweight, human-readable confirmation that the
//      diagnostics pipeline executed correctly.
//
//  RESPONSIBILITIES:
//      - Count diagnostic entries in EngineTrace.md and DiagnosticsReport.md.
//      - Display message counts in the PowerShell console at the end of each run.
//      - Guarantee that counting operations NEVER throw and NEVER block engine flow.
//      - Guarantee that summary output is always produced, even if logs are empty.
//      - Maintain deterministic behavior aligned with the diagnostics pipeline.
//      - Operate safely with log rotation and backup systems.
//
//  NON-RESPONSIBILITIES:
//      - Writing diagnostic entries (handled by EngineTraceWriter and MarkdownLogWriter).
//      - Markdown formatting (handled by Writer.cs).
//      - DiagnosticEntry creation (handled by DLogger.cs).
//      - Log rotation and backup management (handled by DiagnosticsBackupManager.cs).
//      - UI display or report aggregation (handled by future Windows Forms modules).
//
//  ARCHITECTURAL NOTES:
//      - This module is part of the deterministic, self-healing diagnostics pipeline.
//      - It MUST NEVER throw exceptions; failures MUST degrade gracefully.
//      - It MUST NEVER interfere with log writing or backup rotation.
//      - It provides console-level visibility into pipeline activity for development and debugging.
//      - It is intentionally lightweight and side-effect free.
// ====================================================================================================

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class DiagnosticsMessageCounter
    {
        public static void DisplayCounts()
        {
            try
            {
                // Determine runtime folder (bin/Debug/net8.0-windows/)
                string runtimeDir = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location
                );

                // Walk up 3 levels: net8.0-windows → Debug → bin → project root
                string projectRoot = Path.GetFullPath(
                    Path.Combine(runtimeDir, @"..\..\..")
                );

                string logsDir = Path.Combine(projectRoot, "Engine", "Reporting", "Logs");

                string engineTrace = Path.Combine(logsDir, "EngineTrace.md");
                string diagnosticsReport = Path.Combine(logsDir, "DiagnosticsReport.md");

                int engineTraceCount = CountEntries(engineTrace);
                int diagnosticsReportCount = CountEntries(diagnosticsReport);

                Console.WriteLine($"EngineTrace.md messages written: {engineTraceCount}");
                Console.WriteLine($"DiagnosticsReport.md messages written: {diagnosticsReportCount}");
            }
            catch
            {
                // Never throw — diagnostics pipeline must remain safe.
            }
        }

        private static int CountEntries(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return 0;

                return File.ReadLines(filePath)
                           .Count(line => line.StartsWith("### Diagnostic Entry"));
            }
            catch
            {
                // If counting fails, degrade gracefully.
                return 0;
            }
        }
    }
}
