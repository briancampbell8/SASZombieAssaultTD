// =====================================================================================================
//  FILE: ECSRuntimeEntities.cs
//  PATH: Engine/ECS/ECSRuntime/ECSRuntimeEntities.cs
//  SUBSYSTEM: ECS Runtime Entities
//
//  ROLE:
//      Deterministic ECSEntityCore management subsystem.
//      Creates, destroys, and enumerates ECSEntityCore instances for the ECS runtime.
//
//  RESPONSIBILITIES:
//      - Allocate unique entity IDs via ECSRootCore.
//      - Construct ECSEntityCore instances with proper ECSComponents and entity list references.
//      - Maintain active entity collections for runtime systems.
//      - Provide deterministic entity destruction and cleanup notifications.
//
//  NON-RESPONSIBILITIES:
//      - Component storage or mutation (delegated to ECSComponents).
//      - System update sequencing or world lifecycle operations.
//      - Physics, AI, or gameplay logic.
//
//  ARCHITECTURAL NOTES:
//      - ECSEntityCore instances must be created through ECSRuntimeEntities.Create().
//      - Entity list is shared with ECSEntityCore for deterministic iteration.
//      - Subsystem is strictly responsible for entity lifetime, not behavior.
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS
{
    internal sealed class ECSRuntimeEntities
    {
        private readonly List<ECSEntityCore> _entities = new();
        private readonly ECSRootCore _root;
        private readonly ECSComponents _components;

        /// <summary>
        /// Exposes a read‑only view of all entities for enumeration and debug inspection.
        /// </summary>
        public IReadOnlyList<ECSEntityCore> All => _entities;

        /// <summary>
        /// Total number of entities currently managed by the runtime.
        /// </summary>
        public int Count => _entities.Count;

        /// <summary>
        /// Active ECSEntityCore collection for runtime systems.
        /// </summary>
        public IReadOnlyCollection<ECSEntityCore> Active => _entities;

        public ECSRuntimeEntities(ECSRootCore root, ECSComponents components)
        {
            _root = root;
            _components = components;

            DLogger.Log(LogEnums.LogSubsystems.ResourcesPipeline,
                "ECSRuntime.Entities", 1, "Init",
                "ECSRuntimeEntities subsystem initialized.");
        }

        /// <summary>
        /// Creates a new ECSEntityCore and registers it with the ECS root.
        /// </summary>
        public ECSEntityCore Create()
        {
            uint id = (uint)_root.NextEntityId++;

            var entity = new ECSEntityCore(id, _components, _entities);

            _entities.Add(entity);
            _root.OnEntityCreated(entity);

            DLogger.Log(LogEnums.LogSubsystems.ResourcesPipeline,
                "ECSRuntime.Entities", 2, "Create",
                $"Created ECSEntityCore {id}");

            return entity;
        }

        /// <summary>
        /// Destroys a specific ECSEntityCore and notifies the root subsystem.
        /// </summary>
        public void Destroy(ECSEntityCore entity)
        {
            if (entity == null)
                return;

            _entities.Remove(entity);
            _root.OnEntityDestroyed(entity);

            DLogger.Log(LogEnums.LogSubsystems.ResourcesPipeline,
                "ECSRuntime.Entities", 3, "Destroy",
                $"Destroyed ECSEntityCore {entity.Id}");
        }

        /// <summary>
        /// Destroys all entities currently managed by the runtime.
        /// </summary>
        public void DestroyAll()
        {
            foreach (var e in _entities)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline,
                    "ECSRuntime.Entities", 4, "DestroyAll",
                    $"Destroying {e.Id}");
            }

            _entities.Clear();
            _root.OnAllEntitiesDestroyed();
        }

        /// <summary>
        /// Retrieves an ECSEntityCore by its unique identifier.
        /// </summary>
        public ECSEntityCore Get(uint id)
        {
            foreach (var e in _entities)
                if (e.Id == id)
                    return e;

            return null;
        }
    }
}
