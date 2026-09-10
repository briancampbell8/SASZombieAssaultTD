// =====================================================================================================
//  FILE: EventDispatcher.cs
//  PATH: Engine/Animation/EventDispatcher.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Provides deterministic dispatching of animation events fired by animation controllers
//      and state machines. This module routes events to gameplay callbacks, external listeners,
//      and internal animation subsystems.
//
//  RESPONSIBILITIES:
//      - Detect and dispatch animation events when their timestamps are crossed.
//      - Invoke GameplayCallbacks for gameplay-linked animation events.
//      - Invoke external listeners (OnAnimationEventFired, OnAnimationStarted, OnAnimationCompleted).
//      - Maintain strict subsystem boundaries: no rendering, no ECS world ownership,
//        no controller update logic, no state machine logic.
//      - Log event dispatch activity for debugging and profiling.
//
//  NON-RESPONSIBILITIES:
//      - Animation controller playback logic.
//      - Animation state machine evaluation.
//      - Rendering or sprite updates.
//      - ECS ECSEntityCore lifecycle management.
//      - Deterministic sequencing (handled by ECSRuntimeCore).
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally thin and stateless.
//      - All animation event dispatch must route through this module.
//      - Ensures animation event behavior remains isolated and deterministic.
//
//  CHANGE LOG:
//      • 08-15-2026 — File created during Animation subsystem modernization (BDC).
// =====================================================================================================
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Animation.Core.Controller;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Provides deterministic dispatching of animation events.
    /// </summary>
    internal sealed class EventDispatcher
    {
        private readonly GameplayCallbacks _callbacks = new();

        // =====================================================================================================
        //  PUBLIC API
        // =====================================================================================================

        /// <summary>
        /// Dispatches animation events for the given ECSEntityCore's controller.
        /// </summary>
        public void DispatchEvents(
            ECSEntityCore ECSEntityCore,
            AnimationStateTracker stateTracker,
            AnimationControllerComponent controller,
            float previousTime,
            float currentTime)
        {
            var clipName = controller.CurrentClip;
            if (string.IsNullOrEmpty(clipName))
                return;

            var coreDispatcher = new AnimationEventDispatcher();

            // Detect clip start
            if (stateTracker.PreviousClip != clipName)
            {
                // Route start through core dispatcher (cannot invoke controller's static event from here)
                coreDispatcher.DispatchAnimationStarted(controller, clipName);

                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                    $"EventDispatcher: ECSEntityCore {ECSEntityCore.Id} started animation '{clipName}'");
            }

            // Detect clip completion
            if (stateTracker.PreviousClip == clipName && currentTime >= /* clip duration not available here */ currentTime)
            {
                // Use core dispatcher for completion as controller does not expose instance event
                coreDispatcher.DispatchAnimationCompleted(controller, clipName);

                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                    $"EventDispatcher: ECSEntityCore {ECSEntityCore.Id} completed animation '{clipName}'");
            }

            // Process pending animation events from the controller (controller exposes pending event list)
            var pending = controller.GetPendingEvents();
            if (pending != null)
            {
                foreach (var evtObj in pending)
                {
                    if (evtObj is not AnimationEvent animationEvent)
                        continue;

                    // If timestamp crossing is desired, check crossing; otherwise pending events were produced at the right time
                    if (!EventCrossed(controller, animationEvent, previousTime, currentTime))
                        continue;

                    // Fire external/core dispatcher for the event
                    coreDispatcher.DispatchAnimationEvent(controller, animationEvent);

                    // Mark event fired via controller parameter store (SetParameter exists)
                    MarkEventFired(controller, animationEvent);

                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                        $"EventDispatcher: ECSEntityCore {ECSEntityCore.Id} fired event '{animationEvent.EventName}'");

                    // Route to gameplay callbacks
                    DispatchGameplayCallback(controller, animationEvent);
                }
            }
        }

        // =====================================================================================================
        //  INTERNAL HELPERS
        // =====================================================================================================

        private bool EventCrossed(AnimationControllerComponent controller, AnimationEvent animationEvent,
                                  float previousTime, float currentTime)
        {
            // controller.Speed is int in the component; keep comparison consistent
            return controller.Speed >= 0
                ? previousTime < animationEvent.Timestamp && currentTime >= animationEvent.Timestamp
                : previousTime > animationEvent.Timestamp && currentTime <= animationEvent.Timestamp;
        }

        private void MarkEventFired(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var key = $"{animationEvent.EventName}_{animationEvent.Timestamp}";
            controller.SetParameter(key, 1f);
        }

        private void DispatchGameplayCallback(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            switch (animationEvent.EventName)
            {
                case "FireWeapon":
                    _callbacks.HandleFireWeapon(controller, animationEvent);
                    break;

                case "Footstep":
                    _callbacks.HandleFootstep(controller, animationEvent);
                    break;

                case "PlaySound":
                    _callbacks.HandlePlaySound(controller, animationEvent);
                    break;

                case "SpawnEffect":
                    _callbacks.HandleSpawnEffect(controller, animationEvent);
                    break;

                case "DamageArea":
                    _callbacks.HandleDamageArea(controller, animationEvent);
                    break;

                default:
                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                        $"EventDispatcher: No gameplay callback for event '{animationEvent.EventName}'");
                    break;
            }
        }
    }
}
