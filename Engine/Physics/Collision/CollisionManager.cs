// =====================================================================================================
//  FILE: CollisionManager.cs
//  PATH: Engine/Physics/Collision/CollisionManager.cs
//  SUBSYSTEM: Physics/Collision Subsystem
//
//  ROLE:
//      High‑level orchestrator for the entire collision pipeline.
//      Coordinates broad‑phase, narrow‑phase, collision result creation,
//      physical resolution, and event processing.
//
//  RESPONSIBILITIES:
//      - Retrieve ECS entities containing collision + transform components.
//      - Execute BroadPhase AABB culling.
//      - Execute NarrowPhase shape‑specific collision checks.
//      - Create CollisionResult objects for all detected collisions.
//      - Delegate physical resolution to CollisionResolve.
//      - Delegate trigger/exit event processing to CollisionEvents.
//      - Maintain deterministic sequencing and collision state tracking.
//
//  NON-RESPONSIBILITIES:
//      - Performing AABB math (CollisionBroadPhase).
//      - Performing shape math (CollisionNarrowPhase).
//      - Applying physics forces (CollisionResolve).
//      - Publishing collision events (CollisionEvents).
//      - Managing ECS ECSEntityCore/component storage.
//
//  ARCHITECTURAL NOTES:
//      - Replaces the legacy monolithic CollisionSystem.
//      - Enforces strict pipeline order: BroadPhase → NarrowPhase → Result → Resolve → Events.
//      - Integrates ALL collision subsystem modules and engine components.
// =====================================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    internal class CollisionManager : IECSSystem
    {
        public bool IsEnabled { get; private set; } = true;
        public bool IsInitialized { get; private set; } = false;
        public SystemPriority Priority { get; private set; } = SystemPriority.Normal;
        public float LastUpdateTime { get; private set; } = 0f;
        public uint UpdateCount { get; private set; } = 0;

        int IECSSystem.Priority
        {
            get
            {
                NotImplementedGuard.Hit($"{nameof(IECSSystem)}.{nameof(IECSSystem.Priority)} not implemented.");
                return default;
            }
        }

        private readonly ECSEntityCore _ECSEntityCore;
        private readonly ECSComponents _components;
        private readonly ECSRuntimeCore _runtime;
        private readonly ECSRuntimeEvents _eventRouting;

        private readonly ConcurrentBag<CollisionResult> _current = new();
        private readonly ConcurrentBag<CollisionResult> _previous = new();

        // FULL PIPELINE MODULES
        private readonly CollisionBroadPhase _broad;
        private readonly CollisionNarrowPhase _narrow;
        private readonly CollisionResolve _resolve;
        private readonly CollisionEvents _events;

        private bool _initialized;
        private readonly bool _debugOutput = true;

        public CollisionManager(ECSEntityCore ECSEntityCore, ECSRuntimeEvents eventRouting)
        {
            _ECSEntityCore = ECSEntityCore ?? throw new ArgumentNullException(nameof(ECSEntityCore));
            _eventRouting = eventRouting ?? throw new ArgumentNullException(nameof(eventRouting));

            _broad = new CollisionBroadPhase();
            _narrow = new CollisionNarrowPhase();
            _resolve = new CollisionResolve(_runtime);
            _events = new CollisionEvents();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "CollisionManager: Constructed.");
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            IsInitialized = true;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "CollisionManager: Initialization complete.");
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled || !IsInitialized)
                return;

            UpdateCount++;
            LastUpdateTime = deltaTime;

            // Move current → previous
            _previous.Clear();
            foreach (var c in _current)
                _previous.Add(c);

            _current.Clear();

            // Retrieve ECS entities
            //  Fixed code: Cast to IEnumerable first, then execute LINQ Cast<T>
            var entities = _runtime.GetEntitiesWithCollisionAndTransform()
            .Cast<object>()
            .ToList();



            // FULL PIPELINE EXECUTION
            DetectCollisions((IReadOnlyList<object>)entities);                      // BroadPhase + NarrowPhase + Result
            _resolve.ResolvePhysicalCollisions(_current);    // Physical resolution
            _events.ProcessCollisionEvents(_current, _previous); // Trigger + exit events
        }

        private void DetectCollisions(IReadOnlyList<object> entities)
        {
            Parallel.For(0, entities.Count, i =>
            {
                for (int j = i + 1; j < entities.Count; j++)
                {
                    var a = entities[i];
                    var b = entities[j];

                    if (CheckCollision(a, b))
                        _current.Add(CreateCollisionResult(a, b));
                }
            });
        }

        private bool CheckCollision(object a, object b)
        {
            var idA = a is ECSEntityCore entA ? entA.Id : (uint)a;
            var idB = b is ECSEntityCore entB ? entB.Id : (uint)b;

            var colA = _components.GetComponent<ColliderCompCore>(idA);
            var colB = _components.GetComponent<ColliderCompCore>(idB);
            var trA = _components.GetComponent<Engine.Components.TransformComponent>(idA);
            var trB = _components.GetComponent<Engine.Components.TransformComponent>(idB);

            if (colA == null || colB == null || trA == null || trB == null)
                return false;

            if (!colA.Enabled || !colB.Enabled)
                return false;

            if (!CollisionExtensions.CanCollideWith((int)colA.Layer, (int)colB.Mask))
                return false;

            var worldA = new PointF(trA.Position.X + (colA.Shape?.Center.X ?? 0),
                                    trA.Position.Y + (colA.Shape?.Center.Y ?? 0));

            var worldB = new PointF(trB.Position.X + (colB.Shape?.Center.X ?? 0),
                                    trB.Position.Y + (colB.Shape?.Center.Y ?? 0));

            // FULL PIPELINE CALLS
            return _broad.BroadPhaseAABBCheck(colA, worldA, colB, worldB)
                && _narrow.NarrowPhaseShapeCheck(colA, worldA, colB, worldB);
        }

        private CollisionResult CreateCollisionResult(object a, object b)
        {
            var idA = (uint)a;
            var idB = (uint)b;

            var colA = _components.GetComponent<ColliderCompCore>(idA);
            var colB = _components.GetComponent<ColliderCompCore>(idB);

            return new CollisionResult
            {
                ObjectA = a,
                ObjectB = b,
                HasCollision = true,
                IsTrigger = colA?.IsTrigger == true || colB?.IsTrigger == true,
                EntityA = a as ECSEntityCore,
                EntityB = b as ECSEntityCore
            };
        }

        private void Log(string msg)
        {
            if (_debugOutput)
                DLogger.Log($"[{DateTime.Now:HH:mm:ss.fff}] {msg}");
        }

        public void FixedUpdate(float dt)
        {
            throw new NotImplementedException();
        }

        public void LateUpdate(float dt)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
