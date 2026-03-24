/*
File:    SystemManager.cs
Folder:  Engine/Systems/
Purpose:  System management for SAS Zombie Assault TD.
Features: Manages all engine systems and provides diagnostics.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// System management for SAS Zombie Assault TD.
    /// Manages all engine systems and provides diagnostics.
    /// </summary>
    public class SystemManager
    {
        #region Properties

        /// <summary>
        /// Whether the system manager is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        #endregion

        #region Private Fields

        private readonly Dictionary<Type, object> _systems = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new SystemManager instance.
        /// </summary>
        public SystemManager()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a system.
        /// </summary>
        /// <param name="system">The system to register.</param>
        public void RegisterSystem<T>(T system) where T : class
        {
            _systems[typeof(T)] = system;
        }

        /// <summary>
        /// Gets a registered system by type.
        /// </summary>
        /// <typeparam name="T">The system type.</typeparam>
        /// <returns>The system instance or null.</returns>
        public T GetSystem<T>() where T : class
        {
            return _systems.TryGetValue(typeof(T), out var system) ? system as T : null;
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        /// <returns>Diagnostic information.</returns>
        public string GetDiagnostics()
        {
            return $"SystemManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        /// <summary>
        /// Shuts down the system manager.
        /// </summary>
        public void Shutdown()
        {
            IsActive = false;
            _systems.Clear();
        }

        #endregion
    }
}
