// ====================================================================================================
//  FILE: LegacyLogAdapter.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  PURPOSE:
//      Transitional adapter for legacy logging calls.
//      Converts old-style parameters into normalized modern calls.
// ====================================================================================================

using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class LegacyLogAdapter
    {
        // 1. Message-only legacy calls
        public static void Log(string message)
        {
            LegacyOverload.Log(LogSubsystems.Diagnostics, LogCategory.General, message);
        }

        // 2. Category string + message
        public static void Log(string categoryTag, string message)
        {
            LegacyOverload.Log(LogSubsystems.Diagnostics, LogCategory.General, $"{categoryTag}: {message}");
        }

        // 3. Category enum + message
        public static void Log(LogCategory category, string message)
        {
            LegacyOverload.Log(LogSubsystems.Diagnostics, category, message);
        }

        // 4. Level enum + message
        public static void Log(LogLevel level, string message)
        {
            LegacyOverload.Log(LogSubsystems.Diagnostics, (LogCategory)level, message);
        }

        // 5. Subsystem + tag + message
        public static void Log(LogSubsystems subsystem, string tag, string message)
        {
            LegacyOverload.Log(subsystem, LogCategory.General, $"{tag}: {message}");
        }

        // 6. Subsystem + level + message
        public static void Log(LogSubsystems subsystem, LogLevel level, string message)
        {
            LegacyOverload.Log(subsystem, (LogCategory)level, message);
        }

        // 7. Full modern signature
        public static void Log(
            LogSubsystems subsystem,
            LogLevel level,
            LogCategory category,
            string message)
        {
            LegacyOverload.Log(subsystem, level, category, message);
        }
    }
}
