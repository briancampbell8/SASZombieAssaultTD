// =====================================================================================================
//  FILE: EntitiesDebug.cs
//  PATH: Engine/ECS/ECSDebug/EntitiesDebug.cs
//  SUBSYSTEM: ECS ECSDebug
//
//  ROLE:
//      Provides deterministic ECSEntityCore‑level debug and inspection utilities for the ECS subsystem.
//      Supplies safe, predictable introspection of ECSEntityCore state, component listings, and targeted
//      component‑combination search behaviors.
//
//  RESPONSIBILITIES:
//      - Provide GetEntityDebugInfo() behavior for the ECSDebug subsystem.
//      - Provide FindEntitiesWithComponents() behavior for the ECSDebug subsystem.
//      - Integrate deterministic diagnostic tracing using DLogger.Log.
//      - Maintain strict, read‑only inspection semantics.
//
//  NON-RESPONSIBILITIES:
//      - Performing ECSEntityCore lifecycle operations.
//      - Mutating component collections or world state.
//      - Executing system update logic or rendering.
//      - Performing spatial or collision‑related queries.
//
//  ARCHITECTURAL NOTES:
//      - EntitiesDebug is a dedicated subsystem file extracted from the original ECSDebugInspector.
//      - All diagnostic output uses DLogger.Log for deterministic engine tracing.
//      - Suffix naming rule enforced: “Debug” is a suffix, not a prefix.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS.ECSDebug
{
    internal static class EntitiesDebug
    {
        // ---------------------------------------------------------------------------------------------
        // Entity Debug Info
        // ---------------------------------------------------------------------------------------------
        public static string GetEntityDebugInfo(ECSEntityCore entity, ECSRuntimeCore ecsWorld)
        {
            DLogger.Log(LogSubsystems.ECSECSDebug,
                "ECS.EntitiesDebug", $"Generating debug info for ECSEntityCore {entity?.Id}");

            if (entity == null)
                return "Entity is null";

            var info = new StringBuilder();
            info.Append($"Entity.{entity.Id}");

            // Components
            var components = ecsWorld.Components.GetAllComponents<object>(entity.Id);
            var list = components as IEnumerable<object> ?? Array.Empty<object>();

            if (list.Any())
            {
                info.AppendLine(" Components:");
                foreach (var component in list)
                    info.AppendLine($"    - {component?.GetType().Name}");
            }
            else
            {
                info.Append(" (No components)");
            }

            return info.ToString();
        }

        // ---------------------------------------------------------------------------------------------
        // Component Count
        // ---------------------------------------------------------------------------------------------
        private static int GetCount(ECSEntityCore entity, ECSRuntimeCore ecsWorld)
        {
            var components = ecsWorld.Components.GetAllComponents<object>(entity.Id);
            var list = components as IEnumerable<object> ?? Array.Empty<object>();
            return list.Count();
        }

        // ---------------------------------------------------------------------------------------------
        // Multi‑component search
        // ---------------------------------------------------------------------------------------------
        public static List<ECSEntityCore> FindEntitiesWithComponents(ECSRuntimeCore ecsWorld, params Type[] componentTypes)
        {
            DLogger.Log(LogSubsystems.ECSECSDebug,
                "ECS.EntitiesDebug", "Searching for entities with specific component combinations.");

            if (ecsWorld == null || componentTypes == null || componentTypes.Length == 0)
                return new List<ECSEntityCore>();

            var results = new List<ECSEntityCore>();

            foreach (var entity in ecsWorld.Entities.All)
            {
                var components = ecsWorld.Components.GetAllComponents<object>(entity.Id);
                var list = components as IEnumerable<object> ?? Array.Empty<object>();

                bool hasAll = componentTypes.All(t =>
                    list.Any(c => c != null && t.IsAssignableFrom(c.GetType())));

                if (hasAll)
                    results.Add(entity);
            }

            return results;
        }
    }
}
