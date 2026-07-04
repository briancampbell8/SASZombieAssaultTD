// ====================================================================================================
//  FILE: DiagnosticEntry.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics Pipeline (Canonical Diagnostic Record Structure)
//
//  ROLE:
//      Defines the canonical diagnostic record used throughout the engine. Every diagnostic event,
//      whether emitted by DLogger, EngineTraceWriter, MarkdownLogWriter, or any subsystem, is
//      represented as a DiagnosticEntry. This class provides a deterministic, structured container
//      for all diagnostic metadata, ensuring consistent formatting, timestamping, correlation, and
//      categorization across the entire diagnostics pipeline.
//
//  RESPONSIBILITIES:
//      - Store all diagnostic metadata for a single event.
//      - Provide a single authoritative timestamp for the event.
//      - Provide correlation fields for tracing multi-step operations.
//      - Provide categorization fields for filtering and reporting.
//      - Provide pattern metadata for modern diagnostics analysis.
//      - Guarantee deterministic initialization of all fields.
//
//  NON-RESPONSIBILITIES:
//      - Writing diagnostic entries to disk (EngineTraceWriter, MarkdownLogWriter).
//      - Formatting diagnostic output (Writer.cs).
//      - Log rotation or backup management (DiagnosticsBackupManager.cs).
//      - Message counting (DiagnosticsMessageCounter.cs).
//      - Subsystem-specific diagnostics logic (DLogger.cs).
//
//  ARCHITECTURAL NOTES:
//      - Timestamp MUST be created once, at entry construction, using local time.
//      - Writers MUST serialize the Timestamp field and MUST NOT generate their own timestamps.
//      - This class is intentionally lightweight and free of behavior.
//      - This class defines the deterministic schema for all diagnostics emitted by the engine.
//      - Additional metadata fields (PatternTag, OverloadArgs) support modern diagnostics analysis.
// ====================================================================================================
using System;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public class DiagnosticEntry
    {
        // Parameterless constructor required for DLogger object initializer usage
        public DiagnosticEntry(string v)
        {
            Timestamp = System.DateTime.Now; // local time zone
            CorrelationId = Guid.NewGuid().ToString("N"); // tracing
            RequestId = Guid.NewGuid().ToString("N");
            Operation = "";
            Priority = "Normal";
            PatternTag = "";
            OverloadArgs = 0;
        }

        public DiagnosticEntry(string v, string v1) : this(v)
        {
        }

        // Core fields
        public string Subsystem { get; set; } = "";
        public string Level { get; set; } = "";
        public string Category { get; set; } = "";
        public string Priority { get; set; } = "";
        public string Message { get; set; } = "";

        // Timestamp created ONCE at entry construction
        public DateTime Timestamp { get; set; }

        // Correlation / tracing fields
        public string CorrelationId { get; set; }
        public string RequestId { get; set; }
        public string Operation { get; set; }

        // Modern diagnostics metadata
        public string PatternTag { get; set; }
        public int OverloadArgs { get; set; }
    }
}
