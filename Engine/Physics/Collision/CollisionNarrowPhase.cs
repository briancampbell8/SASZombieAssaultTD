// =====================================================================================================
//  FILE: CollisionNarrowPhase.cs
//  PATH: Engine/Physics/Collision/CollisionNarrowPhase.cs
//  SUBSYSTEM: Physics Collision Subsystem
//
//  ROLE:
//      Performs narrow‑phase collision evaluation for all supported collider shapes.
//      Executes precise shape‑specific collision algorithms after broad‑phase culling.
//      Produces deterministic boolean collision results for use by CollisionCore.
//
//  RESPONSIBILITIES:
//      - Evaluate shape‑specific collision interactions (AABB, Circle, AABB↔Circle).
//      - Compute distances, overlap conditions, and geometric relationships.
//      - Provide deterministic narrow‑phase results for the collision pipeline.
//      - Serve as the second stage of collision detection after broad‑phase filtering.
//
//  NON-RESPONSIBILITIES:
//      - Performing AABB broad‑phase culling (handled by CollisionBroadPhase).
//      - Resolving physical collisions or applying forces (handled by CollisionResolve).
//      - Publishing collision events (handled by CollisionEvents).
//      - Managing ECS ECSEntityCore iteration or component retrieval.
//
//  ARCHITECTURAL NOTES:
//      - Invoked by CollisionCore after broad‑phase AABB intersection checks.
//      - Operates only on collider shapes and world‑space positions.
//      - Ensures deterministic and isolated narrow‑phase logic.
// =====================================================================================================

using System.Drawing;
using SASZombieAssaultTD.Engine.Physics.Collision;
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    internal class CollisionNarrowPhase
    {
        internal bool NarrowPhaseShapeCheck(
            ColliderCompCore collisionA,
            PointF posA,
            ColliderCompCore collisionB,
            PointF posB)
        {
            if (collisionA.Shape == null || collisionB.Shape == null)
                return false;

            return collisionA.Shape.ShapeType switch
            {
                CollisionShapeType.AABB when collisionB.Shape.ShapeType == CollisionShapeType.AABB =>
                    CheckBoxBoxCollision(collisionA.Shape, posA, collisionB.Shape, posB),

                CollisionShapeType.Circle when collisionB.Shape.ShapeType == CollisionShapeType.Circle =>
                    CheckCircleCircleCollision(collisionA.Shape, posA, collisionB.Shape, posB),

                CollisionShapeType.AABB when collisionB.Shape.ShapeType == CollisionShapeType.Circle =>
                    CheckBoxCircleCollision(collisionA.Shape, posA, collisionB.Shape, posB),

                CollisionShapeType.Circle when collisionB.Shape.ShapeType == CollisionShapeType.AABB =>
                    CheckCircleBoxCollision(collisionA.Shape, posA, collisionB.Shape, posB),

                _ => false
            };
        }

        private bool CheckBoxBoxCollision(
            CollisionShape boxA,
            PointF posA,
            CollisionShape boxB,
            PointF posB)
        {
            var boundsA = boxA.Bounds;
            var boundsB = boxB.Bounds;
            return boundsA.Intersects(boundsB);
        }

        private bool CheckCircleCircleCollision(
            CollisionShape circleA,
            PointF posA,
            CollisionShape circleB,
            PointF posB)
        {
            var dx = posA.X - posB.X;
            var dy = posA.Y - posB.Y;
            var distance = System.MathF.Sqrt(dx * dx + dy * dy);
            var radiusSum = GetCircleRadius(circleA) + GetCircleRadius(circleB);
            return distance <= radiusSum;
        }

        private bool CheckBoxCircleCollision(
            CollisionShape box,
            PointF boxPos,
            CollisionShape circle,
            PointF circlePos)
        {
            return CheckCircleBoxCollision(circle, circlePos, box, boxPos);
        }

        private bool CheckCircleBoxCollision(
            CollisionShape circle,
            PointF circlePos,
            CollisionShape box,
            PointF boxPos)
        {
            var bounds = box.Bounds;

            var closestX = System.MathF.Max(bounds.Min.X, System.MathF.Min(circlePos.X, bounds.Max.X));
            var closestY = System.MathF.Max(bounds.Min.Y, System.MathF.Min(circlePos.Y, bounds.Max.Y));

            var dx = circlePos.X - closestX;
            var dy = circlePos.Y - closestY;

            var distance = System.MathF.Sqrt(dx * dx + dy * dy);
            var radius = GetCircleRadius(circle);

            return distance <= radius;
        }

        private float GetCircleRadius(CollisionShape circle)
        {
            var bounds = circle.Bounds;
            return System.MathF.Max(bounds.Width, bounds.Height) * 0.5f;
        }
    }
}
