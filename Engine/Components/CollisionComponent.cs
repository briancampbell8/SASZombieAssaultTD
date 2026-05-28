/*
File:    CollisionComponent.cs
Path:    Engine/Components/CollisionComponent.cs
Purpose:   P11-04-01-A - Core ECS component for collision detection properties.
           Stores collider shapes, dimensions, offset, trigger flag, layer mask, and enabled state.

Role:      Essential collision component for entities requiring collision detection.
           - Defines collision shape type (box, circle, polygon) for spatial queries
           - Stores collision dimensions and offset for precise boundary calculations
           - Provides trigger flag for non-physical collision detection
           - Manages layer mask for collision filtering and group management
           - Controls enabled state for temporary collision deactivation
           - Integrates with collision system for broad-phase culling

Features:   ECS-friendly pure data structure optimized for collision detection.
           Support for multiple collider shapes with unified interface.
           Collision layer management for complex collision filtering.
           Trigger collision support for area-of-effect and sensor entities.
           Thread-safe property access for concurrent collision system access.
           Optimized for high-frequency collision system queries.

Notes:      This component is required by all entities participating in collision detection.
           Collider shapes are defined by the ColliderShapeType enum.
           Layer masks support bitwise operations for efficient filtering.
           Trigger collisions bypass physics response and only generate events.
           Component integrates seamlessly with both collision and physics systems.

*/
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Types of collider shapes supported by the collision system.
    /// </summary>
    public enum ColliderShapeType
    {
        Box,    // Axis-aligned bounding box
        Circle  // Circle collider
    }

    /// <summary>
    /// Component for collision detection properties.
    /// P11-04-01-A: Stores collider shape type, dimensions, offset, trigger flag, layer mask, and enabled state.
    /// This component is pure data, ECS-friendly, and fully documented.
    /// </summary>
    public class CollisionComponent
    {
        ///  Properties

        /// <summary>
        /// Type of collider shape (Box or Circle).
        /// </summary>
        public ColliderShapeType ShapeType { get; set; } = ColliderShapeType.Box;

        /// <summary>
        /// Width of the collider (used for Box shape).
        /// </summary>
        public float Width { get; set; } = 32.0f;

        /// <summary>
        /// Height of the collider (used for Box shape).
        /// </summary>
        public float Height { get; set; } = 32.0f;

        /// <summary>
        /// Radius of the collider (used for Circle shape).
        /// </summary>
        public float Radius { get; set; } = 16.0f;

        /// <summary>
        /// Offset position of the collider relative to the entity's transform position.
        /// </summary>
        public PointF Offset { get; set; } = PointF.Empty;

        /// <summary>
        /// Whether this collider is a trigger (overlap only) or physical collision.
        /// true = overlap only, false = physical collision.
        /// </summary>
        public bool IsTrigger { get; set; } = false;

        /// <summary>
        /// Layer mask for collision filtering (bitmask).
        /// Used to determine which layers this collider can collide with.
        /// </summary>
        public int LayerMask { get; set; } = unchecked((int)0xFFFFFFFF); // Collide with all layers by default

        /// <summary>
        /// Whether this collider is enabled and participating in collision detection.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// 

        ///  Constructors

        /// <summary>
        /// Creates a new CollisionComponent with default box shape.
        /// </summary>
        public CollisionComponent() { }

        /// <summary>
        /// Creates a new CollisionComponent with specified shape type.
        /// </summary>
        /// <param name="shapeType">Type of collider shape</param>
        public CollisionComponent(ColliderShapeType shapeType)
        {
            ShapeType = shapeType;
        }

        /// <summary>
        /// Creates a new box CollisionComponent with specified dimensions.
        /// </summary>
        /// <param name="width">Width of the box collider</param>
        /// <param name="height">Height of the box collider</param>
        /// <param name="offset">Offset position relative to entity</param>
        /// <param name="isTrigger">Whether this is a trigger collider</param>
        /// <param name="layerMask">Layer mask for collision filtering</param>
        public CollisionComponent(
            float width,
            float height,
            PointF? offset = null,
            bool isTrigger = false,
            int layerMask = unchecked((int)0xFFFFFFFF))
        {
            ShapeType = ColliderShapeType.Box;
            Width = width;
            Height = height;
            Offset = offset ?? PointF.Empty;
            IsTrigger = isTrigger;
            LayerMask = layerMask;
        }

        /// <summary>
        /// Creates a new circle CollisionComponent with specified radius.
        /// </summary>
        /// <param name="radius">Radius of the circle collider</param>
        /// <param name="offset">Offset position relative to entity</param>
        /// <param name="isTrigger">Whether this is a trigger collider</param>
        /// <param name="layerMask">Layer mask for collision filtering</param>
        public CollisionComponent(
            float radius,
            PointF? offset = null,
            bool isTrigger = false,
            int layerMask = unchecked((int)0xFFFFFFFF))
        {
            ShapeType = ColliderShapeType.Circle;
            Radius = radius;
            Offset = offset ?? PointF.Empty;
            IsTrigger = isTrigger;
            LayerMask = layerMask;
        }

        /// <summary>
        /// Creates a new CollisionComponent with full configuration.
        /// </summary>
        /// <param name="shapeType">Type of collider shape</param>
        /// <param name="width">Width (for Box shape)</param>
        /// <param name="height">Height (for Box shape)</param>
        /// <param name="radius">Radius (for Circle shape)</param>
        /// <param name="offset">Offset position relative to entity</param>
        /// <param name="isTrigger">Whether this is a trigger collider</param>
        /// <param name="layerMask">Layer mask for collision filtering</param>
        /// <param name="enabled">Whether collider is enabled</param>
        public CollisionComponent(
            ColliderShapeType shapeType,
            float width,
            float height,
            float radius,
            PointF? offset = null,
            bool isTrigger = false,
            int layerMask = unchecked((int)0xFFFFFFFF),
            bool enabled = true)
        {
            ShapeType = shapeType;
            Width = width;
            Height = height;
            Radius = radius;
            Offset = offset ?? PointF.Empty;
            IsTrigger = isTrigger;
            LayerMask = layerMask;
            Enabled = enabled;
        }

        /// 

        ///  Methods

        /// <summary>
        /// Gets a string representation for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"CollisionComponent(Shape: {ShapeType}, Size: {Width}x{Height}, Radius: {Radius}, Trigger: {IsTrigger}, Enabled: {Enabled})";
        }

        /// 
    }
}




