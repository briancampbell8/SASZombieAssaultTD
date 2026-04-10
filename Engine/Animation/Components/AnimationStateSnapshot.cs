// ROLE: Immutable snapshot of animation state.
// RESPONSIBILITY: Capture animation state for a specific entity at a point in time.
// TRIGGERS: Instantiated by AnimationStateInspector during state changes.
// INPUTS: Receives entity ID, state info, and timestamp from inspector.
// OUTPUTS: Provides immutable state record for debugging and replay.
// DEPENDENCIES: Uses AnimationStateInfo for state data.
// CONTENTS: AnimationStateSnapshot class with EntityId, StateInfo, Timestamp properties.

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
