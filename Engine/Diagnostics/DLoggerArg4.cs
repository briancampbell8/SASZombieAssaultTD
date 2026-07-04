// ============================================================================
//  FILE: DLoggerArg4.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Defines logging patterns for the DLogger diagnostics system.
//      This file contains ONLY overloads for the specific argument count
//      or category it represents (Arg1, Arg2, Arg3, Arg4, Legacy, Custom, etc).
//
//  FEATURES:
//      - Implements all four‑argument logging overloads for the diagnostics system.
//      - Provides modern prefix‑based output for 4‑parameter structured log patterns.
//      - Introduces explicit log‑level routing, enabling full metadata control.
//      - Supports structured logging via (subsystem, category, level, message) and
//        (subsystem, category, level, exception).
//      - Emits unique PatternTags for Arg4 overloads to ensure deterministic routing.
//      - Guarantees strict isolation: only 4‑argument overloads are defined in this module.
//      - Produces fully normalized, Manager‑ready output with no legacy formatting.
//      - Optimized for high‑detail logging where subsystem, category, and level must
//        be explicitly defined.
//
//  LOCATION: Diagnostics/Patterns/
//  VERSION: 1.0 (Pattern Module)
// ============================================================================

using System;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static partial class DLogger
    {
        // ====================================================================
        // ARG4 OVERLOADS (4 PARAMETERS)
        // ====================================================================

        /// <summary>
        /// Logs a message with explicit subsystem, category, and level routing.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="category">The category of the log entry.</param>
        /// <param name="level">The severity level of the log entry.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(
            LogSubsystems subsystem,
            LogCategory category,
            LogLevel level,
            string message,
            string eventId)
        {
            ResolveAndLog(
                subsystem,
                $"[{level}][{category}] {message}",
                "Arg4_SubsystemCategoryLevelMessage",
                4
            );
        }

        /// <summary>
        /// Logs an exception with explicit subsystem, category, and level routing.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="category">The category of the log entry.</param>
        /// <param name="level">The severity level of the log entry.</param>
        /// <param name="exception">The exception instance to log.</param>
        public static void Log(
            LogSubsystems subsystem,
            LogCategory category,
            LogLevel level,
            Exception exception,
            string message)
        {
            string msg = exception?.Message ?? "(null exception)";

            ResolveAndLog(
                subsystem,
                $"[{level}][{category}] {msg}",
                "Arg4_SubsystemCategoryLevelException",
                4
            );
        }
    }
}
