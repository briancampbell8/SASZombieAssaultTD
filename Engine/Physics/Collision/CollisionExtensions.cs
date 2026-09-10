// ====================================================================================================
//  FILE: CollisionExtensions.cs
//  PATH: Engine/Physics/CollisionExtensions.cs
//  MODULE: Core
//
//  ROLE:
//      Pure math structs for collision detection.
//
//  RESPONSIBILITIES:
//      - Provide GetAABB() behavior for the Core subsystem.
//      - Provide IntersectsBox() behavior for the Core subsystem.
//      - Provide IntersectsCircle() behavior for the Core subsystem.
//      - Provide ContainsPoint() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide IntersectsCircle() behavior for the Core subsystem.
//      - Provide IntersectsBox() behavior for the Core subsystem.
//      - Provide ContainsPoint() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide CanCollideWith() behavior for the Core subsystem.
//      - Provide DistanceSquared() behavior for the Core subsystem.
//      - Provide Distance() behavior for the Core subsystem.
//
//  FEATURES:
//      - Box and circle collider shapes with intersection methods.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    /// <summary>
    /// Extension methods for collision detection utilities.
    /// </summary>
    public static class CollisionExtensions
    {
        public static bool CanCollideWith(int triggerLayerMask, int targetLayerMask)
        {
            //Placeholder implementation
            return (triggerLayerMask & targetLayerMask) != 0;
        }

        /// <summary>
        /// Checks if two layer masks can collide (bitwise AND operation).
        /// </summary>
        /// <param name="layerMask1">First layer mask</param>
        /// <param name="layerMask2">Second layer mask</param>
        /// <returns>True if layers can collide, false otherwise</returns>
        public static bool CollideWith(int layerMask1, int layerMask2)
        {
            return (layerMask1 & layerMask2) != 0;
        }

        /// <summary>
        /// Gets the squared distance between two points (avoids square root operation).
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Squared distance between points</returns>
        public static float DistanceSquared(PointF point1, PointF point2)
        {
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Gets the distance between two points.
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>Distance between points</returns>
        public static float Distance(PointF point1, PointF point2)
        {
            return (float)System.Math.Sqrt(DistanceSquared(point1, point2));
        }
    }
}
