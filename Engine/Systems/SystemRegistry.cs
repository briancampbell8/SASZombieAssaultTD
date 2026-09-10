// =====================================================================================================
//  FILE: SystemRegistry.cs
//  PATH: Engine/Systems/SystemRegistry.cs
//  SUBSYSTEM: Systems Registry / Deterministic Dependency Container
//
//  ROLE:
//      Centralized registration, lookup, lifecycle management, and diagnostics for all engine systems.
//      Provides the unified dependency injection container for the SAS Zombie Assault TD engine.
//
//  RESPONSIBILITIES:
//      - Register and store all engine systems and services.
//      - Provide type-safe lookup via Resolve<T>(), GetService<T>(), GetSystem<T>(), and Get<T>().
//      - Track registration and initialization status for diagnostics.
//      - Initialize all registered services exposing Initialize().
//      - Update all registered services exposing Update(float).
//      - Provide registry statistics for debugging and performance monitoring.
//      - Deterministic shutdown of all registered services.
//
//  NON-RESPONSIBILITIES:
//      - Rendering logic.
//      - Game logic.
//      - File persistence.
//
//  ARCHITECTURAL NOTES:
//      - Uses a type-keyed dictionary for service storage.
//      - Fully compatible with Option‑B deterministic architecture.
//      - Diagnostics routed through DLogger.Log.
//      - No TODOs, no NotImplementedExceptions, no silent failures.
//
//  CHANGE LOG:
//      [2026-07-25 | BDC] Implemented Register(object), Get<T>(), and removed implicit SystemRegistry(SystemManager).
//      [2026-07-30 | Copilot] Implemented Resolve<T>() deterministically and restored full ISystemRegistry compliance.
// =====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Interfaces;

namespace SASZombieAssaultTD.Engine.Systems
{
    public class SystemRegistry : ISystemRegistry
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, ServiceStatus> _serviceStatuses = new();

        private int _initializedServices = 0;
        private int _failedServices = 0;
        internal static object Instance;

        // =============================================================================================
        // REGISTRATION (INTERFACE-COMPLIANT)
        // =============================================================================================

        public void RegisterService<T>(T service) where T : class => Register(typeof(T), service);

        public void RegisterSystem<T>(T system) where T : class => Register(typeof(T), system);

        public void Register(SystemManager systemManager) =>
            Register(typeof(SystemManager), systemManager);

        public void Register(UpdateManager updateManager) =>
            Register(typeof(UpdateManager), updateManager);

        public void Register(RenderManager renderManager) =>
            Register(typeof(RenderManager), renderManager);

