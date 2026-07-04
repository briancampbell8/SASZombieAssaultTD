/* ====================================================================================================
 *  FILE: UpdateManager.cs
 *  PATH: Engine/Systems/UpdateManager.cs
 *  SUBSYSTEM: Systems
 *  ROLE: Central update scheduler responsible for invoking all registered
 *        IUpdateSystem instances each frame in deterministic order.
 *
 *  RESPONSIBILITIES:
 *      - Maintain an ordered list of update-capable systems.
 *      - Execute Update(deltaTime) on all registered systems.
 *      - Provide active/inactive control for the update pipeline.
 *      - Expose diagnostic information for engine introspection.
 *
 *  NON-RESPONSIBILITIES:
 *      - System initialization or shutdown (handled by SystemManager).
 *      - Game logic, flow control, or state transitions.
 *      - Rendering, input processing, or ECS operations.
 *      - System creation or dependency resolution.
 *
 *  DEPENDENCIES:
 *      - IUpdateSystem (update contract)
 *      - DebugLogger (diagnostics)
 *
 *  CALLED BY:
 *      - GameRoot.PerformUpdate()
 *      - Higher-level engine loop
 *
 *  CALLS INTO:
 *      - IUpdateSystem.Update(deltaTime)
 *
 *  ARCHITECTURAL NOTES:
 *      - Must remain deterministic and free of gameplay logic.
 *      - Must not assume ordering beyond list insertion order.
 *      - Must not contain partials; this is a complete standalone program.
 * ==================================================================================================== */

using System;
using System.Collections.Generic;
//

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Systems
{
    ///<summary>
    ///Update management system for SAS Zombie Assault TD.
    ///Manages game loop updates and system scheduling.
    ///</summary>
    public class UpdateManager
    {
        //--------------------------------------------------------------------
        //Properties
        //--------------------------------------------------------------------

        ///<summary>
        ///Whether the update manager is active.
        ///</summary>
        public bool IsActive { get; set; } = true;

        ///<summary>
        ///Current update rate (unused, reserved for future fixed-step logic).
        ///</summary>
        public float UpdateRate { get; set; } = 60.0f;

        //--------------------------------------------------------------------
        //Private Fields
        //--------------------------------------------------------------------

        private readonly List<IUpdateSystem> _systems = new();
        private object TheContainingType;
        private object TheContainingMember;

        //--------------------------------------------------------------------
        //Construction
        //--------------------------------------------------------------------

        ///<summary>
        ///Creates a new UpdateManager instance.
        ///</summary>
        public UpdateManager()
        {
            DLogger.Log("UpdateManager constructed");
        }

        //--------------------------------------------------------------------
        //Public Methods
        //--------------------------------------------------------------------

        ///<summary>
        ///Registers an update system.
        ///</summary>
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

        ///<summary>
        ///Updates all registered systems.
        ///</summary>
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
                catch (NotImplementedException niex)
                {
                    NotImplementedGuard.Hit("NOT_IMPLEMENTED");

                    DLogger.Log(
                        $"UpdateManager.UpdateAll: NOT IMPLEMENTED in '{system.GetType().FullName}'"
                    );
                    DLogger.Log(niex.ToString(), $"UpdateManager.UpdateAll:{system.GetType().FullName}");
                    throw;
                }
                catch (Exception ex)
                {
                    DLogger.Log(
                        $"UpdateManager.UpdateAll: Exception in '{system.GetType().FullName}': {ex.Message}"
                    );
                    DLogger.Log(ex.ToString(),
                        $"UpdateManager.UpdateAll:{system.GetType().FullName}");
                    throw;
                }
            }
        }

        ///<summary>
        ///Gets diagnostic information.
        ///</summary>
        public string GetDiagnostics()
        {
            return $"UpdateManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        ///<summary>
        ///Shuts down the update manager.
        ///</summary>
        public void Shutdown()
        {
            DLogger.Log("UpdateManager.Shutdown: ENTER");

            IsActive = false;
            _systems.Clear();

            DLogger.Log("UpdateManager.Shutdown: EXIT");
        }

        //--------------------------------------------------------------------
        //REAL INITIALIZATION IMPLEMENTATION
        //--------------------------------------------------------------------

        internal void Initialize()
        {
            DLogger.Log("UpdateManager.Initialize: ENTER");

            //Ensure the manager is active before updates begin
            IsActive = true;

            //Diagnostics: report how many systems were registered before initialization
            DLogger.Log(
                $"UpdateManager.Initialize: {_systems.Count} systems registered prior to initialization"
            );

            //Validate that no null entries exist (defensive integrity check)
            for (int i = _systems.Count - 1; i >= 0; i--)
            {
                if (_systems[i] == null)
                {
                    DLogger.Log("UpdateManager.Initialize: Null system removed from list");
                    _systems.RemoveAt(i);
                }
            }

            DLogger.Log("UpdateManager.Initialize: EXIT");
        }
    }

    ///<summary>
    ///Interface for update systems.
    ///</summary>
    public interface IUpdateSystem
    {
        ///<summary>
        ///Updates the system.
        ///</summary>
        void Update(float deltaTime);
    }
}
