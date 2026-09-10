// ====================================================================================================
//  FILE: IAnimationEventReceiver.cs
//  PATH: ./Engine/Animation/Events/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the IAnimationEventReceiver module.
//
//  RESPONSIBILITIES:
//      - Provide GetDebugInfo() behavior for the Core subsystem.
//      - Provide Validate() behavior for the Core subsystem.
//      - Provide ShouldHandleEvent() behavior for the Core subsystem.
//      - Provide SafeInvokeOnAnimationEvent() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Animation.Core.Controller;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    /// <summary>
    /// P11-19-03: Interface for event receivers including OnAnimationEvent and GetDebugInfo methods. Defines the
    /// contract for objects that can receive and handle animation events.
    /// </summary>
    public interface IAnimationEventReceiver
    {
        /// <summary>
        /// Unique identifier for this event receiver. Used for registration and deregistration.
        /// </summary>
        string ReceiverId { get; }

        /// <summary>
        /// Human-readable name for this event receiver. Used for debugging and logging purposes.
        /// </summary>
        string ReceiverName { get; }

        /// <summary>
        /// Priority of this receiver for event processing order. Lower values receive events earlier in the processing
        /// chain.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Whether this receiver is currently enabled. Disabled receivers will not receive events.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// List of event names this receiver is interested in. Empty list means receiver receives all events.
        /// </summary>
        IReadOnlyList<string> InterestedEvents { get; }

        /// <summary>
        /// Called when an animation event is triggered. The core method for handling animation events.
        /// </summary>
        /// <param name="animationEvent">The animation event that was triggered</param>
        /// <param name="context">Context information about the event trigger</param>
      //  void OnAnimationEvent(AnimationEvent animationEvent, AnimationEventContext context);

        /// <summary>
        /// Gets debug information about this event receiver. Provides deterministic debugging information for
        /// inspection.
        /// </summary>
        /// <returns>Debug information string</returns>
        string GetDebugInfo();

        /// <summary>
        /// Validates the configuration of this event receiver. Performs deterministic validation of receiver
        /// properties.
        /// </summary>
        /// <returns>Validation result</returns>
        AnimationEventReceiverValidationResult Validate();
    }

    /// <summary>
    /// P11-19-03: Validation result for animation event receivers. Deterministic validation result structure.
    /// </summary>
    public class AnimationEventReceiverValidationResult
    {
        /// <summary>
        /// Whether the validation passed.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// List of validation errors.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// List of validation warnings.
        /// </summary>
        public IReadOnlyList<string> Warnings { get; }

        /// <summary>
        /// Initializes a new validation result.
        /// </summary>
        /// <param name="isValid">Whether validation passed</param>
        /// <param name="errors">List of errors</param>
        /// <param name="warnings">List of warnings</param>
        public AnimationEventReceiverValidationResult(bool isValid, string[]? errors = null, string[]? warnings = null)
        {
            IsValid = isValid;
            Errors = errors ?? Array.Empty<string>();
            Warnings = warnings ?? Array.Empty<string>();
        }
    }

    /// <summary>
    /// P11-19-03: Base implementation for animation event receivers. Provides common functionality and default
    /// implementations.
    /// </summary>
    public abstract class AnimationEventReceiverBase : IAnimationEventReceiver
    {
        /// <summary>
        /// Unique identifier for this event receiver.
        /// </summary>
        public string ReceiverId { get; }

        /// <summary>
        /// Human-readable name for this event receiver.
        /// </summary>
        public string ReceiverName { get; }

        /// <summary>
        /// Priority of this receiver for event processing order.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Whether this receiver is currently enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// List of event names this receiver is interested in.
        /// </summary>
        public virtual IReadOnlyList<string> InterestedEvents => Array.Empty<string>();

        /// <summary>
        /// Initializes a new animation event receiver base.
        /// </summary>
        /// <param name="receiverId">Unique receiver identifier</param>
        /// <param name="receiverName">Human-readable receiver name</param>
        /// <param name="priority">Processing priority</param>
        protected AnimationEventReceiverBase(string receiverId, string receiverName, int priority = 0)
        {
            ReceiverId = receiverId ?? throw new ArgumentNullException(nameof(receiverId));
            ReceiverName = receiverName ?? throw new ArgumentNullException(nameof(receiverName));
            Priority = priority;
        }

        /// <summary>
        /// Called when an animation event is triggered. Must be implemented by derived classes.
        /// </summary>
        /// <param name="animationEvent">The animation event that was triggered</param>
        /// <param name="context">Context information about the event trigger</param>
        public abstract void OnAnimationEvent(AnimationEvent animationEvent, AnimationEventContext context);

        /// <summary>
        /// Gets debug information about this event receiver. Default implementation for common debugging information.
        /// </summary>
        /// <returns>Debug information string</returns>
        public virtual string GetDebugInfo()
        {
            var info = $"AnimationEventReceiver: {ReceiverName} (ID: {ReceiverId})";
            info += $"\n  Priority: {Priority}";
            info += $"\n  Enabled: {IsEnabled}";
            info += $"\n  Interested Events: {InterestedEvents.Count}";

            if (InterestedEvents.Count > 0)
            {
                info += "\n  Event List:";
                foreach (var eventName in InterestedEvents)
                {
                    info += $"\n    {eventName}";
                }
            }

            return info;
        }

        /// <summary>
        /// Validates the configuration of this event receiver. Default validation for common receiver properties.
        /// </summary>
        /// <returns>Validation result</returns>
        public virtual AnimationEventReceiverValidationResult Validate()
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            //Validate receiver ID
            if (string.IsNullOrEmpty(ReceiverId))
            {
                errors.Add("Receiver ID cannot be null or empty");
            }

            //Validate receiver name
            if (string.IsNullOrEmpty(ReceiverName))
            {
                errors.Add("Receiver name cannot be null or empty");
            }

            //Validate interested events
            foreach (var eventName in InterestedEvents)
            {
                if (string.IsNullOrEmpty(eventName))
                {
                    warnings.Add("Event name in interested events list is null or empty");
                }
            }

            return new AnimationEventReceiverValidationResult(errors.Count == 0, errors.ToArray(), warnings.ToArray());
        }

        /// <summary>
        /// Checks if this receiver should handle the specified event. Default implementation checks against interested
        /// events list.
        /// </summary>
        /// <param name="eventName">Event name to check</param>
        /// <returns>True if receiver should handle the event, false otherwise</returns>
        protected virtual bool ShouldHandleEvent(string eventName) =>
            InterestedEvents.Count == 0 || InterestedEvents.Contains(eventName);

        /// <summary>
        /// Safely invokes the OnAnimationEvent method with error handling. Provides deterministic error handling for
        /// event processing.
        /// </summary>
        /// <param name="animationEvent">The animation event that was triggered</param>
        /// <param name="context">Context information about the event trigger</param>
        public void SafeInvokeOnAnimationEvent(AnimationEvent animationEvent, AnimationEventContext context)
        {
            if (!IsEnabled || !ShouldHandleEvent(animationEvent.EventName))
                return;

            try
            {
                OnAnimationEvent(animationEvent, context);
            }
            catch (Exception ex)
            {
                //Log error but don't rethrow to maintain system stability
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, $"AnimationEventReceiver: Error in receiver '{ReceiverName}' ({ReceiverId}) processing event '{animationEvent.EventName}': {ex.Message}");
            }
        }
    }
}
