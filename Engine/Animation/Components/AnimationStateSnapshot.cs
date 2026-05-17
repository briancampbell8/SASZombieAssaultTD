using System;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// Represents a snapshot of an animation state for an entity.
    /// </summary>
    public class AnimationStateSnapshot
    {
        public int EntityId { get; set; }
        public AnimationStateInfo StateInfo { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
