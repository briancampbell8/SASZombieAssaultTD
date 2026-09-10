// ====================================================================================================
//  FILE: PatternDetector.cs
//  PATH: Tool\PatternDetector.cs
//  PROGRAM: PatternDetector.cs
//  MODULE: Diagnostics & Engine Pipeline (PatternDetector)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for LogModernizer.Tool.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide deterministic engine pipeline handling execution logic.
//      - Maintain runtime flow and process core thread states safely.
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================

using System.Text.RegularExpressions;

namespace LogModernizer.Tool
{
    internal sealed class PatternDetector
    {
        private static readonly Regex LegacyCallRegex = new Regex(
            @"(?<call>(DLogger|Dlogger)\s*\.\s*Log\s*\((?<args>[\s\S]*?)\))\s*;",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public List<LegacyCallMatch> FindLegacyCalls(string source)
        {
            var results = new List<LegacyCallMatch>();

            var matches = LegacyCallRegex.Matches(source);
            foreach (Match m in matches)
            {
                string fullCall = m.Groups["call"].Value;
                string args = m.Groups["args"].Value;

                var parsed = ParseArguments(args);

                parsed.FullText = fullCall + ";";
                results.Add(parsed);
            }

            return results;
        }

        private LegacyCallMatch ParseArguments(string args)
        {
            var parts = SplitArguments(args);

            for (int i = 0; i < parts.Count; i++)
                parts[i] = parts[i].Trim();

            if (parts.Count == 1)
            {
                return new LegacyCallMatch
                {
                    Message = StripQuotes(parts[0])
                };
            }

            if (parts.Count == 2)
            {
                bool firstIsException = parts[0].StartsWith("new ") || parts[0].Contains("Exception");

                if (firstIsException)
                {
                    return new LegacyCallMatch
                    {
                        ExceptionText = parts[0],
                        Message = StripQuotes(parts[1])
                    };
                }

                return new LegacyCallMatch
                {
                    Tag = StripQuotes(parts[0]),
                    Message = StripQuotes(parts[1])
                };
            }

            if (parts.Count == 3)
            {
                return new LegacyCallMatch
                {
                    Tag = StripQuotes(parts[0]),
                    Message = StripQuotes(parts[1]),
                    Priority = parts[2]
                };
            }

            return new LegacyCallMatch
            {
                Message = args
            };
        }

        private List<string> SplitArguments(string args)
        {
            var result = new List<string>();
            bool inQuotes = false;
            int start = 0;

            for (int i = 0; i < args.Length; i++)
            {
                char c = args[i];

                if (c == '"')
                    inQuotes = !inQuotes;

                if (c == ',' && !inQuotes)
                {
                    result.Add(args.Substring(start, i - start));
                    start = i + 1;
                }
            }

            if (start < args.Length)
                result.Add(args.Substring(start));

            return result;
        }

        private string StripQuotes(string s)
        {
            s = s.Trim();
            if (s.StartsWith("\"") && s.EndsWith("\"") && s.Length >= 2)
                return s.Substring(1, s.Length - 2);
            return s;
        }
    }

    internal sealed class LegacyCallMatch
    {
        public string FullText { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Tag { get; set; }
        public string? Priority { get; set; }
        public string? ExceptionText { get; set; }
    }
}
