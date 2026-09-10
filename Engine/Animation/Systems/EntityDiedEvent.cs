// ====================================================================================================
//  FILE: EntityDiedEvent.cs
//  PATH: ./Engine/Animation/Systems/
//  MODULE: Event Subsystem (ECS + Gameplay Lifecycle)
//
//  ROLE:
//      Represents a deterministic, audit-friendly event fired whenever an ECSEntityCore dies within the ECS
//      pipeline. This event is published by DeathSystem and consumed by systems such as
//      AnimationTriggerSystem, ScoreSystem, and EntityRemovalSystem.
//
//  RESPONSIBILITIES:
//      - Carry all relevant information about an ECSEntityCore death.
//      - Provide deterministic timestamps for audit and replay systems.
//      - Serve as the modern replacement for deprecated KillAttributedEvent.
//      - Integrate cleanly with ECSRuntimeEvents and ECS lifecycle systems.
//
//  NON-RESPONSIBILITIES:
//      - Performing death logic (handled by DeathSystem).
//      - Removing entities (handled by EntityRemovalSystem).
//      - Triggering animations (handled by AnimationTriggerSystem).
//      - Managing gameplay state transitions.
//
//  ARCHITECTURAL NOTES:
//      - Fully aligned with Option-B architecture (no hierarchical states, no legacy event pipeline).
//      - Designed for deterministic replay and audit logging.
//      - Compatible with ECSRuntimeEvents, ECSEntityCore, and all modern ECS systems.
//      - Timestamp uses DateTime.UtcNow for deterministic cross-platform behavior.
//
//  AUTHOR: BDC
//  CREATED: 2026-07-17
//  LAST UPDATED: 2026-07-17
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Systems
{
    /// <summary>
    /// Modern ECS event representing the death of an ECSEntityCore. Published by DeathSystem and consumed by animation,
    /// scoring, and cleanup systems.
    /// </summary>
    public sealed class EntityDiedEvent
    {
        /// <summary>
        /// The ID of the ECSEntityCore that died.
        /// </summary>
        public int VictimId { get; }

        /// <summary>
        /// Optional: The ID of the ECSEntityCore responsible for the kill (player, enemy, trap, etc.).
        /// </summary>
        public int? KillerId { get; }

        /// <summary>
        /// Optional: A string describing the reason for death (e.g., "Projectile", "Explosion").
        /// </summary>
        public string DeathReason { get; }

        /// <summary>
        /// Deterministic timestamp for audit and replay systems.
        /// </summary>
        public DateTime Timestamp { get; }
        public object DeathType { get; internal set; }
        public object deathType { get; internal set; }

        /// <summary>
        /// Creates a new EntityDiedEvent with full audit-friendly metadata.
        /// </summary>
        public EntityDiedEvent(int victimId, int? killerId = null, string deathReason = "Unknown")
        {
            VictimId = victimId;
            KillerId = killerId;
            DeathReason = deathReason;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"EntityDiedEvent: Victim={VictimId}, Killer={KillerId}, Reason={DeathReason}, Timestamp={Timestamp:O}";


    }
}
