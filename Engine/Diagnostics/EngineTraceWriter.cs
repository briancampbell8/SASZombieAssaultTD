/* ====================================================================================================
 *  FILE: EngineTraceWriter.cs
 *  PATH: Engine/Diagnostics/EngineTraceWriter.cs
 *  SUBSYSTEM: Diagnostics
 *  ROLE: Dedicated Markdown trace file writer for engine diagnostics.
 *
 *  RESPONSIBILITIES:
 *      - Append all diagnostic log entries to EngineTrace.md.
 *      - Maintain deterministic, audit-friendly Markdown formatting.
 *      - Provide a secondary sink for DebugLogger without replacing it.
 *      - Ensure thread-safe, zero-throw file output.
 *
 *  NON-RESPONSIBILITIES:
 *      - Console logging (handled by DebugLogger).
 *      - Log level filtering.
 *      - Real-time formatting logic (caller provides formatted message).
 *
 *  DEPENDENCIES:
 *      - System.IO
 *      - System.Threading
 *
 *  CALLED BY:
 *      - DebugLogger (optional integration)
 *      - EngineDiagnostics.Trace
 *
 *  ARCHITECTURAL NOTES:
 *      - Must never throw exceptions.
 *      - Must never block engine execution.
 *      - Must remain a pure append-only file writer.
 * ==================================================================================================== */

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class EngineTraceWriter
    {
        private static readonly object _fileLock = new();
        private static readonly string _tracePath =
            Path.Combine(AppContext.BaseDirectory, "EngineTrace.md");

        /// <summary>
        /// Appends a fully formatted Markdown trace entry to EngineTrace.md.
        /// </summary>
        /// <param name="formattedMessage">The message already formatted by DebugLogger or Diagnostics.</param>
        public static void WriteMarkdownEntry(string formattedMessage)
        {
            if (string.IsNullOrWhiteSpace(formattedMessage))
                return;

            try
            {
                var sb = new StringBuilder();

                sb.AppendLine($"- **Time:** {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine($"- **Details:** {formattedMessage}");
                sb.AppendLine();

                lock (_fileLock)
                {
                    File.AppendAllText(_tracePath, sb.ToString());
                }
            }
            catch
            {
                // File writer must never throw.
            }
        }
    }
}
