// ============================================================================
// PATH: Engine/Interfaces/ISystemRegistry.cs
//
// FILE: ISystemRegistry.cs
// PURPOSE:
//     Defines the contract for the engine's centralized system registry.
//     Provides type-safe registration and resolution of engine systems,
//     enabling dependency injection and clean orchestration through GameRoot.
//
// ROLE IN ENGINE:
//     - Core DI contract used by GameRoot, SystemRegistry, and all Managers
//     - Provides type-safe GetSystem<T>() resolution
//     - Ensures systems can be registered, queried, and enumerated
//     - Forms the foundation of the engine's dependency injection architecture
//
// P-MILESTONE ALIGNMENT:
//     - P11-02: System orchestration and dependency resolution
//     - P11-04: Deterministic system lifecycle management
//     - P11-09: Authoritative engine initialization and wiring
//
// NOTES:
//     - Interface only — no implementation details
//     - Implementation provided by SystemRegistry.cs
//     - Must remain stable to prevent architectural drift
// ============================================================================

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Provides a centralized registry for engine systems.
    /// Enables type-safe registration and resolution of systems
    /// used throughout the engine (ECS, Rendering, Input, Audio, etc.).
    /// </summary>
    public interface ISystemRegistry
    {
        /// <summary>
        /// Retrieves a system of type <typeparamref name="T"/> from the registry.
        /// Returns <c>null</c> if the system is not registered.
        /// </summary>
        /// <typeparam name="T">The system type to resolve.</typeparam>
        /// <returns>The system instance, or <c>null</c> if not found.</returns>
        T? GetSystem<T>() where T : class;

        /// <summary>
        /// Registers a system instance of type <typeparamref name="T"/> in the registry.
        /// If a system of this type already exists, it will be replaced.
        /// </summary>
        /// <typeparam name="T">The system type to register.</typeparam>
        /// <param name="system">The system instance to register.</param>
        void RegisterSystem<T>(T system) where T : class;

        /// <summary>
        /// Determines whether a system of type <typeparamref name="T"/> is registered.
        /// </summary>
        /// <typeparam name="T">The system type to check.</typeparam>
        /// <returns><c>true</c> if the system is registered; otherwise <c>false</c>.</returns>
        bool IsRegistered<T>() where T : class;

        /// <summary>
        /// Retrieves all registered system types.
        /// Useful for debugging, diagnostics, and system enumeration.
        /// </summary>
        /// <returns>A collection of registered system <see cref="Type"/> objects.</returns>
        IEnumerable<Type> GetRegisteredTypes();

        /// <summary>
        /// Removes all registered systems from the registry.
        /// Typically used during shutdown or full engine reset.
        /// </summary>
        void Clear();

        /// <summary>
        /// Initializes the system registry.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets a service of type T from the registry.
        /// </summary>
        /// <typeparam name="T">The service type to resolve.</typeparam>
        /// <returns>The service instance, or null if not found.</returns>
        T? GetService<T>() where T : class;

        /// <summary>
        /// Registers a service of type T in the registry.
        /// </summary>
        /// <typeparam name="T">The service type to register.</typeparam>
        /// <param name="service">The service instance to register.</param>
        void RegisterService<T>(T service) where T : class;
    }
}
