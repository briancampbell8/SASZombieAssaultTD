// =====================================================================================================
//  FILE: CollisionBroadPhase.cs
//  PATH: Engine/Physics/Collision/CollisionBroadPhase.cs
//  SUBSYSTEM: Physics Collision Subsystem
//
//  ROLE:
//      Performs broad‑phase collision culling using AABB bounding boxes.
//      Provides fast, coarse collision rejection before narrow‑phase evaluation.
//      Supplies deterministic AABB generation for all collider shapes.
//
//  RESPONSIBILITIES:
//      - Compute world‑space AABB bounds for collider shapes.
//      - Perform AABB intersection tests for broad‑phase filtering.
//      - Provide minimal, deterministic collision‑culling logic.
//      - Serve as the first stage of the collision pipeline.
//
//  NON-RESPONSIBILITIES:
//      - Performing narrow‑phase collision checks.
//      - Resolving physical collisions or applying forces.
//      - Publishing collision events.
//      - Managing ECS ECSEntityCore iteration or component retrieval.
//
//  ARCHITECTURAL NOTES:
//      - This subsystem is invoked by CollisionCore during collision evaluation.
//      - It isolates broad‑phase logic for deterministic sequencing and clarity.
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    internal class CollisionBroadPhase
    {
        internal bool BroadPhaseAABBCheck(
            ColliderCompCore collisionA,
            PointF posA,
            ColliderCompCore collisionB,
            PointF posB)
        {
            var aabbA = GetColliderAABB(collisionA, posA);
            var aabbB = GetColliderAABB(collisionB, posB);
            return aabbA.IntersectsWith(aabbB);
        }

        private RectangleF GetColliderAABB(
            ColliderCompCore collision,
            PointF position)
        {
            if (collision.Shape == null)
                return RectangleF.Empty;

            var bounds = collision.Shape.Bounds;

            return new RectangleF(
                position.X + bounds.Min.X,
                position.Y + bounds.Min.Y,
                bounds.Width,
                bounds.Height);
        }
    }
}
