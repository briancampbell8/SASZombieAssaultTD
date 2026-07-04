// ====================================================================================================
//  FILE: MarkdownLogWriter.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics Pipeline (Markdown Sink)
//
//  ROLE:
//      Writes each diagnostic entry to a unified Markdown log file. This writer is intended for
//      developer-facing diagnostics, providing readable, structured output without altering or
//      reformatting the message content.
//
//  RESPONSIBILITIES:
//      - Append DiagnosticEntry records to a Markdown file.
//      - Preserve message content EXACTLY as provided.
//      - Guarantee non-blocking, non-throwing behavior.
//      - Provide deterministic formatting for developer inspection.
//
//  NON-RESPONSIBILITIES:
//      - Does not generate timestamps (DiagnosticEntry provides them).
//      - Does not categorize or filter diagnostics.
//      - Does not perform log rotation or backup management.
//      - Does not modify message formatting.
//
//  ARCHITECTURAL NOTES:
//      - This writer MUST NEVER throw.
//      - This writer MUST NEVER block engine execution.
//      - This writer MUST NEVER reformat messages.
//      - This writer MUST ALWAYS append exactly what DiagnosticEntry provides.
// ====================================================================================================

using System.IO;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Diagnostics.Writers
{
    public static class MarkdownLogWriter
    {
        private static readonly object _lock = new();
        private static readonly string _baseDir;

        static MarkdownLogWriter()
        {
            try
            {
                // Runtime folder (bin/Debug/net8.0-windows/)
                string runtimeDir = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location
                );

                // Walk up 3 levels: net8.0-windows → Debug → bin → project root
                string projectRoot = Path.GetFullPath(
                    Path.Combine(runtimeDir, @"..\..\..")
                );

                // Final target: Engine/Reporting/Logs
                _baseDir = Path.Combine(projectRoot, "Engine", "Reporting", "Logs");

                if (!Directory.Exists(_baseDir))
                    Directory.CreateDirectory(_baseDir);
            }
            catch
            {
                _baseDir = null; // Safe no-op mode
            }
        }

        /// <summary>
        /// Writes a DiagnosticEntry to the Markdown log file.
        /// </summary>
        public static void Write(DiagnosticEntry entry)
        {
            if (entry == null)
                return;

            if (_baseDir == null)
                return;

            try
            {
                string filePath = Path.Combine(_baseDir, "DiagnosticsReport.md");

                string block =
                    $"### Diagnostic Entry\n" +
                    $"- **Subsystem:** {entry.Subsystem}\n" +
                    $"- **Level:** {entry.Level}\n" +
                    $"- **Category:** {entry.Category}\n" +
                    $"- **Priority:** {entry.Priority}\n" +
                    $"- **Message:** {entry.Message}\n" +
                    $"- **Timestamp:** {entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\n" +
                    $"- **CorrelationId:** {entry.CorrelationId}\n" +
                    $"- **RequestId:** {entry.RequestId}\n" +
                    $"- **Operation:** {entry.Operation}\n" +
                    $"- **PatternTag:** {entry.PatternTag}\n" +
                    $"- **OverloadArgs:** {entry.OverloadArgs}\n\n";

                lock (_lock)
                {
                    File.AppendAllText(filePath, block);
                }
            }
            catch
            {
                // MUST NEVER throw.
            }
        }
    }
}
