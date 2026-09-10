// =====================================================================================================
//  FILE: IECSRuntimeCore.cs
//  PATH: Engine/ECS/ECSRuntimeCore/IECSRuntimeCore.cs
//  SUBSYSTEM: ECS ECSRuntimeCore
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted ECS runtime program.
//      ECSRuntimeCore implements this interface to provide a clean, engine-facing API for startup,
//      update sequencing, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for runtime orchestration.
//      - Enforce the structural sequencing contract: Initialize → Update Loop → Shutdown.
//      - Serve as the base contract for any future top-level ECS runtime modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update rules.
//      - Rendering or frame presentation.
//      - Managing engine-host lifecycle (GameRootMain handles that).
//      - Managing assets, systems pools, or hardware boundaries.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.ECS
{
    public interface IECSRuntimeCore
    {
        // ---------------------------------------------------------------------------------------------
        // Lifecycle
        // ---------------------------------------------------------------------------------------------
        void Initialize();
        void Update(float deltaTime);
        void FixedUpdate(float deltaTime);
        void LateUpdate(float deltaTime);
        void Render();
        void Destroy();
        void Reset();
        void Shutdown();
        void Restart();

        // ---------------------------------------------------------------------------------------------
        // Runtime state
        // ---------------------------------------------------------------------------------------------
        int EntityCount { get; }
        int SystemCount { get; }
        IEnumerable<ECSEntityCore> ActiveEntities { get; }
        IEnumerable<object> ActiveSystems { get; }

        // ---------------------------------------------------------------------------------------------
        // Entity management
        // ---------------------------------------------------------------------------------------------
        ECSEntityCore CreateEntity();
        void DestroyEntity(ECSEntityCore ECSEntityCore);

        // ---------------------------------------------------------------------------------------------
        // Component management
        // ---------------------------------------------------------------------------------------------
        void AddComponent<T>(ECSEntityCore ECSEntityCore, T component) where T : class;
        //    T GetComponent<T>(ECSEntityCore ECSEntityCore) where T : class;
        void RemoveComponent<T>(ECSEntityCore ECSEntityCore) where T : class;
        bool HasComponent<T>(ECSEntityCore ECSEntityCore) where T : class;
        IEnumerable<object> GetAllComponents(ECSEntityCore ECSEntityCore);

        // ---------------------------------------------------------------------------------------------
        // System management
        // ---------------------------------------------------------------------------------------------
        void AddSystem(object system);
        void RemoveSystem(object system);
        T GetSystem<T>() where T : class;
        IEnumerable<object> GetSystems();
        IEnumerable<object> GetSystemsByPriority();

        // ---------------------------------------------------------------------------------------------
        // Queries
        // ---------------------------------------------------------------------------------------------
        IEnumerable<ECSEntityCore> FindEntitiesWithComponent<T>() where T : class;
        IEnumerable<ECSEntityCore> FindEntitiesWithComponents(params Type[] componentTypes);
        IEnumerable<ECSEntityCore> FindEntitiesInRadius(Vector3 center, float radius);
    }
}
