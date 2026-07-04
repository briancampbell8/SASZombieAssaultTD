// ============================================================================
//  FILE: DLoggerArg6.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Defines logging patterns for the DLogger diagnostics system.
//      This file contains ONLY overloads for the specific argument count
//      or category it represents (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Legacy, Custom, etc).
//
//  FEATURES:
//      - Implements all six‑argument logging overloads for the diagnostics system.
//      - Provides modern prefix‑based output for 6‑parameter context‑aware log patterns.
//      - Introduces explicit context routing, enabling environmental or situational tagging.
//      - Supports enriched logging via (subsystem, category, level, severity, context, message)
//        and (subsystem, category, level, severity, context, exception).
//      - Emits unique PatternTags for Arg6 overloads to ensure deterministic routing.
//      - Guarantees strict isolation: only 6‑argument overloads are defined in this module.
//      - Produces fully normalized, Manager‑ready output with no legacy formatting.
//      - Optimized for diagnostics requiring contextual metadata such as scene names,
//        render passes, AI states, loading phases, or user actions.
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
        // ARG6 OVERLOADS (6 PARAMETERS)
        // ====================================================================

        public static void Log(
            LogSubsystems subsystem,
            LogCategory category,
            LogLevel level,
            LogSeverity severity,
            string context,
            string message)
        {
            ResolveAndLog(
                subsystem,
                $"[{level}][{category}][{severity}][{context}] {message}",
                "Arg6_SubsystemCategoryLevelSeverityContextMessage",
                6
            );
        }

        public static void Log(
            LogSubsystems subsystem,
            LogCategory category,
            LogLevel level,
            LogSeverity severity,
            string context,
            Exception exception)
        {
            string msg = exception?.Message ?? "(null exception)";

            ResolveAndLog(
                subsystem,
                $"[{level}][{category}][{severity}][{context}] {msg}",
                "Arg6_SubsystemCategoryLevelSeverityContextException",
                6
            );
        }
    }

    public class LogSeverity
    {
        public const string Information = "INFO";
        public const string Warning = "WARN";
        public const string Error = "ERROR";
        public const string Fatal = "FATAL";
        public const string Critical = "CRITICAL";
        public const string Debug = "DEBUG";
        public const string Trace = "TRACE";
        public const string Verbose = "VERBOSE";
        public const string Recovery = "RECOVERY";
        public const string Recovered = "RECOVERED";
        public const string Exception = "EXCEPTION";
        public const string Unknown = "UNKNOWN";
        public const string Legacy = "LEGACY";
    }
}