        public void Register<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Register(typeof(T), instance);
        }

        public void Register(Type type, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _services[type] = instance;
            _serviceStatuses[type] = ServiceStatus.Registered;

            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                $"SystemRegistry: Registered service '{type.Name}'.");
        }

        public void Register(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Register(instance.GetType(), instance);
        }

        // =============================================================================================
        // LOOKUP (PRIMARY API)
        // =============================================================================================

        T ISystemRegistry.Resolve<T>()
        {
            var type = typeof(T);

            if (_services.TryGetValue(type, out var instance))
            {
                if (instance is T typed)
                    return typed;

                throw new InvalidOperationException(
                    $"SystemRegistry.Resolve<T>: Registered instance for '{type.FullName}' is not of type T.");
            }

            throw new InvalidOperationException(
                $"SystemRegistry.Resolve<T>: No service registered for type '{type.FullName}'.");
        }

        // =============================================================================================
        // LOOKUP (SECONDARY API)
        // =============================================================================================

        public T GetService<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var instance))
            {
                if (instance is T typed)
                    return typed;

                throw new InvalidOperationException(
                    $"SystemRegistry.GetService<T>: Registered instance for '{typeof(T).FullName}' is not of type T.");
            }

            throw new InvalidOperationException(
                $"SystemRegistry.GetService<T>: No service registered for type '{typeof(T).FullName}'.");
        }

        public T GetService<T>(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_services.TryGetValue(type, out var instance))
                return (T)instance;

            throw new InvalidOperationException(
                $"SystemRegistry.GetService<T>(Type): No service registered for type '{type.FullName}'.");
        }

        public object GetService(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_services.TryGetValue(type, out var instance))
                return instance;

            throw new InvalidOperationException(
                $"SystemRegistry.GetService(Type): No service registered for type '{type.FullName}'.");
        }
        public T GetRegistry<T>(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_services.TryGetValue(type, out var instance))
                return (T)instance;

            throw new InvalidOperationException(
                $"SystemRegistry.GetRegistry<T>(Type): No service registered for type '{type.FullName}'.");
        }

        public IEnumerable<object> GetServices(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            foreach (var kvp in _services)
            {
                if (type.IsAssignableFrom(kvp.Key))
                    yield return kvp.Value;
            }
        }

        public T? GetSystem<T>() where T : class =>
            _services.TryGetValue(typeof(T), out var instance)
                ? instance as T
                : null;

        public bool IsRegistered<T>() where T : class =>
            _services.ContainsKey(typeof(T));

        public T? Get<T>() where T : class =>
            _services.TryGetValue(typeof(T), out var instance)
                ? instance as T
                : null;

        internal IEnumerable<object> GetAll() => _services.Values;
        internal IEnumerable<object> GetAllServices() => _services.Values;
        public IEnumerable<Type> GetRegisteredTypes() => _services.Keys;

        // =============================================================================================
        // INITIALIZATION
        // =============================================================================================

        public void Initialize()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                "SystemRegistry: Initializing all services.");

            InitializeAll();
        }

        public void InitializeAll()
        {
            foreach (var kvp in _services.ToList())
            {
                var type = kvp.Key;
                var instance = kvp.Value;

                try
                {
                    var init = instance.GetType().GetMethod("Initialize");

                    if (init != null)
                    {
                        DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                            $"SystemRegistry: Initializing '{type.Name}'.");

                        init.Invoke(instance, null);
                    }
                    else
                    {
                        DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                            $"SystemRegistry: '{type.Name}' has no Initialize() method.");
                    }

                    _serviceStatuses[type] = ServiceStatus.Initialized;
                    _initializedServices++;
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Error,
                        $"SystemRegistry: Initialization FAILED for '{type.Name}' — {ex.Message}");

                    _serviceStatuses[type] = ServiceStatus.Failed;
                    _failedServices++;
                }
            }
        }

        // =============================================================================================
        // UPDATE
        // =============================================================================================

        public void UpdateAll(float deltaTime)
        {
            foreach (var service in _services.Values)
            {
                try
                {
                    var update = service.GetType().GetMethod("Update", new[] { typeof(float) });

                    if (update != null)
                        update.Invoke(service, new object[] { deltaTime });
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Error,
                        $"SystemRegistry: Update FAILED for '{service.GetType().Name}' — {ex.Message}");
                }
            }
        }

        // =============================================================================================
        // SHUTDOWN
        // =============================================================================================

        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                "SystemRegistry: Shutdown initiated.");

            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                {
                    try
                    {
                        DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                            $"SystemRegistry: Disposing '{service.GetType().Name}'.");

                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Error,
                            $"SystemRegistry: Disposal FAILED for '{service.GetType().Name}' — {ex.Message}");
                    }
                }
            }

            Clear();

            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                "SystemRegistry: Shutdown complete.");
        }

        // =============================================================================================
        // CLEAR
        // =============================================================================================

        public void Clear()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info,
                "SystemRegistry: Clearing all services.");

            _services.Clear();
            _serviceStatuses.Clear();

            _initializedServices = 0;
            _failedServices = 0;
        }
    }

    public enum ServiceStatus
    {
        Registered,
        Initialized,
        Failed
    }

    public class SystemRegistryStats
    {
        public int TotalServices { get; set; }
        public int InitializedServices { get; set; }
        public int FailedServices { get; set; }
    }
}
