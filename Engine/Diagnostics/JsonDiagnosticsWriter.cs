// ====================================================================================================
//  FILE: JsonDiagnosticsWriter.cs
//  PATH: ./Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  ROLE:
//      Provide logging, profiling, or diagnostic instrumentation.
//
//  RESPONSIBILITIES:
//      - Provide Write() behavior for the Diagnostics subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
// ====================================================================================================
//  FILE: JsonDiagnosticsWriter.cs
//  AUTHOR: BDC
//  DATE: 2026-06-30
//  PATH: Engine/Diagnostics/JsonDiagnosticsWriter.cs
//  DESCRIPTION:
//      Writes DiagnosticEntry objects to a JSONL file (one JSON object per line).
//      Fully supports the modern 11-field DiagnosticEntry including PatternTag
//      and OverloadArgs.
//
//  VERSION: 3.0
//  CONTRACT:
//      - Must NEVER throw.
//      - Must NEVER block engine execution.
//      - Must ALWAYS serialize the full DiagnosticEntry object.
//      - Must ALWAYS append exactly one JSON object per line.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Text.Json;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    internal class JsonDiagnosticsWriter
    {
        private static readonly object _lock = new();

        private readonly string _path =
            Path.Combine(AppContext.BaseDirectory, "Diagnostics.json");

        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Writes a DiagnosticEntry to a JSONL file.
        /// </summary>
        public void Write(DiagnosticEntry entry)
        {
            if (entry == null)
                return;

            try
            {
                // Serialize the full modern DiagnosticEntry (11 fields)
                string json = JsonSerializer.Serialize(entry, _options);

                lock (_lock)
                {
                    File.AppendAllText(_path, json + Environment.NewLine);
                }
            }
            catch
            {
                // Diagnostics must NEVER throw.
            }
        }
    }
}

