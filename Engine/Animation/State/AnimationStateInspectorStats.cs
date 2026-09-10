// =====================================================================================================
//  FILE: AnimationStateInspectorStats.cs
//  PATH: Engine/Animation/State/AnimationStateInspectorStats.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Represents inspector statistics.
//
//  RESPONSIBILITIES:
//      - Store enabled state.
//      - Store start time.
//      - Store ECSEntityCore count.
//      - Store transition counts.
//      - Store runtime.
//
//  NON-RESPONSIBILITIES:
//      - Inspector lifecycle.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.State
{
    public class AnimationStateInspectorStats
    {
        public bool IsEnabled { get; set; }
        public DateTime StartTime { get; set; }
        public int InspectedEntityCount { get; set; }
        public int TotalTransitionRecords { get; set; }
        public float AverageTransitionsPerEntity { get; set; }
        public float TotalRunTime { get; set; }
    }
}
