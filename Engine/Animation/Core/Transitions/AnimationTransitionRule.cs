// =====================================================================================================
//  FILE: AnimationTransitionRule.cs
//  PATH: Engine/Animation/Core/Transitions/AnimationTransitionRule.cs
//  SUBSYSTEM: Animation Core Transitions
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation.Core.Transitions
{
    internal sealed class AnimationTransitionRule
    {
        public string ParameterName { get; }
        public float Threshold { get; }
        public bool IsGreaterComparison { get; }

        public AnimationTransitionRule(string parameterName, float threshold, bool isGreaterComparison = true)
        {
            ParameterName = parameterName;
            Threshold = threshold;
            IsGreaterComparison = isGreaterComparison;
        }

        public bool Evaluate(AnimationParameterStore parameters)
        {
            if (parameters == null || string.IsNullOrEmpty(ParameterName))
                return false;

            float value = parameters.GetValue(ParameterName);

            return IsGreaterComparison
                ? value >= Threshold
                : value <= Threshold;
        }
    }
}
