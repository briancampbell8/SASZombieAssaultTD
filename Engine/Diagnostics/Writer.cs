// ====================================================================================================
//  FILE: Writer.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics Pipeline (Fan‑Out Stage)
//
//  ROLE:
//      Accepts DiagnosticEntry from EngineTrace,
//      builds Markdown, and fans out to EngineTraceWriter and MarkdownLogWriter.
//
//  CONTRACT:
//      - Must NEVER throw.
//      - Must NEVER block engine execution.
//      - Must ALWAYS append exactly what EngineTrace provides.
//      - Must ALWAYS include PatternTag and OverloadArgs.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics.Writers;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class Writer
    {
        /// <summary>
        /// Accepts DiagnosticEntry from EngineTrace,
        /// builds Markdown, and forwards to sinks.
        /// </summary>
        public static void Process(DiagnosticEntry entry)
        {
            if (entry == null)
                return;

            string markdown;

            try
            {
                markdown = BuildMarkdown(entry);
            }
            catch (Exception ex)
            {
                // Diagnostics pipeline cannot fail.
                markdown =
                    "### Diagnostic Entry\n" +
                    "- **Subsystem:** Diagnostics\n" +
                    "- **Level:** Exception\n" +
                    "- **Category:** Diagnostics\n" +
                    "- **Priority:** Critical\n" +
                    $"- **Message:** {ex}\n" +
                    "- **PatternTag:** Writer_Failure\n" +
                    "- **OverloadArgs:** 0\n";
            }
            // Fan out to sinks (must NEVER fail)
            try { EngineTraceWriter.Write(entry); } catch { }
            try { MarkdownLogWriter.Write(entry); } catch { }

        }

        private static string BuildMarkdown(DiagnosticEntry e)
        {
            return
                $"### Diagnostic Entry\n" +
                $"- **Subsystem:** {e.Subsystem}\n" +
                $"- **Level:** {e.Level}\n" +
                $"- **Category:** {e.Category}\n" +
                $"- **Priority:** {e.Priority}\n" +
                $"- **Message:** {e.Message}\n" +
                $"- **Timestamp:** {e.Timestamp:yyyy-MM-dd HH:mm:ss}\n" +
                $"- **CorrelationId:** {e.CorrelationId}\n" +
                $"- **RequestId:** {e.RequestId}\n" +
                $"- **Operation:** {e.Operation}\n" +
                $"- **PatternTag:** {e.PatternTag}\n" +
                $"- **OverloadArgs:** {e.OverloadArgs}\n";
        }
    }
}
