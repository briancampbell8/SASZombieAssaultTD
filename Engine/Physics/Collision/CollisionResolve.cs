// =====================================================================================================
//  FILE: CollisionResolve.cs
//  PATH: Engine/Physics/Collision/CollisionResolve.cs
//  SUBSYSTEM: Physics Collision Subsystem
//
//  ROLE:
//      Performs physical collision resolution for non‑trigger collisions.
//      Applies positional separation and bounce forces based on collision normals and impulses.
//      Acts as the deterministic resolution stage of the collision pipeline.
//
//  RESPONSIBILITIES:
//      - Resolve overlap by separating entities along the collision normal.
//      - Apply bounce forces to physics components using collision impulse values.
//      - Respect kinematic flags to avoid moving non‑dynamic entities.
//      - Serve as the third stage of collision processing after narrow‑phase evaluation.
//
//  NON-RESPONSIBILITIES:
//      - Performing broad‑phase or narrow‑phase collision checks.
//      - Publishing collision events (handled by CollisionEvents).
//      - Generating collision results (handled by CollisionCore).
//      - Managing ECS ECSEntityCore iteration or component retrieval.
//
//  ARCHITECTURAL NOTES:
//      - Invoked by CollisionCore after collision detection is complete.
//      - Operates only on CollisionResult objects and ECS physics/transform components.
//      - Ensures deterministic and isolated physical resolution logic.
// =====================================================================================================

using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Physics.Components;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    internal class CollisionResolve
    {
        private readonly ECSRuntimeCore _runtime;

        internal CollisionResolve(ECSRuntimeCore runtime)
        {
            _runtime = runtime;
        }

        internal void ResolvePhysicalCollisions(IEnumerable<CollisionResult> collisions)
        {
            foreach (var collision in collisions)
            {
                if (collision.IsTrigger)
                    continue;

                var entityA = collision.EntityA as ECSEntityCore;
                var entityB = collision.EntityB as ECSEntityCore;

                if (entityA == null || entityB == null)
                    continue;

                var physicsA = entityA.HasComponent<PhysicsComponent>()
                    ? entityA.GetComponent<PhysicsComponent>()
                    : null;

                var physicsB = entityB.HasComponent<PhysicsComponent>()
                    ? entityB.GetComponent<PhysicsComponent>()
                    : null;

                var transformA = entityA.HasComponent<TransformComponent>()
                    ? entityA.GetComponent<TransformComponent>()
                    : null;

                var transformB = entityB.HasComponent<TransformComponent>()
                    ? entityB.GetComponent<TransformComponent>()
                    : null;

                bool isKinematicA = physicsA?.IsKinematic ?? true;
                bool isKinematicB = physicsB?.IsKinematic ?? true;

                if (isKinematicA && isKinematicB)
                    continue;

                if (transformA != null && transformB != null)
                {
                    ResolveOverlap(collision, isKinematicA, isKinematicB, transformA, transformB);
                }

                if (physicsA != null && physicsB != null)
                {
                    ApplyBounceForces(collision, physicsA, physicsB);
                }
            }
        }

        private void ResolveOverlap(
            CollisionResult collision,
            bool isKinematicA,
            bool isKinematicB,
            TransformComponent transformA,
            TransformComponent transformB)
        {
            var separation = collision.Normal * collision.PenetrationDepth * 0.5f;

            if (!isKinematicA)
            {
                transformA.X += separation.X;
                transformA.Y += separation.Y;
            }

            if (!isKinematicB)
            {
                transformB.X -= separation.X;
                transformB.Y -= separation.Y;
            }
        }

        private void ApplyBounceForces(
            CollisionResult collision,
            PhysicsComponent physicsA,
            PhysicsComponent physicsB)
        {
            if (physicsA.IsKinematic && physicsB.IsKinematic)
                return;

            Vector3 bounceForce = collision.Normal * collision.Impulse;

            if (!physicsA.IsKinematic)
            {
                physicsA.Velocity = new PointF(
                    physicsA.Velocity.X - bounceForce.X,
                    physicsA.Velocity.Y - bounceForce.Y);
            }

            if (!physicsB.IsKinematic)
            {
                physicsB.Velocity = new PointF(
                    physicsB.Velocity.X + bounceForce.X,
                    physicsB.Velocity.Y + bounceForce.Y);
            }
        }
    }
}
