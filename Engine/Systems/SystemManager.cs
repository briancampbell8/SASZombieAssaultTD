// =====================================================================================================
//  FILE: SystemManager.cs
//  PATH: Engine/Systems/SystemManager.cs
//  SUBSYSTEM: Core Systems / Runtime Subsystem Aggregator
//
//  ROLE:
//      Runtime subsystem coordinator responsible for aggregating, initializing, and exposing all
//      engine systems registered during bootstrap. Acts as the execution‑time counterpart to the
//      SystemRegistry dependency‑injection container.
//
//      SystemManager synchronizes services from SystemRegistry, provides type‑safe lookup for
//      runtime systems, and executes initialization logic for subsystems implementing IInitializable.
//
//  RESPONSIBILITIES:
//      - Maintain a deterministic runtime dictionary of all active engine systems.
//      - Synchronize registered services from SystemRegistry during initialization.
//      - Provide type‑safe Get<T>() lookup for gameplay, rendering, UI, and resource subsystems.
//      - Invoke Initialize() on systems implementing IInitializable.
//      - Track active/inactive state and support controlled shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Rendering, GPU context management, or frame scheduling (handled by RenderManager).
//      - Texture loading or GPU resource creation (handled by TextureManager / GameScene).
//      - Game logic, simulation updates, or state machine transitions.
//      - Dependency injection policy (handled by SystemRegistry).
//
//  ARCHITECTURAL NOTES:
//      - Works in tandem with SystemRegistry: Registry stores authoritative services, SystemManager
//        aggregates them for runtime execution.
//      - Fully compatible with Option‑B deterministic subsystem registration.
//      - No deferred execution, no lazy loading, no reflection‑based casting beyond Initialize().
// =====================================================================================================


using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Provides system registration, lookup, initialization, and shutdown.
    /// </summary>
    public sealed class SystemManager
    {
        private readonly Dictionary<Type, object> _services = new();

        public bool IsRegistered<T>() => _systems.ContainsKey(typeof(T));

        public bool IsActive { get; private set; } = true;

        private readonly Dictionary<Type, object> _systems =
            new Dictionary<Type, object>(capacity: 32);

        private readonly SystemRegistry _systemRegistry;
        internal SystemRegistry Registry;

        /// <summary>
        /// Registers a service by type and instance.
        /// </summary>
        public void RegisterService(Type type, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _services[type] = instance;
        }

        /// <summary>
        /// Constructs the SystemManager using the authoritative bootstrap registry root.
        /// </summary>
        public SystemManager(SystemRegistry systemRegistry)
        {
            _systemRegistry = systemRegistry ?? throw new ArgumentNullException(nameof(systemRegistry));
            DLogger.Log(LogSubsystems.ResourcesPipeline, "SystemManager constructed");
        }

        public SystemManager()
        {
        }

        public void Register<T>(T system) where T : class
        {
            if (system == null) throw new ArgumentNullException(nameof(system));
            var type = typeof(T);
            _systems[type] = system;
            DLogger.Log($"SystemManager.Register: Registered '{type.FullName}'");
        }

        public void Register(object system)
        {
            if (system == null) throw new ArgumentNullException(nameof(system));
            var type = system.GetType();
            _systems[type] = system;
            DLogger.Log($"SystemManager.Register: Registered '{type.FullName}'");
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_systems.TryGetValue(type, out var system))
            {
                DLogger.Log($"SystemManager.Get: Resolved '{type.FullName}'");
                return system as T;
            }
            DLogger.Log($"SystemManager.Get: '{type.FullName}' not found");
            return null;
        }

        public string GetDiagnostics()
        {
            return $"SystemManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        internal void Initialize()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "SystemManager.Initialize: ENTER");

            // Pull across core engines registered during composition bring-up steps
            if (_systemRegistry != null)
            {
                var discoveredServices = _systemRegistry.GetAllServices();
                foreach (var service in discoveredServices)
                {
                    if (service != null)
                    {
                        var type = service.GetType();
                        if (!_systems.ContainsKey(type))
                        {
                            _systems[type] = service;
                            DLogger.Log($"SystemManager.Initialize: Synced '{type.FullName}' from Registry");
                        }
                    }
                }
            }

            if (_systems.Count == 0)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "ENGINE INIT DIAGNOSTIC — SystemManager.Initialize() invoked with ZERO registered systems.");
                DLogger.Log(LogSubsystems.ResourcesPipeline, "SystemManager.Initialize: EXIT (no systems)");
                return;
            }

            foreach (var kvp in _systems)
            {
                var systemType = kvp.Key;
                var systemInstance = kvp.Value;

                DLogger.Log($"SystemManager.Initialize: Processing '{systemType.FullName}'");

                if (systemInstance is IInitializable initializable)
                {
                    try
                    {
                        DLogger.Log($"SystemManager.Initialize: Initializing '{systemType.FullName}'");
                        initializable.Initialize();
                        DLogger.Log($"SystemManager.Initialize: Initialized '{systemType.FullName}'");
                    }
                    catch (Exception ex)
                    {
                        DLogger.Log($"SystemManager.Initialize: Failure in '{systemType.FullName}': {ex.Message}");
                        throw new InvalidOperationException($"SystemManager.Initialize: Critical failure in '{systemType.FullName}'", ex);
                    }
                }
                else
                {
                    DLogger.Log($"SystemManager.Initialize: '{systemType.FullName}' does not implement IInitializable");
                }
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "SystemManager.Initialize: EXIT");
        }

        public void InitializeAll()
        {
            // Import all services from SystemRegistry
            foreach (var type in _systemRegistry.GetRegisteredTypes())
            {
                //     var instance = _systemRegistry.GetService(type);
                var instance = _systemRegistry.GetServices(type);
                RegisterService(type, instance);
            }

            // Initialize services that implement IInitializable
            foreach (var service in _services.Values)
            {
                if (service is IInitializable init)
                    init.Initialize();
            }
        }

        private void RegisterService(object key, object value)
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "SystemManager.Shutdown: ENTER");
            IsActive = false;
            _systems.Clear();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "SystemManager.Shutdown: EXIT");
        }

        public object Resolve(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_services.TryGetValue(type, out var instance))
                return instance;

            // Optional: allow resolving by assignable type (interfaces, base classes)
            foreach (var kvp in _services)
            {
                if (type.IsAssignableFrom(kvp.Key))
                    return kvp.Value;
            }

            throw new InvalidOperationException(
                $"SystemManager.Resolve: No service registered for type '{type.FullName}'.");
        }

        public interface IInitializable
        {
            void Initialize();
        }

        public static implicit operator SystemManager(SystemRegistry v)
        {
            throw new NotImplementedException();
        }
    }
}
