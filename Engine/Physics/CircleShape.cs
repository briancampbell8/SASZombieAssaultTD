// ====================================================================================================
//  FILE: CircleShape.cs
//  PATH: ./Engine/Physics/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the CircleShape module.
//
//  RESPONSIBILITIES:
//      - Provide Clone() behavior for the Core subsystem.
//      - Provide Translate() behavior for the Core subsystem.
//      - Provide ContainsPoint() behavior for the Core subsystem.
//      - Provide Set() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    CircleShape.cs
Purpose: Circular collision shape implementation.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Physics.Collision;
using SASZombieAssaultTD.Engine.VectorMath; //Corrected namespace for Vector3
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Circular collision shape. Suitable for projectiles, explosions, and circular entities.
    /// </summary>
    public sealed class CircleShape : CollisionShape
    {
        private float _radius;

        /// <summary>
        /// Gets or sets the radius of the circle. Ensures the radius is non-negative.
        /// </summary>
        public float Radius
        {
            get => _radius;
            set => _radius = System.Math.Max(0f, value);
        }

        /// <summary>
        /// Gets the diameter of the circle.
        /// </summary>
        public float Diameter => _radius * 2f;

        /// <summary>
        /// Gets the type of this collision shape.
        /// </summary>
        public override CollisionShapeType ShapeType => CollisionShapeType.Circle;

        /// <summary>
        /// Gets the bounding box that encompasses this circle.
        /// </summary>
        public override BoundingBox Bounds =>
            new BoundingBox(Center - new Vector3(_radius, _radius, 0), Center + new Vector3(_radius, _radius, 0));

        /// <summary>
        /// Gets the area of this circle.
        /// </summary>
        public override float Area => MathF.PI * _radius * _radius;

        /// <summary>
        /// Initializes a new CircleShape with a specified center and radius.
        /// </summary>
        /// <param name="center">The center position of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public CircleShape(Vector3 center, float radius) : base(center) => _radius = System.Math.Max(0f, radius);

        /// <summary>
        /// Initializes a new CircleShape with a specified radius and the center at the origin.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        public CircleShape(float radius) : this(Vector3.Zero, radius) { }

        /// <summary>
        /// Creates a copy of this circle shape.
        /// </summary>
        /// <returns>A new CircleShape with the same properties.</returns>
        public override CollisionShape Clone() => new CircleShape(Center, _radius);

        /// <summary>
        /// Translates the circle by the specified offset.
        /// </summary>
        /// <param name="offset">The translation offset.</param>
        public override void Translate(Vector3 offset)
        {
            Center = Center + offset; //Simplified translation logic
        }

        /// <summary>
        /// Checks if a point is inside this circle.
        /// </summary>
        /// <param name="point">The point to test (in world coordinates).</param>
        /// <returns>True if the point is inside the circle; otherwise, false.</returns>
        public override bool ContainsPoint(Vector3 point)
        {
            float dx = point.X - Center.X;
            float dy = point.Y - Center.Y;
            float dz = point.Z - Center.Z;
            return dx * dx + dy * dy + dz * dz <= _radius * _radius;
        }

        /// <summary>
        /// Sets the circle's center and radius.
        /// </summary>
        /// <param name="center">The new center position.</param>
        /// <param name="radius">The new radius.</param>
        public void Set(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Returns a string representation of this circle shape for debugging.
        /// </summary>
        public override string ToString() =>
            $"Circle [Center: {Center}, Radius: {_radius:F2}, Area: {Area:F2}]";
    }
}
