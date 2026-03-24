/*
File:    UpdateManager.cs
Folder:  Engine/Systems/
Purpose:  Update management system for SAS Zombie Assault TD.
Features: Manages game loop updates and system scheduling.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Update management system for SAS Zombie Assault TD.
    /// Manages game loop updates and system scheduling.
    /// </summary>
    public class UpdateManager
    {
        #region Properties

        /// <summary>
        /// Whether the update manager is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Current update rate.
        /// </summary>
        public float UpdateRate { get; set; } = 60.0f;

        #endregion

        #region Private Fields

        private readonly List<IUpdateSystem> _systems = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new UpdateManager instance.
        /// </summary>
        public UpdateManager()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers an update system.
        /// </summary>
        /// <param name="system">The system to register.</param>
        public void RegisterSystem(IUpdateSystem system)
        {
            if (system != null && !_systems.Contains(system))
            {
                _systems.Add(system);
            }
        }

        /// <summary>
        /// Updates all registered systems.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public void UpdateAll(float deltaTime)
        {
            if (!IsActive) return;

            foreach (var system in _systems)
            {
                system.Update(deltaTime);
            }
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        /// <returns>Diagnostic information.</returns>
        public string GetDiagnostics()
        {
            return $"UpdateManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        /// <summary>
        /// Shuts down the update manager.
        /// </summary>
        public void Shutdown()
        {
            IsActive = false;
            _systems.Clear();
        }

        #endregion
    }

    /// <summary>
    /// Interface for update systems.
    /// </summary>
    public interface IUpdateSystem
    {
        /// <summary>
        /// Updates the system.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        void Update(float deltaTime);
    }
}
