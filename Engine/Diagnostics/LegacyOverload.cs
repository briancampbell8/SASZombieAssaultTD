// ====================================================================================================
//  FILE: LegacyOverload.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  PURPOSE:
//      Backward compatibility layer for old logging calls.
//      Normalizes legacy signatures into modern DLogger.Log() calls.
// ====================================================================================================

using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class LegacyOverload
    {
        // --------------------------------------------------------------------
        // 1. Message-only legacy calls
        // --------------------------------------------------------------------
        public static void Log(LogSubsystems diagnostics, string message)
        {
            DLogger.Log(LogSubsystems.Diagnostics, LogLevel.Info, LogCategory.General, message);
        }

        // --------------------------------------------------------------------
        // 2. Category string + message
        // --------------------------------------------------------------------
        public static void Log(string tag, string message)
        {
            DLogger.Log(LogSubsystems.Diagnostics, LogLevel.Info, LogCategory.General, $"{tag}: {message}");
        }

        // --------------------------------------------------------------------
        // 3. Category enum + message
        // --------------------------------------------------------------------
        public static void Log(LogSubsystems diagnostics, LogCategory category, string message)
        {
            DLogger.Log(LogSubsystems.Diagnostics, LogLevel.Info, category, message);
        }

        // --------------------------------------------------------------------
        // 4. Level enum + message
        // --------------------------------------------------------------------
        public static void Log(LogLevel level, string message)
        {
            DLogger.Log(LogSubsystems.Diagnostics, level, LogCategory.General, message);
        }

        // --------------------------------------------------------------------
        // 5. Subsystem + tag + message
        // --------------------------------------------------------------------
        public static void Log(LogSubsystems subsystem, string tag, string message)
        {
            DLogger.Log(subsystem, LogLevel.Info, LogCategory.General, $"{tag}: {message}");
        }

        // --------------------------------------------------------------------
        // 6. Subsystem + level + message
        // --------------------------------------------------------------------
        public static void Log(LogSubsystems subsystem, LogLevel level, string message)
        {
            DLogger.Log(subsystem, level, LogCategory.General, message);
        }

        // --------------------------------------------------------------------
        // 7. Full modern signature
        // --------------------------------------------------------------------
        public static void Log(
            LogSubsystems subsystem,
            LogLevel level,
            LogCategory category,
            string message)
        {
            DLogger.Log(subsystem, level, category, message);
        }
    }
}
