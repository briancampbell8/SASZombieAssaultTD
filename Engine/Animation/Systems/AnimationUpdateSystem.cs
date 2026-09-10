// ====================================================================================================
//  FILE: AnimationUpdateSystem.cs
//  PATH: ./Engine/Animation/Systems/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationUpdateSystem module.
//
//  RESPONSIBILITIES:
//      - Provide Update() behavior for the Core subsystem.
//      - Provide SetTimeScale() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
// ====================================================================================================

using System;
using System.Linq;
using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Animation.Systems
{
    /// <summary>
    /// System responsible for updating animation states on entities. Processes animation time progression and state
    /// machine updates.
    /// </summary>
    public class AnimationUpdateSystem : AnimationUpdateSystemBase
    {
        private readonly ECSRuntimeCore _world;
        private float _timeScale = 1.0f;
        private ECSEntityCore _ECSEntityCoreManager;
        private ECSComponents _ECSEntityCoreComponents;

        public AnimationUpdateSystem()
        {
            _world = ECSRuntimeCore.Instance
                ?? throw new InvalidOperationException("ECSRuntimeCore.Instance must be initialized before AnimationUpdateSystem.");
        }

        public void SetTimeScale(float timeScale) => _timeScale = timeScale;

        /// <summary>
        /// Updates all animation controllers in the world.
        /// </summary>
        public void Update(float deltaTime)
        {
            var scaledDelta = deltaTime * _timeScale;

            // Update all entities that have AnimationControllerComponent
            foreach (var ECSEntityCore in _world.FindEntitiesWithComponent<AnimationControllerComponent>())
            {
                UpdateAnimationController(ECSEntityCore, scaledDelta);
            }
        }

        private void UpdateAnimationController(ECSEntityCore ECSEntityCore, float deltaTime)
        {
            var controller = _ECSEntityCoreComponents.GetComponent<AnimationControllerComponent>(ECSEntityCore.Id);

            if (controller == null || !controller.IsActive)
                return;

            // Update animation playback time
            controller.UpdatePlaybackTime(deltaTime);

            // Update state machine if present
            var stateMachine =
                _ECSEntityCoreComponents.GetComponent<AnimationMachineComponent>(ECSEntityCore.Id);
            if (stateMachine != null && stateMachine.IsRunning)
            {
                stateMachine.Update(deltaTime);
            }

            // Process animation events
            ProcessAnimationEvents(ECSEntityCore, controller, OnAnimationEventFired);
        }

        private Action<AnimationControllerComponent, AnimationEvent> OnAnimationEventFired => OnAnimationEventFired;

        /// <summary>
        /// Processes animation events for an ECSEntityCore.
        /// </summary>
        private void ProcessAnimationEvents(
            ECSEntityCore ECSEntityCore,
            AnimationControllerComponent controller,
            Action<AnimationControllerComponent, AnimationEvent> onAnimationEventFired)
        {
            var events = controller.GetPendingEvents();
            if (events == null || !events.Any())
                return;

            foreach (var animationEvent in events)
            {
                try
                {
                    switch (animationEvent)
                    {
                        case AnimationStateChangeEvent stateChange:
                            OnAnimationStateChangeEvent?.Invoke(controller, stateChange);
                            break;

                        case AnimationClipEvent clipEvent:
                            OnAnimationClipEvent?.Invoke(controller, clipEvent);
                            break;

                        case AnimationLoopEvent loopEvent:
                            OnAnimationLoopEvent?.Invoke(controller, loopEvent);
                            break;

                        case AnimationParameterEvent paramEvent:
                            OnAnimationParameterEvent?.Invoke(controller, paramEvent);
                            break;

                        default:
                            onAnimationEventFired?.Invoke(controller, (AnimationEvent)animationEvent);
                            break;
                    }

                    MarkEventFired(controller, (AnimationEvent)animationEvent);

                    var baseEvent = (AnimationEvent)animationEvent;
                    DLogger.Log($"AnimationUpdateSystem: ECSEntityCore {ECSEntityCore.Id} fired event '{baseEvent.EventName}' at time {baseEvent.Timestamp:F3}");
                }
                catch (Exception ex)
                {
                    var eventName = animationEvent is AnimationEvent ae ? ae.EventName : "unknown";
                    DLogger.Log($"AnimationUpdateSystem: Failed to process event '{eventName}' for ECSEntityCore {ECSEntityCore.Id} - {ex.Message}");
                }
            }

            controller.ClearPendingEvents();
        }

        // -------------------------------------------------------------------------------------------------
        // Event declarations
        // -------------------------------------------------------------------------------------------------

        public event Action<AnimationControllerComponent, AnimationStateChangeEvent> OnAnimationStateChangeEvent;

        public event Action<AnimationControllerComponent, AnimationClipEvent> OnAnimationClipEvent;

        public event Action<AnimationControllerComponent, AnimationLoopEvent> OnAnimationLoopEvent;

        public event Action<AnimationControllerComponent, AnimationParameterEvent> OnAnimationParameterEvent;

        // -------------------------------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------------------------------

        private void MarkEventFired(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var eventKey = $"{animationEvent.EventName}_{animationEvent.Timestamp}";
            controller.SetParameter(eventKey, 1f);
        }

        private void Log(string message)
        {
            DLogger.Log($"[AnimationUpdateSystem] {message}");
        }
    }
}
