//============================================================================
//PATH: Engine/Registry/SystemRegistry.cs
//
//FILE: SystemRegistry.cs
//PURPOSE:
//    Provides the thread-safe implementation of the ISystemRegistry interface.
//    Acts as the centralized dependency injection container for all engine
//    systems, enabling type-safe registration and resolution.
//
//ROLE IN ENGINE:
//    - Core DI implementation used by GameRoot and all Managers
//    - Stores and resolves all engine systems (ECS, Rendering, Input, Audio, etc.)
//    - Ensures deterministic system access throughout the engine lifecycle
//
//P-MILESTONE ALIGNMENT:
//    - P11-02: System orchestration and dependency resolution
//    - P11-04: Deterministic system lifecycle management
//    - P11-09: Authoritative engine initialization and wiring
//
//NOTES:
//    - Thread-safe implementation using lock-based synchronization
//    - Must remain stable to prevent architectural drift
//    - Supports helper methods for diagnostics and debugging
//============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Interfaces;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Registry
{
    ///<summary>
    ///Thread-safe implementation of <see cref="ISystemRegistry"/>.
    ///Provides centralized system registration and type-safe resolution
    ///for all engine systems used throughout the engine lifecycle.
    ///</summary>
    public class SystemRegistry : ISystemRegistry
    {
        ///<summary>
        ///Internal storage for registered systems.
        ///Keys are system types; values are system instances.
        ///</summary>
        private readonly Dictionary<Type, object> _systems = new();

        ///<summary>
        ///Lock object used to ensure thread-safe access to the registry.
        ///</summary>
        private readonly object _lock = new();

        //====================================================================
        //ISystemRegistry IMPLEMENTATION
        //====================================================================

        ///<inheritdoc />
        public T? GetSystem<T>() where T : class
        {
            lock (_lock)
            {
                return _systems.TryGetValue(typeof(T), out var system)
                    ? system as T
                    : null;
            }
        }

        ///<inheritdoc />
        public void RegisterSystem<T>(T system) where T : class
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            lock (_lock)
            {
                _systems[typeof(T)] = system;
            }
        }

        ///<inheritdoc />
        public bool IsRegistered<T>() where T : class
        {
            lock (_lock)
            {
                return _systems.ContainsKey(typeof(T));
            }
        }

        ///<inheritdoc />
        public IEnumerable<Type> GetRegisteredTypes()
        {
            lock (_lock)
            {
                //Return a copy to prevent external mutation
                return _systems.Keys.ToList();
            }
        }

        ///<inheritdoc />
        public void Clear()
        {
            lock (_lock)
            {
                _systems.Clear();
            }
        }

        ///<inheritdoc />
        public void Initialize()
        {
            //Initialize the registry (no-op for now)
        }

        ///<inheritdoc />
        public T? GetService<T>() where T : class
        {
            //Alias for GetSystem for service compatibility
            return GetSystem<T>();
        }

        ///<inheritdoc />
        public void RegisterService<T>(T service) where T : class
        {
            //Alias for RegisterSystem for service compatibility
            RegisterSystem(service);
        }

        //====================================================================
        //HELPER METHODS (Not part of ISystemRegistry but required by mapping)
        //====================================================================

        ///<summary>
        ///Attempts to retrieve a system of type <typeparamref name="T"/>.
        ///Returns true if the system exists; otherwise false.
        ///</summary>
        ///<typeparam name="T">The system type to resolve.</typeparam>
        ///<param name="system">The resolved system instance, or null.</param>
        ///<returns>True if the system exists; otherwise false.</returns>
        public bool TryGetSystem<T>(out T? system) where T : class
        {
            lock (_lock)
            {
                if (_systems.TryGetValue(typeof(T), out var obj))
                {
                    system = obj as T;
                    return system != null;
                }

                system = null;
                return false;
            }
        }

        ///<summary>
        ///Gets the number of registered systems in the registry.
        ///Useful for diagnostics and debugging.
        ///</summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _systems.Count;
                }
            }
        }
    }
}
