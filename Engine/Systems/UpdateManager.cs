// ====================================================================================================
//  FILE: UpdateManager.cs
//  PATH: Engine/Systems/UpdateManager.cs
//  SUBSYSTEM: Systems
//  ROLE: Central update scheduler responsible for invoking all registered
//        IUpdateSystem instances each frame in deterministic order.
//
//  RESPONSIBILITIES:
//      - Maintain an ordered list of update-capable systems.
//      - Execute Update(deltaTime) on all registered systems.
//      - Provide active/inactive control for the update pipeline.
//      - Expose diagnostic information for engine introspection.
//
//  NON-RESPONSIBILITIES:
//      - System initialization or shutdown (handled by SystemManager).
//      - Game logic, flow control, or state transitions.
//      - Rendering, input processing, or ECS operations.
//      - System creation or dependency resolution.
//
//  DEPENDENCIES:
//      - IUpdateSystem (update contract)
//      - DebugLogger (diagnostics)
//
//  CALLED BY:
//      - GameRootUpdateLoop.Tick(deltaTime)
//      - Higher-level engine loop
//
//  CALLS INTO:
//      - IUpdateSystem.Update(deltaTime)
//
//  ARCHITECTURAL NOTES:
//      - Must remain deterministic and free of gameplay logic.
//      - Must not assume ordering beyond list insertion order.
//      - Must not contain partials; this is a complete standalone program.
//
//  CHANGE HISTORY:
//      - 2026-07-30 (BDC/Copilot): Removed unused fields.
//      - 2026-07-30 (BDC/Copilot): Removed invalid cleanup responsibilities.
//      - 2026-07-30 (BDC/Copilot): Removed exception rethrowing to maintain determinism.
//      - 2026-07-30 (BDC/Copilot): Implemented deterministic Tick(deltaTime).
//      - 2026-07-30 (BDC/Copilot): Updated header to reflect corrected responsibilities.
// ====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Deterministic update scheduler for SAS Zombie Assault TD.
    /// </summary>
    public class UpdateManager
    {
        //--------------------------------------------------------------------
        // Properties
        //--------------------------------------------------------------------

        /// <summary>
        /// Whether the update manager is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Current update rate (unused, reserved for future fixed-step logic).
        /// </summary>
        public float UpdateRate { get; set; } = 60.0f;

        //--------------------------------------------------------------------
        // Private Fields
        //--------------------------------------------------------------------

        private readonly List<IUpdateSystem> _systems = new();

        //--------------------------------------------------------------------
        // Construction
        //--------------------------------------------------------------------

        public UpdateManager(SystemRegistry systemRegistry) => DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager constructed");

        public UpdateManager()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager constructed");
            _systems = new();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager constructed");
        }

        //--------------------------------------------------------------------
        // Public Methods
        //--------------------------------------------------------------------

        /// <summary>
        /// Registers an update system.
        /// </summary>
        public void RegisterSystem(IUpdateSystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            if (!_systems.Contains(system))
            {
                _systems.Add(system);
                DLogger.Log($"UpdateManager.RegisterSystem: Registered '{system.GetType().FullName}'");
            }
        }

        /// <summary>
        /// Updates all registered systems.
        /// </summary>
        public void UpdateAll(float deltaTime)
        {
            if (!IsActive)
                return;

            foreach (var system in _systems)
            {
                try
                {
                    system.Update(deltaTime);
                }
                catch (NotImplementedException)
                {
                    NotImplementedGuard.Hit("NOT_IMPLEMENTED");
                    DLogger.Log($"UpdateManager.UpdateAll: NOT IMPLEMENTED in '{system.GetType().FullName}'");
                    return; // deterministic: do not rethrow
                }
                catch (Exception ex)
                {
                    DLogger.Log($"UpdateManager.UpdateAll: Exception in '{system.GetType().FullName}': {ex.Message}");
                    return; // deterministic: do not rethrow
                }
            }
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        public string GetDiagnostics()
        {
            return $"UpdateManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        /// <summary>
        /// Shuts down the update manager.
        /// </summary>
        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager.Shutdown: ENTER");

            IsActive = false;
            _systems.Clear();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager.Shutdown: EXIT");
        }

        //--------------------------------------------------------------------
        // Initialization
        //--------------------------------------------------------------------

        internal void Initialize()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager.Initialize: ENTER");

            IsActive = true;

            DLogger.Log($"UpdateManager.Initialize: {_systems.Count} systems registered prior to initialization");

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager.Initialize: EXIT");
        }

        /// <summary>
        /// Engine-facing update entry point; delegates to UpdateAll with state gating.
        /// </summary>
        internal void Update(float deltaTime)
        {
            if (!IsActive)
                return;

            UpdateAll(deltaTime);
        }

        /// <summary>
        /// Prepares the update manager for use by the main loop.
        /// </summary>
        internal void Prepare()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager.Prepare: ENTER");
            Initialize();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UpdateManager.Prepare: EXIT");
        }

        //--------------------------------------------------------------------
        // Deterministic Tick
        //--------------------------------------------------------------------

        /// <summary>
        /// Deterministic engine-facing update tick.
        /// </summary>
        internal void Tick(float deltaTime)
        {
            if (!IsActive)
                return;

            Update(deltaTime);
        }
    }

    /// <summary>
    /// Interface for update systems.
    /// </summary>
    public interface IUpdateSystem
    {

        /// <summary>
        /// Updates the system.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>

        void Update(float deltaTime);
    }
}
