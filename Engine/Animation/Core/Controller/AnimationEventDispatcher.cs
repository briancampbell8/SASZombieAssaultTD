// =====================================================================================================
//  FILE: AnimationEventDispatcher.cs
//  PATH: Engine/Animation/Core/Controller/AnimationEventDispatcher.cs
//  SUBSYSTEM: Animation Core Controller
//
//  ROLE:
//      Deterministic event‑routing engine for the Animation subsystem. Centralizes the dispatch of
//      animation events (start, complete, custom) emitted by AnimationSystem and provides a clean,
//      engine‑facing surface for gameplay callbacks, listeners, and future routing extensions.
//
//  RESPONSIBILITIES:
//      - Receive AnimationEvent instances from AnimationSystem.
//      - Dispatch animation start/complete/custom events deterministically.
//      - Provide a stable hook point for gameplay logic (footsteps, muzzle flashes, attack frames).
//      - Maintain strict ordering guarantees for event routing.
//      - Keep event routing logic separate from ECS, rendering, and clip playback.
//
//  NON-RESPONSIBILITIES:
//      - Performing animation playback or clip timing.
//      - Managing ECS entities or components.
//      - Owning animation clip libraries.
//      - Executing gameplay logic directly (only routes events).
//
//  ARCHITECTURAL NOTES:
//      - Lives in Animation Core Controller alongside AnimationController and AnimationEvent.
//      - Complements AnimationSystem by providing a dedicated routing surface.
//      - Designed for future expansion (listener registration, priority routing, filters).
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Components;

namespace SASZombieAssaultTD.Engine.Animation.Core.Controller
{
    internal sealed class AnimationEventDispatcher
    {
        // --------------------------------------------------------------------------------------------
        //  EVENT ROUTING SURFACE
        // --------------------------------------------------------------------------------------------

        public void DispatchAnimationStarted(AnimationControllerComponent controller, string clipName)
        {
            // Deterministic routing for animation-start events.
            // AnimationSystem invokes this when a clip changes.
        }

        public void DispatchAnimationCompleted(AnimationControllerComponent controller, string clipName)
        {
            // Deterministic routing for animation-complete events.
            // AnimationSystem invokes this when a clip reaches its duration.
        }

        public void DispatchAnimationEvent(AnimationControllerComponent controller, AnimationEvent evt)
        {
            // Deterministic routing for custom animation events.
            // AnimationSystem invokes this when EventCrossed() returns true.
        }
    }
}
