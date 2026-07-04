// ============================================================================
//  FILE: DLoggerArg3.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Defines logging patterns for the DLogger diagnostics system.
//      This file contains ONLY overloads for the specific argument count
//      or category it represents (Arg1, Arg2, Arg3, Legacy, Custom, etc).
//
//  FEATURES:
//      - Implements all three‑argument logging overloads for the diagnostics system.
//      - Provides modern prefix‑based output for 3‑parameter structured log patterns.
//      - Introduces explicit category routing, enabling subsystem‑category pairing.
//      - Supports structured logging via (subsystem, category, message) and
//        (subsystem, category, exception).
//      - Emits unique PatternTags for Arg3 overloads to ensure deterministic routing.
//      - Guarantees strict isolation: only 3‑argument overloads are defined in this module.
//      - Produces fully normalized, Manager‑ready output with no legacy formatting.
//      - Optimized for structured logging where both subsystem and category context
//        are required.
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
        // ARG3 OVERLOADS (3 PARAMETERS)
        // ====================================================================

        /// <summary>
        /// Logs a message with explicit subsystem and category routing.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="category">The category of the log entry.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(LogSubsystems subsystem, LogCategory category, string message, string eventId, int severity)
        {
            ResolveAndLog(
                subsystem,
                $"[INFO][{category}] {message}",
                "Arg3_SubsystemCategoryMessage",
                3
            );
        }

        /// <summary>
        /// Logs an exception with explicit subsystem and category routing.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="category">The category of the log entry.</param>
        /// <param name="exception">The exception instance to log.</param>
        public static void Log(LogSubsystems subsystem, LogCategory category, Exception exception, string message)
        {
            string msg = exception?.Message ?? "(null exception)";

            ResolveAndLog(
                subsystem,
                $"[ERROR][{category}] {msg}",
                "Arg3_SubsystemCategoryException",
                3
            );
        }
    }
}
