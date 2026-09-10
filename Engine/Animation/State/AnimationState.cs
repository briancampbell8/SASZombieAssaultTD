// =====================================================================================================
//  FILE: AnimationState.cs
//  PATH: Engine/Animation/State/AnimationState.cs
//  SUBSYSTEM: Animation State
//
//  ROLE:
//      Pure deterministic data container representing a single animation state snapshot. Stores
//      state-machine values, transition progress, animation time, parameters, blend weights, and
//      timestamp for inspection and analysis.
//
//  RESPONSIBILITIES:
//      - Hold current/previous state names.
//      - Hold transition progress and animation time.
//      - Hold parameter and blend-weight dictionaries.
//      - Hold timestamp and ECSEntityCore identifier.
//      - Remain immutable except for direct field assignment.
//
//  NON-RESPONSIBILITIES:
//      - ECS queries or animation controller access.
//      - Transition history management.
//      - Parameter or blend-weight analysis.
//      - Inspector lifecycle or reporting.
//      - Logging or diagnostics.
//
//  ARCHITECTURAL NOTES:
//      - Part of the Animation State subsystem.
//      - Used by AnimationStateRetriever, AnimationStateInspector, and related modules.
//      - Must remain a pure data model with no side effects.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.State
{
    public class AnimationState
    {
        public int EntityId { get; set; }
        public string CurrentState { get; set; }
        public string PreviousState { get; set; }
        public float TransitionProgress { get; set; }
        public float AnimationTime { get; set; }
        public Dictionary<string, float> Parameters { get; set; }
        public Dictionary<string, float> BlendWeights { get; set; }
        public bool IsTransitioning { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
