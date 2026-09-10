// =====================================================================================================
//  FILE: AnimationBlendWeightInfo.cs
//  PATH: Engine/Animation/State/AnimationBlendWeightInfo.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Represents analyzed blend-weight data.
//
//  RESPONSIBILITIES:
//      - Store blend-weight dictionary.
//      - Store active blends.
//      - Store min/max values.
//      - Store total weight.
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
    public class AnimationBlendWeightInfo
    {
        public int EntityId { get; set; }
        public Dictionary<string, float> BlendWeights { get; set; }
        public int BlendCount { get; set; }
        public List<KeyValuePair<string, float>> ActiveBlends { get; set; }
        public float MaxBlendWeight { get; set; }
        public float MinBlendWeight { get; set; }
        public float TotalBlendWeight { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
