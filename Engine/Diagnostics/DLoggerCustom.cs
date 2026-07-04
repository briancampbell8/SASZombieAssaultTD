// ============================================================================
//  FILE: DLoggerCustom.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Provides custom logging patterns for engine‑specific diagnostics needs.
//      This module is reserved for specialized logging scenarios that do not
//      fit into the structured Arg1–Arg7 pattern hierarchy.
//
//  FEATURES:
//      - Implements custom logging overloads for specialized engine diagnostics.
//      - Provides modern prefix‑based output for custom metadata patterns.
//      - Allows callers to define custom tags for unique diagnostic signatures.
//      - Supports custom routing via (subsystem, customTag, message) and
//        (subsystem, customTag, exception).
//      - Emits unique PatternTags for custom overloads to ensure deterministic routing.
//      - Guarantees strict isolation: only custom overloads are defined in this module.
//      - Produces fully normalized, Manager‑ready output with no legacy formatting.
//      - Optimized for engine‑specific events, experimental diagnostics, and
//        subsystem‑unique logging requirements.
//
//  LOCATION: Diagnostics/Patterns/
//  VERSION: 1.0 (Custom Pattern Module)
// ============================================================================

using System;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static partial class DLogger
    {
        // ====================================================================
        // CUSTOM OVERLOADS
        // ====================================================================

        /// <summary>
        /// Logs a custom message with a caller‑defined diagnostic tag.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="customTag">A caller‑defined tag describing the custom event.</param>
        /// <param name="message">The message to log.</param>
        public static void LogCustom(
            LogSubsystems subsystem,
            string customTag,
            string message)
        {
            ResolveAndLog(
                subsystem,
                $"[CUSTOM][{customTag}] {message}",
                $"Custom_{customTag}_Message",
                3
            );
        }

        /// <summary>
        /// Logs a custom exception with a caller‑defined diagnostic tag.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="customTag">A caller‑defined tag describing the custom event.</param>
        /// <param name="exception">The exception instance to log.</param>
        public static void LogCustom(
            LogSubsystems subsystem,
            string customTag,
            Exception exception)
        {
            string msg = exception?.Message ?? "(null exception)";

            ResolveAndLog(
                subsystem,
                $"[CUSTOM][{customTag}] {msg}",
                $"Custom_{customTag}_Exception",
                3
            );
        }
    }
}
