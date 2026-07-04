// ============================================================================
//  FILE: DLoggerArg1.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Defines logging patterns for the DLogger diagnostics system.
//      This file contains ONLY overloads for the specific argument count
//
//  ROLE: Assigns a unique PatternTag
//        and a prefix for category it represents (Arg1).
//
//  FEATURES:
//      - Implements all single‑argument logging overloads for the diagnostics system.
//      - Provides modern prefix‑based output for 1‑parameter log patterns.
//      - Handles both simple message logging and exception logging in isolated form.
//      - Ensures deterministic routing by emitting unique PatternTags for Arg1 patterns.
//      - Guarantees subsystem consistency by defaulting to Diagnostics for Arg1 entries.
//      - Produces fully normalized, Manager‑ready output with no legacy formatting.
//      - Maintains strict isolation: no multi‑argument overloads, no shared logic.
//      - Designed for high‑performance, low‑overhead logging of simple events.
//
//  NOTES:
//      - All pattern modules output modern prefix-based diagnostics format.
//      - No resolver logic is implemented here; routing is handled by
//        DLogger.cs (Diagnostics Manager).
//      - Each overload must produce a unique PatternTag to avoid collisions.
//      - All modules must remain isolated and deterministic.
//      - This file is part of the modular diagnostics architecture and
//        should not contain unrelated logic.
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
        // ARG1 OVERLOADS (1 PARAMETER)
        // ====================================================================

        /// <summary>
        /// Logs a general informational message using the Diagnostics subsystem.
        /// </summary>
        /// <param name="message">The message to log.</param>


        /// <summary>
        /// Logs an exception using the Diagnostics subsystem with Exception category.
        /// </summary>
        /// <param name="exception">The exception instance to log.</param>
        public static void Log(Exception exception)
        {
            string msg = exception?.Message ?? "(null exception)";

            ResolveAndLog(
                LogSubsystems.Diagnostics,
                $"[ERROR][Exception] {msg}",
                "Arg1_Exception",
                1
            );
        }
    }
}
