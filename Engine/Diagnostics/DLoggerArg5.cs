using System;

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
