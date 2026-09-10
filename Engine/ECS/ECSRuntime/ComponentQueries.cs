// =====================================================================================================
//  FILE: ComponentQueries.cs
//  PATH: Engine/ECS/ECSRuntimeCore/ComponentQueries.cs
//  SUBSYSTEM: ECS ECSRuntimeCore
//
//  ROLE:
//      Provides deterministic component‑query operations for the ECSRuntimeCore subsystem.
//      Supplies safe, predictable multi‑component ECSEntityCore search behaviors backed by ECSComponents.
//      Ensures stable query access patterns with diagnostic tracing for engine introspection.
//
//  RESPONSIBILITIES:
//      - Provide FindEntitiesWithComponents(params Type[]) behavior.
//      - Provide GetEntitiesWith(params Type[]) behavior.
//      - Provide GetEntitiesWith<T>() behavior.
//      - Provide GetEntitiesWith<T1, T2>() behavior.
//      - Provide GetEntitiesWithComponents<T1, T2>() behavior.
//      - Integrate deterministic diagnostic tracing using DLogger.Log.
//
//  NON-RESPONSIBILITIES:
//      - Managing component storage or lifecycle operations.
//      - Performing ECSEntityCore creation, destruction, or world‑level orchestration.
//      - Executing spatial or collision‑related queries.
//      - Mutating ECSComponents internal structures.
//
//  ARCHITECTURAL NOTES:
//      - ComponentQueries is a dedicated subsystem file extracted from ECSRuntimeCore.cs.
//      - Operates strictly on ECSRuntimeCore‑provided ECSEntityCore and ECSComponents references.
//      - All diagnostic output uses DLogger.Log for deterministic engine tracing.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS.ECSRuntime
{
    internal sealed class ComponentQueries
    {
        private readonly ECSRuntimeCore _runtime;
        private readonly ECSComponents _components;

        public ComponentQueries(ECSRuntimeCore runtime, ECSComponents components)
        {
            _runtime = runtime;
            _components = components;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 1, "Init",
                "ComponentQueries subsystem initialized.");
        }

        // ---------------------------------------------------------------------------------------------
        // Multi‑component search
        // ---------------------------------------------------------------------------------------------
        public IEnumerable<ECSEntityCore> FindEntitiesWithComponents(params Type[] componentTypes)
        {
            if (componentTypes == null || componentTypes.Length == 0)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 2, "Query",
                    "Empty componentTypes array supplied.");
                return Enumerable.Empty<ECSEntityCore>();
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 3, "Query",
                $"Searching for entities with ALL of the following components: {string.Join(", ", componentTypes.Select(t => t.Name))}");

            var hasComponentMethod = _runtime.GetType().GetMethod(nameof(_runtime.HasComponent));
            var matchedEntities = _runtime.ActiveEntities
                .Where(e => componentTypes.All(type =>
                {
                    var genericMethod = hasComponentMethod?.MakeGenericMethod(type);
                    return genericMethod != null && (bool)genericMethod.Invoke(_runtime, new object[] { e });
                }))
                .ToList();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 4, "Result",
                $"Found {matchedEntities.Count} entities with ALL requested components.");

            return matchedEntities;
        }

        public IEnumerable<ECSEntityCore> GetEntitiesWith(params Type[] componentTypes)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 5, "Query",
                $"GetEntitiesWith invoked for component types: {string.Join(", ", componentTypes.Select(t => t.Name))}");

            return FindEntitiesWithComponents(componentTypes);
        }

        // ---------------------------------------------------------------------------------------------
        // Single‑component search
        // ---------------------------------------------------------------------------------------------
        public IEnumerable<ECSEntityCore> GetEntitiesWith<T>() where T : class
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 6, "Query",
                $"Searching for entities with component: {typeof(T).Name}");

            var result = _runtime.ActiveEntities
                .Where(e => _runtime.HasComponent<T>(e))
                .ToList();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 7, "Result",
                $"Found {result.Count} entities with component {typeof(T).Name}.");

            return result;
        }

        // ---------------------------------------------------------------------------------------------
        // Dual‑component search
        // ---------------------------------------------------------------------------------------------
        public IEnumerable<ECSEntityCore> GetEntitiesWith<T1, T2>()
            where T1 : class
            where T2 : class
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 8, "Query",
                $"Searching for entities with components: {typeof(T1).Name} AND {typeof(T2).Name}");

            var result = _runtime.ActiveEntities
                .Where(e =>
                    _runtime.HasComponent<T1>(e) &&
                    _runtime.HasComponent<T2>(e)
                )
                .ToList();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 9, "Result",
                $"Found {result.Count} entities with BOTH {typeof(T1).Name} AND {typeof(T2).Name}.");

            return result;
        }

        public IEnumerable<ECSEntityCore> GetEntitiesWithComponents<T1, T2>()
            where T1 : class
            where T2 : class
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.ComponentQueries", 10, "Alias",
                $"GetEntitiesWithComponents<T1,T2> invoked for {typeof(T1).Name} AND {typeof(T2).Name}");

            return GetEntitiesWith<T1, T2>();
        }
    }
}
