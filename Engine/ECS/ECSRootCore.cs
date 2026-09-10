// ====================================================================================================
//  FILE: ECSRootCore.cs
//  PATH: Engine/ECS/
//  MODULE: ECS Root
//
//  ROLE:
//      Encapsulate core root‑level ECS behavior for the ECSRootCore module.
//      Serves as the foundational Core program for all ECS subsystems.
//      Provides root ECS services required across ECSDebug, ECSEntity, ECSRuntime,
//      ECSSystem, and all ECS root programs.
//
//  RESPONSIBILITIES:
//      - Provide root‑level ECS service contracts.
//      - Provide event dispatch infrastructure for the ECS subsystem.
//      - Provide shared ECS facilities required by multiple ECS subfolders.
//      - Act as the stable foundation layer beneath ECSManager.
//
//  NON-RESPONSIBILITIES:
//      - Low‑level data persistence or file serialization.
//      - Subsystem‑specific pipeline management (handled by each subfolder’s Core program).
//      - High‑level ECS orchestration (handled by ECSManager).
//
//  NOTES:
//      NEW PROGRAM STUB.
//      This file replaces legacy EventManager.cs and establishes the ECSRootCore foundation.
//      Implementation intentionally minimal until remaining legacy ECS programs are analyzed.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// ECSRootCore acts as the ECS kernel: a shared foundation layer providing root-level services, event dispatch, and
    /// global ECS counters. All higher-level ECS subsystems (Manager, Runtime, Entity, System, Query) may report into
    /// or consume services from this core.
    /// </summary>
    public sealed class ECSRootCore
    {
        // ---------------------------------------------------------------------------------------------
        // Root-Level Counters
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Total number of entities currently known to the ECS world.
        /// </summary>
        public int EntityCount { get; private set; }

        /// <summary>
        /// Total number of systems currently registered in the ECS world.
        /// </summary>
        public int SystemCount { get; private set; }

        /// <summary>
        /// Total number of components currently tracked by the ECS world.
        /// </summary>
        public int ComponentCount { get; private set; }

        /// <summary>
        /// Total number of events dispatched through the root event infrastructure.
        /// </summary>
        public int EventCount { get; private set; }

        /// <summary>
        /// Total number of queries executed through the root query infrastructure.
        /// </summary>
        public int QueryCount { get; private set; }

        /// <summary>
        /// Total number of commands executed through the root command infrastructure.
        /// </summary>
        public int CommandCount { get; private set; }

        // ---------------------------------------------------------------------------------------------
        // Root-Level Events
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when an ECSEntityCore is created.
        /// </summary>
        public event Action<ECSEntityCore>? EntityCreated;

        /// <summary>
        /// Raised when an ECSEntityCore is destroyed.
        /// </summary>
        public event Action<ECSEntityCore>? EntityDestroyed;

        /// <summary>
        /// Raised when a system is added.
        /// </summary>
        public event Action<Type>? SystemAdded;

        /// <summary>
        /// Raised when a system is removed.
        /// </summary>
        public event Action<Type>? SystemRemoved;

        /// <summary>
        /// Raised when a generic ECS event is dispatched.
        /// </summary>
        public event Action<object>? EventDispatched;

        // ---------------------------------------------------------------------------------------------
        // Root-Level Query & Command Delegates
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Delegate used to resolve queries for entities with specific component types. ECSRuntimeCore or ECSQueryCore
        /// may assign this.
        /// </summary>
        public Func<Type[], IReadOnlyCollection<ECSEntityCore>>? QueryEntitiesWithComponents { get; set; }

        /// <summary>
        /// Delegate used to resolve queries for entities within a radius. ECSRuntimeCore or ECSQueryCore may assign
        /// this.
        /// </summary>
        public Func<Vector3, float, IReadOnlyCollection<ECSEntityCore>>? QueryEntitiesInRadius { get; set; }

        /// <summary>
        /// Delegate used to execute ECS commands (e.g., deferred operations).
        /// </summary>
        public Action<object>? ExecuteCommand { get; set; }

        // ---------------------------------------------------------------------------------------------
        // Constructors
        // ---------------------------------------------------------------------------------------------

        public ECSRootCore()
        {
            EntityCount = 0;
            SystemCount = 0;
            ComponentCount = 0;
            EventCount = 0;
            QueryCount = 0;
            CommandCount = 0;
        }

        // ---------------------------------------------------------------------------------------------
        // Root-Level Reporting API
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Reports that an ECSEntityCore has been created and raises the corresponding event.
        /// </summary>
        public void ReportEntityCreated(ECSEntityCore ECSEntityCore)
        {
            EntityCount++;
            EntityCreated?.Invoke(ECSEntityCore);
        }

        /// <summary>
        /// Reports that an ECSEntityCore has been destroyed and raises the corresponding event.
        /// </summary>
        public void ReportEntityDestroyed(ECSEntityCore ECSEntityCore)
        {
            if (EntityCount > 0)
                EntityCount--;

            EntityDestroyed?.Invoke(ECSEntityCore);
        }

        /// <summary>
        /// Reports that a system has been added and raises the corresponding event.
        /// </summary>
        public void ReportSystemAdded(Type systemType)
        {
            SystemCount++;
            SystemAdded?.Invoke(systemType);
        }

        /// <summary>
        /// Reports that a system has been removed and raises the corresponding event.
        /// </summary>
        public void ReportSystemRemoved(Type systemType)
        {
            if (SystemCount > 0)
                SystemCount--;

            SystemRemoved?.Invoke(systemType);
        }

        /// <summary>
        /// Reports that a component has been added.
        /// </summary>
        public void ReportComponentAdded()
        {
            ComponentCount++;
        }

        /// <summary>
        /// Reports that a component has been removed.
        /// </summary>
        public void ReportComponentRemoved()
        {
            if (ComponentCount > 0)
                ComponentCount--;
        }

        // ---------------------------------------------------------------------------------------------
        // Root-Level Event Dispatch
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Dispatches a generic ECS event through the root event infrastructure.
        /// </summary>
        public void DispatchEvent(object evt)
        {
            EventCount++;
            EventDispatched?.Invoke(evt);
        }

        // ---------------------------------------------------------------------------------------------
        // Root-Level Query API
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Executes a query for entities that have all of the specified component types.
        /// </summary>
        public IReadOnlyCollection<ECSEntityCore> ExecuteComponentQuery(params Type[] componentTypes)
        {
            QueryCount++;

            if (QueryEntitiesWithComponents == null || componentTypes == null || componentTypes.Length == 0)
                return Array.Empty<ECSEntityCore>();

            return QueryEntitiesWithComponents(componentTypes);
        }

        /// <summary>
        /// Executes a query for entities within a given radius of a center point.
        /// </summary>
        public IReadOnlyCollection<ECSEntityCore> ExecuteRadiusQuery(Vector3 center, float radius)
        {
            QueryCount++;

            if (QueryEntitiesInRadius == null || radius <= 0f)
                return Array.Empty<ECSEntityCore>();

            return QueryEntitiesInRadius(center, radius);
        }

        // ---------------------------------------------------------------------------------------------
        // Root-Level Command API
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Executes a generic ECS command through the root command infrastructure.
        /// </summary>
        public void ExecuteEcsCommand(object command)
        {
            CommandCount++;

            ExecuteCommand?.Invoke(command);
        }

        // ---------------------------------------------------------------------------------------------
        // Reset & Diagnostics
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Resets all root-level counters to zero.
        /// </summary>
        public void ResetCounters()
        {
            EntityCount = 0;
            SystemCount = 0;
            ComponentCount = 0;
            EventCount = 0;
            QueryCount = 0;
            CommandCount = 0;
        }

        /// <summary>
        /// Returns a diagnostic string summarizing root-level ECS state.
        /// </summary>
        public override string ToString() =>
            $"ECSRootCore(Entities:{EntityCount}, Systems:{SystemCount}, Components:{ComponentCount}, " +
            $"Events:{EventCount}, Queries:{QueryCount}, Commands:{CommandCount})";
    
        public int NextEntityId { get; set; }

        public void OnEntityCreated(ECSEntityCore ECSEntityCore)
        {
            EntityCount++;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Entities", 2, "Create", $"Created ECSEntityCore {ECSEntityCore.Id}");
            // optional: logging or hooks
        }

        public void OnEntityDestroyed(ECSEntityCore ECSEntityCore)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Entities", 3, "Destroy", $"Destroyed ECSEntityCore {ECSEntityCore.Id}");
            EntityCount--;
            // optional: logging or hooks
        }

        public void OnAllEntitiesDestroyed()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Entities", 4, "DestroyAll", "Destroyed all entities");
            EntityCount = 0;
            // optional: logging or hooks
        }

    }
}
