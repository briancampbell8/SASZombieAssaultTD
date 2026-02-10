using System;
using System.Diagnostics;
using System.Reflection;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
namespace SASZombieAssaultTD.Engine.Systems.Diagnostics
{
    /// <summary>
    /// Tracks heartbeat ticks for the engine loop, providing simple timing and count diagnostics.
    /// </summary>
    public sealed class HeartbeatMonitor
    {
        /// <summary>
        /// Total number of heartbeat ticks observed since startup.
        /// </summary>
        public long TickCount { get; private set; }

        /// <summary>
        /// The time (UTC) when the first heartbeat was recorded.
        /// </summary>
        public DateTime? FirstTickUtc { get; private set; }

        /// <summary>
        /// The time (UTC) when the most recent heartbeat was recorded.
        /// </summary>
        public DateTime? LastTickUtc { get; private set; }

        /// <summary>
        /// The duration between the last two ticks, if at least two ticks have occurred.
        /// </summary>
        public TimeSpan? LastDelta { get; private set; }

        /// <summary>
        /// Records a heartbeat tick at the current UTC time.
        /// </summary>
        public void Tick()
        {
            var now = DateTime.UtcNow;

            if (FirstTickUtc is null)
            {
                FirstTickUtc = now;
            }

            if (LastTickUtc is not null)
            {
                LastDelta = now - LastTickUtc.Value;
            }

            LastTickUtc = now;
            TickCount++;
        }

        /// <summary>
        /// Resets all heartbeat counters and timestamps.
        /// </summary>
        public void Reset()
        {
            TickCount = 0;
            FirstTickUtc = null;
            LastTickUtc = null;
            LastDelta = null;
        }
    }
}