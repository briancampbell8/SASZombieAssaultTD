// =====================================================================================================
//  FILE: CollisionRuntime.cs
//  PATH: Engine/ECS/ECSRuntimeCore/CollisionRuntime.cs
//  SUBSYSTEM: ECS ECSRuntimeCore
//
//  ROLE:
//      Provides deterministic collision‑related world query operations for the ECSRuntimeCore subsystem.
//      Supplies spatial‑grid‑driven collision pair generation and exposes collision‑ready ECSEntityCore sets.
//      Ensures safe, predictable, engine‑level collision enumeration without performing physics resolution.
//
//  RESPONSIBILITIES:
//      - Provide GetPotentialCollisions() behavior for the ECSRuntimeCore subsystem.
//      - Provide collision‑ready ECSEntityCore enumeration using ECS component presence.
//      - Integrate diagnostic tracing for collision query operations.
//      - Maintain deterministic ordering and safe iteration of ECSEntityCore collections.
//
//  NON-RESPONSIBILITIES:
//      - Performing physics collision resolution or response.
//      - Managing spatial grid cell allocation or partitioning logic.
//      - Mutating ECSEntityCore state, components, or world topology.
//      - Executing rendering or update‑loop responsibilities.
//
//  ARCHITECTURAL NOTES:
//      - CollisionRuntime is a dedicated subsystem file extracted from ECSRuntimeCore.cs.
//      - Operates strictly on ECSRuntimeCore‑provided ECSEntityCore collections and spatial grid.
//      - All diagnostic output uses DLogger.Log for deterministic engine tracing.
// =====================================================================================================
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Physics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS.ECSRuntime
{
    internal sealed class CollisionRuntime
    {
        private readonly ECSRuntimeCore _runtime;

        public CollisionRuntime(ECSRuntimeCore runtime)
        {
            _runtime = runtime;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.Collision", 1, "Init", "CollisionRuntime subsystem initialized.");
        }

        /// <summary>
        /// Returns all potential collision pairs based on entities containing ColliderComponent.
        /// Deterministic ordering is preserved.
        /// </summary>
        public IEnumerable<(ECSEntityCore, ECSEntityCore)> GetPotentialCollisions()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.Collision", 2, "Query", "Collecting collidable entities.");

            var collidables = _runtime
                .Entities
                .All
                .Where(e => _runtime.HasComponent<ColliderCompCore>(e))
                .ToList();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.Collision", 3, "Query", $"Collidable ECSEntityCore count: {collidables.Count}");

            for (int i = 0; i < collidables.Count; i++)
            {
                for (int j = i + 1; j < collidables.Count; j++)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.Collision", 4, "Pair",
                        $"Yielding potential collision pair: {collidables[i].Id} ↔ {collidables[j].Id}");

                    yield return (collidables[i], collidables[j]);
                }
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.Collision", 5, "Complete", "Collision pair enumeration completed.");
        }
    }
}
