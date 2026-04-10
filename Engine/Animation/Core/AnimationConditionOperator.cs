// FILE PATH: Engine/Animation/Core/AnimationConditionOperator.cs
// EXECUTION TRIGGER: Referenced by AnimationTransition and AnimationStateMachine during condition evaluation
// PROGRAM PURPOSE: Enumeration of comparison operators for animation parameter conditions used to evaluate transition conditions and blend tree parameters
// PROGRAM CALLS: None (enum definition)
// PROGRAM CONTENTS: AnimationConditionOperator enum with Equals, NotEquals, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, WithinRange, OutsideRange values

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
