// ====================================================================================================
//  FILE: ECSEntityCore.cs
//  PATH: Engine/ECS/ECSEntity/
//  SUBSYSTEM: ECS > EntityCore
//
//  ROLE:
//      Represents a single ECS entity and provides access to its components.
//      Acts as the primary identity object for ECS queries and system processing.
//
//  RESPONSIBILITIES:
//      - Store entity ID.
//      - Provide deterministic component access via ECSComponents.
//      - Expose ECS query helpers for systems such as TriggerSystem.
//      - Maintain stable identity for hashing and dictionary usage.
//
//  NON-RESPONSIBILITIES:
//      - Component storage (handled by ECSComponents).
//      - System execution (handled by ECSRuntimeCore).
//      - Event routing (handled by EventRouting / ECSRuntimeEvents).
//
//  ARCHITECTURAL NOTES:
//      - Entities are lightweight identity objects.
//      - All component logic is delegated to ECSComponents.
//      - Query helpers must remain stable for subsystem compatibility.
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.ECS
{
    public sealed class ECSEntityCore
    {
        // ----------------------------------------------------------------------------------------------
        // Identity
        // ----------------------------------------------------------------------------------------------
        public uint Id { get; }

        // ----------------------------------------------------------------------------------------------
        // Component Storage (delegated to ECSComponents)
        // ----------------------------------------------------------------------------------------------
        private readonly ECSComponents _components;

        // ----------------------------------------------------------------------------------------------
        // Entity Registry (owned by ECSRuntimeCore, injected here)
        // ----------------------------------------------------------------------------------------------
        private readonly List<ECSEntityCore> _entities;

        // ----------------------------------------------------------------------------------------------
        // Constructor
        // ----------------------------------------------------------------------------------------------
        public ECSEntityCore(uint id, ECSComponents components, List<ECSEntityCore> entities)
        {
            Id = id;
            _components = components ?? throw new ArgumentNullException(nameof(components));
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        public ECSEntityCore(int v)
        {
            V = v;
        }

        public ECSEntityCore(uint eCSEntityCoreId)
        {
            ECSEntityCoreId = eCSEntityCoreId;
        }

        // ----------------------------------------------------------------------------------------------
        // Component Access
        // ----------------------------------------------------------------------------------------------
        public bool HasComponent<T>() where T : class =>
            _components.HasComponent<T>(Id);

        public T GetComponent<T>() where T : class =>
            _components.GetComponent<T>(Id);
        public T GetAllEntities<T>() where T : class =>
           _components.GetComponent<T>(Id);

        // ----------------------------------------------------------------------------------------------
        // ECS Query Helpers (required by TriggerSystem) GetAllEntities
        // ----------------------------------------------------------------------------------------------
        public IReadOnlyList<ECSEntityCore> GetEntitiesWithTriggerAndTransform =>
            _entities.Where(e =>
                _components.HasComponent<TriggerComponent>(e.Id) &&
                _components.HasComponent<TransformComponent>(e.Id)
            ).ToList();

        public IReadOnlyList<ECSEntityCore> GetEntitiesWithCollisionAndTransform =>
            _entities.Where(e =>
                _components.HasComponent<CollisionComponent>(e.Id) &&
                _components.HasComponent<TransformComponent>(e.Id)
            ).ToList();

        public Vector3 Position { get; internal set; }
        public bool IsValid { get; internal set; }
        public int V { get; }
        public uint ECSEntityCoreId { get; }
        public IEnumerable<object> Entities { get; internal set; }
        public IEnumerable<object> ComponentsList { get; internal set; }
        public int ComponentCount { get; internal set; }
        public bool Destroyed { get; internal set; }

        // ----------------------------------------------------------------------------------------------
        // Debugging
        // ----------------------------------------------------------------------------------------------
        public override string ToString() =>
            $"ECSEntityCore(Id={Id})";
    }
}
