// ====================================================================================================
//  FILE: LogLevelResolver.cs
//  PATH: Tool\LogLevelResolver.cs
//  PROGRAM: LogLevelResolver.cs
//  MODULE: Diagnostics & Engine Pipeline (LogLevelResolver)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for LogModernizer.Tool.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide public interface and handling execution for Resolve().
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
    internal sealed class LogLevelResolver
    {
        public string Resolve(LegacyCallMatch match)
        {
            string message = match.Message;

            if (message.Contains("fatal", System.StringComparison.OrdinalIgnoreCase))
                return "LogLevel.Fatal";

            if (message.Contains("error", System.StringComparison.OrdinalIgnoreCase))
                return "LogLevel.Error";

            if (message.Contains("warn", System.StringComparison.OrdinalIgnoreCase))
                return "LogLevel.Warning";

            if (message.Contains("debug", System.StringComparison.OrdinalIgnoreCase))
                return "LogLevel.Debug";

            return "LogLevel.Info";
        }
    }
}
