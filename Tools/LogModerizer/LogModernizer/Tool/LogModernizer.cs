// ====================================================================================================
//  FILE: LogModernizer.cs
//  PATH: Tool\LogModernizer.cs
//  PROGRAM: LogModernizer.cs
//  MODULE: Diagnostics & Engine Pipeline (LogModernizer)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for LogModernizer.Tool.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide public interface and handling execution for Rewrite().
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================

namespace LogModernizer.Tool
{
    internal sealed class LogModernizer
    {
        private readonly PatternDetector _detector;
        private readonly SubsystemResolver _subsystemResolver;
        private readonly CategoryResolver _categoryResolver;
        private readonly LogLevelResolver _levelResolver;
        private readonly MessageNormalizer _normalizer;

        public LogModernizer()
        {
            _detector = new PatternDetector();
            _subsystemResolver = new SubsystemResolver();
            _categoryResolver = new CategoryResolver();
            _levelResolver = new LogLevelResolver();
            _normalizer = new MessageNormalizer();
        }

        public string Rewrite(string filePath, string source, out int rewrittenCount)
        {
            rewrittenCount = 0;

            var matches = _detector.FindLegacyCalls(source);
            if (matches.Count == 0)
                return source;

            string updatedSource = source;

            foreach (var match in matches)
            {
                var subsystem = _subsystemResolver.Resolve(filePath);
                var category = _categoryResolver.Resolve(match, filePath);
                var level = _levelResolver.Resolve(match);
                var message = _normalizer.Normalize(match.Message);

                string newCall = BuildStructuredCall(subsystem, level, category, message);

                updatedSource = updatedSource.Replace(match.FullText, newCall);
                rewrittenCount++;
            }

            return updatedSource;
        }

        private string BuildStructuredCall(string subsystem, string level, string category, string message)
        {
            return $"DLogger.Log({subsystem}, {level}, {category}, \"{message}\");";
        }
    }
}
