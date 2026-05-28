/* ====================================================================================================
 *  FILE: ModernLoggingSystemExtensions.cs
 *  PATH: Engine/Core/ModernLoggingSystemExtensions.cs
 *  SUBSYSTEM: Diagnostics
 *  ROLE: Backward-compatibility façade for legacy 2-parameter logging calls.
 *
 *  RESPONSIBILITIES:
 *      - Accept legacy (category, message) logging calls.
 *      - Normalize legacy category strings to DebugLogger.LogLevel enum values.
 *      - Forward normalized log events to DebugLogger using the modern 3-parameter API.
 *      - Maintain compatibility during migration away from ModernLoggingSystem.
 *
 *  NON-RESPONSIBILITIES:
 *      - Monitoring state management (handled by DiagnosticsMonitor).
 *      - Log formatting, routing, or sink management (handled by DebugLogger).
 *      - Threshold evaluation or emergency-stop logic.
 *
 *  ARCHITECTURAL NOTES:
 *      - This file must remain minimal and deterministic.
 *      - All category normalization must be explicit and stable.
 *      - No monitoring or state must be introduced here.
 * ==================================================================================================== */

using SASZombieAssaultTD.Engine.Diagnostics;
using System;

namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Provides backward-compatible logging entry points for legacy systems.
    /// Converts legacy category strings into DebugLogger.LogLevel enum values.
    /// </summary>
    public static class ModernLoggingSystemExtensions
    {
        /// <summary>
        /// Accepts a legacy (category, message) logging call and forwards it to DebugLogger.
        /// </summary>
        /// <param name="category">Legacy category string (DEBUG, INFO, WARNING, ERROR, CRITICAL).</param>
        /// <param name="message">Log message content.</param>
        public static void Log(string category, string message)
        {
            var normalized = category?.ToUpperInvariant() ?? "DEBUG";

            DebugLogger.LogLevel level = normalized switch
            {
                "DEBUG" => DebugLogger.LogLevel.Debug,
                "INFO" => DebugLogger.LogLevel.Info,
                "INFORMATION" => DebugLogger.LogLevel.Info,
                "WARNING" => DebugLogger.LogLevel.Warning,
                "WARN" => DebugLogger.LogLevel.Warning,
                "ERROR" => DebugLogger.LogLevel.Error,
                "ERR" => DebugLogger.LogLevel.Error,
                "CRITICAL" => DebugLogger.LogLevel.Critical,
                "CRIT" => DebugLogger.LogLevel.Critical,
                _ => DebugLogger.LogLevel.Debug
            };

            DebugLogger.Log(level, category, message);
        }
    }
}
