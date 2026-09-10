// ====================================================================================================
//  FILE: AnimationBlendWeightInfo.cs
//  PATH: ./Engine/Animation/Core/AnimationBlendWeightInfo.cs
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
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    public class AnimationBlendWeightInfo
    {
        public int EntityId { get; set; }
        public Dictionary<string, float> BlendWeights { get; set; } = new();
        public int BlendCount { get; set; }
        public List<KeyValuePair<string, float>> ActiveBlends { get; set; } = new();
        public float MaxBlendWeight { get; set; }
        public float MinBlendWeight { get; set; }
        public float TotalBlendWeight { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

