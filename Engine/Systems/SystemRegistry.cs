/*
File:    SystemRegistry.cs
Purpose:  System registration and management for SAS Zombie Assault TD.
Features:  Service registration, lifecycle management, and diagnostics.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Registry for managing system services and their lifecycle.
    /// Provides comprehensive service registration and dependency injection.
    /// </summary>
    public sealed class SystemRegistry : SASZombieAssaultTD.Engine.Interfaces.ISystemRegistry
    {
        ///  Private Fields

        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, ServiceStatus> _serviceStatuses = new();
        private int _initializedServices = 0;
        private int _failedServices = 0;

        /// 

        ///  Service Registration

        /// <summary>
        /// Registers a service with the registry.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="service">Service instance.</param>
        public void RegisterService<T>(T service) where T : class
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            
            _services[typeof(T)] = service;
            _serviceStatuses[typeof(T)] = ServiceStatus.Registered;
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <returns>Service instance, or null if not found.</returns>
        public T? GetService<T>() where T : class
        {
            return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
        }

        /// <summary>
        /// Checks if a service is registered.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <returns>True if service is registered.</returns>
        public bool HasService<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Gets all registered services.
        /// </summary>
        /// <returns>All registered services.</returns>
        public IEnumerable<object> GetAllServices()
        {
            return _services.Values;
        }

        /// 

        ///  ISystemRegistry Implementation

        /// <summary>
        /// Retrieves a system of type T from the registry.
        /// Adapts GetService calls to the canonical GetSystem interface.
        /// </summary>
        /// <typeparam name="T">The system type to resolve.</typeparam>
        /// <returns>The system instance, or null if not found.</returns>
        public T? GetSystem<T>() where T : class
        {
            return GetService<T>();
        }

        /// <summary>
        /// Registers a system instance of type T in the registry.
        /// Adapts RegisterService calls to the canonical RegisterSystem interface.
        /// </summary>
        /// <typeparam name="T">The system type to register.</typeparam>
        /// <param name="system">The system instance to register.</param>
        public void RegisterSystem<T>(T system) where T : class
        {
            RegisterService(system);
        }

        /// <summary>
        /// Determines whether a system of type T is registered.
        /// Adapts HasService calls to the canonical IsRegistered interface.
        /// </summary>
        /// <typeparam name="T">The system type to check.</typeparam>
        /// <returns>True if the system is registered; otherwise false.</returns>
        public bool IsRegistered<T>() where T : class
        {
            return HasService<T>();
        }

        /// <summary>
        /// Retrieves all registered system types.
        /// Adapts service keys to the canonical GetRegisteredTypes interface.
        /// </summary>
        /// <returns>A collection of registered system Type objects.</returns>
        public IEnumerable<Type> GetRegisteredTypes()
        {
            return _services.Keys;
        }

        /// <summary>
        /// Removes all registered systems from the registry.
        /// Clears all services and status information.
        /// </summary>
        public void Clear()
        {
            _services.Clear();
            _serviceStatuses.Clear();
            _initializedServices = 0;
            _failedServices = 0;
        }

        /// <summary>
        /// Initializes the system registry.
        /// Adapts InitializeAll calls to the canonical Initialize interface.
        /// </summary>
        public void Initialize()
        {
            InitializeAll();
        }

        /// 

        ///  Service Lifecycle

        /// <summary>
        /// Initializes all registered services.
        /// </summary>
        public void InitializeAll()
        {
            foreach (var kvp in _services.ToList())
            {
                try
                {
                    // Initialize service if it has an Initialize method
                    var serviceType = kvp.Value.GetType();
                    var initializeMethod = serviceType.GetMethod("Initialize");
                    initializeMethod?.Invoke(kvp.Value, null);
                    
                    _serviceStatuses[kvp.Key] = ServiceStatus.Initialized;
                    _initializedServices++;
                }
                catch (Exception)
                {
                    _serviceStatuses[kvp.Key] = ServiceStatus.Failed;
                    _failedServices++;
                }
            }
        }

        /// <summary>
        /// Updates all services that support updating.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void UpdateAll(float deltaTime)
        {
            foreach (var service in _services.Values)
            {
                try
                {
                    var serviceType = service.GetType();
                    var updateMethod = serviceType.GetMethod("Update", new[] { typeof(float) });
                    updateMethod?.Invoke(service, new object[] { deltaTime });
                }
                catch (Exception)
                {
                    // Log error but continue updating other services
                }
            }
        }

        /// 

        ///  Statistics

        /// <summary>
        /// Gets system registry statistics.
        /// </summary>
        /// <returns>System registry statistics.</returns>
        public SystemRegistryStats GetStats()
        {
            return new SystemRegistryStats
            {
                TotalServices = _services.Count,
                InitializedServices = _initializedServices,
                FailedServices = _failedServices
            };
        }

        /// 
    }

    /// <summary>
    /// Service status enumeration.
    /// </summary>
    public enum ServiceStatus
    {
        Registered,
        Initialized,
        Failed
    }

    /// <summary>
    /// Statistics for system registry performance monitoring.
    /// </summary>
    public class SystemRegistryStats
    {
        /// <summary>
        /// Total number of registered services.
        /// </summary>
        public int TotalServices { get; set; }

        /// <summary>
        /// Number of successfully initialized services.
        /// </summary>
        public int InitializedServices { get; set; }

        /// <summary>
        /// Number of failed service initializations.
        /// </summary>
        public int FailedServices { get; set; }
    }
}
