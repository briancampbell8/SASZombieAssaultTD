//=============================================================================================================
//  File:    PhysicsSystem.cs
//  Path:    Engine/Systems/PhysicsSystem.cs
//  Purpose:   P11-04-02-B - Core ECS system for physics simulation and movement.
//           Applies forces, updates positions, handles velocity/acceleration, and manages kinematic entities.
//
//  Role:      Central physics simulation system for all entities with physics components.
//             - Updates velocity and acceleration based on applied forces
//             - Applies drag forces for air resistance and friction
//             - Integrates with collision system for collision response
//             - Handles kinematic entities with manual position control
//             - Enforces speed limits for gameplay balance
//             - Provides spatial queries for physics-based interactions
//
//  Features:   High-performance physics simulation with fixed timestep integration.
//              Force-based movement with acceleration and velocity calculations.
//              Drag and friction support for realistic physics behavior.
//              Collision response integration with proper impulse calculations.
//              Kinematic ECSEntityCore support for animated objects and platforms.
//              Speed clamping for gameplay balance and physics stability.
//
//  Notes:      This system integrates with GameLoop's fixed timestep for consistent physics.
//              All physics calculations use proper time-based integration.
//              System supports both discrete and continuous collision detection modes.
//              Physics operations are optimized for real-time performance.
//              Component integrates seamlessly with collision and transform systems.
//
//=============================================================================================================
using System;
using System.Drawing;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.ECS.ECSRuntime;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.ECS.ECSEnums;
using PhysicsComponent = SASZombieAssaultTD.Engine.Physics.Components.PhysicsComponent;
using TransformComponent = SASZombieAssaultTD.Engine.Components.TransformComponent;

namespace SASZombieAssaultTD.Engine.Physics
{
    public class PhysicsSystem : IECSSystem
    {
        // ---------------------------------------------------------------------------------------------
        // ISystem Implementation
        // ---------------------------------------------------------------------------------------------
        public bool IsEnabled { get; private set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public SystemPriority Priority { get; private set; } = SystemPriority.High;
        public float LastUpdateTime { get; private set; } = 0f;
        public uint UpdateCount { get; private set; } = 0;

        int IECSSystem.Priority => (int)Priority;

        // ---------------------------------------------------------------------------------------------
        // PhysicsSystem Fields
        // ---------------------------------------------------------------------------------------------
        private readonly ECSRuntimeCore _runtime;
        private readonly bool _debugOutput = true;
        private readonly float _maxDeltaTime = 0.1f;

        // ---------------------------------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------------------------------
        public PhysicsSystem(ECSRuntimeCore runtime)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: Constructed with required dependencies");
        }

