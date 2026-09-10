// ====================================================================================================
//  FILE: ISystemRegistry.cs
//  PATH: ./Engine/Interfaces/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ISystemRegistry module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//PATH: Engine/Interfaces/ISystemRegistry.cs
//
//FILE: ISystemRegistry.cs
//PURPOSE:
//    Defines the contract for the engine's centralized system registry.
//    Provides type-safe registration and resolution of engine systems,
//    enabling dependency injection and clean orchestration through GameRoot.
//
//ROLE IN ENGINE:
//    - Core DI contract used by GameRoot, SystemRegistry, and all Managers
//    - Provides type-safe GetSystem<T>() resolution
//    - Ensures systems can be registered, queried, and enumerated
//    - Forms the foundation of the engine's dependency injection architecture
//
//P-MILESTONE ALIGNMENT:
//    - P11-02: System orchestration and dependency resolution
//    - P11-04: Deterministic system lifecycle management
//    - P11-09: Authoritative engine initialization and wiring
//
//NOTES:
//    - Interface only — no implementation details
//    - Implementation provided by SystemRegistry.cs
//    - Must remain stable to prevent architectural drift
//============================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Systems;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Provides a centralized registry for engine systems. Enables type-safe registration and resolution of systems
    /// used throughout the engine (ECS, Rendering, Input, Audio, etc.).
    /// </summary>
    public interface ISystemRegistry
    {
        // SYSTEMS (Option B)
        T? GetSystem<T>() where T : class;
        void RegisterSystem<T>(T system) where T : class;
        bool IsRegistered<T>() where T : class;

        // SERVICES (DI)
        T? GetService<T>() where T : class;
        T GetService<T>(Type type);
        object GetService(Type type);
        void RegisterService<T>(T service) where T : class;

        // MULTI-SERVICE QUERIES
        IEnumerable<object> GetServices(Type type);

        // ENUMERATION
        IEnumerable<Type> GetRegisteredTypes();

        // LIFECYCLE
        void Clear();
        void Initialize();
        void Shutdown();

        // STRONGLY-TYPED REGISTRATION (Option B)
        void Register(SystemManager systemManager);
        void Register(UpdateManager updateManager);
        void Register(RenderManager renderManager);
        void Register(object sceneManager);
        T Resolve<T>();
        void Register<T>(T sceneManager);
    }

}
