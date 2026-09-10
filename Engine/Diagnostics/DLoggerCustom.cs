// ====================================================================================================
//  FILE: DLoggerCustom.cs
//  PATH: ./Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  ROLE:
//      Provide logging, profiling, or diagnostic instrumentation.
//
//  RESPONSIBILITIES:
//      - Provide LogCustom() behavior for the Diagnostics subsystem.
//      - Provide LogCustom() behavior for the Diagnostics subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

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

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

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

        /// <summary>
        /// Core render‑aware logging overload used by the rendering pipeline. Executes optional render callbacks and
        /// logs the message safely.
        /// </summary>
        internal static void Log(string format, Action<object> render, LogEnums.LogLevel level, string message)
        {
            // Execute optional render callback safely
            if (render != null)
            {
                try
                {
                    render(message);
                }
                catch (Exception ex)
                {
                    ResolveAndLog(
                        LogSubsystems.General,
                        $"[RenderCallbackError] {ex.Message}",
                        "Render_Callback_Error",
                        2
                    );
                }
            }

            // Log the message through the normal diagnostics pipeline
            ResolveAndLog(
                LogSubsystems.General,
                message,
                "Render_Log_Message",
                1
            );
        }

        /// <summary>
        /// Logs a custom message with a caller-defined diagnostic tag.
        /// </summary>
        internal static void LogCustom(object info, string message)
        {
            ResolveAndLog(
                LogSubsystems.General,
                message,
                "Internal_Log_Message",
                1
            );
        }
    }
}
