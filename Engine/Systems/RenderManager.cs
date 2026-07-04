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

//
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering.D3D11;
namespace SASZombieAssaultTD.Engine.Systems
{
    ///<summary>
    ///Manages the rendering pipeline and dispatches render calls to registered systems.
    ///</summary>
    public sealed class RenderManager
    {
        //----------------------------------------------------------------------------------------------------
        // Properties
        //----------------------------------------------------------------------------------------------------

        ///<summary>
        ///Whether the render manager is active and should process render calls.
        ///</summary>
        public bool IsActive { get; set; } = true;

        ///<summary>
        ///The active render context used by all render systems.
        ///Must be assigned externally before rendering begins.
        ///</summary>
        public IRenderContext? Context { get; private set; }

        //----------------------------------------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------------------------------------

        private readonly List<IRenderSystem> _systems = new();
        internal static object Instance;

        //----------------------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------------------

        ///<summary>
        ///Creates a new RenderManager instance.
        ///</summary>
        public RenderManager()
        {
            DLogger.Log("RenderManager constructed");
        }

        //----------------------------------------------------------------------------------------------------
        // Public API
        //----------------------------------------------------------------------------------------------------

        ///<summary>
        ///Assigns the render context used by all render systems.
        ///</summary>
        public void SetRenderContext(IRenderContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DLogger.Log($"RenderManager.SetRenderContext: Assigned '{context.GetType().Name}'");
        }

        ///<summary>
        ///Registers a render system for participation in the render pipeline.
        ///</summary>
        public void RegisterSystem(IRenderSystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            if (!_systems.Contains(system))
            {
                _systems.Add(system);
                DLogger.Log($"RenderManager.RegisterSystem: Registered '{system.GetType().FullName}'");
            }
        }

        ///<summary>
        ///Invokes Render(context) on all registered render systems.
        ///</summary>
        public void RenderAll()
        {
            if (!IsActive)
                return;

            if (Context == null)
            {
                DLogger.Log("RenderManager.RenderAll: No render context assigned");
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
                    DLogger.Log(
                        $"RenderManager.RenderAll: Exception in '{system.GetType().FullName}': {ex.Message}"
                    );
                    DLogger.Log(
                        LogSubsystems.Systems,
                        LogLevel.Error,
                        $"RenderManager.RenderAll: Exception in '{system.GetType().FullName}': {ex.Message}"
                        );
                    throw;
                }
            }
        }

        ///<summary>
        ///Returns diagnostic information about the render manager.
        ///</summary>
        public string GetDiagnostics()
        {
            return $"RenderManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        ///<summary>
        ///Shuts down the render manager and clears all registered systems.
        ///</summary>
        public void Shutdown()
        {
            DLogger.Log("RenderManager.Shutdown: ENTER");

            IsActive = false;
            _systems.Clear();

            DLogger.Log("RenderManager.Shutdown: EXIT");
        }

        //----------------------------------------------------------------------------------------------------
        // Initialization
        //----------------------------------------------------------------------------------------------------

        ///<summary>
        ///Initializes the render manager and validates the render context.
        ///</summary>
        internal void Initialize()
        {
            DLogger.Log("RenderManager.Initialize: ENTER");

            IsActive = true;

            if (Context == null)
            {
                DLogger.Log("RenderManager.Initialize: No render context assigned");
                throw new InvalidOperationException("RenderManager requires a valid IRenderContext before initialization.");
            }

            try
            {
                DLogger.Log("RenderManager.Initialize: Initializing render context...");
                Context.Initialize();
            }
            catch (Exception ex)
            {
                DLogger.Log("RenderManager.Initialize: Render context initialization failed");
                DLogger.Log(
                    LogSubsystems.Systems,
                    LogLevel.Error,
                    ex.ToString(),
                    "RenderManager.Initialize");
                throw;
            }

            //Remove null systems (defensive cleanup)
            for (int i = _systems.Count - 1; i >= 0; i--)
            {
                if (_systems[i] == null)
                {
                    DLogger.Log("RenderManager.Initialize: Null render system removed");
                    _systems.RemoveAt(i);
                }
            }

            DLogger.Log($"RenderManager.Initialize: {_systems.Count} systems registered");
            DLogger.Log("RenderManager.Initialize: EXIT");
        }

        internal void SetRenderContext(RenderContextD3D11 renderContext)
        {
            throw new NotImplementedException();
        }

        internal void SetRenderContext(Rendering.RenderContextD3D11Adapter renderContextAdapter)
        {
            throw new NotImplementedException();
        }
    }

    //--------------------------------------------------------------------------------------------------------
    // Interfaces
    //--------------------------------------------------------------------------------------------------------

    public interface IRenderSystem
    {
        void Render(IRenderContext context);
    }

    public interface IRenderContext
    {
        void Initialize();
        void Shutdown();
    }
}
