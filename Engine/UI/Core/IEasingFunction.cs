/*
File:    IEasingFunction.cs
Purpose: Easing function interface for UI animations in SAS Zombie Assault TD.
Features: Smooth interpolation curves for UI transitions and animations.
Standards: XML documentation with detailed parameter descriptions and usage examples.
Integration: Core UI animation system for smooth transitions.
Performance: Optimized for frequent animation frame calculations.
*/

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Interface for easing functions used in UI animations.
    /// Provides smooth interpolation curves for UI transitions.
    /// This interface defines the contract for animation easing functions
    /// that control the rate of change in UI animations.
    /// </summary>
    /// <remarks>
    /// Easing functions are mathematical functions that define the rate of change
    /// of a parameter over time. They are commonly used in animations to create
    /// more natural and visually appealing transitions between UI states.
    /// 
    /// Common Easing Types:
    /// - Linear: Constant rate of change (no easing)
    /// - Ease-In: Slow start, accelerates towards end
    /// - Ease-Out: Fast start, decelerates towards end
    /// - Ease-In-Out: Slow start and end, fast middle
    /// - Bounce: Elastic bouncing effect
    /// - Elastic: Spring-like oscillation
    /// 
    /// Mathematical Foundation:
    /// Easing functions typically accept a parameter 't' representing the
    /// progress of the animation (0.0 = start, 1.0 = end) and return a
    /// modified value that creates the desired easing effect.
    /// 
    /// Usage Pattern:
    /// Easing functions are used by animation systems to interpolate
    /// between start and end values for properties like position, scale,
    /// rotation, and opacity during UI transitions.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example easing function implementation
    /// public class QuadraticEaseIn : IEasingFunction
    /// {
    ///     public float Ease(float t)
    ///     {
    ///         return t * t; // Quadratic ease-in
    ///     }
    /// }
    /// 
    /// // Usage in animation system
    /// public class UIAnimation
    /// {
    ///     private readonly IEasingFunction _easing;
    ///     
    ///     public float GetValue(float progress)
    ///     {
    ///         var easedProgress = _easing.Ease(progress);
    ///         return StartValue + (EndValue - StartValue) * easedProgress;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IEasingFunction
    {
        /// <summary>
        /// Calculates the eased value for the given progress.
        /// This method applies the easing function to transform linear progress
        /// into a curved progression that creates smooth animation effects.
        /// </summary>
        /// <param name="t">Progress value between 0.0 and 1.0 (inclusive)</param>
        /// <returns>Eased value, typically also between 0.0 and 1.0</returns>
        /// <remarks>
        /// The input parameter 't' represents the linear progress of an animation
        /// from start (0.0) to completion (1.0). The easing function transforms
        /// this linear progress into a non-linear progression that creates the
        /// desired visual effect.
        /// 
        /// Input Constraints:
        /// - Values below 0.0 should be clamped to 0.0 or extrapolated
        /// - Values above 1.0 should be clamped to 1.0 or extrapolated
        /// - Typical implementations handle edge cases gracefully
        /// 
        /// Output Characteristics:
        /// - Most easing functions return values in the [0.0, 1.0] range
        /// - Some easing functions (like elastic) may overshoot slightly
        /// - Output should be monotonic (non-decreasing) for standard easing
        /// 
        /// Performance Considerations:
        /// - This method is called frequently during animation frames
        /// - Implementations should be computationally inexpensive
        /// - Consider caching complex calculations if possible
        /// - Avoid memory allocations in the easing calculation
        /// </remarks>
        /// <example>
        /// <code>
        /// // Linear easing (no transformation)
        /// public float Ease(float t) => t;
        /// 
        /// // Quadratic ease-in
        /// public float Ease(float t) => t * t;
        /// 
        /// // Quadratic ease-out
        /// public float Ease(float t) => 1.0f - (1.0f - t) * (1.0f - t);
        /// 
        /// // Usage in animation loop
        /// for (float t = 0.0f; t <= 1.0f; t += 0.016f) // 60 FPS
        /// {
        ///     var easedT = easingFunction.Ease(t);
        ///     var currentValue = start + (end - start) * easedT;
        ///     UpdateUIElement(currentValue);
        /// }
        /// </code>
        /// </example>
        float Ease(float t);
    }
}
