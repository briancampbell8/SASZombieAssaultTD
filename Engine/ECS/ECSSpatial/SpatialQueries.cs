// =====================================================================================================
//  FILE: SpatialQueries.cs
//  PATH: Engine/ECS/ECSRuntimeCore/SpatialQueries.cs
//  SUBSYSTEM: ECS ECSRuntimeCore
//
//  ROLE:
//      Provides deterministic spatial‑query operations for the ECSRuntimeCore subsystem.
//      Supplies safe, predictable radius‑based and area‑based ECSEntityCore lookup behaviors.
//      Ensures stable spatial access patterns with diagnostic tracing.
//
//  RESPONSIBILITIES:
//      - Provide FindEntitiesInRadius() behavior for the ECSRuntimeCore subsystem.
//      - Provide GetEntitiesInArea() behavior for the ECSRuntimeCore subsystem.
//      - Provide FindEntitiesWithComponent<T>() spatial‑filtered behavior.
//      - Integrate deterministic diagnostic tracing using DLogger.Log.
//
//  NON-RESPONSIBILITIES:
//      - Performing collision resolution or physics simulation.
//      - Managing spatial grid cell allocation or partitioning logic.
//      - Executing ECSEntityCore lifecycle or component storage operations.
//      - Performing multi‑component queries or system orchestration.
//
//  ARCHITECTURAL NOTES:
//      - SpatialQueries is a dedicated subsystem file extracted from ECSRuntimeCore.cs.
//      - Operates strictly on ECSRuntimeCore‑provided ECSEntityCore collections.
//      - All diagnostic output uses DLogger.Log for deterministic engine tracing.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.ECS.ECSSpatial
{
    internal sealed class SpatialQueries
    {
        private readonly ECSRuntimeCore _runtime;

        public SpatialQueries(ECSRuntimeCore runtime)
        {
            _runtime = runtime;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 1, "Init", "SpatialQueries subsystem initialized.");
        }

        /// <summary>
        /// Finds all entities within a radius of a given center point.
        /// </summary>
        public IEnumerable<ECSEntityCore> FindEntitiesInRadius(Vector3 center, float radius)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 2, "Query",
                $"Searching for entities within radius {radius} of ({center.X},{center.Y},{center.Z}).");

            var result = _runtime
                .Entities
                .All
                .Where(e =>
                {
                    var pos = e.Position;
                    float dist = Vector3.Distance(center, pos);

                    bool inside = dist <= radius;

                    DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 3, "Check",
                        $"Entity {e.Id}: Distance={dist}, InsideRadius={inside}");

                    return inside;
                })
                .ToList();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 4, "Result",
                $"Found {result.Count} entities inside radius {radius}.");

            return result;
        }

        /// <summary>
        /// Finds all entities inside a 3D axis‑aligned bounding box.
        /// </summary>
        public IEnumerable<ECSEntityCore> GetEntitiesInArea(Vector3 min, Vector3 max)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 5, "Query",
                $"Searching for entities inside area Min({min.X},{min.Y},{min.Z}) → Max({max.X},{max.Y},{max.Z}).");

            var result = _runtime
                .Entities
                .All
                .Where(e =>
                {
                    var pos = e.Position;

                    bool inside =
                        pos.X >= min.X && pos.X <= max.X &&
                        pos.Y >= min.Y && pos.Y <= max.Y &&
                        pos.Z >= min.Z && pos.Z <= max.Z;

                    DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 6, "Check",
                        $"Entity {e.Id}: Position=({pos.X},{pos.Y},{pos.Z}), InsideArea={inside}");

                    return inside;
                })
                .ToList();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 7, "Result",
                $"Found {result.Count} entities inside area bounds.");

            return result;
        }

        /// <summary>
        /// Finds all entities that contain component T (spatial‑agnostic).
        /// </summary>
        public IEnumerable<ECSEntityCore> FindEntitiesWithComponent<T>() where T : ECSComponents
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 8, "Query",
                $"Searching for entities with component {typeof(T).Name}.");

            var result = _runtime
                .Entities
                .All
                .Where(e => e.HasComponent<T>())
                .ToList();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntimeCore.SpatialQueries", 9, "Result",
                $"Found {result.Count} entities with component {typeof(T).Name}.");

            return result;
        }
    }
}
