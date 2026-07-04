// ====================================================================================================
//  FILE: DebugLogger.cs
//  PATH: Engine/Diagnostics/
//  PROGRAM: DebugLogger.cs
//  MODULE: Diagnostics Pipeline
//
//  ROLE:
//      Provides lightweight development-time logging helpers.
//      Acts as a convenience wrapper for emitting quick diagnostic messages
//      that are forwarded into the unified diagnostics pipeline.
//
//  RESPONSIBILITIES:
//      - Offer simple debug logging methods for rapid engine development.
//      - Forward debug messages to DLogger and Writer for structured output.
//      - Provide minimal-overhead logging suitable for early engine bring-up.
//      - Maintain consistent formatting with the rest of the diagnostics system.
//
//  NON-RESPONSIBILITIES:
//      - Performing structured log normalization (handled by DLogger).
//      - Managing engine resources, assets, or gameplay logic.
//      - Handling rendering, audio, or subsystem operations.
//      - Generating reports, analytics, or long-term storage.
//
//  ARCHITECTURAL NOTES:
//      - DebugLogger is optional and may be excluded in release builds.
//      - All production diagnostics must flow through DLogger → Writer.
//      - DebugLogger should remain minimal and free of side effects outside logging.
// ====================================================================================================


namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Legacy façade for old DebugLogger calls.
    /// Routes all calls into the modern Logger pipeline.
    /// </summary>
    public static class DebugLogger
    {
        public static void Log(LogSubsystems subsystem, LogLevel level, string message)
            => DLogger.Log(LogSubsystems.Diagnostics, LogLevel.Info, LogCategory.Diagnostics, "level");

        public static void Log(LogSubsystems subsystem,
            LogLevel level,
            string category,
            string message)
            => DLogger.Log(LogSubsystems.Diagnostics, LogLevel.Info, LogCategory.Diagnostics, "subsystem, level, category, message");
        public static void Log(LogSubsystems subsystem,
                    LogLevel level,
                    LogCategory category,
                    string message)
                    => DLogger.Log(LogSubsystems.Diagnostics, LogLevel.Info, LogCategory.Diagnostics, "subsystem, level, category.ToString(), message");
    }
}
