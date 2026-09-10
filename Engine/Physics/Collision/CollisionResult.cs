// =====================================================================================================
//  FILE: CollisionResult.cs
//  PATH: Engine/Physics/Collision/CollisionResult.cs
//  SUBSYSTEM: Physics Collision Subsystem
//
//  ROLE:
//      Represents the complete data structure describing a collision event.
//      Stores geometric, physical, and ECS‑level information for use by all collision subsystems.
//      Acts as the canonical container passed through detection, resolution, and event stages.
//
//  RESPONSIBILITIES:
//      - Hold collision state (normal, penetration depth, impulse, collision point).
//      - Track involved objects and ECS entities.
//      - Distinguish between trigger and physical collisions.
//      - Provide factory helpers for creating default and populated collision results.
//
//  NON-RESPONSIBILITIES:
//      - Performing collision detection (handled by CollisionCore, BroadPhase, NarrowPhase).
//      - Resolving collisions or applying forces (handled by CollisionResolve).
//      - Publishing collision events (handled by CollisionEvents).
//      - Managing ECS ECSEntityCore iteration or component retrieval.
//
//  ARCHITECTURAL NOTES:
//      - CollisionResult is the shared data contract across all collision subsystems.
//      - Produced by CollisionCore and consumed by resolution and event processors.
//      - Ensures deterministic and consistent collision information throughout the pipeline.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    internal class CollisionResult
    {
        internal bool HasCollision { get; set; }
        internal object ObjectA { get; set; }
        internal object ObjectB { get; set; }

        internal Vector3 CollisionPoint { get; set; }
        internal Vector3 Normal { get; set; }
        internal float PenetrationDepth { get; set; }
        internal float Impulse { get; set; }
        internal float CollisionTime { get; set; }

        internal CollisionType Type { get; set; }
        internal bool IsTrigger { get; set; }

        internal ECSEntityCore EntityA { get; set; }
        internal ECSEntityCore EntityB { get; set; }

        internal bool WasResolved { get; set; }

        internal Dictionary<string, object> AdditionalData { get; set; } =
            new Dictionary<string, object>();

        internal static CollisionResult NoCollision()
        {
            return new CollisionResult
            {
                HasCollision = false,
                CollisionPoint = Vector3.Zero,
                Normal = Vector3.Up,
                PenetrationDepth = 0f,
                Impulse = 0f,
                CollisionTime = 0f,
                Type = CollisionType.None,
                WasResolved = false
            };
        }

        internal static CollisionResult Create(
            object objA,
            object objB,
            Vector3 point,
            Vector3 normal,
            float depth)
        {
            return new CollisionResult
            {
                HasCollision = true,
                ObjectA = objA,
                ObjectB = objB,
                CollisionPoint = point,
                Normal = normal,
                PenetrationDepth = depth,
                Impulse = 0f,
                CollisionTime = 0f,
                Type = CollisionType.Solid,
                WasResolved = false
            };
        }
    }
}
