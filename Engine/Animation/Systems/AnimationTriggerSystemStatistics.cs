// ====================================================================================================
//  FILE: AnimationTriggerSystemStatistics.cs
//  PATH: ./Engine/Animation/Systems/
//  MODULE: Animation Subsystem
//
//  ROLE:
//      Provides deterministic animation-trigger behavior for death events within the ECS pipeline.
//      This system listens for EntityDiedEvent notifications and triggers "Death" animations on
//      entities containing AnimationControllerComponent.
//
//  RESPONSIBILITIES:
//      - Subscribe to EntityDiedEvent via ECSRuntimeEvents.
//      - Trigger death animations on entities with AnimationControllerComponent.
//      - Maintain audit-friendly logging for animation triggers.
//      - Provide lifecycle controls: Initialize, Enable, Disable, Toggle, Destroy, Reset.
//      - Track update statistics for diagnostics.
//
//  NON-RESPONSIBILITIES:
//      - Rendering API calls.
//      - Asset management or animation data loading.
//      - State machine transitions.
//      - Physics or gameplay logic.
//
//  ARCHITECTURAL NOTES:
//      - Modernized to align with Option-B architecture (no hierarchical states, no legacy events).
//      - Replaces deprecated KillAttributedEvent with modern EntityDiedEvent.
//      - Fully compatible with ECSRuntimeEvents, ECSEntityCore, and AnimationControllerComponent.
//      - Deterministic and audit-friendly for debugging and replay systems.
//
//  AUTHOR: BDC
//  CREATED: 2026-07-17
//  LAST UPDATED: 2026-07-17
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation.Systems
{
    // --------------------------------------------------------------------------------------------
    //  STATISTICS
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Statistics about the animation trigger system state.
    /// </summary>
    public sealed class AnimationTriggerSystemStatistics
    {
        public bool Initialized { get; set; }
        public bool SubscribedToGameEvent { get; set; }

        public override string ToString() =>
            $"Animation Trigger System Statistics - Initialized: {Initialized}, Subscribed: {SubscribedToGameEvent}";
    }
}
