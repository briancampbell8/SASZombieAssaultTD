/*
File:    AnimationSystem.cs
Purpose: P11-16-05 - System for managing animation playback and state machines.
*/
using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Animation.Events;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.ECS;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;
using DamageComponent = SASZombieAssaultTD.Engine.Components.DamageComponent;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Animation.Systems
//
{
    ///<summary>
    ///P11-16-05: System for managing animation playback and state machines.
    ///Updates animation controllers and state machines based on deltaTime.
    ///</summary>
    public sealed class AnimationSystem : ISystem
    {
        public bool IsEnabled { get; private set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public SystemPriority Priority { get; private set; } = SystemPriority.Normal;
        public float LastUpdateTime { get; private set; } = 0f;
        public uint UpdateCount { get; private set; } = 0;

        private readonly ECSWorld _ecsWorld;
        private readonly Dictionary<uint, AnimationControllerComponent> _animationControllers = new();
        private readonly Dictionary<uint, AnimationStateMachine> _stateMachines = new();
        private int _entitiesProcessed;
        private float _totalAnimationTime;
        private int _clipsUpdated;
        private int _statesUpdated;
        private int _transitionsTriggered;

        public int EntitiesProcessed => _entitiesProcessed;
        public float TotalAnimationTime => _totalAnimationTime;
        public int ClipsUpdated => _clipsUpdated;
        public int StatesUpdated => _statesUpdated;
        public int TransitionsTriggered => _transitionsTriggered;

        public event Action<AnimationControllerComponent, string>? OnAnimationStarted;
        public event Action<AnimationControllerComponent, string>? OnAnimationCompleted;
        public event Action<AnimationControllerComponent, AnimationEvent>? OnAnimationEventFired;
        public event Action<AnimationStateMachine, AnimationTransition, object>? OnStateMachineTransition;

        public AnimationSystem(ECSWorld ecsWorld)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
            DLogger.Log(LogSubsystems.Animation, LogLevel.Info, "AnimationSystem: Initialized");
        }

        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Info, "AnimationSystem: System initialized");
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized) return;

            ResetFrameStats(deltaTime);

            try
            {
                UpdateAnimationControllers(deltaTime);
                UpdateStateMachines(deltaTime);
                UpdateRenderableComponents();

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Processed {_entitiesProcessed} entities, {_totalAnimationTime:F3}s total animation time");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error, $"AnimationSystem: Error during update: {ex.Message}");
            }
        }

        private void ResetFrameStats(float deltaTime)
        {
            UpdateCount++;
            LastUpdateTime = deltaTime;
            _entitiesProcessed = 0;
            _totalAnimationTime = 0f;
            _clipsUpdated = 0;
            _statesUpdated = 0;
            _transitionsTriggered = 0;
        }

        private void UpdateAnimationControllers(float deltaTime)
        {
            foreach (var entity in _ecsWorld.GetEntitiesWith<AnimationControllerComponent>())
            {
                var controller = entity.GetComponent<AnimationControllerComponent>();
                if (controller == null) continue;

                var previousTime = controller.PlaybackTime;
                var previousClip = controller.CurrentClip;

                controller.Update(deltaTime);
                _totalAnimationTime += deltaTime;
                _clipsUpdated++;
                _entitiesProcessed++;

                FireAnimationEvents(controller, previousTime, previousClip);
            }
        }

        private void FireAnimationEvents(AnimationControllerComponent controller, float previousTime, string? previousClip)
        {
            var currentClip = controller.CurrentClip;
            var currentTime = controller.PlaybackTime;
            var clip = GetClip(currentClip);

            if (previousClip != currentClip && !string.IsNullOrEmpty(currentClip))
            {
                OnAnimationStarted?.Invoke(controller, currentClip);
                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} started animation '{currentClip}'");
            }

            if (previousClip == currentClip && clip != null && currentTime >= clip.Duration)
            {
                OnAnimationCompleted?.Invoke(controller, currentClip);
                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} completed animation '{currentClip}'");
            }

            if (clip == null) return;

            foreach (var animationEvent in clip.Events)
            {
                if (EventCrossed(controller, animationEvent, previousTime, currentTime) && !HasEventFired(controller, animationEvent))
                {
                    OnAnimationEventFired?.Invoke(controller, animationEvent);
                    MarkEventFired(controller, animationEvent);
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} fired event '{animationEvent.EventName}' at time {animationEvent.Timestamp:F3}");
                    HandleGameplayCallbacks(controller, animationEvent);
                }
            }
        }

        private static bool EventCrossed(AnimationControllerComponent controller, AnimationEvent animationEvent, float previousTime, float currentTime)
        {
            return controller.Speed >= 0
                ? previousTime < animationEvent.Timestamp && currentTime >= animationEvent.Timestamp
                : previousTime > animationEvent.Timestamp && currentTime <= animationEvent.Timestamp;
        }

        private bool HasEventFired(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var eventKey = $"{animationEvent.EventName}_{animationEvent.Timestamp}";
            return controller.HasParameter(eventKey);
        }

        private void MarkEventFired(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var eventKey = $"{animationEvent.EventName}_{animationEvent.Timestamp}";
            controller.SetParameter(eventKey, 1f);
        }

        private void HandleGameplayCallbacks(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            switch (animationEvent.EventName)
            {
                case "FireWeapon":
                    HandleFireWeaponEvent(controller, animationEvent);
                    break;
                case "Footstep":
                    HandleFootstepEvent(controller, animationEvent);
                    break;
                case "PlaySound":
                    HandlePlaySoundEvent(controller, animationEvent);
                    break;
                case "SpawnEffect":
                    HandleSpawnEffectEvent(controller, animationEvent);
                    break;
                case "DamageArea":
                    HandleDamageAreaEvent(controller, animationEvent);
                    break;
                default:
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: No specific handler for event '{animationEvent.EventName}'");
                    break;
            }
        }

        private void HandleFireWeaponEvent(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var damage = animationEvent.GetParameter<float>("Damage");
            if (damage.Equals(default(float))) damage = 10f;
            var damageComponent = new DamageComponent { Damage = damage };
            controller.Entity.AddComponent(damageComponent);
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} fired weapon for {damage} damage");
        }

        private void HandleFootstepEvent(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var soundName = animationEvent.GetParameter<string>("Sound");
            if (string.IsNullOrEmpty(soundName)) soundName = "footstep";
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} footstep sound '{soundName}'");
        }

        private void HandlePlaySoundEvent(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var soundName = animationEvent.GetParameter<string>("SoundName");
            if (string.IsNullOrEmpty(soundName)) soundName = "unknown";
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} playing sound '{soundName}'");
        }

        private void HandleSpawnEffectEvent(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var effectType = animationEvent.GetParameter<string>("EffectType");
            if (string.IsNullOrEmpty(effectType)) effectType = "default";
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} spawning effect '{effectType}'");
        }

        private void HandleDamageAreaEvent(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var damage = animationEvent.GetParameter<float>("Damage");
            if (damage.Equals(default(float))) damage = 25f;
            var damageComponent = new DamageComponent { Damage = damage };
            controller.Entity.AddComponent(damageComponent);
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationSystem: Entity {controller.Entity.Id} created area damage: {damage}");
        }

        private void UpdateStateMachines(float deltaTime)
        {
            //Update all animation state machines
            foreach (var entity in _ecsWorld.GetEntitiesWith<AnimationStateMachineComponent>())
            {
                var stateMachine = entity.GetComponent<AnimationStateMachineComponent>();
                if (stateMachine != null && stateMachine.IsRunning)
                {
                    stateMachine.Update(deltaTime);
                }
            }
        }

        private void UpdateRenderableComponents()
        {
            foreach (var entity in _ecsWorld.GetEntitiesWith<ECS.SpriteComponent>())
            {
                var renderable = entity.GetComponent<ECS.SpriteComponent>();
                var animationController = entity.GetComponent<AnimationControllerComponent>();

                if (animationController != null)
                {
                    UpdateRenderableFromController(renderable, animationController);
                }
            }
        }

        private void UpdateRenderableFromController(ECS.SpriteComponent renderable, AnimationControllerComponent controller)
        {
            var currentClip = controller.GetCurrentClip();
            if (currentClip == null) return;

            renderable.SpriteIndex = controller.GetCurrentSpriteIndex() ?? renderable.SpriteIndex;
            renderable.TransformOffset = controller.GetCurrentTransformOffset();
            var colorTint = controller.GetCurrentColorTint();
            renderable.ColorTint = new SASZombieAssaultTD.Engine.Core.Color(colorTint);
            renderable.AnimationTime = controller.PlaybackTime;
        }

        public AnimationClip? GetClip(string clipName)
        {
            return new AnimationClip(clipName, 1.0f, false);
        }

        //Missing ISystem interface methods
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (!IsEnabled || !IsInitialized) return;
            //Animation system doesn't need fixed update logic
        }

        public void LateUpdate(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized) return;
            //Animation system doesn't need late update logic
        }

        public void Render()
        {
            if (!IsEnabled || !IsInitialized) return;
            //Animation system doesn't need direct rendering logic
        }

        public void Enable()
        {
            IsEnabled = true;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Info, "AnimationSystem: System enabled");
        }

        public void Disable()
        {
            IsEnabled = false;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Info, "AnimationSystem: System disabled");
        }

        public void Toggle()
        {
            IsEnabled = !IsEnabled;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Info, $"AnimationSystem: System {(IsEnabled ? "enabled" : "disabled")}");
        }

        public void Destroy()
        {
            _animationControllers.Clear();
            _stateMachines.Clear();
            IsEnabled = false;
            IsInitialized = false;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Info, "AnimationSystem: System destroyed");
        }

        public void Reset()
        {
            _animationControllers.Clear();
            _stateMachines.Clear();
            _entitiesProcessed = 0;
            _totalAnimationTime = 0f;
            UpdateCount = 0;
            LastUpdateTime = 0f;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Info, "AnimationSystem: System reset");
        }
    }
}









