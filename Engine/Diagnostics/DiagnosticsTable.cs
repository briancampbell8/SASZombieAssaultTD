// ====================================================================================================
//  FILE: DiagnosticsTable.cs
//  PATH: ./Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  ROLE:
//      Provide logging, profiling, or diagnostic instrumentation.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Diagnostics subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
// -----------------------------------------------------------------------------
// File: DiagnosticsTable.cs
// Purpose: Data model for diagnostics table storage
// -----------------------------------------------------------------------------

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Represents the diagnostics table stored in DiagnosticsTable.json.
    /// This structure can be expanded as your engine grows.
    /// </summary>
    internal class DiagnosticsTable
    {
        /// <summary>
        /// A list of diagnostic entries written by the engine.
        /// </summary>
        public List<DiagnosticsEntry> Entries { get; set; }

        public DiagnosticsTable() => Entries = new List<DiagnosticsEntry>();
    }

    /// <summary>
    /// Represents a single diagnostic entry.
    /// </summary>
    internal class DiagnosticsEntry
    {
        public DateTime Timestamp { get; set; }
        public string Subsystem { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }

        public DiagnosticsEntry()
        {
            Timestamp = System.DateTime.Now;
            Subsystem = string.Empty;
            Category = string.Empty;
            Message = string.Empty;
        }
    }
}

