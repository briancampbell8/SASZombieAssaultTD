// ====================================================================================================
//  FILE: AnimationParameterInfo.cs
//  PATH: ./Engine/Animation/Core/AnimationParameterInfo.cs
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
    public class AnimationParameterInfo
    {
        public int EntityId { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new();
        public int ParameterCount { get; set; }
        public List<KeyValuePair<string, float>> ActiveParameters { get; set; } = new();
        public float MaxParameterValue { get; set; }
        public float MinParameterValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

