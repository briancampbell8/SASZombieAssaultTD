// ====================================================================================================
//  FILE: AnimationTransitionRecord.cs
//  PATH: ./Engine/Animation/Core/AnimationTransitionRecord.cs
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationTypes module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Core.Transitions
{
    public class AnimationTransitionRecord
    {
        public int EntityId { get; set; }
        public string FromState { get; set; } = string.Empty;
        public string ToState { get; set; } = string.Empty;
        public float Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

