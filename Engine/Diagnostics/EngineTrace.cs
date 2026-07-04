// ====================================================================================================
//  FILE: EngineTrace.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics Pipeline (Buffer Relay Stage)
//
//  ROLE:
//      Receives a fully normalized DiagnosticEntry from DLogger and forwards it
//      directly to Writer.cs for Markdown generation. EngineTrace.cs is the
//      deterministic relay stage of the diagnostics pipeline — it performs NO
//      formatting, NO JSON serialization, NO file output, and NO transformation.
//      It simply guarantees that every DiagnosticEntry buffer is delivered.
//
//  RESPONSIBILITIES:
//      - Relay DiagnosticEntry deterministically to Writer.cs.
//      - Guarantee that ALL diagnostic messages (Info → Exception) are forwarded.
//      - Guarantee that NO diagnostic entry is ever skipped, filtered, or lost.
//      - Guarantee that relay operations NEVER throw and NEVER block engine flow.
//      - Convert internal failures into DiagnosticEntry fallback messages.
//
//  NON-RESPONSIBILITIES:
//      - JSON serialization (removed from pipeline).
//      - JSON snapshot file output (removed from pipeline).
//      - Markdown formatting (handled by Writer.cs).
//      - File writing (handled by EngineTraceWriter.cs and MarkdownLogWriter.cs).
//      - UI, filtering, or report aggregation (future modules).
//
//  ARCHITECTURAL NOTES:
//      - This module is part of the deterministic, self-healing diagnostics pipeline.
//      - It MUST NEVER fail silently; failures MUST be converted into DiagnosticEntry.
//      - It MUST NEVER block engine execution; all operations are atomic and safe.
//      - EngineTrace.cs is the canonical buffer relay stage — pure, unfiltered,
//        and guaranteed to deliver DiagnosticEntry objects to Writer.cs.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics.Writers;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Central dispatch point for all diagnostic entries. EngineTrace receives fully constructed
    /// DiagnosticEntry objects from DLogger and forwards them to the active writers. This class
    /// performs no formatting, timestamping, or categorization; it simply guarantees that every
    /// diagnostic event is written to the configured sinks.
    ///
    /// Writers include:
    ///   - EngineTraceWriter (primary structured writer)
    ///   - MarkdownLogWriter (developer-friendly readable logs)
    ///   - Additional writers registered by diagnostics subsystems
    ///
    /// EngineTrace is intentionally minimal and deterministic.
    /// </summary>
    public static class EngineTrace
    {
        /// <summary>
        /// Writes a diagnostic entry to all active writers.
        /// </summary>
        public static void Write(DiagnosticEntry entry)
        {
            if (entry == null)
                return;

            // Primary structured writer
            EngineTraceWriter.Write(entry);

            // Developer-friendly markdown writer
            MarkdownLogWriter.Write(entry);



            // Additional writers may be added here if needed
            // Example:
            // JsonLogWriter.Write(entry);
            // CsvLogWriter.Write(entry);
        }
    }
}
