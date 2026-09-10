// =====================================================================================================
//  FILE: AnimationStateSnapshot.cs
//  PATH: Engine/Animation/State/AnimationStateSnapshot.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Represents a stored snapshot of an animation state at a specific moment.
//
//  RESPONSIBILITIES:
//      - Store ECSEntityCore ID.
//      - Store AnimationState.
//      - Store timestamp.
//
//  NON-RESPONSIBILITIES:
//      - Analysis.
//      - Inspector lifecycle.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.State
{
    public class AnimationStateSnapshot
    {
        public int EntityId { get; set; }
        public AnimationState StateInfo { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
