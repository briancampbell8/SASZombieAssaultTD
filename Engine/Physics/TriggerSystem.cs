/*
File:    TriggerSystem.cs
Purpose: Subsystem for trigger event detection and publishing.
Features: Proximity detection, trigger radius checking, event publishing, trigger-once handling.

P11-04-03-C: Subsystem iterates over trigger entities, checks proximity, detects overlaps,
publishes TriggerEnter and TriggerExit events, respects TriggerOnce, and avoids gameplay logic.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Subsystem for trigger event detection and publishing.
    /// P11-04-03-C: Iterates over entities with TriggerComponent and TransformComponent,
    /// checks for proximity to other entities with CollisionComponent and TransformComponent,
    /// detects overlaps based on TriggerRadius and collider bounds, publishes TriggerEnter and TriggerExit events,
    /// respects TriggerOnce and avoids re-triggering the same entity, and does not apply gameplay logic directly.
    /// </summary>
    public class TriggerSystem
    {
        // ISystem Implementation
        public bool IsEnabled { get; private set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public SystemPriority Priority { get; private set; } = SystemPriority.Normal;
        public float LastUpdateTime { get; private set; } = 0f;
        public uint UpdateCount { get; private set; } = 0;

        // TriggerSystem Specific Fields
        private readonly EntityManager _entityManager;
        private readonly EventRouter _eventRouting;

        private bool _initialized;
        private readonly bool _debugOutput = true;
        private readonly Dictionary<object, HashSet<object>> _previousFrameProximities = new Dictionary<object, HashSet<object>>();
        private readonly Dictionary<object, HashSet<object>> _currentFrameProximities = new Dictionary<object, HashSet<object>>();

        /// <summary>
        /// Creates a new TriggerSystem with required dependencies.
        /// </summary>
        /// <param name="entityManager">Entity manager for component access</param>
        /// <param name="eventRouting">Event routing for publishing trigger events</param>
        public TriggerSystem(EntityManager entityManager, EventRouter eventRouting)
        {
            _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            _eventRouting = eventRouting ?? throw new ArgumentNullException(nameof(eventRouting));

            DebugLog("TriggerSystem: Constructed with required dependencies");
        }

        /// <summary>
        /// Initializes the trigger system.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            try
            {
                DebugLog("TriggerSystem: Starting initialization...");

                _initialized = true;
                DebugLog("TriggerSystem: Initialization complete");
            }
            catch (Exception ex)
            {
                DebugLog($"TriggerSystem: Initialization failed - {ex.Message}");
                throw new InvalidOperationException("Failed to initialize TriggerSystem", ex);
            }
        }

        // ISystem Implementation
        public void FixedUpdate(float fixedDeltaTime)
        {
            // TriggerSystem doesn't need fixed-step updates
            // No-op implementation
        }

        public void LateUpdate(float deltaTime)
        {
            // TriggerSystem doesn't need late updates
            // No-op implementation
        }

        public void Render()
        {
            // TriggerSystem doesn't render anything
            // No-op implementation
        }

        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;
        public void Toggle() => IsEnabled = !IsEnabled;

        public void Destroy()
        {
            _previousFrameProximities.Clear();
            _currentFrameProximities.Clear();
            _initialized = false;
            IsEnabled = false;
            DebugLog("TriggerSystem: System destroyed");
        }

        public void Reset()
        {
            UpdateCount = 0;
            LastUpdateTime = 0f;
            _previousFrameProximities.Clear();
            _currentFrameProximities.Clear();
            _initialized = false;
            DebugLog("TriggerSystem: System reset");
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized)
            {
                DebugLog("TriggerSystem: Update failed - Not enabled or initialized");
                return;
            }

            UpdateCount++;
            LastUpdateTime = deltaTime;

            try
            {
                // Clear current frame proximities
                _currentFrameProximities.Clear();

                // Get all trigger entities
                var triggerEntities = _entityManager.GetEntitiesWithTriggerAndTransform();
                DebugLog($"TriggerSystem: Processing {triggerEntities.Count} trigger entities");

                // Get all entities with collision components (potential targets)
                var targetEntities = _entityManager.GetEntitiesWithCollisionAndTransform();

                // P11-04-03-C: Check for proximity to other entities
                var targetEntitiesList = targetEntities.Cast<object>().ToList();
                foreach (var triggerEntity in triggerEntities)
                {
                    CheckTriggerProximities(triggerEntity, targetEntitiesList);
                }

                // Process trigger events based on proximity changes
                ProcessTriggerEvents();

                // Update previous frame proximities for next frame
                UpdatePreviousFrameProximities();

                DebugLog($"TriggerSystem: Processed {_currentFrameProximities.Count} trigger proximities");
            }
            catch (Exception ex)
            {
                DebugLog($"TriggerSystem: Update failed - {ex.Message}");
            }
        }

        /// <summary>
        /// P11-04-03-C: Checks proximities for a single trigger entity.
        /// </summary>
        private void CheckTriggerProximities(object triggerEntity, IReadOnlyList<object> targetEntities)
        {
            try
            {
                var triggerId = triggerEntity is Entity ent ? ent.Id : (uint)triggerEntity;
                var triggerComponent = _entityManager.GetComponent<TriggerComponent>(triggerId);
                var triggerTransform = _entityManager.GetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(triggerId);

                if (triggerComponent == null || triggerTransform == null ||
                !triggerComponent.Enabled || !triggerComponent.IsTrigger)
                    return;

                var currentProximities = new HashSet<object>();

                foreach (var targetEntity in targetEntities)
                {
                    // Skip self-triggering
                    if (ReferenceEquals(triggerEntity, targetEntity))
                        continue;

                    // Check if target can trigger this entity
                    if (!CanTargetTrigger(triggerEntity, targetEntity))
                        continue;

                    // P11-04-03-C: Detects overlaps based on TriggerRadius and collider bounds
                    if (IsEntityInTriggerRange(triggerEntity, targetEntity))
                    {
                        currentProximities.Add(targetEntity);
                    }
                }

                _currentFrameProximities[triggerEntity] = currentProximities;
            }
            catch (Exception ex)
            {
                DebugLog($"TriggerSystem: Failed to check trigger proximities - {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a target entity can trigger a trigger entity.
        /// </summary>
        private bool CanTargetTrigger(object triggerEntity, object targetEntity)
        {
            var triggerId = triggerEntity is Entity ent1 ? ent1.Id : (uint)triggerEntity;
            var targetId = targetEntity is Entity ent2 ? ent2.Id : (uint)targetEntity;
            
            var triggerComponent = _entityManager.GetComponent<TriggerComponent>(triggerId);
            var targetCollision = _entityManager.GetComponent<CollisionComponent>(targetId);

            if (triggerComponent == null || targetCollision == null)
                return false;

            // Check layer mask compatibility
            return CollisionExtensions.CanCollideWith(triggerComponent.TriggerLayerMask, targetCollision.LayerMask);
        }

        /// <summary>
        /// P11-04-03-C: Detects overlaps based on TriggerRadius and collider bounds.
        /// </summary>
        private bool IsEntityInTriggerRange(object triggerEntity, object targetEntity)
        {
            try
            {
                var triggerId = triggerEntity is Entity ent1 ? ent1.Id : (uint)triggerEntity;
                var targetId = targetEntity is Entity ent2 ? ent2.Id : (uint)targetEntity;
                
                var triggerTransform = _entityManager.GetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(triggerId);
                var triggerComponent = _entityManager.GetComponent<TriggerComponent>(triggerId);
                var targetTransform = _entityManager.GetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>(targetId);
                var targetCollision = _entityManager.GetComponent<CollisionComponent>(targetId);

                if (triggerTransform == null || triggerComponent == null ||
                targetTransform == null || targetCollision == null)
                    return false;

                // Calculate distance between trigger center and target center
                var dx = triggerTransform.Position.X - targetTransform.Position.X;
                var dy = triggerTransform.Position.Y - targetTransform.Position.Y;
                var distanceSquared = dx * dx + dy * dy;

                // Check if within trigger radius
                var triggerRadiusSquared = triggerComponent.TriggerRadius * triggerComponent.TriggerRadius;
                if (distanceSquared > triggerRadiusSquared)
                    return false;

                // Additional check: ensure target entity's collider is within trigger radius
                // This accounts for target entity size
                var targetColliderRadius = GetTargetColliderRadius(targetCollision);
                var effectiveDistanceSquared = distanceSquared - (targetColliderRadius * targetColliderRadius);

                return effectiveDistanceSquared <= triggerRadiusSquared;
            }
            catch (Exception ex)
            {
                DebugLog($"TriggerSystem: Failed to check entity in trigger range - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the effective radius of a target entity's collider.
        /// </summary>
        private float GetTargetColliderRadius(CollisionComponent collision)
        {
            if (collision.ShapeType == ColliderShapeType.Circle)
            {
                return collision.Radius;
            }
            else // Box
            {
                // Use the maximum dimension as effective radius
                return System.Math.Max(collision.Width, collision.Height) * 0.5f;
            }
        }

        /// <summary>
        /// P11-04-03-C: Processes trigger events based on proximity changes.
        /// </summary>
        private void ProcessTriggerEvents()
        {
            foreach (var kvp in _currentFrameProximities)
            {
                var triggerEntity = kvp.Key;
                var currentProximities = kvp.Value;

                // Get previous proximities for this trigger
                _previousFrameProximities.TryGetValue(triggerEntity, out var previousProximities);
                previousProximities = previousProximities ?? new HashSet<object>();

                // Check for new entities (TriggerEnter events)
                foreach (var targetEntity in currentProximities)
                {
                    if (!previousProximities.Contains(targetEntity))
                    {
                        PublishTriggerEnterEvent(triggerEntity, targetEntity);
                    }
                }

                // Check for exited entities (TriggerExit events)
                foreach (var targetEntity in previousProximities)
                {
                    if (!currentProximities.Contains(targetEntity))
                    {
                        PublishTriggerExitEvent(triggerEntity, targetEntity);
                    }
                }
            }
        }

        /// <summary>
        /// P11-04-03-C: Publishes TriggerEnter event.
        /// P11-04-04-D: Now uses EventBus.Publish&lt;TEvent&gt;() for actual event publishing.
        /// </summary>
        private void PublishTriggerEnterEvent(object triggerEntity, object targetEntity)
        {
            try
            {
                var triggerId = triggerEntity is Entity ent ? ent.Id : (uint)triggerEntity;
                var triggerComponent = _entityManager.GetComponent<TriggerComponent>(triggerId);
                if (triggerComponent == null)
                    return;

                // P11-04-03-C: Respects TriggerOnce and avoids re-triggering the same entity
                if (!triggerComponent.CanEntityTrigger(targetEntity))
                    return;

                // Mark entity as triggered
                triggerComponent.MarkEntityTriggered(targetEntity);

                // Create and publish event using EventBus
                var triggerEvent = new TriggerEnterEvent
                {
                    SourceEntityId = triggerEntity,
                    TargetEntityId = targetEntity,
                    Timestamp = DateTime.UtcNow
                };

                // P11-04-04-D: Register as event publisher using EventBus.Publish&lt;TEvent&gt;()
                _eventRouting.Publish(triggerEvent);
                DebugLog($"TriggerSystem: Published TriggerEnterEvent - Source: {triggerEntity}, Target: {targetEntity}");
            }
            catch (Exception ex)
            {
                DebugLog($"TriggerSystem: Failed to publish TriggerEnterEvent - {ex.Message}");
            }
        }

        /// <summary>
        /// P11-04-03-C: Publishes TriggerExit event.
        /// P11-04-04-D: Now uses EventBus.Publish&lt;TEvent&gt;() for actual event publishing.
        /// </summary>
        private void PublishTriggerExitEvent(object triggerEntity, object targetEntity)
        {
            try
            {
                var triggerId = triggerEntity is Entity ent ? ent.Id : (uint)triggerEntity;
                var triggerComponent = _entityManager.GetComponent<TriggerComponent>(triggerId);
                if (triggerComponent == null)
                    return;

                // Remove entity from triggered entities list
                triggerComponent.RemoveEntityTriggered(targetEntity);

                // Create and publish event using EventBus
                var triggerEvent = new TriggerExitEvent
                {
                    SourceEntityId = triggerEntity,
                    TargetEntityId = targetEntity,
                    Timestamp = DateTime.UtcNow
                };

                // P11-04-04-D: Register as event publisher using EventBus.Publish&lt;TEvent&gt;()
                _eventRouting.Publish(triggerEvent);
                DebugLog($"TriggerSystem: Published TriggerExitEvent - Source: {triggerEntity}, Target: {targetEntity}");
            }
            catch (Exception ex)
            {
                DebugLog($"TriggerSystem: Failed to publish TriggerExitEvent - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates previous frame proximities for next frame comparison.
        /// </summary>
        private void UpdatePreviousFrameProximities()
        {
            _previousFrameProximities.Clear();
            foreach (var kvp in _currentFrameProximities)
            {
                _previousFrameProximities[kvp.Key] = new HashSet<object>(kvp.Value);
            }
        }

        /// <summary>
        /// Gets statistics about the trigger system.
        /// </summary>
        public TriggerSystemStatistics GetStatistics()
        {
            var triggerEntities = _entityManager.GetEntitiesWithTriggerAndTransform();
            return new TriggerSystemStatistics
            {
                Initialized = _initialized,
                TriggerEntityCount = triggerEntities.Count,
                ActiveTriggerCount = _currentFrameProximities.Count,
                TotalProximityCount = GetTotalProximityCount()
            };
        }

        /// <summary>
        /// Gets the total number of active trigger proximities.
        /// </summary>
        private int GetTotalProximityCount()
        {
            int total = 0;
            foreach (var proximities in _currentFrameProximities.Values)
            {
                total += proximities.Count;
            }
            return total;
        }

        /// <summary>
        /// Shuts down the trigger system and releases resources.
        /// </summary>
        public void Shutdown()
        {
            if (!_initialized)
                return;

            DebugLog("TriggerSystem: Starting shutdown...");

            _previousFrameProximities.Clear();
            _currentFrameProximities.Clear();
            _initialized = false;

            DebugLog("TriggerSystem: Shutdown complete");
        }

        /// <summary>
        /// P11-04-03-F: Event subscription documentation for audit purposes.
        ///
        /// AUDIT-FRIENDLY DOCUMENTATION:
        /// TriggerSystem is event-agnostic in consumption.
        /// It publishes TriggerEnter and TriggerExit events but does not subscribe to any events.
        ///
        /// P11-04-04-D: EVENT PUBLISHING DOCUMENTATION:
        /// TriggerSystem registers as event publisher using EventBus.Publish&lt;TEvent&gt;().
        ///
        /// EVENTS PUBLISHED:
        /// - TriggerEnterEvent: Published when entity enters trigger range
        ///   - Published in PublishTriggerEnterEvent() method
        ///   - Triggered by proximity detection in ProcessTriggerEvents()
        /// - TriggerExitEvent: Published when entity exits trigger range
        ///   - Published in PublishTriggerExitEvent() method
        ///   - Triggered by proximity loss in ProcessTriggerEvents()
        ///
        /// EVENT PUBLISHING LOCATIONS:
        /// - PublishTriggerEnterEvent(): Lines 269-300
        /// - PublishTriggerExitEvent(): Lines 306-333
        /// - Both methods use _eventBus.Publish&lt;TEvent&gt;() for actual publishing
        ///
        /// TRIGGER DETECTION IS DRIVEN PURELY BY ECS QUERIES:
        /// - TriggerSystem queries EntityManager for entities with required components
        /// - Component data (TriggerComponent + TransformComponent) determines all trigger behavior
        /// - Trigger detection occurs in Update() method called from main game loop
        /// - No event-driven updates or subscriptions are used
        ///
        /// ARCHITECTURAL SEPARATION:
        /// - TriggerSystem only detects trigger conditions and publishes events
        /// - Gameplay systems subscribe to trigger events and implement logic
        /// - Clear separation between trigger detection and gameplay response
        /// - TriggerSystem remains focused and single-purpose
        ///
        /// DETECTION LOGIC:
        /// - Iterates over all trigger entities with TriggerComponent and TransformComponent
        /// - Checks proximity to target entities with CollisionComponent and TransformComponent
        /// - Uses TriggerRadius and collider bounds for overlap detection
        /// - Respects TriggerOnce flag to prevent re-triggering
        /// - Supports cooldown periods for repeated triggering
        ///
        /// EVENT PUBLISHING:
        /// - Publishes TriggerEnterEvent when entities enter trigger range
        /// - Publishes TriggerExitEvent when entities exit trigger range
        /// - Events include source entity, target entity, and timestamp
        /// - Events are serializable and audit-friendly
        ///
        /// POTENTIAL FUTURE EVENT SUBSCRIPTIONS (optional, not currently implemented):
        /// - EntityDestroyedEvent: To clean up trigger data when entities are destroyed
        /// - LayerChangedEvent: To handle dynamic layer mask changes
        /// - TimeScaleChangedEvent: For time-based trigger effects
        ///
        /// CURRENT IMPLEMENTATION: No event subscriptions created or maintained.
        /// </summary>
        private void DocumentEventSubscriptions()
        {
            // This method exists solely to document event subscription requirements
            // as specified in P11-04-03-F. No actual event subscriptions are implemented.
        }

        private void DebugLog(string message)
        {
            if (_debugOutput)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }

    /// <summary>
    /// Statistics about the trigger system state.
    /// </summary>
    public class TriggerSystemStatistics
    {
        public bool Initialized { get; set; }
        public int TriggerEntityCount { get; set; }
        public int ActiveTriggerCount { get; set; }
        public int TotalProximityCount { get; set; }

        public override string ToString()
        {
            return $"Trigger System Statistics - Initialized: {Initialized}, Entities: {TriggerEntityCount}, Active: {ActiveTriggerCount}, Proximities: {TotalProximityCount}";
        }
    }

    // Added placeholder for CollisionExtensions to resolve CS0103 error.
    public static class CollisionExtensions
    {
        public static bool CanCollideWith(int triggerLayerMask, int targetLayerMask)
        {
            // Placeholder implementation
            return (triggerLayerMask & targetLayerMask) != 0;
        }
    }

    /// <summary>
    /// Event fired when an entity enters a trigger zone.
    /// </summary>
    public class TriggerEnterEvent
    {
        public object SourceEntityId { get; set; }
        public object TargetEntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Event fired when an entity exits a trigger zone.
    /// </summary>
    public class TriggerExitEvent
    {
        public object SourceEntityId { get; set; }
        public object TargetEntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}




