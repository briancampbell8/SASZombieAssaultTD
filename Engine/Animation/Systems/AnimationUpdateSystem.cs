/*
File:    AnimationUpdateSystem.cs
Purpose: System for updating animation states on entities.
Features: ECS integration, animation state updates, time-based progression.

P11-04-07-B: System updates animation states for entities with AnimationControllerComponent.
Manages animation time progression and state machine updates with comprehensive logging.
*/
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Animation.Events;
using SASZombieAssaultTD.Engine.Animation.Systems;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Events;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Animation.Systems
{
    /// <summary>
    /// System responsible for updating animation states on entities.
    /// Processes animation time progression and state machine updates.
    /// </summary>
    public class AnimationUpdateSystem : AnimationUpdateSystemBase
    {
        private readonly ECSWorld _world;
        private float _timeScale = 1.0f;

        /// <summary>
        /// Initializes the animation update system.
        /// </summary>
        /// <param name="world">The ECS world to operate on.</param>
        public AnimationUpdateSystem(ECSWorld world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        /// <summary>
        /// Updates all animation controllers in the world.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        public void Update(float deltaTime)
        {
            var scaledDelta = deltaTime * _timeScale;
            var animationEntities = _world.GetEntitiesWith<AnimationControllerComponent>();

            foreach (var entity in animationEntities)
            {
                try
                {
                    UpdateAnimationController(entity, scaledDelta);
                }
                catch (Exception ex)
                {
                    DebugLog($"AnimationUpdateSystem: Failed to update animation for Entity {entity.Id} - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Updates animation controller for a specific entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        private void UpdateAnimationController(Entity entity, float deltaTime)
        {
            var controller = entity.GetComponent<AnimationControllerComponent>();
            if (controller == null || !controller.IsActive)
                return;

            // Update animation playback time
            controller.UpdatePlaybackTime(deltaTime);

            // Update state machine if present
            var stateMachine = entity.GetComponent<AnimationStateMachineComponent>();
            if (stateMachine != null && stateMachine.IsRunning)
            {
                stateMachine.Update(deltaTime);
            }

            // Process animation events
            ProcessAnimationEvents(entity, controller, GetOnAnimationEventFired());
        }

        private Action<AnimationControllerComponent, AnimationEvent> GetOnAnimationEventFired()
        {
            throw new NotImplementedException();
        }

        //   private Action<AnimationControllerComponent, AnimationEvent> GetOnAnimationEventFired()
        //   {
        //       return OnAnimationEventFired;
        //   }

        /// <summary>
        /// Processes animation events for an entity.
        /// </summary>
        /// <param name="entity">The entity to process events for.</param>
        /// <param name="controller">The animation controller.</param>
        private void ProcessAnimationEvents(Entity entity, AnimationControllerComponent controller, Action<AnimationControllerComponent, AnimationEvent> onAnimationEventFired)
        {
            var events = controller.GetPendingEvents();
            if (events == null || !events.Any())
                return;

            foreach (var animationEvent in events)
            {
                try
                {
                    // Fire animation event using appropriate overload based on event type
                    if (animationEvent is AnimationStateChangeEvent stateChange)
                    {
                        OnAnimationStateChangeEvent?.Invoke(controller, stateChange);
                    }
                    else if (animationEvent is AnimationClipEvent clipEvent)
                    {
                        OnAnimationClipEvent?.Invoke(controller, clipEvent);
                    }
                    else if (animationEvent is AnimationLoopEvent loopEvent)
                    {
                        OnAnimationLoopEvent?.Invoke(controller, loopEvent);
                    }
                    else if (animationEvent is AnimationParameterEvent paramEvent)
                    {
                        OnAnimationParameterEvent?.Invoke(controller, paramEvent);
                    }
                    else
                    {
                        onAnimationEventFired?.Invoke(controller, (AnimationEvent)animationEvent);
                    }

                    MarkEventFired(controller, (AnimationEvent)animationEvent);
                    var baseEvent = (AnimationEvent)animationEvent;
                    DebugLog($"AnimationUpdateSystem: Entity {entity.Id} fired event '{baseEvent.EventName}' at time {baseEvent.Timestamp:F3}");
                }
                catch (Exception ex)
                {
                    var eventName = animationEvent is AnimationEvent ae ? ae.EventName : "unknown";
                    DebugLog($"AnimationUpdateSystem: Failed to process event '{eventName}' for Entity {entity.Id} - {ex.Message}");
                }
            }

            controller.ClearPendingEvents();
        }

        
        /// <summary>
        /// Event fired when an animation state change occurs.
        /// Adapts AnimationStateChangeEvent calls to the canonical event system.
        /// </summary>
        /// <param name="controller">The animation controller.</param>
        /// <param name="stateChange">The state change event.</param>
        public event Action<AnimationControllerComponent, AnimationStateChangeEvent> OnAnimationStateChangeEvent;

        /// <summary>
        /// Event fired when an animation clip event occurs.
        /// Adapts AnimationClipEvent calls to the canonical event system.
        /// </summary>
        /// <param name="controller">The animation controller.</param>
        /// <param name="clipEvent">The clip event.</param>
        public event Action<AnimationControllerComponent, AnimationClipEvent> OnAnimationClipEvent;

        /// <summary>
        /// Event fired when an animation loop event occurs.
        /// Adapts AnimationLoopEvent calls to the canonical event system.
        /// </summary>
        /// <param name="controller">The animation controller.</param>
        /// <param name="loopEvent">The loop event.</param>
        public event Action<AnimationControllerComponent, AnimationLoopEvent> OnAnimationLoopEvent;

        /// <summary>
        /// Event fired when an animation parameter event occurs.
        /// Adapts AnimationParameterEvent calls to the canonical event system.
        /// </summary>
        /// <param name="controller">The animation controller.</param>
        /// <param name="paramEvent">The parameter event.</param>
        public event Action<AnimationControllerComponent, AnimationParameterEvent> OnAnimationParameterEvent;

        /// <summary>
        /// Marks an animation event as fired.
        /// </summary>
        /// <param name="controller">The animation controller.</param>
        /// <param name="animationEvent">The event to mark.</param>
        private void MarkEventFired(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var eventKey = $"{animationEvent.EventName}_{animationEvent.Timestamp}";
            controller.SetParameter(eventKey, 1f);
        }

        /// <summary>
        /// Sets the time scale for animation updates.
        /// </summary>
        /// <param name="timeScale">The time scale factor.</param>
        public void SetTimeScale(float timeScale)
        {
            _timeScale = System.MathF.Max(0f, timeScale);
        }

        /// <summary>
        /// Debug logging method.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void DebugLog(string message)
        {
            // TODO: Replace with proper logging system when available
            System.Diagnostics.Debug.WriteLine($"[AnimationUpdateSystem] {message}");
        }
    }
}
