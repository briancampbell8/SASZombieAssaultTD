// =====================================================================================================
//  FILE: ECSRuntimeCore.cs
//  PATH: Engine/ECS/ECSRuntime/ECSRuntimeCore.cs
//  SUBSYSTEM: ECS Runtime Core
//
//  ROLE:
//      Central coordinator for ECS runtime behavior.
//      Owns and wires all ECS subsystems: entities, systems, updates, and world lifecycle.
//      Provides deterministic entry points for update, fixed update, late update, and rendering.
//
//  RESPONSIBILITIES:
//      - Manage ECS entity lifecycle (create, destroy, retrieve).
//      - Manage ECS component storage through ECSComponents.
//      - Coordinate ECS systems and update sequencing.
//      - Provide deterministic world lifecycle operations (initialize, reset, shutdown).
//      - Expose runtime-safe entity and system enumeration.
//
//  NON-RESPONSIBILITIES:
//      - Performing physics resolution or spatial queries.
//      - Rendering or animation responsibilities.
//      - AI, pathfinding, or gameplay logic.
//      - Component implementation or storage mutation beyond ECSComponents.
//
//  ARCHITECTURAL NOTES:
//      - ECSRuntimeCore is the root of all ECS subsystems.
//      - ECSEntityCore instances must be created exclusively through ECSRuntimeCore.
//      - All subsystems operate deterministically and avoid side effects outside ECSRuntimeCore.
// =====================================================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Physics.Colliders;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.ECS.ECSComponents;

namespace SASZombieAssaultTD.Engine.ECS
{
    public sealed class ECSRuntimeCore : IECSRuntimeCore
    {
        // ---------------------------------------------------------------------------------------------
        // Root
        // ---------------------------------------------------------------------------------------------
        public ECSRootCore Root { get; }

        // ---------------------------------------------------------------------------------------------
        // Component storage
        // ---------------------------------------------------------------------------------------------
        public ECSComponents Components { get; }
        private readonly ECSComponents _components;

        // ---------------------------------------------------------------------------------------------
        // Subsystems
        // ---------------------------------------------------------------------------------------------
        internal ECSRuntimeSys Systems { get; }
        internal ECSRuntimeUpd Updates { get; }
        internal ECSRuntimeWorld World { get; }
        internal ECSRuntimeEntities Entities { get; private set; }

        public static ECSRuntimeCore Instance { get; internal set; }

        // ---------------------------------------------------------------------------------------------
        // Construction
        // ---------------------------------------------------------------------------------------------
        public ECSRuntimeCore(ECSRootCore root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));

            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", "ECSRuntimeCore initializing subsystems.");

            Components = new ECSComponents();
            _components = Components;

            // Updated constructor to match modern ECSRuntimeEntities
            Entities = new ECSRuntimeEntities(root, Components);

