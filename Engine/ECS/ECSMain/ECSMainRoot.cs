// ====================================================================================================
//  FILE: ECSMainRoot.cs
//  PATH: Engine/ECS/ECSMain/ECSMainRoot.cs
//  SUBSYSTEM: ECS ECSMain
//
//  ROLE:
//      High‑level ECS façade layer that wraps and exposes ECSRuntimeCore functionality.
//      Provides a stable, engine‑facing entry point for ECSEntityCore, component, system, and query operations,
//      while delegating all deterministic sequencing and low‑level orchestration to ECSRuntimeCore.
//
//  SEQUENCING:
//      Initialize → Update → FixedUpdate → LateUpdate → Render → Destroy
//      (all lifecycle calls are forwarded directly to ECSRuntimeCore)
//
//  RESPONSIBILITIES:
//      - Construct and hold a reference to IECSRuntimeCore.
//      - Forward lifecycle calls to the underlying ECSRuntimeCore instance.
//      - Expose ECS statistics (ECSEntityCore/system counts).
//      - Provide convenience wrappers for ECSEntityCore, component, system, and query operations.
//      - Maintain a thin, stable façade boundary above ECSRuntimeCore.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deterministic sequencing (owned by ECSRuntimeCore).
//      - Cross‑subsystem orchestration above the ECS directory level.
//      - Defining or owning ECS systems, components, or entities.
//      - Performing low‑level ECSRuntime subsystem operations directly.
//
//  NOTES:
//      - ECSMainRoot is intentionally thin and must not duplicate ECSRuntimeCore responsibilities.
//      - ECSRuntimeCore remains the single deterministic sequencer for ECS systems.
//      - ECSMainRoot exists solely as the engine‑facing façade layer.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.ECS.ECSMain
{
    public class ECSMainRoot
    {
        private readonly IECSRuntimeCore _world;
        private readonly Dictionary<Type, object> _systemCache = new();
        private bool _isInitialized;
        private bool _ecsComponent;

        public IECSRuntimeCore World => _world;
        public bool IsInitialized => _isInitialized;
        public int SystemCount => _world.SystemCount;
        public int EntityCount => _world.EntityCount;
        public ECSComponents escComponents;
        // ================================================================================================
        //  Constructors
        // ================================================================================================
        public ECSMainRoot()
        {
            // ECSRuntimeCore now requires ECSRootCore
            _world = new ECSRuntimeCore(new ECSRootCore());

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSMainRoot", 1, "Init", "ECSMainRoot created with new ECSRuntimeCore.");
        }

        public ECSMainRoot(IECSRuntimeCore world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSMainRoot", 1, "Init", "ECSMainRoot created with injected IECSRuntimeCore.");
        }

        // ================================================================================================
        //  Lifecycle Management
        // ================================================================================================
        public void Initialize()
        {
            if (_isInitialized)
                return;

            _world.Initialize();
            _isInitialized = true;
        }

        public void Update(float deltaTime)
        {
            if (_isInitialized)
                _world.Update(deltaTime);
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            if (_isInitialized)
                _world.FixedUpdate(fixedDeltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            if (_isInitialized)
                _world.LateUpdate(deltaTime);
        }

        public void Render()
        {
            if (_isInitialized)
                _world.Render();
        }

        public void Destroy()
        {
            if (_isInitialized)
            {
                _world.Destroy();
                _isInitialized = false;
            }
        }

        public void Reset()
        {
            if (_isInitialized)
            {
                _world.Reset();
                _isInitialized = false;
            }
        }

        // ================================================================================================
        //  Entity Management
        // ================================================================================================
        public ECSEntityCore CreateEntity() => _world.CreateEntity();

        public bool DestroyEntity(ECSEntityCore ECSEntityCore)
        {
            if (ECSEntityCore == null)
                return false;

            _world.DestroyEntity(ECSEntityCore);
            return true;
        }

        // ================================================================================================
        //  Component Management
        // ================================================================================================
        public bool AddComponent<T>(ECSEntityCore ECSEntityCore, T component) where T : class
        {
            if (ECSEntityCore == null || component == null)
                return false;

            _world.AddComponent(ECSEntityCore, component);
            return true;
        }

        public T GetComponent<T>(ECSEntityCore ECSEntityCore) where T : class =>
            escComponents.GetComponent<T>(ECSEntityCore.Id);

        public bool RemoveComponent<T>(ECSEntityCore ECSEntityCore) where T : class
        {
            if (ECSEntityCore == null)
                return false;

            _world.RemoveComponent<T>(ECSEntityCore);
            return true;
        }

        public bool HasComponent<T>(ECSEntityCore ECSEntityCore) where T : class =>
            _world.HasComponent<T>(ECSEntityCore);

        public IEnumerable<object> GetAllComponents(ECSEntityCore ECSEntityCore) =>
            _world.GetAllComponents(ECSEntityCore);

        // ================================================================================================
        //  System Management
        // ================================================================================================
        public bool AddSystem(object system)
        {
            if (system == null)
                return false;

            _world.AddSystem(system);
            CacheSystem(system.GetType(), system);
            return true;
        }

        public bool RemoveSystem(object system)
        {
            if (system == null)
                return false;

            _world.RemoveSystem(system);
            UncacheSystem(system.GetType());
            return true;
        }

        public T GetSystem<T>() where T : class
        {
            var type = typeof(T);

            if (_systemCache.TryGetValue(type, out var cached) && cached is T typed)
                return typed;

            var system = _world.GetSystem<T>();
            if (system != null)
                CacheSystem(type, system);

            return system;
        }

        public IEnumerable<object> GetSystems() => _world.GetSystems();
        public IEnumerable<object> GetSystemsByPriority() => _world.GetSystemsByPriority();

        // ================================================================================================
        //  Queries
        // ================================================================================================
        public IEnumerable<ECSEntityCore> FindEntitiesWithComponent<T>() where T : class =>
            _world.FindEntitiesWithComponent<T>();

        public IEnumerable<ECSEntityCore> FindEntitiesWithComponents(params Type[] componentTypes) =>
            _world.FindEntitiesWithComponents(componentTypes);

        public IEnumerable<ECSEntityCore> FindEntitiesInRadius(Vector3 center, float radius) =>
            _world.FindEntitiesInRadius(center, radius);

        public IEnumerable<ECSEntityCore> GetActiveEntities() => _world.ActiveEntities;
        public IEnumerable<object> GetActiveSystems() => _world.ActiveSystems;

        // ================================================================================================
        //  Private Helpers
        // ================================================================================================
        private void CacheSystem(Type type, object system)
        {
            if (type != null && system != null)
                _systemCache[type] = system;
        }

        private void UncacheSystem(Type type)
        {
            if (type != null)
                _systemCache.Remove(type);
        }

        // ================================================================================================
        //  Debug
        // ================================================================================================
        public override string ToString() =>
            $"ECSMainRoot(World:{_world.EntityCount} entities, {_world.SystemCount} systems, Initialized:{_isInitialized})";
    }
}
