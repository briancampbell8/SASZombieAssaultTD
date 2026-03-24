/*
File:    RenderManager.cs
Folder:  Engine/Systems/
Purpose:  Render management system for SAS Zombie Assault TD.
Features: Manages rendering pipeline and render context.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Render management system for SAS Zombie Assault TD.
    /// Manages rendering pipeline and render context.
    /// </summary>
    public class RenderManager
    {
        #region Properties

        /// <summary>
        /// Whether the render manager is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Current render context.
        /// </summary>
        public IRenderContext Context { get; set; }

        #endregion

        #region Private Fields

        private readonly List<IRenderSystem> _systems = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new RenderManager instance.
        /// </summary>
        public RenderManager()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a render system.
        /// </summary>
        /// <param name="system">The system to register.</param>
        public void RegisterSystem(IRenderSystem system)
        {
            if (system != null && !_systems.Contains(system))
            {
                _systems.Add(system);
            }
        }

        /// <summary>
        /// Renders all registered systems.
        /// </summary>
        public void RenderAll()
        {
            if (!IsActive) return;

            foreach (var system in _systems)
            {
                system.Render(Context);
            }
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        /// <returns>Diagnostic information.</returns>
        public string GetDiagnostics()
        {
            return $"RenderManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        /// <summary>
        /// Shuts down the render manager.
        /// </summary>
        public void Shutdown()
        {
            IsActive = false;
            _systems.Clear();
        }

        #endregion
    }

    /// <summary>
    /// Interface for render systems.
    /// </summary>
    public interface IRenderSystem
    {
        /// <summary>
        /// Renders the system.
        /// </summary>
        /// <param name="context">Render context.</param>
        void Render(IRenderContext context);
    }

    /// <summary>
    /// Basic render context interface.
    /// </summary>
    public interface IRenderContext
    {
        /// <summary>
        /// Initializes the render context.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shuts down the render context.
        /// </summary>
        void Shutdown();
    }
}
