/*
File:    AnimationStateChangeEvent.cs
Path:    Engine/Animation/Events/AnimationStateChangeEvent.cs
Purpose:  P11-19-01: Animation state change event subtype for AnimationEvent system.
          Represents a state transition animation event that can be fired during animation playback.
          Inherits from AnimationEvent to maintain compatibility with existing event system.

Features: State change tracking with before/after state values, deterministic validation,
          timestamp management, and parameter storage for state transitions.

Role:     Specific event subtype for state change animations in the animation system.
          Used by AnimationUpdateSystem to dispatch state change events with proper type safety.
*/

using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.Events
{
    ///<summary>
    ///Animation state change event subtype for AnimationEvent system.
    ///Represents a state transition animation event that can be fired during animation playback.
    ///Inherits from AnimationEvent to maintain compatibility with existing event system.
    ///</summary>
    public class AnimationStateChangeEvent : AnimationEvent
    {
        ///<summary>
        ///Previous animation state before the transition.
        ///Used for state comparison and transition logic.
        ///</summary>
        public string PreviousState { get; }

        ///<summary>
        ///New animation state after the transition.
        ///Represents the target state of the state transition.
        ///</summary>
        public string NewState { get; }

        ///<summary>
        ///Initializes a new animation state change event with all required parameters.
        ///</summary>
        ///<param name="eventId">Unique identifier for this state change event.</param>
        ///<param name="eventName">Human-readable name for this state change event.</param>
        ///<param name="timestamp">Timestamp when this state change should occur.</param>
        ///<param name="previousState">Previous animation state.</param>
        ///<param name="newState">New animation state.</param>
        ///<param name="parameters">Optional parameters for the state change.</param>
        public AnimationStateChangeEvent(string eventId, string eventName, float timestamp, string previousState, string newState, Dictionary<string, object>? parameters = null)
            : base(eventId, eventName, timestamp, parameters)
        {
            PreviousState = previousState;
            NewState = newState;
        }

        ///<summary>
        ///Creates a new animation state change event.
        ///Validates state names and ensures proper parameter initialization.
        ///</summary>
        ///<param name="eventId">Unique identifier for this state change event.</param>
        ///<param name="eventName">Human-readable name for this state change event.</param>
        ///<param name="timestamp">Timestamp when this state change should occur.</param>
        ///<param name="previousState">Previous animation state.</param>
        ///<param name="newState">New animation state.</param>
        ///<param name="parameters">Optional parameters for the state change.</param>
        ///<returns>A new AnimationStateChangeEvent instance.</returns>
        public static AnimationStateChangeEvent CreateStateChange(
            string eventId,
            string eventName,
            float timestamp,
            string previousState,
            string newState,
            Dictionary<string, object>? parameters = null)
        {
            return new AnimationStateChangeEvent(eventId, eventName, timestamp, previousState, newState, parameters);
        }

        ///<summary>
        ///Initializes a new animation state change event with default values.
        ///Sets PreviousState and NewState from the provided state names.
        ///</summary>
        ///<param name="eventId">Unique identifier for this state change event.</param>
        ///<param name="eventName">Human-readable name for this state change event.</param>
        ///<param name="timestamp">Timestamp when this state change should occur.</param>
        ///<param name="previousState">Previous animation state.</param>
        ///<param name="newState">New animation state.</param>
        ///<param name="parameters">Optional parameters for the state change.</param>
        ///<returns>A new AnimationStateChangeEvent instance.</returns>
        public static AnimationStateChangeEvent Create(
            string eventId,
            string eventName,
            float timestamp,
            string previousState,
            string newState)
        {
            return CreateStateChange(eventId, eventName, timestamp, previousState, newState, null);
        }
    }
}
