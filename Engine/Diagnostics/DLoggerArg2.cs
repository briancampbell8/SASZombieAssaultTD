// ====================================================================================================
//  FILE: DLoggerArg2.cs
//  PATH: ./Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  ROLE:
//      Provide logging, profiling, or diagnostic instrumentation.
//
//  RESPONSIBILITIES:
//      - Provide Log() behavior for the Diagnostics subsystem.
//      - Provide Log() behavior for the Diagnostics subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
// ============================================================================
//  FILE: DLoggerArg2.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Defines logging patterns for the DLogger diagnostics system.
//      This file contains ONLY overloads for the specific argument count
//      or category it represents (Arg1, Arg2, Legacy, Custom, etc).
//
//  FEATURES:
//      - Implements all two‑argument logging overloads for the diagnostics system.
//      - Provides modern prefix‑based output for 2‑parameter log patterns.
//      - Introduces explicit subsystem routing, allowing callers to specify context.
//      - Supports structured logging via (subsystem, message) and (subsystem, exception).
//      - Emits unique PatternTags for Arg2 overloads to ensure deterministic routing.
//      - Guarantees strict isolation: only 2‑argument overloads are defined in this module.
//      - Produces fully normalized, Manager‑ready output with no legacy formatting.
//      - Optimized for mid‑level logging where subsystem context is required but full
//        category/level metadata is not.
//
//  LOCATION: Diagnostics/Patterns/
//  VERSION: 1.0 (Pattern Module)
// ============================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static partial class DLogger
    {
        // ====================================================================
        // ARG2 OVERLOADS (2 PARAMETERS)
        // ====================================================================

        /// <summary>
        /// Logs a message with an explicitly defined subsystem.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(string category, LogSubsystems subsystem, string message)
        {
            ResolveAndLog(
                subsystem,
                $"[INFO][General] {message}",
                "Arg2_SubsystemMessage",
                2
            );
        }

        /// <summary>
        /// Logs an exception with an explicitly defined subsystem.
        /// </summary>
        /// <param name="subsystem">The subsystem producing the log.</param>
        /// <param name="exception">The exception instance to log.</param>
        public static void Log(LogSubsystems subsystem, Exception exception)
        {
            string msg = exception?.Message ?? "(null exception)";

            ResolveAndLog(
                subsystem,
                $"[ERROR][Exception] {msg}",
                "Arg2_SubsystemException",
                2
            );
        }
    }
}

