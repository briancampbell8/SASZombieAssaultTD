/*
File:    GetTransitionProgress.cs
Purpose: Returns animation progress for UI transitions (fade, slide, expand).
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Returns animation progress for UI transitions (fade, slide, expand).
    /// </summary>
    public static class GetTransitionProgress
    {
        /// <summary>
        /// Gets linear transition progress.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <returns>Progress value from 0.0 to 1.0.</returns>
        public static float Linear(float elapsedTime, float duration)
        {
            if (duration <= 0) return 1.0f;
            return global::System.Math.Clamp(elapsedTime / duration, 0.0f, 1.0f);
        }
        
        /// <summary>
        /// Gets ease-in transition progress.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <returns>Progress value from 0.0 to 1.0.</returns>
        public static float EaseIn(float elapsedTime, float duration)
        {
            float t = Linear(elapsedTime, duration);
            return t * t;
        }
        
        /// <summary>
        /// Gets ease-out transition progress.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <returns>Progress value from 0.0 to 1.0.</returns>
        public static float EaseOut(float elapsedTime, float duration)
        {
            float t = Linear(elapsedTime, duration);
            return t * (2.0f - t);
        }
        
        /// <summary>
        /// Gets ease-in-out transition progress.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <returns>Progress value from 0.0 to 1.0.</returns>
        public static float EaseInOut(float elapsedTime, float duration)
        {
            float t = Linear(elapsedTime, duration);
            return t < 0.5f ? 2.0f * t * t : -1.0f + (4.0f - 2.0f * t) * t;
        }
        
        /// <summary>
        /// Gets bounce transition progress.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <returns>Progress value from 0.0 to 1.0.</returns>
        public static float Bounce(float elapsedTime, float duration)
        {
            float t = Linear(elapsedTime, duration);
            if (t < 0.5f)
            {
                return 8.0f * t * t * t * t;
            }
            else
            {
                float f = (t - 1.0f);
                return 1.0f - 8.0f * f * f * f * f;
            }
        }
        
        /// <summary>
        /// Gets elastic transition progress.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <returns>Progress value from 0.0 to 1.0.</returns>
        public static float Elastic(float elapsedTime, float duration)
        {
            float t = Linear(elapsedTime, duration);
            if (t == 0.0f || t == 1.0f) return t;
            
            float p = duration * 0.3f;
            float s = p / 4.0f;
            return -1.0f * (float)global::System.Math.Pow(2.0, -10.0 * t) * (float)global::System.Math.Sin((t - s) * (2.0 * global::System.Math.PI) / p);
        }
        
        /// <summary>
        /// Gets custom transition progress using a specific easing function.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <param name="easingType">Type of easing to apply.</param>
        /// <returns>Progress value from 0.0 to 1.0.</returns>
        public static float GetProgress(float elapsedTime, float duration, string easingType = "linear")
        {
            return easingType.ToLower() switch
            {
                "linear" => Linear(elapsedTime, duration),
                "easein" => EaseIn(elapsedTime, duration),
                "easeout" => EaseOut(elapsedTime, duration),
                "easeinout" => EaseInOut(elapsedTime, duration),
                "bounce" => Bounce(elapsedTime, duration),
                "elastic" => Elastic(elapsedTime, duration),
                _ => Linear(elapsedTime, duration)
            };
        }
        
        /// <summary>
        /// Checks if a transition is complete.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since transition start.</param>
        /// <param name="duration">Total duration of transition.</param>
        /// <returns>True if complete, false otherwise.</returns>
        public static bool IsComplete(float elapsedTime, float duration)
        {
            return elapsedTime >= duration;
        }
    }
}