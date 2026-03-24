/*
File:    CapsuleShape.cs
Purpose: P11-14-01 - Capsule collision shape implementation.
*/
using Vector3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;
using System;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// P11-14-01: Capsule collision shape (circle with height).
    /// Perfect for elongated entities like characters, bullets, and projectiles.
    /// Pure data component with no logic.
    /// </summary>
    public sealed class CapsuleShape : CollisionShape
    {
        private float _radius;
        private float _height;

        /// <summary>
        /// Gets or sets the radius of the capsule's hemispherical ends.
        /// </summary>
        public float Radius
        {
            get => _radius;
            set => _radius = System.MathF.Max(0f, value);
        }

        /// <summary>
        /// Gets or sets the height of the capsule's cylindrical middle section.
        /// Total height = Height + 2 * Radius.
        /// </summary>
        public float Height
        {
            get => _height;
            set => _height = System.MathF.Max(0f, value);
        }

        /// <summary>
        /// Gets the total height of the capsule (including hemispheres).
        /// </summary>
        public float TotalHeight => _height + 2f * _radius;

        /// <summary>
        /// Gets the half-height of the capsule's cylindrical section.
        /// </summary>
        public float HalfHeight => _height * 0.5f;

        /// <summary>
        /// Gets the type of this collision shape.
        /// </summary>
        public override CollisionShapeType ShapeType => CollisionShapeType.Capsule;

        /// <summary>
        /// Gets the bounding box that encompasses this capsule.
        /// </summary>
        public override BoundingBox Bounds
        {
            get
            {
                var halfTotalHeight = TotalHeight * 0.5f;
                var min = Center - new Vector3(_radius, halfTotalHeight, 0);
                var max = Center + new Vector3(_radius, halfTotalHeight, 0);
                return new BoundingBox(min, max);
            }
        }

        /// <summary>
        /// Gets the area of this capsule.
        /// Area = cylindrical area + 2 * hemispherical area
        /// </summary>
        public override float Area => (2f * MathF.PI * _radius * _height) + (4f * MathF.PI * _radius * _radius);

        /// <summary>
        /// Initializes a new CapsuleShape.
        /// </summary>
        /// <param name="center">The center position of the capsule.</param>
        /// <param name="radius">The radius of the capsule's hemispherical ends.</param>
        /// <param name="height">The height of the capsule's cylindrical middle section.</param>
        public CapsuleShape(Vector3 center, float radius, float height) : base(center)
        {
            _radius = System.MathF.Max(0f, radius);
            _height = System.MathF.Max(0f, height);
        }

        /// <summary>
        /// Initializes a new CapsuleShape with radius and height only (center at origin).
        /// </summary>
        /// <param name="radius">The radius of the capsule's hemispherical ends.</param>
        /// <param name="height">The height of the capsule's cylindrical middle section.</param>
        public CapsuleShape(float radius, float height) : this(Vector3.Zero, radius, height)
        {
        }

        /// <summary>
        /// Creates a copy of this capsule shape.
        /// </summary>
        /// <returns>A new CapsuleShape with the same properties.</returns>
        public override CollisionShape Clone()
        {
            return new CapsuleShape(Center, _radius, _height);
        }

        /// <summary>
        /// Translates the capsule by the specified offset.
        /// </summary>
        /// <param name="offset">The translation offset.</param>
        public override void Translate(Vector3 offset)
        {
            Center += offset;
        }

        /// <summary>
        /// Checks if a point is inside this capsule.
        /// </summary>
        /// <param name="point">The point to test (in world coordinates).</param>
        /// <returns>True if the point is inside the capsule.</returns>
        public override bool ContainsPoint(Vector3 point)
        {
            // Transform point to local space relative to capsule center
            var localPoint = point - Center;

            // Check if point is within cylindrical middle section
            if (System.MathF.Abs(localPoint.Y) <= HalfHeight)
            {
                // Check horizontal distance within radius
                return System.MathF.Abs(localPoint.X) <= _radius;
            }

            // Check if point is within hemispherical ends
            var hemisphereCenter = new Vector3(0, System.MathF.Sign(localPoint.Y) * HalfHeight, 0);
            var distanceToHemisphere = Vector3.Distance(localPoint, hemisphereCenter);
            return distanceToHemisphere <= _radius;
        }

        /// <summary>
        /// Sets the capsule properties.
        /// </summary>
        /// <param name="center">The new center position.</param>
        /// <param name="radius">The new radius.</param>
        /// <param name="height">The new height.</param>
        public void Set(Vector3 center, float radius, float height)
        {
            Center = center;
            Radius = radius;
            Height = height;
        }

        /// <summary>
        /// Gets the top hemisphere center position.
        /// </summary>
        /// <returns>The top hemisphere center in world coordinates.</returns>
        public Vector3 GetTopHemisphereCenter()
        {
            return Center + new Vector3(0, HalfHeight, 0);
        }

        /// <summary>
        /// Gets the bottom hemisphere center position.
        /// </summary>
        /// <returns>The bottom hemisphere center in world coordinates.</returns>
        public Vector3 GetBottomHemisphereCenter()
        {
            return Center - new Vector3(0, HalfHeight, 0);
        }

        /// <summary>
        /// Gets a string representation of this capsule shape for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"Capsule [Center: {Center}, Radius: {_radius:F2}, Height: {_height:F2}]";
        }
    }
}
