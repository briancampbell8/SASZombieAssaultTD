// ====================================================================================================
//  FILE: AABBShape.cs
//  PATH: ./Engine/Physics/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AABBShape module.
//
//  RESPONSIBILITIES:
//      - Provide Clone() behavior for the Core subsystem.
//      - Provide Translate() behavior for the Core subsystem.
//      - Provide ContainsPoint() behavior for the Core subsystem.
//      - Provide Set() behavior for the Core subsystem.
//      - Provide SetFromMinMax() behavior for the Core subsystem.
//      - Provide GetCorners() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    AABBShape.cs
Purpose: P11-14-01 - Axis-Aligned Bounding Box collision shape implementation.
*/

using SASZombieAssaultTD.Engine.Physics.Collision;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Axis-Aligned Bounding Box collision shape. Optimized for rectangular entities, walls, and obstacles.
    /// </summary>
    public sealed class AABBShape : CollisionShape
    {
        private Vector3 _size;
        private Vector3 _halfSize; //Cached half-size for performance

        /// <summary>
        /// Gets or sets the size of the AABB. Updates the cached half-size when set.
        /// </summary>
        public Vector3 Size
        {
            get => _size;
            set
            {
                _size = new Vector3(
                    System.Math.Max(0, value.X),
                    System.Math.Max(0, value.Y),
                    System.Math.Max(0, value.Z));
                _halfSize = _size * 0.5f; //Update cached half-size
            }
        }

        /// <summary>
        /// Gets the half-size of the AABB (extents from center).
        /// </summary>
        public Vector3 HalfSize => _halfSize;

        /// <summary>
        /// Gets the minimum corner of the AABB.
        /// </summary>
        public Vector3 Min => Center - _halfSize;

        /// <summary>
        /// Gets the maximum corner of the AABB.
        /// </summary>
        public Vector3 Max => Center + _halfSize;

        /// <summary>
        /// Gets the type of this collision shape.
        /// </summary>
        public override CollisionShapeType ShapeType => CollisionShapeType.AABB;

        /// <summary>
        /// Gets the bounding box that encompasses this AABB (itself).
        /// </summary>
        public override BoundingBox Bounds => new BoundingBox(Min, Max);

        /// <summary>
        /// Gets the area of this AABB.
        /// </summary>
        public override float Area => _size.X * _size.Y;

        /// <summary>
        /// Initializes a new AABBShape.
        /// </summary>
        /// <param name="center">The center position of the AABB.</param>
        /// <param name="size">The size of the AABB.</param>
        public AABBShape(Vector3 center, Vector3 size) : base(center) => Size = new Vector3(
                System.Math.Max(0, size.X),
                System.Math.Max(0, size.Y),
                System.Math.Max(0, size.Z)); //Replace Vector3.Max with manual calculation

        /// <summary>
        /// Initializes a new AABBShape with size only (center at origin).
        /// </summary>
        /// <param name="size">The size of the AABB.</param>
        public AABBShape(Vector3 size) : this(Vector3.Zero, size)
        {
        }

        /// <summary>
        /// Creates a copy of this AABB shape.
        /// </summary>
        /// <returns>A new AABBShape with the same properties.</returns>
        public override CollisionShape Clone()
        {
            return new AABBShape(Center, _size);
        }

        /// <summary>
        /// Translates the AABB by the specified offset.
        /// </summary>
        /// <param name="offset">The translation offset.</param>
        public override void Translate(Vector3 offset)
        {
            Center += offset;
        }

        /// <summary>
        /// Checks if a point is inside this AABB.
        /// </summary>
        /// <param name="point">The point to test (in world coordinates).</param>
        /// <returns>True if the point is inside the AABB.</returns>
        public override bool ContainsPoint(Vector3 point)
        {
            return Bounds.Contains(point);
        }

        /// <summary>
        /// Sets the AABB properties.
        /// </summary>
        /// <param name="center">The new center position.</param>
        /// <param name="size">The new size.</param>
        public void Set(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size; //Automatically updates cached half-size
        }

        /// <summary>
        /// Sets the AABB from min and max corners.
        /// </summary>
        /// <param name="min">The minimum corner.</param>
        /// <param name="max">The maximum corner.</param>
        public void SetFromMinMax(Vector3 min, Vector3 max)
        {
            Center = (min + max) * 0.5f;
            Size = max - min; //Automatically updates cached half-size
        }

        /// <summary>
        /// Gets the corners of this AABB.
        /// </summary>
        /// <returns>Array of 4 corner points in clockwise order starting from bottom-left.</returns>
        public Vector3[] GetCorners()
        {
            return new[]
            {
                Center + new Vector3(-_halfSize.X, -_halfSize.Y, 0), //Bottom-left
                Center + new Vector3(_halfSize.X, -_halfSize.Y, 0),  //Bottom-right
                Center + new Vector3(_halfSize.X, _halfSize.Y, 0),   //Top-right
                Center + new Vector3(-_halfSize.X, _halfSize.Y, 0)   //Top-left
            };
        }

        /// <summary>
        /// Gets a string representation of this AABB shape for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"AABB [Center: {Center}, Size: {_size}, Area: {Area:F2}]";
        }
    }
}
