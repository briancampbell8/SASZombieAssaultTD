// ====================================================================================================
//  FILE: ColliderCompCore.cs
//  PATH: Engine/Physics/Colliders/ColliderCompCore.cs
//  MODULE: Physics Colliders
//
//  ROLE:
//      Defines the core collider component used by the Physics Collision Subsystem.
//      Stores collider state, flags, and indexing information for ECS entities.
//
//  RESPONSIBILITIES:
//      - Represent collider metadata for an ECS ECSEntityCore.
//      - Provide flags for static, trigger, and sensor behavior.
//      - Supply indexing information for collision lookup tables.
//      - Support collision detection systems by exposing collider state.
//
//  NON-RESPONSIBILITIES:
//      - Performing collision detection or shape math.
//      - Managing transform or physics components.
//      - Rendering or imaging operations.
//      - Publishing collision events.
//      - Performing spatial queries.
//
//  NOTES:
//      ColliderCompCore is a lightweight data component.
//      It contains no simulation logic and performs no physics calculations.
//      It is updated only to maintain internal collider flags.
// ====================================================================================================


using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Physics.Collision;

namespace SASZombieAssaultTD.Engine.Physics.Colliders
{
    internal class ColliderCompCore
    {
        private CollisionShape Shape;

        public bool Enabled { get; set; } = true;

        public bool IsTrigger { get; set; } = false;

        public CollisionShape GetWorldShape()
        {
            // If your shape is already world-space:
            return Shape;

            // OR if you need transform application:
            // return LocalShape.ToWorldSpace(_transform.Position, _transform.Rotation);
        }

        public int EntityId { get; set; }
        public int EntityIndex { get; set; } = -1;
        public int Index { get; set; }
        public bool IsStatic { get; set; }
        public bool IsSensor { get; set; }

        public ColliderCompCore(int ECSEntityCoreId, int index)
        {
            EntityId = ECSEntityCoreId;
            Index = index;
            IsStatic = true;
            IsTrigger = false;
            IsSensor = false;
        }
        public void Update()
        {
            if (IsStatic)
            {
                IsTrigger = false;
                IsSensor = false;
            }
        }
        public struct ColliderCompCoreComparer : IEqualityComparer<ColliderCompCore>
        {
            public bool Equals(ColliderCompCore x, ColliderCompCore y) => x.EntityId == y.EntityId;
            public int GetHashCode(ColliderCompCore obj) => obj.EntityId.GetHashCode();
        }

    }
}