            Systems = new ECSRuntimeSys(this);
            Updates = new ECSRuntimeUpd(Systems);
            World = new ECSRuntimeWorld(this);

            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", "ECSRuntimeCore subsystems initialized.");
        }

        // ---------------------------------------------------------------------------------------------
        // Runtime state
        // ---------------------------------------------------------------------------------------------
        public int EntityCount => Entities.Count;
        public int SystemCount => Systems.Count;

        public IEnumerable<ECSEntityCore> ActiveEntities => Entities.Active;

        public IEnumerable<object> ActiveSystems
        {
            get
            {
                foreach (var s in Systems.Active)
                    yield return s;
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Lifecycle
        // ---------------------------------------------------------------------------------------------
        public void Initialize() => DLogger.Log(LogSubsystems.ECSECSRuntime,
            "ECSRuntimeCore", "Initialize()");
        public void Update(float deltaTime)
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime,
                "ECSRuntimeCore", $"Update(deltaTime={deltaTime})");
            Updates.Update(this, deltaTime);
        }

        public void FixedUpdate(float deltaTime)
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", $"FixedUpdate(deltaTime={deltaTime})");
            Updates.FixedUpdate(this, deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", $"LateUpdate(deltaTime={deltaTime})");
            Updates.LateUpdate(this, deltaTime);
        }

        public void Render()
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", "Render()");
            Updates.Render(this);
        }

        public void Destroy()
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", "Destroy()");
            World.Shutdown();
        }

        public void Reset()
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", "Reset()");
            World.Reset();
        }

        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", "Shutdown()");
            World.Shutdown();
        }

        public void Restart()
        {
            DLogger.Log(LogSubsystems.ECSECSRuntime, "ECSRuntimeCore", "Restart()");
            World.Shutdown();
            World.Reset();
        }

        // ---------------------------------------------------------------------------------------------
        // Entity management
        // ---------------------------------------------------------------------------------------------
        public ECSEntityCore CreateEntity() => Entities.Create();

        public void DestroyEntity(ECSEntityCore entity)
        {
            if (entity == null)
                return;

            Components.ClearComponents<MovementComponent>(entity.Id);
            Entities.Destroy(entity);
        }

        public ECSEntityCore GetEntity(uint id) => Entities.Get(id);

        // ---------------------------------------------------------------------------------------------
        // Component management (delegated to ECSComponents)
        // ---------------------------------------------------------------------------------------------
        public void AddComponent<T>(ECSEntityCore entity, T component) where T : class
        {
            if (entity == null || component == null)
                return;

            Components.AddComponent(entity.Id, component);
        }

        public T GetComponent<T>(ECSEntityCore entity) where T : class =>
            entity == null ? null : Components.GetComponent<T>(entity.Id);

        public void RemoveComponent<T>(ECSEntityCore entity) where T : class
        {
            if (entity == null)
                return;

            Components.RemoveComponent<T>(entity.Id);
        }

        public bool HasComponent<T>(ECSEntityCore entity) where T : class =>
            entity != null && Components.HasComponent<T>(entity.Id);

        public bool TryGetComponent<T>(ECSEntityCore entity, out T component) where T : class =>
            Components.TryGetComponent(entity.Id, out component);

        // ---------------------------------------------------------------------------------------------
        // System management
        // ---------------------------------------------------------------------------------------------
        public void AddSystem(object system) => Systems.Add(system);
        public void RemoveSystem(object system) => Systems.Remove(system);

        T IECSRuntimeCore.GetSystem<T>()
        {
            if (!typeof(IECSSystem).IsAssignableFrom(typeof(T)))
                return null;

            return ((dynamic)Systems).Get<T>();
        }

        public T GetSystem<T>() where T : class, IECSSystem => Systems.Get<T>();

        public IEnumerable<object> GetSystems()
        {
            foreach (var s in Systems.Active)
                yield return s;
        }

        public IEnumerable<object> GetSystemsByPriority()
        {
            foreach (var s in Systems.ByPriority)
                yield return s;
        }

        // ---------------------------------------------------------------------------------------------
        // Queries (delegated to ECSComponents)
        // ---------------------------------------------------------------------------------------------
        public IEnumerable<ECSEntityCore> GetEntitiesWith<T>() where T : class
        {
            foreach (var e in Entities.All)
                if (HasComponent<T>(e))
                    yield return e;
        }

        public IEnumerable<ECSEntityCore> GetEntitiesWithCollisionAndTransform()
        {
            foreach (var e in Entities.All)
            {
                if (HasComponent<ColliderCompCore>(e) &&
                    HasComponent<TransformComponent>(e))
                {
                    yield return e;
                }
            }
        }

        public IEnumerable<ECSEntityCore> FindEntitiesWithComponent<T>() where T : class
        {
            foreach (var e in Entities.All)
                if (Components.HasComponent<T>(e.Id))
                    yield return e;
        }

        public IEnumerable<ECSEntityCore> FindEntitiesWithComponents(params Type[] componentTypes)
        {
            if (componentTypes == null || componentTypes.Length == 0)
                yield break;

            foreach (var e in Entities.All)
            {
                bool hasAll = componentTypes.All(t =>
                    Components.GetAllComponents<object>(e.Id)
                              .Any(c => t.IsAssignableFrom(c.GetType()))
                );

                if (hasAll)
                    yield return e;
            }
        }

        public IEnumerable<object> GetAllComponents(ECSEntityCore entity)
        {
            if (entity == null)
                yield break;

            foreach (var c in Components.GetAllComponents<object>(entity.Id))
                yield return c;
        }

        public IEnumerable<ECSEntityCore> FindEntitiesInRadius(Vector3 center, float radius)
        {
            float r2 = radius * radius;

            foreach (var e in Entities.All)
            {
                if (!HasComponent<TransformComponent>(e))
                    continue;

                var transform = GetComponent<TransformComponent>(e);
                var diff = transform.Position - center;
                float len2 = diff.MagnitudeSquared;

                if (len2 <= r2)
                    yield return e;
            }
        }

        internal object GetComponent<T>(uint id)
        {
            throw new NotImplementedException();
        }

        internal object GetComponent<T>(object id)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<ECSEntityCore> GetEntitiesWith<T1, T2>()
        {
            throw new NotImplementedException();
        }
    }
}
