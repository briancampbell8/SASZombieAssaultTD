// -----------------------------------------------------------------------------
// File: DiagnosticsTableLoader.cs
// Purpose: Ensures the diagnostics table file exists, loads it, and repairs it
// -----------------------------------------------------------------------------

using System.IO;
using System.Text.Json;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    internal static class DiagnosticsTableLoader
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        /// <summary>
        /// Ensures the diagnostics table file exists.
        /// Creates the directory and file if missing.
        /// Repairs corrupted or empty files.
        /// </summary>
        internal static DiagnosticsTable EnsureExists(string path)
        {
            // Ensure directory exists
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // If file does not exist, create empty table
            if (!File.Exists(path))
            {
                var empty = new DiagnosticsTable();
                File.WriteAllText(path, JsonSerializer.Serialize(empty, _jsonOptions));
                return empty;
            }

            // Read file
            var json = File.ReadAllText(path);

            // If empty or whitespace, recreate
            if (string.IsNullOrWhiteSpace(json))
            {
                var empty = new DiagnosticsTable();
                File.WriteAllText(path, JsonSerializer.Serialize(empty, _jsonOptions));
                return empty;
            }

            // Try to deserialize
            try
            {
                return JsonSerializer.Deserialize<DiagnosticsTable>(json, _jsonOptions)
                       ?? new DiagnosticsTable();
            }
            catch
            {
                // Corrupted JSON — recreate
                var empty = new DiagnosticsTable();
                File.WriteAllText(path, JsonSerializer.Serialize(empty, _jsonOptions));
                return empty;
            }
        }
    }
}
