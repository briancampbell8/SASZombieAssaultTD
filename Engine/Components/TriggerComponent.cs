// ====================================================================================================
//  FILE: TriggerComponent.cs
//  PATH: Engine/Components/TriggerComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core engine component for representing trigger detection state. Stores trigger flags,
//      radius, one‑time trigger settings, cooldown values, layer masks, and triggered‑ECSEntityCore
//      tracking used by gameplay, physics, and collision systems.
//
//  RESPONSIBILITIES:
//      - Store trigger configuration (IsTrigger, TriggerRadius, TriggerOnce)
//      - Track which entities have triggered this component
//      - Provide helper methods for trigger eligibility and tracking
//      - Store cooldown and layer‑mask filtering values
//      - Act as a lightweight data carrier for systems that process trigger behavior
//
//  NON-RESPONSIBILITIES:
//      - Executing trigger detection or spatial queries
//      - Publishing trigger events or managing event lifecycles
//      - Managing world‑level ECSEntityCore lifecycle or ECS attachment
//      - Performing physics, collision, or gameplay logic
//
//  ARCHITECTURAL NOTES:
//      - This is a pure engine component with minimal behavioral helpers
//      - Integrates with trigger, collision, and gameplay systems but does not implement them
//      - Kept lightweight to preserve subsystem boundaries
// ====================================================================================================


using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Component for trigger event generation properties. P11-04-03-B: Stores trigger flag, radius, one-time trigger
    /// flag, and triggered ECSEntityCore tracking. This component is pure data, ECS-friendly, and fully documented.
    /// </summary>
    public class TriggerComponent
    {
        /// Properties

        /// <summary>
        /// Whether this ECSEntityCore generates trigger events. True if this ECSEntityCore should detect and publish trigger events.
        /// </summary>
        public bool IsTrigger { get; set; } = true;

        /// <summary>
        /// Distance threshold for triggering in world units. Entities within this radius will trigger events.
        /// </summary>
        public float TriggerRadius { get; set; } = 50.0f;

        /// <summary>
        /// Whether this trigger fires only once per target ECSEntityCore. True = triggers only once per target, False = can
        /// trigger multiple times.
        /// </summary>
        public bool TriggerOnce { get; set; } = false;

        /// <summary>
        /// Tracks which entities have already triggered this one. Used to prevent re-triggering when TriggerOnce is
        /// true. Key: Entity reference, Value: Timestamp of first trigger (for potential cooldown logic).
        /// </summary>
        public Dictionary<object, DateTime> TriggeredEntities { get; private set; } = new();

        /// <summary>
        /// Whether this component is enabled and participating in trigger detection.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Optional cooldown period between triggers for the same target (in seconds). Only used when TriggerOnce is
        /// false to prevent spam.
        /// </summary>
        public float TriggerCooldown { get; set; } = 0.0f;

        /// <summary>
        /// Layer mask for filtering which entities can trigger this. Uses bitwise AND operation with target ECSEntityCore
        /// layers.
        /// </summary>
        public int TriggerLayerMask { get; set; } = -1; //Trigger with all layers by default

        ///

        /// Constructors

        /// <summary>
        /// Creates a new TriggerComponent with default values.
        /// </summary>
        public TriggerComponent()
        { }

        /// <summary>
        /// Creates a new TriggerComponent with specified radius.
        /// </summary>
        /// <param name="triggerRadius">Distance threshold for triggering</param>
        public TriggerComponent(float triggerRadius)
        {
            TriggerRadius = System.Math.Max(triggerRadius, 0.0f);
        }

        /// <summary>
        /// Creates a new TriggerComponent with full configuration.
        /// </summary>
        /// <param name="isTrigger">Whether this ECSEntityCore generates trigger events</param>
        /// <param name="triggerRadius">Distance threshold for triggering</param>
        /// <param name="triggerOnce">Whether trigger fires only once per target</param>
        /// <param name="triggerCooldown">Cooldown period between triggers</param>
        /// <param name="triggerLayerMask">Layer mask for filtering trigger targets</param>
        /// <param name="enabled">Whether component is enabled</param>
        public TriggerComponent(
            bool isTrigger = true,
            float triggerRadius = 50.0f,
            bool triggerOnce = false,
            float triggerCooldown = 0.0f,
            int triggerLayerMask = -1,
            bool enabled = true)
        {
            IsTrigger = isTrigger;
            TriggerRadius = System.Math.Max(triggerRadius, 0.0f);
            TriggerOnce = triggerOnce;
            TriggerCooldown = System.Math.Max(triggerCooldown, 0.0f);
            TriggerLayerMask = triggerLayerMask;
            Enabled = enabled;
        }

        ///

        /// Methods

        /// <summary>
        /// Checks if a specific ECSEntityCore has already triggered this component.
        /// </summary>
        /// <param name="ECSEntityCore">Entity to check</param>
        /// <returns>True if ECSEntityCore has already triggered this component</returns>
        public bool HasEntityTriggered(object ECSEntityCore)
        {
            return ECSEntityCore != null && TriggeredEntities.ContainsKey(ECSEntityCore);
        }

        /// <summary>
        /// Marks an ECSEntityCore as having triggered this component.
        /// </summary>
        /// <param name="ECSEntityCore">Entity that triggered this component</param>
        public void MarkEntityTriggered(object ECSEntityCore)
        {
            if (ECSEntityCore != null)
            {
                TriggeredEntities[ECSEntityCore] = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Removes an ECSEntityCore from the triggered entities list. Used when an ECSEntityCore exits the trigger area.
        /// </summary>
        /// <param name="ECSEntityCore">Entity to remove</param>
        public void RemoveEntityTriggered(object ECSEntityCore)
        {
            if (ECSEntityCore != null)
            {
                TriggeredEntities.Remove(ECSEntityCore);
            }
        }

        /// <summary>
        /// Checks if an ECSEntityCore can trigger this component based on cooldown and trigger-once settings.
        /// </summary>
        /// <param name="ECSEntityCore">Entity to check</param>
        /// <returns>True if ECSEntityCore can trigger this component</returns>
        public bool CanEntityTrigger(object ECSEntityCore)
        {
            if (ECSEntityCore == null || !Enabled || !IsTrigger)
                return false;

            //Check if trigger-once and already triggered
            if (TriggerOnce && HasEntityTriggered(ECSEntityCore))
                return false;

            //Check cooldown
            if (TriggerCooldown > 0.0f && HasEntityTriggered(ECSEntityCore))
            {
                var lastTriggerTime = TriggeredEntities[ECSEntityCore];
                var timeSinceLastTrigger = DateTime.UtcNow - lastTriggerTime;
                if (timeSinceLastTrigger.TotalSeconds < TriggerCooldown)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Clears all triggered entities (useful for reset scenarios).
        /// </summary>
        public void ClearTriggeredEntities()
        {
            TriggeredEntities.Clear();
        }

        /// <summary>
        /// Gets the number of entities that have triggered this component.
        /// </summary>
        /// <returns>Count of triggered entities</returns>
        public int GetTriggeredEntityCount()
        {
            return TriggeredEntities.Count;
        }

        /// <summary>
        /// Gets a string representation for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"TriggerComponent(Radius: {TriggerRadius}, Once: {TriggerOnce}, Triggered: {TriggeredEntities.Count}, Enabled: {Enabled})";
        }

        ///
    }
}
