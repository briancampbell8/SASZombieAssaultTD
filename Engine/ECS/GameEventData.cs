using System;

namespace SASZombieAssaultTD.Engine.ECS
{
    // Lightweight game event payload used by animation and other systems.
    public sealed class GameEventData
    {
        public uint EntityId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public float Timestamp { get; set; }
        public object? Payload { get; set; }
    }
}