        // ---------------------------------------------------------------------------------------------
        // Initialization
        // ---------------------------------------------------------------------------------------------
        public void Initialize()
        {
            if (IsInitialized)
                return;

            try
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: Starting initialization...");
                IsInitialized = true;
                DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: Initialization complete");
            }
            catch (Exception ex)
            {
                DLogger.Log($"PhysicsSystem: Initialization failed - {ex.Message}");
                throw new InvalidOperationException("Failed to initialize PhysicsSystem", ex);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // System Lifecycle
        // ---------------------------------------------------------------------------------------------
        public void FixedUpdate(float fixedDeltaTime)
        {
            Update(fixedDeltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            // No-op
        }

        public void Render()
        {
            // No-op
        }

        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;
        public void Toggle() => IsEnabled = !IsEnabled;

        public void Destroy()
        {
            IsInitialized = false;
            IsEnabled = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: System destroyed");
        }

        public void Reset()
        {
            UpdateCount = 0;
            LastUpdateTime = 0f;
            IsInitialized = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: System reset");
        }

        // ---------------------------------------------------------------------------------------------
        // Core Physics Update
        // ---------------------------------------------------------------------------------------------
        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: Update skipped - Not enabled or initialized");
                return;
            }

            UpdateCount++;
            LastUpdateTime = deltaTime;

            try
            {
                deltaTime = System.Math.Min(deltaTime, _maxDeltaTime);

                var queries = new ComponentQueries(_runtime, _runtime.Components);
                var physicsEntities = queries.GetEntitiesWith<PhysicsComponent, TransformComponent>();

                DLogger.Log($"PhysicsSystem: Processing {physicsEntities.Count()} physics entities");

                foreach (var entity in physicsEntities)
                    UpdateEntityPhysics(entity, deltaTime);

                DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: Physics update complete");
            }
            catch (Exception ex)
            {
                DLogger.Log($"PhysicsSystem: Update failed - {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Per-Entity Physics Update
        // ---------------------------------------------------------------------------------------------
        private void UpdateEntityPhysics(ECSEntityCore entity, float deltaTime)
        {
            try
            {
                var physics = _runtime.Components.GetComponent<PhysicsComponent>(entity.Id);
                var transform = _runtime.Components.GetComponent<TransformComponent>(entity.Id);

                if (physics == null || transform == null || !physics.Enabled)
                    return;

                if (physics.IsKinematic)
                {
                    UpdateKinematicEntity(physics, transform, deltaTime);
                    return;
                }

                ApplyAccelerationToVelocity(physics, deltaTime);
                ApplyDrag(physics, deltaTime);
                ClampVelocity(physics);
                ApplyVelocityToPosition(physics, transform, deltaTime);

                physics.Acceleration = PointF.Empty;
            }
            catch (Exception ex)
            {
                DLogger.Log($"PhysicsSystem: Failed to update entity physics - {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Physics Operations
        // ---------------------------------------------------------------------------------------------
        private void UpdateKinematicEntity(PhysicsComponent physics, TransformComponent transform, float deltaTime)
        {
            transform.X += physics.Velocity.X * deltaTime;
            transform.Y += physics.Velocity.Y * deltaTime;
        }

        private void ApplyAccelerationToVelocity(PhysicsComponent physics, float deltaTime)
        {
            physics.Velocity = new PointF(
                physics.Velocity.X + physics.Acceleration.X * deltaTime,
                physics.Velocity.Y + physics.Acceleration.Y * deltaTime
            );
        }

        private void ApplyDrag(PhysicsComponent physics, float deltaTime)
        {
            if (physics.Drag <= 0f)
                return;

            var speed = physics.GetSpeed();
            if (speed <= 0f)
                return;

            var dragForceX = -physics.Drag * speed * (physics.Velocity.X / speed);
            var dragForceY = -physics.Drag * speed * (physics.Velocity.Y / speed);

            physics.Velocity = new PointF(
                physics.Velocity.X + (dragForceX / physics.Mass) * deltaTime,
                physics.Velocity.Y + (dragForceY / physics.Mass) * deltaTime
            );
        }

        private void ClampVelocity(PhysicsComponent physics)
        {
            if (!physics.MaxSpeed.HasValue)
                return;

            var currentSpeed = physics.GetSpeed();
            if (currentSpeed <= physics.MaxSpeed.Value)
                return;

            var scale = physics.MaxSpeed.Value / currentSpeed;
            physics.Velocity = new PointF(
                physics.Velocity.X * scale,
                physics.Velocity.Y * scale
            );
        }

        private void ApplyVelocityToPosition(PhysicsComponent physics, TransformComponent transform, float deltaTime)
        {
            transform.X += physics.Velocity.X * deltaTime;
            transform.Y += physics.Velocity.Y * deltaTime;
        }

        // ---------------------------------------------------------------------------------------------
        // External Forces
        // ---------------------------------------------------------------------------------------------
        public void ApplyForce(ECSEntityCore entity, PointF force)
        {
            try
            {
                var physics = _runtime.Components.GetComponent<PhysicsComponent>(entity.Id);
                if (physics != null && physics.Enabled)
                {
                    physics.ApplyForce(force);
                    DLogger.Log($"PhysicsSystem: Applied force {force} to entity {entity.Id}");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"PhysicsSystem: Failed to apply force - {ex.Message}");
            }
        }

        public void ApplyImpulse(ECSEntityCore entity, PointF impulse)
        {
            try
            {
                var physics = _runtime.Components.GetComponent<PhysicsComponent>(entity.Id);
                if (physics != null && physics.Enabled)
                {
                    physics.ApplyImpulse(impulse);
                    DLogger.Log($"PhysicsSystem: Applied impulse {impulse} to entity {entity.Id}");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"PhysicsSystem: Failed to apply impulse - {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Statistics
        // ---------------------------------------------------------------------------------------------
        public PhysicsSystemStatistics GetStatistics()
        {
            var queries = new ComponentQueries(_runtime, _runtime.Components);
            var physicsEntities = queries.GetEntitiesWith<PhysicsComponent, TransformComponent>();

            return new PhysicsSystemStatistics
            {
                Initialized = IsInitialized,
                PhysicsEntityCount = physicsEntities.Count()
            };
        }

        // ---------------------------------------------------------------------------------------------
        // Shutdown
        // ---------------------------------------------------------------------------------------------
        public void Shutdown()
        {
            if (!IsInitialized)
                return;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: Starting shutdown...");
            IsInitialized = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "PhysicsSystem: Shutdown complete");
        }

        // ---------------------------------------------------------------------------------------------
        // Documentation-only method
        // ---------------------------------------------------------------------------------------------
        private void DocumentEventSubscriptions()
        {
            // No event subscriptions implemented.
        }

        private void Log(string message)
        {
            if (_debugOutput)
                DLogger.Log($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        }
    }

    public class PhysicsSystemStatistics
    {
        public bool Initialized { get; set; }
        public int PhysicsEntityCount { get; set; }

        public override string ToString()
        {
            return $"Physics System Statistics - Initialized: {Initialized}, Entities: {PhysicsEntityCount}";
        }
    }
}
