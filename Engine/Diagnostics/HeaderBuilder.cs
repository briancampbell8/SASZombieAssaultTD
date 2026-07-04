// -----------------------------------------------------------------------------
// File: HeaderBuilder.cs
// Purpose: Deterministic header sweep for SAS Zombie Assault TD
// Scans entire project root and builds Diagnostics HeaderTable.json
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class HeaderBuilder
    {
        // Root of the project — Program.cs lives here


        private static readonly string ProjectRoot =
            @"E:\BDC\Projects\SASZombieAssaultTD";

        // Output location for the diagnostics table
        private static readonly string OutputPath =
            @"E:\BDC\Projects\SASZombieAssaultTD\Engine\Diagnostics\JsonTables\HeaderTable.json";

        // ---------------------------------------------------------------------
        // Entry point for header table generation
        // ---------------------------------------------------------------------
        public static void Build()
        {
            Console.WriteLine("HeaderBuilder: Starting sweep...");
            Console.WriteLine($"ProjectRoot = {ProjectRoot}");
            Console.WriteLine($"Directory.Exists = {Directory.Exists(ProjectRoot)}");

            if (!Directory.Exists(ProjectRoot))
            {
                Console.WriteLine("ERROR: Project root does not exist. Aborting.");
                return;
            }

            // -----------------------------------------------------------------
            // 1. Collect all .cs files under the project root
            // -----------------------------------------------------------------
            var files = Directory.GetFiles(ProjectRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains(@"\bin\"))
                .Where(f => !f.Contains(@"\obj\"))
                .ToList();

            Console.WriteLine($"Files found = {files.Count}");

            if (files.Count == 0)
            {
                Console.WriteLine("WARNING: No .cs files found. Table will be empty.");
            }

            // -----------------------------------------------------------------
            // 2. Build entries
            // -----------------------------------------------------------------
            var entries = new List<HeaderEntry>();

            foreach (var file in files)
            {
                var text = File.ReadAllText(file);

                string ns = ExtractNamespace(text);
                string cls = ExtractClass(text);
                string subsystem = InferSubsystem(file);
                string purpose = InferPurpose(file, cls);

                entries.Add(new HeaderEntry
                {
                    File = MakeRelative(file),
                    Namespace = ns,
                    Class = cls,
                    Subsystem = subsystem,
                    Purpose = purpose
                });

                Console.WriteLine($"Processed: {MakeRelative(file)}");
            }

            // -----------------------------------------------------------------
            // 3. Write JSON table
            // -----------------------------------------------------------------
            var table = new HeaderTable { Entries = entries };

            var json = JsonConvert.SerializeObject(table, Formatting.Indented);
            File.WriteAllText(OutputPath, json);

            Console.WriteLine();
            Console.WriteLine("HeaderBuilder: Completed.");
            Console.WriteLine($"Output written to: {OutputPath}");
        }

        // ---------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------

        private static string MakeRelative(string fullPath)
        {
            return fullPath.Replace(ProjectRoot + @"\", "");
        }

        private static string ExtractNamespace(string text)
        {
            var line = text.Split('\n')
                .FirstOrDefault(l => l.Trim().StartsWith("namespace "));
            return line?.Trim().Replace("namespace ", "") ?? "Unknown";
        }

        private static string ExtractClass(string text)
        {
            var line = text.Split('\n')
                .FirstOrDefault(l => l.Contains("class "));
            if (line == null) return "Unknown";

            var parts = line.Trim().Split(' ');
            int idx = Array.IndexOf(parts, "class");
            return (idx >= 0 && idx < parts.Length - 1) ? parts[idx + 1] : "Unknown";
        }

        private static string InferSubsystem(string file)
        {
            if (file.Contains(@"\Engine\Diagnostics")) return "Engine.Diagnostics";
            if (file.Contains(@"\Engine\Systems")) return "Engine.Systems";
            if (file.Contains(@"\Engine\Rendering")) return "Engine.Rendering";
            if (file.Contains(@"\Engine\Platform")) return "Engine.Platform";
            if (file.Contains(@"\Engine")) return "Engine";

            return "General";
        }

        private static string InferPurpose(string file, string cls)
        {
            return $"Component: {cls}";
        }

        // ---------------------------------------------------------------------
        // Data structures
        // ---------------------------------------------------------------------

        public class HeaderTable
        {
            public List<HeaderEntry> Entries { get; set; }
        }

        public class HeaderEntry
        {
            public string File { get; set; }
            public string Namespace { get; set; }
            public string Class { get; set; }
            public string Subsystem { get; set; }
            public string Purpose { get; set; }
        }
    }
}
