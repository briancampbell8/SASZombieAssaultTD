/* ====================================================================================================
 *  FILE: RenderManager.cs
 *  PATH: Engine/Systems/
 *  SUBSYSTEM: Systems
 *  ROLE: Central render scheduler responsible for invoking all registered
 *        IRenderSystem instances each frame in deterministic order.
 *
 *  RESPONSIBILITIES:
 *      - Maintain an ordered list of render-capable systems.
 *      - Execute Render(context) on all registered systems.
 *      - Maintain and expose the active render context.
 *      - Provide diagnostics for engine introspection.
 *
 *  NON-RESPONSIBILITIES:
 *      - Creating or owning the render context (provided externally).
 *      - System initialization or shutdown (handled by SystemManager).
 *      - Game logic, ECS operations, or state transitions.
 *      - Resource loading or GPU pipeline configuration.
 *
 *  DEPENDENCIES:
 *      - IRenderSystem (render contract)
 *      - IRenderContext (context contract)
 *      - DebugLogger (diagnostics)
 *
 *  CALLED BY:
 *      - GameRoot.PerformRender()
 *      - Higher-level engine loop
 *
 *  CALLS INTO:
 *      - IRenderSystem.Render(IRenderContext)
 *
 *  ARCHITECTURAL NOTES:
 *      - Must remain deterministic and free of gameplay logic.
 *      - Must not assume ordering beyond list insertion order.
 *      - Must not create or destroy render contexts.
 *      - Must not contain partials; this is a complete standalone program.
 * ==================================================================================================== */

using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Manages the rendering pipeline and dispatches render calls to registered systems.
    /// </summary>
    public class RenderManager
    {
        // ----------------------------------------------------------------------------------------------------
        //  Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Whether the render manager is active and should process render calls.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// The active render context used by all render systems.
        /// Must be assigned externally before rendering begins.
        /// </summary>
        public IRenderContext Context { get; set; }

        // ----------------------------------------------------------------------------------------------------
        //  Private Fields
        // ----------------------------------------------------------------------------------------------------

        private readonly List<IRenderSystem> _systems = new();
        private object TheType;
        private object TheMember;

        // ----------------------------------------------------------------------------------------------------
        //  Construction
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Creates a new RenderManager instance.
        /// </summary>
        public RenderManager()
        {
            DebugLogger.LogInfo("RenderManager constructed");
        }

        // ----------------------------------------------------------------------------------------------------
        //  Public API
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Registers a render system for participation in the render pipeline.
        /// </summary>
        /// <param name="system">The render system to register.</param>
        public void RegisterSystem(IRenderSystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            if (!_systems.Contains(system))
            {
                _systems.Add(system);
                DebugLogger.LogInfo($"RenderManager.RegisterSystem: Registered '{system.GetType().FullName}'");
            }
        }

        /// <summary>
        /// Invokes Render(context) on all registered render systems.
        /// </summary>
        public void RenderAll()
        {
            if (!IsActive)
                return;

            if (Context == null)
            {
                DebugLogger.LogError("RenderManager.RenderAll: No render context assigned");
                return;
            }

            foreach (var system in _systems)
            {
                try
                {
                    system.Render(Context);
                }
                catch (Exception ex)
                {
                    DebugLogger.LogError(
                        $"RenderManager.RenderAll: Exception in '{system.GetType().FullName}': {ex.Message}"
                    );
                    DebugLogger.Exception(ex, $"RenderManager.RenderAll:{system.GetType().FullName}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns diagnostic information about the render manager.
        /// </summary>
        public string GetDiagnostics()
        {
            return $"RenderManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        /// <summary>
        /// Shuts down the render manager and clears all registered systems.
        /// </summary>
        public void Shutdown()
        {
            DebugLogger.LogInfo("RenderManager.Shutdown: ENTER");

            IsActive = false;
            _systems.Clear();

            DebugLogger.LogInfo("RenderManager.Shutdown: EXIT");
        }

        // ----------------------------------------------------------------------------------------------------
        //  REAL INITIALIZATION IMPLEMENTATION
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Initializes the render manager and validates the render context.
        /// </summary>
        internal void Initialize()
        {
            DebugLogger.LogInfo("RenderManager.Initialize: ENTER");

            // Ensure the manager is active
            IsActive = true;

            // Validate that a render context has been assigned
            if (Context == null)
            {
                DebugLogger.LogError("RenderManager.Initialize: No render context assigned");
                throw new InvalidOperationException("RenderManager requires a valid IRenderContext before initialization.");
            }

            // Initialize the render context itself
            try
            {
                DebugLogger.LogInfo("RenderManager.Initialize: Initializing render context...");
                Context.Initialize();
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("RenderManager.Initialize: Render context initialization failed");
                DebugLogger.Exception(ex, "RenderManager.Initialize");
                throw;
            }

            // Validate registered systems (remove nulls)
            for (int i = _systems.Count - 1; i >= 0; i--)
            {
                if (_systems[i] == null)
                {
                    DebugLogger.LogWarning("RenderManager.Initialize: Null render system removed");
                    _systems.RemoveAt(i);
                }
            }

            DebugLogger.LogInfo($"RenderManager.Initialize: {_systems.Count} systems registered");

            DebugLogger.LogInfo("RenderManager.Initialize: EXIT");
        }
    }

    // --------------------------------------------------------------------------------------------------------
    //  Interfaces
    // --------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Contract for render-capable systems.
    /// </summary>
    public interface IRenderSystem
    {
        /// <summary>
        /// Renders the system using the provided render context.
        /// </summary>
        void Render(IRenderContext context);
    }

    /// <summary>
    /// Contract for render context implementations.
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
