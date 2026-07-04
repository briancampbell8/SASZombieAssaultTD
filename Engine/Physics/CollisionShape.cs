/*
File:    CollisionShape.cs
Purpose: P11-14-01 - Base class for collision shapes in the physics system.
*/
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Physics
{
    ///<summary>
    ///P11-14-01: Base class for all collision shapes.
    ///Provides common interface for different geometric shapes used in collision detection.
    ///All shapes are pure data with no logic - logic handled by collision systems.
    ///</summary>
    public abstract class CollisionShape
    {
        private Vector3 _center;

        ///<summary>
        ///Gets or sets the center position of the shape.
        ///This is relative to the entity's transform position.
        ///</summary>
        public Vector3 Center
        {
            get => _center;
            set => _center = value;
        }

        ///<summary>
        ///Gets the type of this collision shape.
        ///</summary>
        public abstract CollisionShapeType ShapeType { get; }

        ///<summary>
        ///Gets the bounding box that encompasses this shape.
        ///Used for broad-phase collision detection.
        ///</summary>
        public abstract BoundingBox Bounds { get; }

        ///<summary>
        ///Gets the area of this shape.
        ///Used for physics calculations and debugging.
        ///</summary>
        public abstract float Area { get; }

        ///<summary>
        ///Initializes a new CollisionShape.
        ///</summary>
        ///<param name="center">The center position of the shape.</param>
        protected CollisionShape(Vector3 center)
        {
            _center = center;
        }

        ///<summary>
        ///Creates a copy of this collision shape.
        ///</summary>
        ///<returns>A new CollisionShape with the same properties.</returns>
        public abstract CollisionShape Clone();

        ///<summary>
        ///Translates the shape by the specified offset.
        ///</summary>
        ///<param name="offset">The translation offset.</param>
        public abstract void Translate(Vector3 offset);

        ///<summary>
        ///Checks if a point is inside this shape.
        ///</summary>
        ///<param name="point">The point to test (in world coordinates).</param>
        ///<returns>True if the point is inside the shape.</returns>
        public abstract bool ContainsPoint(Vector3 point);

        ///<summary>
        ///Gets a string representation of this collision shape for debugging.
        ///</summary>
        public override string ToString()
        {
            return $"{ShapeType} [Center: {_center}, Area: {Area:F2}]";
        }
    }

    ///<summary>
    ///Enumeration of collision shape types.
    ///</summary>
    public enum CollisionShapeType
    {
        ///<summary>
        ///Unknown or undefined shape type.
        ///</summary>
        Unknown,

        ///<summary>
        ///Circular collision shape.
        ///</summary>
        Circle,

        ///<summary>
        ///Axis-Aligned Bounding Box collision shape.
        ///</summary>
        AABB,

        ///<summary>
        ///Capsule collision shape (circle with height).
        ///</summary>
        Capsule
    }

    ///<summary>
    ///Represents an axis-aligned bounding box.
    ///Used for broad-phase collision detection and shape bounds.
    ///</summary>
    public struct BoundingBox
    {
        ///<summary>
        ///The minimum corner of the box.
        ///</summary>
        public Vector3 Min;

        ///<summary>
        ///The maximum corner of the box.
        ///</summary>
        public Vector3 Max;

        ///<summary>
        ///Gets the center of the bounding box.
        ///</summary>
        public Vector3 Center => (Min + Max) * 0.5f;

        ///<summary>
        ///Gets the size of the bounding box.
        ///</summary>
        public Vector3 Size => Max - Min;

        ///<summary>
        ///Gets the width of the bounding box.
        ///</summary>
        public float Width => Max.X - Min.X;

        ///<summary>
        ///Gets the height of the bounding box.
        ///</summary>
        public float Height => Max.Y - Min.Y;

        ///<summary>
        ///Initializes a new BoundingBox.
        ///</summary>
        ///<param name="min">The minimum corner.</param>
        ///<param name="max">The maximum corner.</param>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        ///<summary>
        ///Creates a bounding box from a center and size.
        ///</summary>
        ///<param name="center">The center position.</param>
        ///<param name="size">The size of the box.</param>
        ///<returns>A new BoundingBox.</returns>
        public static BoundingBox FromCenterAndSize(Vector3 center, Vector3 size)
        {
            var halfSize = size * 0.5f;
            return new BoundingBox(center - halfSize, center + halfSize);
        }

        ///<summary>
        ///Checks if this bounding box intersects with another.
        ///</summary>
        ///<param name="other">The other bounding box.</param>
        ///<returns>True if the boxes intersect.</returns>
        public bool Intersects(BoundingBox other)
        {
            return Min.X <= other.Max.X && Max.X >= other.Min.X &&
            Min.Y <= other.Max.Y && Max.Y >= other.Min.Y;
        }

        ///<summary>
        ///Checks if this bounding box contains a point.
        ///</summary>
        ///<param name="point">The point to test.</param>
        ///<returns>True if the point is inside the box.</returns>
        public bool Contains(Vector3 point)
        {
            return point.X >= Min.X && point.X <= Max.X &&
            point.Y >= Min.Y && point.Y <= Max.Y;
        }

        ///<summary>
        ///Expands this bounding box to include another bounding box.
        ///</summary>
        ///<param name="other">The bounding box to include.</param>
        public void Encapsulate(BoundingBox other)
        {
            Min = Vector3Extensions.Min(Min, other.Min);
            Max = Vector3Extensions.Max(Max, other.Max);
        }

        ///<summary>
        ///Expands this bounding box to include a point.
        ///</summary>
        ///<param name="point">The point to include.</param>
        public void Encapsulate(Vector3 point)
        {
            Min = Vector3Extensions.Min(Min, point);
            Max = Vector3Extensions.Max(Max, point);
        }

        ///<summary>
        ///Gets a string representation of this bounding box for debugging.
        ///</summary>
        public override string ToString()
        {
            return $"BoundingBox [Min: {Min}, Max: {Max}, Size: {Size}]";
        }
    }
}




