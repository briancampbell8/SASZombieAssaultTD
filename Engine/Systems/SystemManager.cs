/* ====================================================================================================
 *  FILE: SystemManager.cs
 *  PATH: Engine/Systems/SystemManager.cs
 *  SUBSYSTEM: Systems
 *  ROLE: Central registry and initializer for engine systems.
 *
 *  RESPONSIBILITIES:
 *      - Maintain a registry of engine systems keyed by concrete type.
 *      - Provide system lookup and resolution for dependent subsystems.
 *      - Execute initialization for systems implementing IInitializable.
 *      - Execute shutdown and clear all registered systems.
 *      - Maintain active/inactive state for the manager.
 *
 *  NON-RESPONSIBILITIES:
 *      - Public API exposure beyond system registration and lookup.
 *      - Game logic, flow control, or subsystem orchestration.
 *      - ECS operations or entity lifecycle management.
 *      - Rendering, updating, or input processing.
 *
 *  DEPENDENCIES:
 *      - DebugLogger (diagnostics)
 *      - IInitializable (initialization contract)
 *
 *  CALLED BY:
 *      - GameRoot.PerformInitialization()
 *      - GameRoot.PerformShutdown()
 *
 *  CALLS INTO:
 *      - IInitializable.Initialize()
 *
 *  ARCHITECTURAL NOTES:
 *      - Must remain deterministic and free of gameplay logic.
 *      - Must not assume ordering beyond dictionary enumeration.
 *      - Must not create systems; only registers and initializes them.
 *      - Must not contain partials; this is a complete standalone program.
 * ==================================================================================================== */

using System;
using System.Collections.Generic;
//

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Systems
{
    ///<summary>
    ///Provides system registration, lookup, initialization, and shutdown.
    ///</summary>
    public class SystemManager
    {
        ///<summary>
        ///Indicates whether the manager is active.
        ///</summary>
        public bool IsActive { get; private set; } = true;

        ///<summary>
        ///Stores registered systems keyed by concrete type.
        ///</summary>
        private readonly Dictionary<Type, object> _systems = new();
        private object TheContainingType;
        private object TheContainingMember;

        ///<summary>
        ///Constructs the SystemManager.
        ///</summary>
        public SystemManager()
        {
            DLogger.Log("SystemManager constructed");
        }

        ///<summary>
        ///Registers a system instance under its concrete type.
        ///</summary>
        public void RegisterSystem<T>(T system) where T : class
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            var type = typeof(T);
            _systems[type] = system;

            DLogger.Log($"SystemManager.RegisterSystem: Registered '{type.FullName}'");
        }

        ///<summary>
        ///Returns a registered system instance or null.
        ///</summary>
        public T GetSystem<T>() where T : class
        {
            var type = typeof(T);

            if (_systems.TryGetValue(type, out var system))
            {
                DLogger.Log($"SystemManager.GetSystem: Resolved '{type.FullName}'");
                return system as T;
            }

            DLogger.Log($"SystemManager.GetSystem: '{type.FullName}' not found");
            return null;
        }

        ///<summary>
        ///Returns diagnostic information about the registry.
        ///</summary>
        public string GetDiagnostics()
        {
            return $"SystemManager: {_systems.Count} systems registered, Active: {IsActive}";
        }

        ///<summary>
        ///Initializes all registered systems implementing IInitializable.
        ///</summary>
        internal void Initialize()
        {
            DLogger.Log("SystemManager.Initialize: ENTER");

            if (_systems.Count == 0)
            {
                DLogger.Log(
                    "ENGINE INIT DIAGNOSTIC — SystemManager.Initialize() invoked with ZERO registered systems.\n" +
                    "Subsystem: SystemManager\n" +
                    "File: SystemManager.cs\n" +
                    "Method: Initialize()\n" +
                    $"Timestamp: {DateTime.Now:O}\n" +
                    "Expected: At least one system registered before initialization.\n" +
                    "Actual: _systems.Count == 0\n" +
                    "Impact: Initialization stops at SystemManager.\n" +
                    "Next Step: Register required systems before calling GameRoot.Initialize()."
                );

                DLogger.Log("SystemManager.Initialize: No systems registered");
                DLogger.Log("SystemManager.Initialize: EXIT (no systems)");
                return;
            }

            foreach (var kvp in _systems)
            {
                var systemType = kvp.Key;
                var systemInstance = kvp.Value;

                DLogger.Log($"SystemManager.Initialize: Processing '{systemType.FullName}'");

                //Detect accidental NotImplemented placeholders
                if (systemInstance is null)
                {
                    DLogger.Log($"SystemManager.Initialize: NULL system instance for '{systemType.FullName}'");
                    continue;
                }

                if (systemInstance is IInitializable initializable)
                {
                    try
                    {
                        DLogger.Log($"SystemManager.Initialize: Initializing '{systemType.FullName}'");
                        initializable.Initialize();
                        DLogger.Log($"SystemManager.Initialize: Initialized '{systemType.FullName}'");
                    }
                    catch (NotImplementedException niex)
                    {
                        NotImplementedGuard.Hit("NOT_IMPLEMENTED");

                        DLogger.Log(
                            $"SystemManager.Initialize: NOT IMPLEMENTED in '{systemType.FullName}'"
                        );
                        DLogger.Log(niex.ToString(), $"SystemManager.Initialize:{systemType.FullName}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        DLogger.Log(
                            $"SystemManager.Initialize: Failure in '{systemType.FullName}': {ex.Message}"
                        );
                        DLogger.Log(ex.ToString(),
                            $"SystemManager.Initialize:{systemType.FullName}");

                        throw new InvalidOperationException(
                            $"SystemManager.Initialize: Critical failure in '{systemType.FullName}'",
                            ex
                        );
                    }
                }
                else
                {
                    DLogger.Log(
                        $"SystemManager.Initialize: '{systemType.FullName}' does not implement IInitializable"
                    );
                }
            }

            DLogger.Log("SystemManager.Initialize: EXIT");
        }

        ///<summary>
        ///Clears all systems and marks the manager inactive.
        ///</summary>
        public void Shutdown()
        {
            DLogger.Log("SystemManager.Shutdown: ENTER");

            IsActive = false;
            _systems.Clear();

            DLogger.Log("SystemManager.Shutdown: EXIT");
        }
    }

    ///<summary>
    ///Defines a system requiring explicit initialization.
    ///</summary>
    public interface IInitializable
    {
        void Initialize();
    }
}
