// =====================================================================================================
//  FILE: AnimationTransitionRecord.cs
//  PATH: Engine/Animation/State/AnimationTransitionRecord.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Represents a single animation transition event.
//
//  RESPONSIBILITIES:
//      - Store from/to state.
//      - Store duration.
//      - Store timestamp.
//
//  NON-RESPONSIBILITIES:
//      - Analysis.
//      - Inspector lifecycle.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.State
{
    public class AnimationTransitionRecord
    {
        public int EntityId { get; set; }
        public string FromState { get; set; }
        public string ToState { get; set; }
        public float Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
