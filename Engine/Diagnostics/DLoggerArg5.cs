// ====================================================================================================
//  FILE: DLoggerArg5.cs
//  PATH: ./Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  ROLE:
//      Provide logging, profiling, or diagnostic instrumentation.
//
//  RESPONSIBILITIES:
//      - Provide Log() behavior for the Diagnostics subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static partial class DLogger
    {
        public static void Log(
            string subsystem,
            string category,
            string message,
            string eventId,
            int severity)
        {
            // Build synthetic raw message
            string raw = $"[{category}][{severity}] {message} (EventId: {eventId})";

            // Convert subsystem string → enum
            LogSubsystems ss = Enum.TryParse<LogSubsystems>(subsystem, true, out var parsed)
                ? parsed
                : LogSubsystems.Engine;

            // Pattern tag for diagnostics
            string patternTag = "Arg5";

            // Forward to the unified resolver
            ResolveAndLog(
                ss,
                raw,
                patternTag,
                overloadArgs: 5
            );
        }
    }
}

