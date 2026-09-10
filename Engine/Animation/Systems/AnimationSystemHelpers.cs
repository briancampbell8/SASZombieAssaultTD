// =====================================================================================================
//  FILE: AnimationSystemHelpers.cs
//  PATH: Engine/Systems/AnimationSystemHelpers.cs
//  SUBSYSTEM: Systems
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Animation.Core.Clips;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Systems
{
    internal sealed class AnimationSystemHelpers
    {
        // Tracks fired animation events per ECSEntityCore to prevent duplicates
        private readonly Dictionary<int, HashSet<string>> _firedEvents = new();

        public ECSEntityCoreCore ecsEntityCore { get; private set; }

        // -------------------------------------------------------------------------------------------------
        //  CLIP RESOLUTION
        // -------------------------------------------------------------------------------------------------

        public AnimationClip? ResolveClip(string? clipName)
        {
            if (string.IsNullOrEmpty(clipName))
                return null;

            // Placeholder: real implementation should query the ClipLibrary.
            return new AnimationClip(clipName, 1.0f, false);
        }

        // -------------------------------------------------------------------------------------------------
        //  EVENT FIRING LOGIC (DETERMINISTIC)
        // -------------------------------------------------------------------------------------------------

        public void FireAnimationEvents(
            AnimationControllerComponent controller,
            float previousTime,
            string? previousClip,
            Action<AnimationControllerComponent, string>? onStarted,
            Action<AnimationControllerComponent, string>? onCompleted,
            Action<AnimationControllerComponent, AnimationEvent>? onEventFired,
            ECSEntityCoreCore ECSEntityCoreCore)
        {
            var currentClip = controller.CurrentClip;
            var currentTime = controller.PlaybackTime;
            var clip = ResolveClip(currentClip);

            // Animation start event
            ECSEntityCoreCore = ecsEntityCore as ECSEntityCoreCore;
            if (ECSEntityCoreCore != null)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                    $"AnimationSystem: ECSEntityCore {ECSEntityCoreCore.Id} started animation '{currentClip}'");
            }

            // Animation complete event
            if (previousClip == currentClip && clip != null && currentTime >= clip.Duration)
            {
                onCompleted?.Invoke(controller, currentClip);
                DLogger.Log(
                    LogSubsystems.Animation,
                    LogEnums.LogLevel.Debug,
                    $"AnimationSystem: ECSEntityCore {((dynamic)controller.EntityCore).Id} " +
                    $"completed animation '{currentClip}'");
            }

            if (clip == null)
                return;

            // Custom animation events
            foreach (var animationEvent in clip.Events)
            {
                if (EventCrossed(previousTime, currentTime, animationEvent.Time) &&
                    !HasEventFired(controller, animationEvent))
                {
                    onEventFired?.Invoke(controller, animationEvent);
                    MarkEventFired(controller, animationEvent);

                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                        $"AnimationSystem: ECSEntityCore {((dynamic)controller.EntityCore).Id} fired event '{animationEvent.EventName}'");

                    HandleGameplayCallbacks(controller, animationEvent);
                }
            }
        }
        // -------------------------------------------------------------------------------------------------
        //  EVENT TRACKING HELPERS
        // -------------------------------------------------------------------------------------------------

        private bool EventCrossed(float prevTime, float currTime, float eventTime)
        {
            return prevTime < eventTime && currTime >= eventTime;
        }

        private bool HasEventFired(AnimationControllerComponent controller, AnimationEvent evt)
        {
            int id = (int)((dynamic)controller.EntityCore).Id;

            if (!_firedEvents.TryGetValue(id, out var set))
                return false;

            return set.Contains(evt.EventName);
        }

        private void MarkEventFired(AnimationControllerComponent controller, AnimationEvent evt)
        {
            int id = (int)((dynamic)controller.EntityCore).Id;

            if (!_firedEvents.TryGetValue(id, out var set))
            {
                set = new HashSet<string>();
                _firedEvents[id] = set;
            }

            set.Add(evt.EventName);
        }

        // -------------------------------------------------------------------------------------------------
        //  GAMEPLAY CALLBACK HOOK
        // -------------------------------------------------------------------------------------------------

        private void HandleGameplayCallbacks(AnimationControllerComponent controller, AnimationEvent evt)
        {
            // Deterministic gameplay callback hook.
            // Example: attack frames, footstep sounds, muzzle flashes, etc.
        }
    }

    public class ECSEntityCoreCore
    {
        public int Id { get; set; }

        public ECSEntityCoreCore(int id)
        {
            Id = id;
        }
    }
}
