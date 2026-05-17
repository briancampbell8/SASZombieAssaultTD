/*
File:    AnimationTransition.cs
Path:    Engine/Animation/AnimationTransition.cs
Purpose:   Animation state transition support.
           Represents transitions between animation states.

Role:      Essential animation transition system.
           - Defines transition rules between animation states
           - Manages transition conditions and timing
           - Supports smooth state transitions

Features:   Animation state transition definitions.
           Transition condition management.
           Integration with animation state machine.

Notes:      This system defines transition behavior for animations.
           Works in conjunction with AnimationStateMachine.
*/

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    /// <summary>
    /// Represents a transition between animation states.
    /// </summary>
    public class AnimationTransition
    {
        /// <summary>
        /// The target state of the transition.
        /// </summary>
        public object TargetState { get; set; }

        /// <summary>
        /// The actions to perform during the transition.
        /// </summary>
        public List<AnimationTransitionAction> Actions { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationTransition"/> class.
        /// </summary>
        /// <param name="targetState">The target state of the transition.</param>
        public AnimationTransition(object targetState)
        {
            TargetState = targetState ?? throw new System.ArgumentNullException(nameof(targetState));
        }

        /// <summary>
        /// Adds an action to the transition.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void AddAction(AnimationTransitionAction action)
        {
            if (action == null)
                throw new System.ArgumentNullException(nameof(action));

            Actions.Add(action);
        }
    }

    /// <summary>
    /// Represents an action to perform during an animation transition.
    /// </summary>
    public class AnimationTransitionAction
    {
        /// <summary>
        /// The type of the transition action.
        /// </summary>
        public AnimationTransitionActionType Type { get; set; }

        /// <summary>
        /// The parameters associated with the action.
        /// </summary>
        public Dictionary<string, object> Parameters { get; } = new();

        /// <summary>
        /// Gets a parameter value by key.
        /// </summary>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        /// <param name="key">The key of the parameter.</param>
        /// <param name="defaultValue">The default value if the key is not found.</param>
        /// <returns>The parameter value if found; otherwise, the default value.</returns>
        public T GetParameter<T>(string key, T defaultValue = default!)
        {
            if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        /// <param name="key">The key of the parameter.</param>
        /// <param name="value">The value to set.</param>
        public void SetParameter(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new System.ArgumentException("Key cannot be null or empty.", nameof(key));

            Parameters[key] = value;
        }
    }

    /// <summary>
    /// Defines the types of actions that can be performed during an animation transition.
    /// </summary>
    public enum AnimationTransitionActionType
    {
        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        SetParameter,

        /// <summary>
        /// Fires an event.
        /// </summary>
        FireEvent,

        /// <summary>
        /// Plays an animation clip.
        /// </summary>
        PlayClip
    }
}

