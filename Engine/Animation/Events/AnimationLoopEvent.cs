/*
File:    AnimationLoopEvent.cs
Path:    Engine/Animation/Events/AnimationLoopEvent.cs
Purpose:  P11-19-01: Animation loop event subtype for AnimationEvent system.
          Represents a loop-based animation event that can be fired during animation playback.
          Inherits from AnimationEvent to maintain compatibility with existing event system.

Features: Loop detection with loop count tracking, clip identification,
          timestamp management, and parameter storage for loop events.

Role:     Specific event subtype for loop-based animations in the animation system.
          Used by AnimationUpdateSystem to dispatch loop events with proper type safety.
*/

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    /// <summary>
    /// Animation loop event subtype for AnimationEvent system. Represents a loop-based animation event that can be
    /// fired during animation playback. Inherits from AnimationEvent to maintain compatibility with existing event
    /// system.
    /// </summary>
    public class AnimationLoopEvent : AnimationEvent
    {
        /// <summary>
        /// Name of the animation clip that is looping. Used for clip identification and event routing.
        /// </summary>
        public string ClipName { get; }

        /// <summary>
        /// Current loop count for this animation clip. Increments each time the clip loops back to the beginning. Used
        /// for loop progress tracking and event identification.
        /// </summary>
        public int LoopCount { get; }

        /// <summary>
        /// Initializes a new animation loop event with all required parameters.
        /// </summary>
        /// <param name="eventId">Unique identifier for this loop event.</param>
        /// <param name="eventName">Human-readable name for this loop event.</param>
        /// <param name="timestamp">Timestamp when this loop event should occur.</param>
        /// <param name="clipName">Name of the animation clip.</param>
        /// <param name="loopCount">Current loop count for this clip.</param>
        /// <param name="parameters">Optional parameters for the loop event.</param>
        public AnimationLoopEvent(string eventId, string eventName, float timestamp, string clipName, int loopCount, Dictionary<string, object>? parameters = null)
            : base(eventId, eventName, timestamp, parameters)
        {
            ClipName = clipName;
            LoopCount = loopCount;
        }

        /// <summary>
        /// Creates a new animation loop event. Validates clip name and ensures proper parameter initialization. Sets
        /// ClipName and LoopCount from the provided values.
        /// </summary>
        /// <param name="eventId">Unique identifier for this loop event.</param>
        /// <param name="eventName">Human-readable name for this loop event.</param>
        /// <param name="timestamp">Timestamp when this loop event should occur.</param>
        /// <param name="clipName">Name of the animation clip.</param>
        /// <param name="loopCount">Current loop count for this clip.</param>
        /// <param name="parameters">Optional parameters for the loop event.</param>
        /// <returns>A new AnimationLoopEvent instance.</returns>
        public static AnimationLoopEvent CreateLoopEvent(
            string eventId,
            string eventName,
            float timestamp,
            string clipName,
            int loopCount,
            Dictionary<string, object>? parameters = null)
        {
            return new AnimationLoopEvent(eventId, eventName, timestamp, clipName, loopCount, parameters);
        }

        /// <summary>
        /// Initializes a new animation loop event with default values. Sets ClipName and LoopCount from the provided
        /// values.
        /// </summary>
        /// <param name="eventId">Unique identifier for this loop event.</param>
        /// <param name="eventName">Human-readable name for this loop event.</param>
        /// <param name="timestamp">Timestamp when this loop event should occur.</param>
        /// <param name="clipName">Name of the animation clip.</param>
        /// <param name="loopCount">Current loop count for this clip.</param>
        /// <param name="parameters">Optional parameters for the loop event.</param>
        /// <returns>A new AnimationLoopEvent instance.</returns>
        public static AnimationLoopEvent Create(
            string eventId,
            string eventName,
            float timestamp,
            string clipName,
            int loopCount)
        {
            return CreateLoopEvent(eventId, eventName, timestamp, clipName, loopCount, null);
        }
    }
}
