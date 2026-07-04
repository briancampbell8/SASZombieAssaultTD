// ============================================================================
//  FILE: LegacyOverload.cs
//  PATH: Engine/Diagnostics/LegacyOverload.cs
//  PURPOSE: Backward compatibility layer for old logging calls.
//           Will be removed once all call sites are migrated.
//
//  Version: 1.0
//  Date: 2026-06-26
// ============================================================================

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class LegacyOverload
    {
        public static void Log(string message)
            => DLogger.Log(message);

        public static void Log(string tag, string message)
            => DLogger.Log(tag, message);

        public static void Log(LogLevel level, string message)
            => DLogger.Log(level, message);

        public static void Log(LogCategory category, string message)
            => DLogger.Log(category, message);

        public static void Log(LogSubsystems subsystem, string tag, string message)
            => DLogger.Log(subsystem, tag, message);

        public static void Log(LogSubsystems subsystem, LogLevel level, string message)
            => DLogger.Log(
                subsystem, level,
    LogCategory.Unknown,
                message);

        public static void Log(LogSubsystems subsystem, LogLevel level, LogCategory category, string message)
            => DLogger.Log(subsystem, level, category, message);
    }
}
