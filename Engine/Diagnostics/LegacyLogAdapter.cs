// ============================================================================
//  FILE: LegacyLogAdapter.cs
//  PATH: Engine/Diagnostics/
//  PURPOSE: Transitional adapter for legacy logging calls.
//           Converts old-style parameters (strings, missing subsystem,
//           missing category, missing level) into normalized modern calls.
//
//  This file works *with* LegacyOverload.cs.
//  It will be removed once all call sites are migrated.
//
//  Version: 1.0
//  Date: 2026-06-26
// ============================================================================

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class LegacyLogAdapter
    {
        // --------------------------------------------------------------------
        // 1. Message-only legacy calls
        // --------------------------------------------------------------------
        public static void Log(string message)
        {
            LegacyOverload.Log(message);
        }

        // --------------------------------------------------------------------
        // 2. Category string + message
        // --------------------------------------------------------------------
        public static void Log(string categoryTag, string message)
        {
            LegacyOverload.Log(categoryTag, message);
        }

        // --------------------------------------------------------------------
        // 3. Category enum + message
        // --------------------------------------------------------------------
        public static void Log(LogCategory category, string message)
        {
            LegacyOverload.Log(category, message);
        }

        // --------------------------------------------------------------------
        // 4. Level enum + message
        // --------------------------------------------------------------------
        public static void Log(LogLevel level, string message)
        {
            LegacyOverload.Log(level, message);
        }

        // --------------------------------------------------------------------
        // 5. Subsystem + tag + message
        //    (tag may be level or category — DLogger handles normalization)
        // --------------------------------------------------------------------
        public static void Log(LogSubsystems subsystem, string tag, string message)
        {
            LegacyOverload.Log(subsystem, tag, message);
        }

        // --------------------------------------------------------------------
        // 6. Subsystem + level + message
        // --------------------------------------------------------------------
        public static void Log(LogSubsystems subsystem, LogLevel level, string message)
        {
            LegacyOverload.Log(subsystem, level, message);
        }

        // --------------------------------------------------------------------
        // 7. Full modern signature (legacy systems may still call this)
        // --------------------------------------------------------------------
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
