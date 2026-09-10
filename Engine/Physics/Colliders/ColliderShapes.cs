// ====================================================================================================
//  FILE: ColliderShapes.cs
//  PATH: ./Engine/Physics/Components/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ColliderShapes module.
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
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    ColliderShapes.cs
Purpose: Pure math structs for collision detection.
Features: Box and circle collider shapes with intersection methods.

P11-04-01-B: Structs include helper methods with pure math and full XML documentation.
*/

using System.Drawing;

namespace SASZombieAssaultTD.Engine.Physics.Colliders
{
    /// <summary>
    /// Represents a box collider shape with width and height. P11-04-01-B: Includes Width, Height, and helper methods
    /// (AABB, IntersectsBox, IntersectsCircle, ContainsPoint). All methods are pure math with no engine dependencies.
    /// </summary>
    public struct BoxColliderShape
    {
        /// <summary>
        /// Width of the box collider.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height of the box collider.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Creates a new BoxColliderShape with specified dimensions.
        /// </summary>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        public BoxColliderShape(float width, float height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the axis-aligned bounding box (AABB) for this box at the specified position.
        /// </summary>
        /// <param name="position">Center position of the box</param>
        /// <returns>Rectangle representing the AABB</returns>
        public RectangleF GetAABB(PointF position)
        {
            var halfWidth = Width * 0.5f;
            var halfHeight = Height * 0.5f;

            return new RectangleF(
            position.X - halfWidth,
            position.Y - halfHeight,
            Width,
            Height
            );
        }

        /// <summary>
        /// Checks if this box intersects with another box.
        /// </summary>
        /// <param name="position1">Center position of this box</param>
        /// <param name="other">Other box collider shape</param>
        /// <param name="position2">Center position of the other box</param>
        /// <returns>True if boxes intersect, false otherwise</returns>
        public bool IntersectsBox(PointF position1, BoxColliderShape other, PointF position2)
        {
            var aabb1 = GetAABB(position1);
            var aabb2 = other.GetAABB(position2);

            return aabb1.IntersectsWith(aabb2);
        }

        /// <summary>
        /// Checks if this box intersects with a circle.
        /// </summary>
        /// <param name="position">Center position of this box</param>
        /// <param name="circle">Circle collider shape</param>
        /// <param name="circlePosition">Center position of the circle</param>
        /// <returns>True if box intersects with circle, false otherwise</returns>
        public bool IntersectsCircle(PointF position, CircleColliderShape circle, PointF circlePosition)
        {
            return circle.IntersectsBox(circlePosition, this, position);
        }

        /// <summary>
        /// Checks if this box contains a point.
        /// </summary>
        /// <param name="position">Center position of this box</param>
        /// <param name="point">Point to test</param>
        /// <returns>True if box contains the point, false otherwise</returns>
        public bool ContainsPoint(PointF position, PointF point)
        {
            var aabb = GetAABB(position);
            return aabb.Contains(point);
        }

        /// <summary>
        /// Gets a string representation for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"BoxColliderShape({Width}x{Height})";
        }
    }

    /// <summary>
    /// Represents a circle collider shape with radius. P11-04-01-B: Includes Radius and helper methods
    /// (IntersectsCircle, IntersectsBox, ContainsPoint). All methods are pure math with no engine dependencies.
    /// </summary>
    public struct CircleColliderShape
    {
        /// <summary>
        /// Radius of the circle collider.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Creates a new CircleColliderShape with specified radius.
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        public CircleColliderShape(float radius) => Radius = radius;

        /// <summary>
        /// Checks if this circle intersects with another circle.
        /// </summary>
        /// <param name="position1">Center position of this circle</param>
        /// <param name="other">Other circle collider shape</param>
        /// <param name="position2">Center position of the other circle</param>
        /// <returns>True if circles intersect, false otherwise</returns>
        public bool IntersectsCircle(PointF position1, CircleColliderShape other, PointF position2)
        {
            var dx = position1.X - position2.X;
            var dy = position1.Y - position2.Y;
            var distanceSquared = dx * dx + dy * dy;
            var radiusSum = Radius + other.Radius;
            var radiusSumSquared = radiusSum * radiusSum;

            return distanceSquared <= radiusSumSquared;
        }

        /// <summary>
        /// Checks if this circle intersects with a box.
        /// </summary>
        /// <param name="position">Center position of this circle</param>
        /// <param name="box">Box collider shape</param>
        /// <param name="boxPosition">Center position of the box</param>
        /// <returns>True if circle intersects with box, false otherwise</returns>
        public bool IntersectsBox(PointF position, BoxColliderShape box, PointF boxPosition)
        {
            //Find the closest point on the box to the circle center
            var boxAABB = box.GetAABB(boxPosition);
            var closestX = System.Math.Max(boxAABB.Left, System.Math.Min(position.X, boxAABB.Right));
            var closestY = System.Math.Max(boxAABB.Top, System.Math.Min(position.Y, boxAABB.Bottom));

            //Calculate distance from circle center to closest point
            var dx = position.X - closestX;
            var dy = position.Y - closestY;
            var distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= (Radius * Radius);
        }

        /// <summary>
        /// Checks if this circle contains a point.
        /// </summary>
        /// <param name="position">Center position of this circle</param>
        /// <param name="point">Point to test</param>
        /// <returns>True if circle contains the point, false otherwise</returns>
        public bool ContainsPoint(PointF position, PointF point)
        {
            var dx = position.X - point.X;
            var dy = position.Y - point.Y;
            var distanceSquared = dx * dx + dy * dy;
            var radiusSquared = Radius * Radius;

            return distanceSquared <= radiusSquared;
        }

        /// <summary>
        /// Gets a string representation for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"CircleColliderShape(Radius: {Radius})";
        }
    }
}
