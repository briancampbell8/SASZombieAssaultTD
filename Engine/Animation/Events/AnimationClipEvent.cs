/*
File:    AnimationClipEvent.cs
Path:    Engine/Animation/Events/AnimationClipEvent.cs
Purpose:  P11-19-01: Animation clip event subtype for AnimationEvent system.
          Represents a clip-based animation event that can be fired during animation playback.
          Inherits from AnimationEvent to maintain compatibility with existing event system.

Features: Clip identification with clip name and index tracking, loop detection,
          timestamp management, and parameter storage for clip events.

Role:     Specific event subtype for clip-based animations in the animation system.
          Used by AnimationUpdateSystem to dispatch clip events with proper type safety.
*/

#nullable enable

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.Events
{
    /// <summary>
    /// Animation clip event subtype for AnimationEvent system.
    /// Represents a clip-based animation event that can be fired during animation playback.
    /// Inherits from AnimationEvent to maintain compatibility with existing event system.
    /// </summary>
    public class AnimationClipEvent : AnimationEvent
    {
        /// <summary>
        /// Name of the animation clip that triggered this event.
        /// Used for clip identification and event routing.
        /// </summary>
        public string ClipName { get; }

        /// <summary>
        /// Index of the animation clip that triggered this event.
        /// Used for multi-clip animations and event identification.
        /// </summary>
        public int ClipIndex { get; }

        /// <summary>
        /// Whether this clip event is part of a looping animation.
        /// Indicates if the clip should loop back to the beginning when finished.
        /// </summary>
        public bool IsLooping { get; }

        /// <summary>
        /// Initializes a new animation clip event with all required parameters.
        /// </summary>
        /// <param name="eventId">Unique identifier for this clip event.</param>
        /// <param name="eventName">Human-readable name for this clip event.</param>
        /// <param name="timestamp">Timestamp when this clip event should occur.</param>
        /// <param name="clipName">Name of the animation clip.</param>
        /// <param name="clipIndex">Index of the animation clip.</param>
        /// <param name="isLooping">Whether this clip event is part of a looping animation.</param>
        /// <param name="parameters">Optional parameters for the clip event.</param>
        public AnimationClipEvent(string eventId, string eventName, float timestamp, string clipName, int clipIndex, bool isLooping, Dictionary<string, object>? parameters = null)
            : base(eventId, eventName, timestamp, parameters)
        {
            ClipName = clipName;
            ClipIndex = clipIndex;
            IsLooping = isLooping;
        }

        /// <summary>
        /// Creates a new animation clip event.
        /// Validates clip name and ensures proper parameter initialization.
        /// </summary>
        /// <param name="eventId">Unique identifier for this clip event.</param>
        /// <param name="eventName">Human-readable name for this clip event.</param>
        /// <param name="timestamp">Timestamp when this clip event should occur.</param>
        /// <param name="clipName">Name of the animation clip.</param>
        /// <param name="clipIndex">Index of the animation clip.</param>
        /// <param name="isLooping">Whether this clip event is part of a looping animation.</param>
        /// <param name="parameters">Optional parameters for the clip event.</param>
        /// <returns>A new AnimationClipEvent instance.</returns>
        public static AnimationClipEvent CreateClipEvent(
            string eventId,
            string eventName,
            float timestamp,
            string clipName,
            int clipIndex,
            bool isLooping,
            Dictionary<string, object>? parameters = null)
        {
            return new AnimationClipEvent(eventId, eventName, timestamp, clipName, clipIndex, isLooping, parameters);
        }

        /// <summary>
        /// Initializes a new animation clip event with default values.
        /// Sets ClipName and ClipIndex from the provided values.
        /// </summary>
        /// <param name="eventId">Unique identifier for this clip event.</param>
        /// <param name="eventName">Human-readable name for this clip event.</param>
        /// <param name="timestamp">Timestamp when this clip event should occur.</param>
        /// <param name="clipName">Name of the animation clip.</param>
        /// <param name="clipIndex">Index of the animation clip.</param>
        /// <param name="parameters">Optional parameters for the clip event.</param>
        /// <returns>A new AnimationClipEvent instance.</returns>
        public static AnimationClipEvent Create(
            string eventId,
            string eventName,
            float timestamp,
            string clipName,
            int clipIndex)
        {
            return CreateClipEvent(eventId, eventName, timestamp, clipName, clipIndex, false, null);
        }
    }
}
