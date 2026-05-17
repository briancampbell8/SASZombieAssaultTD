using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    public class AnimationStateInfo
    {
        public int EntityId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public string PreviousState { get; set; } = string.Empty;
        public float TransitionProgress { get; set; }
        public float AnimationTime { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new();
        public Dictionary<string, float> BlendWeights { get; set; } = new();
        public bool IsTransitioning { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AnimationTransitionRecord
    {
        public int EntityId { get; set; }
        public string FromState { get; set; } = string.Empty;
        public string ToState { get; set; } = string.Empty;
        public float Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }

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
