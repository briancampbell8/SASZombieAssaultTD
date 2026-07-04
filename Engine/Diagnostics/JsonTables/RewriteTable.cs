/*====================================================================================================
  FILE:        RewriteTable.cs
  PATH:        Engine/Diagnostics/JsonTables/RewriteTable.cs
  PURPOSE:     Loads and manages the JSON rewrite table used by DLogger for incremental modernization.
  ROLE:
      - Provides read/write access to rewrite table entries.
      - Supports call-site lookup by file and line number.
      - Supplies DLogger with old/new mode state for each legacy log call.
      - Enables controlled, file-by-file modernization without modifying source code.

  FEATURES:
      - Loads RewriteTable.json from Diagnostics/JsonTables.
      - Stores entries keyed by (file, line).
      - Supports mode switching: "old" → "new".
      - Provides safe, deterministic lookup for DLogger.
      - Designed for incremental modernization starting with Scene.cs.

  NOTES:
      - RewriteTable.json is the authoritative source of truth for modernization.
      - DLogger uses this table to decide whether to emit legacy or modernized log output.
      - Table grows automatically as legacy calls are encountered.
====================================================================================================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SASZombieAssaultTD.Engine.Diagnostics.JsonTables
{
    internal sealed class RewriteTable
    {
        private const string TablePath = "Engine/Diagnostics/JsonTables/RewriteTable.json";

        //==============================================================================================
        //  ENTRY MODEL
        //==============================================================================================
        public sealed class Entry
        {
            public string File { get; set; } = string.Empty;
            public int Line { get; set; }
            public string Old { get; set; } = string.Empty;
            public string New { get; set; } = string.Empty;
            public string Mode { get; set; } = "old"; // "old" or "new"
            public string Subsystem { get; set; } = string.Empty;
            public string Level { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Priority { get; set; } = string.Empty;
        }

        //==============================================================================================
        //  CUSTOM COMPARER FOR (file, line)
        //==============================================================================================
        private sealed class FileLineComparer : IEqualityComparer<(string file, int line)>
        {
            public bool Equals((string file, int line) x, (string file, int line) y)
            {
                return x.line == y.line &&
                       string.Equals(x.file, y.file, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode((string file, int line) obj)
            {
                return HashCode.Combine(
                    obj.line,
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.file ?? string.Empty)
                );
            }
        }

        //==============================================================================================
        //  INTERNAL STORAGE
        //==============================================================================================
        private readonly Dictionary<(string file, int line), Entry> _entries;

        private RewriteTable(Dictionary<(string file, int line), Entry> entries)
        {
            _entries = entries;
        }

        //==============================================================================================
        //  LOAD TABLE
        //==============================================================================================
        public static RewriteTable Load()
        {
            // Create empty table if file doesn't exist
            if (!File.Exists(TablePath))
                return new RewriteTable(new Dictionary<(string, int), Entry>(new FileLineComparer()));

            string json = File.ReadAllText(TablePath);
            List<Entry>? list = JsonSerializer.Deserialize<List<Entry>>(json);

            var dict = new Dictionary<(string, int), Entry>(new FileLineComparer());

            if (list != null)
            {
                foreach (Entry e in list)
                {
                    if (!string.IsNullOrWhiteSpace(e.File))
                        dict[(e.File, e.Line)] = e;
                }
            }

            return new RewriteTable(dict);
        }

        //==============================================================================================
        //  SAVE TABLE
        //==============================================================================================
        public void Save()
        {
            var list = new List<Entry>(_entries.Values);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(list, options);

            string directory = Path.GetDirectoryName(TablePath) ?? string.Empty;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(TablePath, json);
        }

        //==============================================================================================
        //  LOOKUP
        //==============================================================================================
        public Entry? Get(string file, int line)
        {
            if (string.IsNullOrWhiteSpace(file))
                return null;

            return _entries.TryGetValue((file, line), out Entry entry) ? entry : null;
        }

        //==============================================================================================
        //  CREATE OR GET
        //==============================================================================================
        public Entry GetOrCreate(string file, int line)
        {
            if (string.IsNullOrWhiteSpace(file))
                file = "<unknown>";

            if (_entries.TryGetValue((file, line), out Entry existing))
                return existing;

            var entry = new Entry
            {
                File = file,
                Line = line,
                Mode = "old"
            };

            _entries[(file, line)] = entry;
            return entry;
        }

        //==============================================================================================
        //  MODE SWITCH
        //==============================================================================================
        public void SetNewMode(Entry entry)
        {
            if (entry != null)
                entry.Mode = "new";
        }
    }
}
