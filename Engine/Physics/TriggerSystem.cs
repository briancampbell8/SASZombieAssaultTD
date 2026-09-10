// ====================================================================================================
//  FILE: TriggerSystem.cs
//  PATH: ./Engine/Physics/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the TriggerSystem module.
//
//  RESPONSIBILITIES:
//      - Provide Initialize() behavior for the Core subsystem.
//      - Provide FixedUpdate() behavior for the Core subsystem.
//      - Provide LateUpdate() behavior for the Core subsystem.
//      - Provide Render() behavior for the Core subsystem.
//      - Provide Enable() behavior for the Core subsystem.
//      - Provide Disable() behavior for the Core subsystem.
//      - Provide Toggle() behavior for the Core subsystem.
//      - Provide Destroy() behavior for the Core subsystem.
//      - Provide Reset() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//      - Provide GetStatistics() behavior for the Core subsystem.
//      - Provide Shutdown() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide CanCollideWith() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    TriggerSystem.cs
Purpose: Subsystem for trigger event detection and publishing.
Features: Proximity detection, trigger radius checking, event publishing, trigger-once handling.

P11-04-03-C: Subsystem iterates over trigger entities, checks proximity, detects overlaps,
publishes TriggerEnter and TriggerExit events, respects TriggerOnce and avoids gameplay logic.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Physics.Collision;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Subsystem for trigger event detection and publishing. P11-04-03-C: Iterates over entities with TriggerComponent
    /// and TransformComponent, checks for proximity to other entities with CollisionComponent and TransformComponent,
    /// detects overlaps based on TriggerRadius and collider bounds, publishes TriggerEnter and TriggerExit events,
    /// respects TriggerOnce and avoids re-triggering the same ECSEntityCore, and does not apply gameplay logic directly.
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
        private readonly ECSEntityCore _ECSEntityCore;
        private readonly ECSComponents _components;
        private readonly EventRouting _eventRouting;

        private bool _initialized;
        private readonly bool _debugOutput = true;

        private readonly Dictionary<ECSEntityCore, HashSet<ECSEntityCore>> _previousFrameProximities =
            new Dictionary<ECSEntityCore, HashSet<ECSEntityCore>>();

        private readonly Dictionary<ECSEntityCore, HashSet<ECSEntityCore>> _currentFrameProximities =
            new Dictionary<ECSEntityCore, HashSet<ECSEntityCore>>();

        /// <summary>
        /// Creates a new TriggerSystem with required dependencies.
        /// </summary>
        /// <param name="ECSEntityCore">Entity manager for component access</param>
        /// <param name="components">Component storage for ECS entities</param>
        /// <param name="eventRouting">Event routing for publishing trigger events</param>
        public TriggerSystem(ECSEntityCore ECSEntityCore, ECSComponents components, EventRouting eventRouting)
        {
            _ECSEntityCore = ECSEntityCore ?? throw new ArgumentNullException(nameof(ECSEntityCore));
            _components = components ?? throw new ArgumentNullException(nameof(components));
            _eventRouting = eventRouting ?? throw new ArgumentNullException(nameof(eventRouting));

            DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: Constructed with required dependencies");
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
                DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: Starting initialization...");

                _initialized = true;
                IsInitialized = true;

                DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: Initialization complete");
            }
            catch (Exception ex)
            {
                DLogger.Log($"TriggerSystem: Initialization failed - {ex.Message}");
                throw new InvalidOperationException("Failed to initialize TriggerSystem", ex);
            }
        }

        // ISystem Implementation
        public void FixedUpdate(float fixedDeltaTime)
        {
            // TriggerSystem doesn't need fixed-step updates
        }

        public void LateUpdate(float deltaTime)
        {
            // TriggerSystem doesn't need late updates
        }

        public void Render()
        {
            // TriggerSystem doesn't render anything
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
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: System destroyed");
        }

        public void Reset()
        {
            UpdateCount = 0;
            LastUpdateTime = 0f;
            _previousFrameProximities.Clear();
            _currentFrameProximities.Clear();
            _initialized = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: System reset");
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: Update failed - Not enabled or initialized");
                return;
            }

            UpdateCount++;
            LastUpdateTime = deltaTime;

            try
            {
                _currentFrameProximities.Clear();

                var triggerEntities = _ECSEntityCore.GetEntitiesWithTriggerAndTransform;
                DLogger.Log($"TriggerSystem: Processing {triggerEntities.Count} trigger entities");

                var targetEntities = _ECSEntityCore.GetEntitiesWithCollisionAndTransform;
                var targetEntitiesList = targetEntities.ToList();

                foreach (var triggerEntity in triggerEntities)
                {
                    CheckTriggerProximities(triggerEntity, targetEntitiesList);
                }

                ProcessTriggerEvents();
                UpdatePreviousFrameProximities();

                DLogger.Log($"TriggerSystem: Processed {_currentFrameProximities.Count} trigger proximities");
            }
            catch (Exception ex)
            {
                DLogger.Log($"TriggerSystem: Update failed - {ex.Message}");
            }
        }

        /// <summary>
        /// P11-04-03-C: Checks proximities for a single trigger ECSEntityCore.
        /// </summary>
        private void CheckTriggerProximities(ECSEntityCore triggerEntity, IReadOnlyList<ECSEntityCore> targetEntities)
        {
            try
            {
                var triggerId = triggerEntity.Id;
                var triggerComponent = _components.GetComponent<TriggerComponent>(triggerId);
                var triggerTransform = _components.GetComponent<TransformComponent>(triggerId);

                if (triggerComponent == null || triggerTransform == null ||
                    !triggerComponent.Enabled || !triggerComponent.IsTrigger)
                    return;

                var currentProximities = new HashSet<ECSEntityCore>();

                foreach (var targetEntity in targetEntities)
                {
                    if (ReferenceEquals(triggerEntity, targetEntity))
                        continue;

                    if (!CanTargetTrigger(triggerEntity, targetEntity))
                        continue;

                    if (IsEntityInTriggerRange(triggerEntity, targetEntity))
                    {
                        currentProximities.Add(targetEntity);
                    }
                }

                _currentFrameProximities[triggerEntity] = currentProximities;
            }
            catch (Exception ex)
            {
                DLogger.Log($"TriggerSystem: Failed to check trigger proximities - {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a target ECSEntityCore can trigger a trigger ECSEntityCore.
        /// </summary>
        private bool CanTargetTrigger(ECSEntityCore triggerEntity, ECSEntityCore targetEntity)
        {
            var triggerId = triggerEntity.Id;
            var targetId = targetEntity.Id;

            var triggerComponent = _components.GetComponent<TriggerComponent>(triggerId);
            var targetCollision = _components.GetComponent<CollisionComponent>(targetId);

            if (triggerComponent == null || targetCollision == null)
                return false;

            return CollisionExtensions.CanCollideWith(triggerComponent.TriggerLayerMask, targetCollision.LayerMask);
        }

        /// <summary>
        /// P11-04-03-C: Detects overlaps based on TriggerRadius and collider bounds.
        /// </summary>
        private bool IsEntityInTriggerRange(ECSEntityCore triggerEntity, ECSEntityCore targetEntity)
        {
            try
            {
                var triggerId = triggerEntity.Id;
                var targetId = targetEntity.Id;

                var triggerTransform = _components.GetComponent<TransformComponent>(triggerId);
                var triggerComponent = _components.GetComponent<TriggerComponent>(triggerId);
                var targetTransform = _components.GetComponent<TransformComponent>(targetId);
                var targetCollision = _components.GetComponent<CollisionComponent>(targetId);

                if (triggerTransform == null || triggerComponent == null ||
                    targetTransform == null || targetCollision == null)
                    return false;

                var dx = triggerTransform.Position.X - targetTransform.Position.X;
                var dy = triggerTransform.Position.Y - targetTransform.Position.Y;
                var distanceSquared = dx * dx + dy * dy;

                var triggerRadiusSquared = triggerComponent.TriggerRadius * triggerComponent.TriggerRadius;
                if (distanceSquared > triggerRadiusSquared)
                    return false;

                var targetColliderRadius = GetTargetColliderRadius(targetCollision);
                var effectiveDistanceSquared = distanceSquared - (targetColliderRadius * targetColliderRadius);

                return effectiveDistanceSquared <= triggerRadiusSquared;
            }
            catch (Exception ex)
            {
                DLogger.Log($"TriggerSystem: Failed to check entity in trigger range - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the effective radius of a target ECSEntityCore's collider.
        /// </summary>
        private float GetTargetColliderRadius(CollisionComponent collision)
        {
            if (collision.ShapeType == ColliderShapeType.Circle)
            {
                return collision.Radius;
            }

            return System.Math.Max(collision.Width, collision.Height) * 0.5f;
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

                _previousFrameProximities.TryGetValue(triggerEntity, out var previousProximities);
                previousProximities ??= new HashSet<ECSEntityCore>();

                foreach (var targetEntity in currentProximities)
                {
                    if (!previousProximities.Contains(targetEntity))
                    {
                        PublishTriggerEnterEvent(triggerEntity, targetEntity);
                    }
                }

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
        /// P11-04-03-C: Publishes TriggerEnter event. P11-04-04-D: Now uses EventRouting.Publish&lt;TEvent&gt;() for actual
        /// event publishing.
        /// </summary>
        private void PublishTriggerEnterEvent(ECSEntityCore triggerEntity, ECSEntityCore targetEntity)
        {
            try
            {
                var triggerId = triggerEntity.Id;
                var triggerComponent = _components.GetComponent<TriggerComponent>(triggerId);
                if (triggerComponent == null)
                    return;

                if (!triggerComponent.CanEntityTrigger(targetEntity))
                    return;

                triggerComponent.MarkEntityTriggered(targetEntity);

                var triggerEvent = new TriggerEnterEvent
                {
                    SourceEntityId = triggerEntity,
                    TargetEntityId = targetEntity,
                    Timestamp = DateTime.UtcNow
                };

                _eventRouting.Publish(triggerEvent);
                DLogger.Log($"TriggerSystem: Published TriggerEnterEvent - Source: {triggerEntity}, Target: {targetEntity}");
            }
            catch (Exception ex)
            {
                DLogger.Log($"TriggerSystem: Failed to publish TriggerEnterEvent - {ex.Message}");
            }
        }

        /// <summary>
        /// P11-04-03-C: Publishes TriggerExit event. P11-04-04-D: Now uses EventRouting.Publish&lt;TEvent&gt;() for actual
        /// event publishing.
        /// </summary>
        private void PublishTriggerExitEvent(ECSEntityCore triggerEntity, ECSEntityCore targetEntity)
        {
            try
            {
                var triggerId = triggerEntity.Id;
                var triggerComponent = _components.GetComponent<TriggerComponent>(triggerId);
                if (triggerComponent == null)
                    return;

                triggerComponent.RemoveEntityTriggered(targetEntity);

                var triggerEvent = new TriggerExitEvent
                {
                    SourceEntityId = triggerEntity,
                    TargetEntityId = targetEntity,
                    Timestamp = DateTime.UtcNow
                };

                _eventRouting.Publish(triggerEvent);
                DLogger.Log($"TriggerSystem: Published TriggerExitEvent - Source: {triggerEntity}, Target: {targetEntity}");
            }
            catch (Exception ex)
            {
                DLogger.Log($"TriggerSystem: Failed to publish TriggerExitEvent - {ex.Message}");
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
                _previousFrameProximities[kvp.Key] = new HashSet<ECSEntityCore>(kvp.Value);
            }
        }

        /// <summary>
        /// Gets statistics about the trigger system.
        /// </summary>
        public TriggerSystemStatistics GetStatistics()
        {
            var triggerEntities = _ECSEntityCore.GetEntitiesWithTriggerAndTransform;
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

            DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: Starting shutdown...");

            _previousFrameProximities.Clear();
            _currentFrameProximities.Clear();
            _initialized = false;
            IsInitialized = false;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "TriggerSystem: Shutdown complete");
        }

        /// <summary>
        /// P11-04-03-F: Event subscription documentation for audit purposes.
        /// </summary>
        private void DocumentEventSubscriptions()
        {
            // Documentation-only method, no runtime behavior.
        }

        private void Log(string message)
        {
            if (_debugOutput)
            {
                DLogger.Log($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
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

    /// <summary>
    /// Event fired when an ECSEntityCore enters a trigger zone.
    /// </summary>
    public class TriggerEnterEvent
    {
        public object SourceEntityId { get; set; }
        public object TargetEntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Event fired when an ECSEntityCore exits a trigger zone.
    /// </summary>
    public class TriggerExitEvent
    {
        public object SourceEntityId { get; set; }
        public object TargetEntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
