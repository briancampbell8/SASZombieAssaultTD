// =====================================================================================================
//  FILE: AnimationParameterInfo.cs
//  PATH: Engine/Animation/State/AnimationParameterInfo.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Represents analyzed animation parameter data.
//
//  RESPONSIBILITIES:
//      - Store parameter dictionary.
//      - Store active parameters.
//      - Store min/max values.
//      - Store timestamp.
//
//  NON-RESPONSIBILITIES:
//      - ECS queries.
//      - Inspector lifecycle.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.State
{
    public class AnimationParameterInfo
    {
        public int EntityId { get; set; }
        public Dictionary<string, float> Parameters { get; set; }
        public int ParameterCount { get; set; }
        public List<KeyValuePair<string, float>> ActiveParameters { get; set; }
        public float MaxParameterValue { get; set; }
        public float MinParameterValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
