// ====================================================================================================
//  FILE: DLoggerArg7.cs
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
//  FILE: DLoggerArg7.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Defines logging patterns for the DLogger diagnostics system.
//      This file contains ONLY overloads for the specific argument count
//      or category it represents (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7,
//      Legacy, Custom, etc).
//
//  FEATURES:
//      - Implements all seven‑argument logging overloads for the diagnostics system.
//      - Provides modern prefix‑based output for 7‑parameter event‑aware log patterns.
//      - Introduces explicit event routing, enabling detailed lifecycle and pipeline tagging.
//      - Supports enriched logging via
//        (subsystem, category, level, severity, context, eventName, message)
//        and
//        (subsystem, category, level, severity, context, eventName, exception).
//      - Emits unique PatternTags for Arg7 overloads to ensure deterministic routing.
//      - Guarantees strict isolation: only 7‑argument overloads are defined in this module.
//      - Produces fully normalized, Manager‑ready output with no legacy formatting.
//      - Optimized for diagnostics requiring full metadata stacks such as render passes,
//        resource lifecycle events, AI state transitions, and system‑level operations.
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
        // ARG7 OVERLOADS (7 PARAMETERS)
        // ====================================================================

        /// <summary>
        /// Logs a message with explicit subsystem, category, level, severity, context, and event routing.
        /// </summary>
        public static void Log(
            LogSubsystems subsystem,
            LogCategory category,
            LogEnums.LogLevel level,
            LogSeverity severity,
            string context,
            string eventName,
            string message)
        {
            ResolveAndLog(
                subsystem,
                $"[{level}][{category}][{severity}][{context}][{eventName}] {message}",
                "Arg7_SubsystemCategoryLevelSeverityContextEventMessage",
                7
            );
        }

        /// <summary>
        /// Logs an exception with explicit subsystem, category, level, severity, context, and event routing.
        /// </summary>
        public static void Log(
            LogSubsystems subsystem,
            LogCategory category,
            LogEnums.LogLevel level,
            LogSeverity severity,
            string context,
            string eventName,
            Exception exception)
        {
            string msg = exception?.Message ?? "(null exception)";

            ResolveAndLog(
                subsystem,
                $"[{level}][{category}][{severity}][{context}][{eventName}] {msg}",
                "Arg7_SubsystemCategoryLevelSeverityContextEventException",
                7
            );
        }
    }
}
