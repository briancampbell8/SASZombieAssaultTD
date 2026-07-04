//============================================================================
//PATH: Engine/Interfaces/IManager.cs
//
//FILE: IManager.cs
//PURPOSE:
//    Defines the contract for all engine managers.
//    Provides standardized lifecycle management, diagnostics, and state tracking
//    for all manager implementations in the engine architecture.
//
//ROLE IN ENGINE:
//    - Base interface for SystemManager, UpdateManager, RenderManager, InputManager
//    - Provides consistent initialization and shutdown patterns
//    - Ensures all managers have diagnostic capabilities
//    - Forms the foundation of the engine's manager architecture
//
//P-MILESTONE ALIGNMENT:
//    - P11-02: System orchestration and dependency resolution
//    - P11-04: Deterministic system lifecycle management
//    - P11-09: Authoritative engine initialization and wiring
//
//NOTES:
//    - Interface only — no implementation details
//    - Implementation provided by ManagerBase.cs
//
//============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public enum ManagerStatus
    {
        Inactive = 0,
        Active = 1,
        Uninitialized,
        Initializing,
        Running,
        ShuttingDown,
        Shutdown,
        Error
    }

    public class ManagerDiagnostics
    {
        public string Name { get; set; } = string.Empty;
        public ManagerStatus Status { get; set; }
        public int SystemCount { get; set; }
        public TimeSpan LastUpdateTime { get; set; }
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();

        ///<summary>
        ///Updates a specific metric in the diagnostics.
        ///</summary>
        ///<param name="key">The metric key.</param>
        ///<param name="value">The metric value.</param>
        public void UpdateMetric(string key, object value)
        {
            Metrics[key] = value;
        }

        ///<summary>
        ///Retrieves a specific metric by key.
        ///</summary>
        ///<param name="key">The metric key.</param>
        ///<returns>The metric value, or null if not found.</returns>
        public object? GetMetric(string key)
        {
            Metrics.TryGetValue(key, out var value);
            return value;
        }
    }

    ///<summary>
    ///Defines the contract for all engine managers.
    ///Provides standardized lifecycle management, diagnostics, and state tracking
    ///for all manager implementations in the engine architecture.
    ///</summary>
    public interface IManager
    {
        ///<summary>
        ///Gets the current status of the manager.
        ///</summary>
        ManagerStatus Status { get; }

        ///<summary>
        ///Gets the name of the manager.
        ///</summary>
        string Name { get; }

        ///<summary>
        ///Initializes the manager.
        ///Called once during engine startup.
        ///</summary>
        void Initialize();

        ///<summary>
        ///Initializes the manager asynchronously.
        ///Called once during engine startup.
        ///</summary>
        Task InitializeAsync();

        ///<summary>
        ///Shuts down the manager.
        ///Called once during engine shutdown.
        ///</summary>
        void Shutdown();

        ///<summary>
        ///Shuts down the manager asynchronously.
        ///Called once during engine shutdown.
        ///</summary>
        Task ShutdownAsync();

        ///<summary>
        ///Gets diagnostic information about the manager.
        ///</summary>
        ///<returns>Diagnostic information including status and metrics.</returns>
        ManagerDiagnostics GetDiagnostics();

        ///<summary>
        ///Event triggered when the manager is initialized.
        ///</summary>
        event Action OnInitialized;

        ///<summary>
        ///Event triggered when the manager is shut down.
        ///</summary>
        event Action OnShutdown;
    }
}
