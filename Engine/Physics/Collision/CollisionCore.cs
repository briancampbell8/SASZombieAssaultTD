// =====================================================================================================
//  FILE: CollisionCore.cs
//  PATH: Engine/Physics/Collision/CollisionCore.cs
//  SUBSYSTEM: Physics Collision Subsystem
//
//  ROLE:
//      Serves as the central orchestrator for the collision subsystem.
//      Coordinates broad‑phase, narrow‑phase, resolution, and event‑processing modules.
//      Provides deterministic sequencing for collision detection each engine frame.
//
//  RESPONSIBILITIES:
//      - Initialize collision subsystem dependencies.
//      - Retrieve ECS entities containing collision and transform components.
//      - Execute broad‑phase and narrow‑phase collision checks.
//      - Generate collision results and forward them to resolution and event processors.
//      - Maintain deterministic update ordering and collision state tracking.
//
//  NON-RESPONSIBILITIES:
//      - Performing shape‑specific collision math (delegated to CollisionNarrowPhase).
//      - Performing AABB culling logic (delegated to CollisionBroadPhase).
//      - Resolving physical collisions or applying forces (delegated to CollisionResolve).
//      - Publishing collision events (delegated to CollisionEvents).
//
//  ARCHITECTURAL NOTES:
//      - CollisionCore replaces the monolithic CollisionManager from the legacy engine.
//      - It delegates all specialized collision logic to dedicated subsystem modules.
//      - Ensures deterministic sequencing: BroadPhase → NarrowPhase → Resolve → Events.
// =====================================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.ECS.ECSRuntime;
using SASZombieAssaultTD.Engine.Physics.Components;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    internal class CollisionCore : IECSSystem
    {
        public bool IsEnabled { get; private set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public SystemPriority Priority { get; private set; } = SystemPriority.Normal;
        public float LastUpdateTime { get; private set; } = 0f;
        public uint UpdateCount { get; private set; } = 0;
        public int ActualCollisions => _currentCollisions.Count;

        int IECSSystem.Priority => (int)Priority;

        private readonly ECSRuntimeCore _runtime;
        private readonly ECSRuntimeEvents _eventRouting;
        private readonly ECSComponents _components;

        private readonly ConcurrentBag<CollisionResult> _currentCollisions = new();
        private readonly ConcurrentBag<CollisionResult> _previousCollisions = new();

        private bool _initialized;
        private bool _checkCollisionEnabled = true;
        private readonly bool _debugOutput = true;

        private readonly CollisionBroadPhase _broadPhase;
        private readonly CollisionNarrowPhase _narrowPhase;
        private readonly CollisionResolve _resolve;
        private readonly CollisionEvents _events;

        public CollisionCore(ECSRuntimeCore runtime, ECSRuntimeEvents eventRouting, ECSComponents components)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _eventRouting = eventRouting ?? throw new ArgumentNullException(nameof(eventRouting));
            _components = components ?? throw new ArgumentNullException(nameof(components));

            _broadPhase = new CollisionBroadPhase();
            _narrowPhase = new CollisionNarrowPhase();
            _resolve = new CollisionResolve(_runtime);
            _events = new CollisionEvents();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "CollisionCore: Constructed with required dependencies");
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            try
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "CollisionCore: Starting initialization...");
                _initialized = true;
                IsInitialized = true;
                DLogger.Log(LogSubsystems.ResourcesPipeline, "CollisionCore: Initialization complete");
            }
            catch (Exception ex)
            {
                DLogger.Log($"CollisionCore: Initialization failed - {ex.Message}");
                throw new InvalidOperationException("Failed to initialize CollisionCore", ex);
            }
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "CollisionCore: Update skipped - Not enabled or initialized");
                return;
            }

            UpdateCount++;
            LastUpdateTime = deltaTime;

            try
            {
                _previousCollisions.Clear();
                foreach (var collision in _currentCollisions)
                    _previousCollisions.Add(collision);

                _currentCollisions.Clear();

                var queries = new ComponentQueries(_runtime, _components);
                var collisionEntities = queries.GetEntitiesWith<TransformComponent, PhysicsComponent>();

                if (!collisionEntities.Any())
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "CollisionCore: No collision entities available");
                    return;
                }

                DLogger.Log($"CollisionCore: Processing {collisionEntities.Count()} collision entities");

                var ECSEntityCoreList = collisionEntities.ToList();
                DetectCollisions(ECSEntityCoreList);

                _resolve.ResolvePhysicalCollisions(_currentCollisions);
                _events.ProcessCollisionEvents(_currentCollisions, _previousCollisions);

                DLogger.Log($"CollisionCore: Detected {_currentCollisions.Count} collisions");
            }
            catch (Exception ex)
            {
                DLogger.Log($"CollisionCore: Update failed - {ex.Message}");
            }
        }

        private void DetectCollisions(IReadOnlyList<ECSEntityCore> entities)
        {
            Parallel.For(0, entities.Count, i =>
            {
                for (int j = i + 1; j < entities.Count; j++)
                {
                    var ECSEntityCoreA = entities[i];
                    var ECSEntityCoreB = entities[j];

                    if (CheckCollision(ECSEntityCoreA, ECSEntityCoreB))
                    {
                        // CollisionResult added inside CheckCollision
                    }
                }
            });
        }

        private bool CheckCollision(ECSEntityCore ECSEntityCoreA, ECSEntityCore ECSEntityCoreB)
        {
            if (!_checkCollisionEnabled)
                return false;

            // Placeholder for real broad-phase and narrow-phase logic.
            // Example:
            //
            // if (!_broadPhase.MightOverlap(ECSEntityCoreA, ECSEntityCoreB))
            //     return false;
            //
            // if (_narrowPhase.TryComputeCollision(ECSEntityCoreA, ECSEntityCoreB, out var result))
            // {
            //     _currentCollisions.Add(result);
            //     return true;
            // }

            return false;
        }

        public void FixedUpdate(float dt) { }

        public void LateUpdate(float dt) { }

        public void Render() { }
    }
}
