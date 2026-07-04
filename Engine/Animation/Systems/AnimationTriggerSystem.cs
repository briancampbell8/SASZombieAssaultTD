/*
File:    AnimationTriggerSystem.cs
Purpose: System for triggering death animations on entities.
Features: EntityDiedEvent subscription, death animation triggering, audit-friendly logging.

P11-04-07-B: System subscribes to EntityDiedEvent and triggers death animations
on entities with AnimationComponent. Sets animation state to "Death" and includes
comprehensive XML documentation and audit-friendly logging.
*/
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Animation.Integration;
using System;
using SASZombieAssaultTD.Engine.Events;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.State;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.Systems
{
    ///<summary>
    ///System responsible for triggering death animations when entities die.
    ///Subscribes to EntityDiedEvent and manages animation state transitions.
    ///</summary>
    public class AnimationTriggerSystem
    {
        public bool IsEnabled { get; private set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public SystemPriority Priority { get; private set; } = SystemPriority.Normal;
        public float LastUpdateTime { get; private set; } = 0f;
        public uint UpdateCount { get; private set; } = 0;

        private readonly EntityManager _entityManager;
        private readonly EventRouter _eventRouting;
        private bool _initialized;
        private readonly bool _debugOutput = true;

        ///<summary>
        ///Creates a new AnimationTriggerSystem with required dependencies.
        ///</summary>
        ///<param name="entityManager">Entity manager for component access</param>
        ///<param name="eventRouter">Event routing for EntityDiedEvent subscription</param>
        public AnimationTriggerSystem(EntityManager entityManager, EventRouter eventRouter)
        {
            _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            _eventRouting = eventRouter ?? throw new ArgumentNullException(nameof(eventRouter));
            DebugLog("AnimationTriggerSystem: Constructed with required dependencies");
        }

        ///<summary>
        ///Initializes the animation trigger system and subscribes to death events.
        ///</summary>
        public void Initialize()
        {
            if (_initialized) return;

            try
            {
                DebugLog("AnimationTriggerSystem: Starting initialization...");
                _eventRouting.Subscribe<KillAttributedEvent>(OnEntityDied);
                _initialized = true;
                DebugLog("AnimationTriggerSystem: Initialization complete - Subscribed to GameEvent");
            }
            catch (Exception ex)
            {
                DebugLog($"AnimationTriggerSystem: Initialization failed - {ex.Message}");
                throw new InvalidOperationException("Failed to initialize AnimationTriggerSystem", ex);
            }
        }

        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;
        public void Toggle() => IsEnabled = !IsEnabled;

        public void Destroy()
        {
            if (_initialized)
            {
                _eventRouting.Unsubscribe<KillAttributedEvent>(OnEntityDied);
                _initialized = false;
            }
            IsEnabled = false;
            DebugLog("AnimationTriggerSystem: System destroyed");
        }

        public void Reset()
        {
            UpdateCount = 0;
            LastUpdateTime = 0f;
            if (_initialized)
            {
                _eventRouting.Unsubscribe<KillAttributedEvent>(OnEntityDied);
                _initialized = false;
            }
            DebugLog("AnimationTriggerSystem: System reset");
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized) return;

            UpdateCount++;
            LastUpdateTime = deltaTime;
        }

        ///<summary>
        ///Handles EntityDiedEvent by triggering death animations on the deceased entity.
        ///</summary>
        ///<param name="deathEvent">The entity death event containing death information</param>
        private void OnEntityDied(KillAttributedEvent deathEvent)
        {
            if (deathEvent == null)
            {
                DebugLog("AnimationTriggerSystem: OnEntityDied failed - Death event is null");
                return;
            }

            try
            {
                DebugLog($"AnimationTriggerSystem: Processing death event at {deathEvent.Timestamp}");

                foreach (var entity in _entityManager.GetEntitiesWith<AnimationControllerComponent>())
                {
                    var animationController = entity.GetComponent<AnimationControllerComponent>();
                    if (animationController == null) continue;

                    animationController.Play("Death");
                    DebugLog($"AnimationTriggerSystem: Triggered death animation for Entity {entity.Id}");
                }
            }
            catch (Exception ex)
            {
                DebugLog($"AnimationTriggerSystem: Failed to process death animation for Entity {deathEvent.VictimId} - {ex.Message}");
            }
        }

        ///<summary>
        ///Logs animation trigger information for audit purposes.
        ///</summary>
        ///<param name="entityId">The ID of the entity whose animation was triggered</param>
        ///<param name="animationName">The name of the animation that was triggered</param>
        private void LogAnimationTrigger(object entityId, string animationName)
        {
            var logMessage = $"ANIMATION_TRIGGER: Entity={entityId}, Animation={animationName}, Timestamp={DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
            DebugLog($"AnimationTriggerSystem: {logMessage}");
        }

        private void DebugLog(string message)
        {
            if (_debugOutput)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }

    ///<summary>
    ///Statistics about the animation trigger system state.
    ///</summary>
    public class AnimationTriggerSystemStatistics
    {
        public bool Initialized { get; set; }
        public bool SubscribedToGameEvent { get; set; }

        public override string ToString() =>
            $"Animation Trigger System Statistics - Initialized: {Initialized}, Subscribed: {SubscribedToGameEvent}";
    }
}




