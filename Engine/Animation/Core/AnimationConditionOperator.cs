// File: Engine/Animation/AnimationConditionOperator.cs
// Purpose: Defines comparison operators for animation parameter conditions
// Integration: Used by AnimationParameters.ParameterCondition for state transitions

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    /// <summary>
    /// Animation condition operators for parameter comparisons.
    /// Used in animation state machines and blend trees to determine transitions.
    /// Supports numeric, boolean, and string parameter types.
    /// </summary>
    public enum AnimationConditionOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        WithinRange,
        OutsideRange
    }
}
