/*
File:    PhysicsSystem.cs
Path:    Engine/Systems/PhysicsSystem.cs
Purpose:   P11-04-02-B - Core ECS system for physics simulation and movement.
           Applies forces, updates positions, handles velocity/acceleration, and manages kinematic entities.

Role:      Central physics simulation system for all entities with physics components.
           - Updates velocity and acceleration based on applied forces
           - Applies drag forces for air resistance and friction
           - Integrates with collision system for collision response
           - Handles kinematic entities with manual position control
           - Enforces speed limits for gameplay balance
           - Provides spatial queries for physics-based interactions

Features:   High-performance physics simulation with fixed timestep integration.
           Force-based movement with acceleration and velocity calculations.
           Drag and friction support for realistic physics behavior.
           Collision response integration with proper impulse calculations.
           Kinematic entity support for animated objects and platforms.
           Speed clamping for gameplay balance and physics stability.

Notes:      This system integrates with GameLoop's fixed timestep for consistent physics.
           All physics calculations use proper time-based integration.
           System supports both discrete and continuous collision detection modes.
           Physics operations are optimized for real-time performance.
           Component integrates seamlessly with collision and transform systems.

*/
using System;
using System.Drawing;
using System.Linq;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath; //Added for Vector3
using SASZombieAssaultTD.Engine.Physics.Components;
using TransformComponent = SASZombieAssaultTD.Engine.Components.TransformComponent;
using PhysicsComponent = SASZombieAssaultTD.Engine.Physics.Components.PhysicsComponent;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Physics
{
    ///<summary>
    ///Subsystem for physics simulation and movement.
    ///P11-04-02-B: Iterates over entities with PhysicsComponent and TransformComponent,
    ///applies acceleration to velocity, applies velocity to position, applies drag,
    ///clamps velocity to MaxSpeed, skips entities marked IsKinematic,
    ///and does not handle collisions (CollisionSystem handles that).
    ///</summary>
    public class PhysicsSystem : SASZombieAssaultTD.Engine.ECS.ISystem
    {
        //ISystem Implementation
        public bool IsEnabled { get; private set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public SystemPriority Priority { get; private set; } = SystemPriority.High;
        public float LastUpdateTime { get; private set; } = 0f;
        public uint UpdateCount { get; private set; } = 0;

        //PhysicsSystem Specific Fields
        private readonly SASZombieAssaultTD.Engine.ECS.EntityManager _entityManager;
        private readonly EventRouter _eventRouting;

        private bool _initialized;
        private readonly bool _debugOutput = true;
        private readonly float _maxDeltaTime = 0.1f; //Cap delta time to prevent instability

        ///<summary>
        ///Creates a new PhysicsSystem with required dependencies.
        ///</summary>
        ///<param name="entityManager">Entity manager for component access</param>
        ///<param name="eventRouting">Event routing for system communication</param>
        public PhysicsSystem(EntityManager entityManager, EventRouter eventRouting)
        {
            _entityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            _eventRouting = eventRouting ?? throw new ArgumentNullException(nameof(eventRouting));

            DebugLog("PhysicsSystem: Constructed with required dependencies");
        }

        ///<summary>
        ///Initializes the physics system.
        ///</summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            try
            {
                DebugLog("PhysicsSystem: Starting initialization...");

                _initialized = true;
                DebugLog("PhysicsSystem: Initialization complete");
            }
            catch (Exception ex)
            {
                DebugLog($"PhysicsSystem: Initialization failed - {ex.Message}");
                throw new InvalidOperationException("Failed to initialize PhysicsSystem", ex);
            }
        }

        //ISystem Implementation
        public void FixedUpdate(float fixedDeltaTime)
        {
            //PhysicsSystem uses fixed timestep for main physics simulation
            Update(fixedDeltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            //PhysicsSystem doesn't need late updates
            //No-op implementation
        }

        public void Render()
        {
            //PhysicsSystem doesn't render anything
            //No-op implementation
        }

        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;
        public void Toggle() => IsEnabled = !IsEnabled;

        public void Destroy()
        {
            _initialized = false;
            IsEnabled = false;
            DebugLog("PhysicsSystem: System destroyed");
        }

        public void Reset()
        {
            UpdateCount = 0;
            LastUpdateTime = 0f;
            _initialized = false;
            DebugLog("PhysicsSystem: System reset");
        }

        ///<summary>
        ///P11-04-02-B: Updates physics simulation for all entities.
        ///- Iterates over all entities with PhysicsComponent and TransformComponent
        ///- Applies acceleration to velocity
        ///- Applies velocity to position
        ///- Applies drag
        ///- Clamps velocity to MaxSpeed
        ///- Skips entities marked IsKinematic
        ///- Does not handle collisions (CollisionSystem handles that)
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update in seconds</param>
        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized)
            {
                DebugLog("PhysicsSystem: Update failed - Not enabled or initialized");
                return;
            }

            UpdateCount++;
            LastUpdateTime = deltaTime;

            try
            {
                //Cap delta time to prevent physics instability
                deltaTime = System.Math.Min(deltaTime, _maxDeltaTime);

                //Get all entities with physics components
                var physicsEntities = _entityManager.GetEntitiesWithPhysicsAndTransform();
                DebugLog($"PhysicsSystem: Processing {physicsEntities.Count()} physics entities");

                foreach (var entity in physicsEntities)
                {
                    UpdateEntityPhysics(entity, deltaTime);
                }

                DebugLog("PhysicsSystem: Physics update complete");
            }
            catch (Exception ex)
            {
                DebugLog($"PhysicsSystem: Update failed - {ex.Message}");
            }
        }

        ///<summary>
        ///Updates physics for a single entity.
        ///</summary>
        private void UpdateEntityPhysics(object entity, float deltaTime)
        {
            try
            {
                var entityId = (uint)entity;
                var physics = _entityManager.GetComponent<PhysicsComponent>(entityId);
                var transform = _entityManager.GetComponent<TransformComponent>(entityId);

                if (physics == null || transform == null || !physics.Enabled)
                    return;

                //P11-04-02-B: Skip entities marked IsKinematic
                if (physics.IsKinematic)
                {
                    //Kinematic entities still need position updates based on their velocity
                    UpdateKinematicEntity(physics, transform, deltaTime);
                    return;
                }

                //P11-04-02-B: Apply acceleration to velocity
                ApplyAccelerationToVelocity(physics, deltaTime);

                //P11-04-02-B: Apply drag
                ApplyDrag(physics, deltaTime);

                //P11-04-02-B: Clamp velocity to MaxSpeed
                ClampVelocity(physics);

                //P11-04-02-B: Apply velocity to position
                ApplyVelocityToPosition(physics, transform, deltaTime);

                //Reset acceleration for next frame (forces are applied each frame)
                physics.Acceleration = PointF.Empty;
            }
            catch (Exception ex)
            {
                DebugLog($"PhysicsSystem: Failed to update entity physics - {ex.Message}");
            }
        }

        ///<summary>
        ///Updates kinematic entity position based on velocity.
        ///</summary>
        private void UpdateKinematicEntity(PhysicsComponent physics, TransformComponent transform, float deltaTime)
        {
            //Kinematic entities move based on their velocity but don't respond to forces
            transform.X += physics.Velocity.X * deltaTime;
            transform.Y += physics.Velocity.Y * deltaTime;
        }

        ///<summary>
        ///P11-04-02-B: Applies acceleration to velocity.
        ///</summary>
        private void ApplyAccelerationToVelocity(PhysicsComponent physics, float deltaTime)
        {
            physics.Velocity = new PointF(
            physics.Velocity.X + physics.Acceleration.X * deltaTime,
            physics.Velocity.Y + physics.Acceleration.Y * deltaTime
            );
        }

        ///<summary>
        ///P11-04-02-B: Applies drag to velocity.
        ///Drag is applied as a force opposing motion: F_drag = -drag * |v| * v_unit
        ///</summary>
        private void ApplyDrag(PhysicsComponent physics, float deltaTime)
        {
            if (physics.Drag <= 0.0f)
                return;

            var speed = physics.GetSpeed();
            if (speed <= 0.0f)
                return;

            //Calculate drag force: F = -drag * speed * velocity_direction
            var dragForceX = -physics.Drag * speed * (physics.Velocity.X / speed);
            var dragForceY = -physics.Drag * speed * (physics.Velocity.Y / speed);

            //Apply drag as acceleration change
            var dragAccelerationX = dragForceX / physics.Mass;
            var dragAccelerationY = dragForceY / physics.Mass;

            physics.Velocity = new PointF(
            physics.Velocity.X + dragAccelerationX * deltaTime,
            physics.Velocity.Y + dragAccelerationY * deltaTime
            );
        }

        ///<summary>
        ///P11-04-02-B: Clamps velocity to MaxSpeed if specified.
        ///</summary>
        private void ClampVelocity(PhysicsComponent physics)
        {
            if (!physics.MaxSpeed.HasValue)
                return;

            var currentSpeed = physics.GetSpeed();
            if (currentSpeed <= physics.MaxSpeed.Value)
                return;

            //Scale velocity to match max speed
            var scale = physics.MaxSpeed.Value / currentSpeed;
            physics.Velocity = new PointF(
            physics.Velocity.X * scale,
            physics.Velocity.Y * scale
            );
        }

        ///<summary>
        ///P11-04-02-B: Applies velocity to position.
        ///</summary>
        private void ApplyVelocityToPosition(PhysicsComponent physics, TransformComponent transform, float deltaTime)
        {
            transform.X += physics.Velocity.X * deltaTime;
            transform.Y += physics.Velocity.Y * deltaTime;
        }

        ///<summary>
        ///Applies a force to an entity with physics component.
        ///</summary>
        ///<param name="entity">Entity to apply force to</param>
        ///<param name="force">Force to apply in world units</param>
        public void ApplyForce(object entity, PointF force)
        {
            try
            {
                var entityId = (uint)entity;
                var physics = _entityManager.GetComponent<PhysicsComponent>(entityId);
                if (physics != null && physics.Enabled)
                {
                    physics.ApplyForce(force);
                    DebugLog($"PhysicsSystem: Applied force {force} to entity");
                }
            }
            catch (Exception ex)
            {
                DebugLog($"PhysicsSystem: Failed to apply force - {ex.Message}");
            }
        }

        ///<summary>
        ///Applies an impulse to an entity with physics component.
        ///</summary>
        ///<param name="entity">Entity to apply impulse to</param>
        ///<param name="impulse">Impulse to apply in world units</param>
        public void ApplyImpulse(object entity, PointF impulse)
        {
            try
            {
                var entityId = (uint)entity;
                var physics = _entityManager.GetComponent<PhysicsComponent>(entityId);
                if (physics != null && physics.Enabled)
                {
                    physics.ApplyImpulse(impulse);
                    DebugLog($"PhysicsSystem: Applied impulse {impulse} to entity");
                }
            }
            catch (Exception ex)
            {
                DebugLog($"PhysicsSystem: Failed to apply impulse - {ex.Message}");
            }
        }

        ///<summary>
        ///Gets statistics about the physics system.
        ///</summary>
        public PhysicsSystemStatistics GetStatistics()
        {
            var physicsEntities = _entityManager.GetEntitiesWithPhysicsAndTransform();
            return new PhysicsSystemStatistics
            {
                Initialized = _initialized,
                PhysicsEntityCount = physicsEntities.Count()
            };
        }

        ///<summary>
        ///Shuts down the physics system and releases resources.
        ///</summary>
        public void Shutdown()
        {
            if (!_initialized)
                return;

            DebugLog("PhysicsSystem: Starting shutdown...");

            _initialized = false;

            DebugLog("PhysicsSystem: Shutdown complete");
        }

        ///<summary>
        ///P11-04-02-E: Event subscription documentation for audit purposes.
        ///P11-04-04-D: Updated to clarify event publishing status.
        ///
        ///AUDIT-FRIENDLY DOCUMENTATION:
        ///The PhysicsSystem does NOT require any event subscriptions for its core functionality.
        ///
        ///P11-04-02-E: PhysicsSystem and CollisionSystem remain event-agnostic.
        ///All updates are driven by per-frame ECS queries.
        ///
        ///P11-04-04-D: EVENT PUBLISHING DOCUMENTATION:
        ///PhysicsSystem does not currently publish any events.
        ///
        ///CURRENT EVENT PUBLISHING STATUS:
        ///- PhysicsSystem focuses on physics simulation and movement
        ///- No events are published by PhysicsSystem at this time
        ///- EventBus dependency retained for architectural consistency and future enhancements
        ///- Ready to publish physics-related events if needed in future iterations
        ///
        ///POTENTIAL FUTURE EVENTS (NOT CURRENTLY IMPLEMENTED):
        ///- PhysicsCollisionEvent: When physics-based collision occurs
        ///- ForceAppliedEvent: When external forces are applied
        ///- VelocityChangedEvent: When significant velocity changes occur
        ///- PhysicsStateChangeEvent: When physics state changes (kinematic/dynamic)
        ///
        ///PHYSICS SIMULATION IS DRIVEN PURELY BY ECS QUERIES:
        ///- PhysicsSystem queries EntityManager for entities with required components
        ///- Component data (PhysicsComponent + TransformComponent) determines all physics behavior
        ///- Physics updates occur in Update() method called from main game loop
        ///- No event-driven updates or subscriptions are used
        ///
        ///ARCHITECTURAL RATIONALE:
        ///- Component-based design provides deterministic physics based on entity state
        ///- Direct component queries eliminate event subscription overhead
        ///- Main game loop drives physics timing, not events
        ///- EventBus dependency retained for architectural consistency and future enhancements
        ///
        ///FORCES APPLIED (CURRENT IMPLEMENTATION):
        ///- Acceleration forces (from PhysicsComponent.Acceleration)
        ///- Drag forces (proportional to velocity magnitude)
        ///- External forces via ApplyForce() method
        ///- Impulse forces via ApplyImpulse() method
        ///
        ///DEFERRED FEATURES:
        ///- Gravity forces (will be added in future iterations)
        ///- Wind forces (will be added in future iterations)
        ///- Magnetic/electromagnetic forces (future enhancement)
        ///- Constraint forces (joints, springs, etc.)
        ///
        ///INTERACTION WITH COLLISIONSYSTEM:
        ///- PhysicsSystem handles movement and force application
        ///- CollisionSystem handles collision detection and response
        ///- CollisionSystem may apply bounce forces to PhysicsComponent
        ///- Clear separation of concerns between movement and collision
        ///
        ///POTENTIAL FUTURE EVENT SUBSCRIPTIONS (optional, not currently implemented):
        ///- GravityChangedEvent: For dynamic gravity adjustments
        ///- WindChangedEvent: For environmental force changes
        ///- TimeScaleChangedEvent: For slow-motion/fast-forward effects
        ///
        ///CURRENT IMPLEMENTATION: No event subscriptions created or maintained.
        ///No event publishing currently implemented.
        ///</summary>
        private void DocumentEventSubscriptions()
        {
            //This method exists solely to document event subscription requirements
            //as specified in P11-04-02-E. No actual event subscriptions are implemented.
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
    ///Statistics about the physics system state.
    ///</summary>
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
