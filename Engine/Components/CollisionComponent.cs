//=====================================================================================================
//  FILE: CollisionComponent.cs
//  PATH: Engine/Components/CollisionComponent.cs
//  SUBSYSTEM: Components
//
//  ROLE:
//      Core component for collision detection. Stores collider shape, dimensions, offset, trigger
//      state, layer mask, and enabled state used by the engine’s collision and physics systems.
//
//  RESPONSIBILITIES:
//      - Define collider shape type (box or circle)
//      - Store collider dimensions (width, height, radius)
//      - Store positional offset relative to the ECSEntityCore transform
//      - Provide trigger flag for non-physical overlap detection
//      - Provide layer mask for collision filtering
//      - Provide enabled/disabled state for collision participation
//
//  NON-RESPONSIBILITIES:
//      - Executing collision detection logic
//      - Performing physics resolution or spatial queries
//      - Managing world-level ECSEntityCore allocation or lifecycle sequencing
//      - Acting as a system or orchestrator
//
//  ARCHITECTURAL NOTES:
//      - This is a pure data component with no behavioral logic
//      - Layer masks use bitwise operations for efficient filtering
//      - Collider shapes are defined by the ColliderShapeType enum
//      - Integrates with collision and physics systems but does not implement them
//=====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.Components
{
    ///<summary>
    ///Types of collider shapes supported by the collision system.
    ///</summary>
    public enum ColliderShapeType
    {
        Box,    //Axis-aligned bounding box
        Circle  //Circle collider
    }

    ///<summary>
    ///Component for collision detection properties.
    ///P11-04-01-A: Stores collider shape type, dimensions, offset, trigger flag, layer mask, and enabled state.
    ///This component is pure data, ECS-friendly, and fully documented.
    ///</summary>
    public class CollisionComponent
    {
        /// Properties

        ///<summary>
        ///Type of collider shape (Box or Circle).
        ///</summary>
        public ColliderShapeType ShapeType { get; set; } = ColliderShapeType.Box;

        ///<summary>
        ///Width of the collider (used for Box shape).
        ///</summary>
        public float Width { get; set; } = 32.0f;

        ///<summary>
        ///Height of the collider (used for Box shape).
        ///</summary>
        public float Height { get; set; } = 32.0f;

        ///<summary>
        ///Radius of the collider (used for Circle shape).
        ///</summary>
        public float Radius { get; set; } = 16.0f;

        ///<summary>
        ///Offset position of the collider relative to the ECSEntityCore's transform position.
        ///</summary>
        public PointF Offset { get; set; } = PointF.Empty;

        ///<summary>
        ///Whether this collider is a trigger (overlap only) or physical collision.
        ///true = overlap only, false = physical collision.
        ///</summary>
        public bool IsTrigger { get; set; } = false;

        ///<summary>
        ///Layer mask for collision filtering (bitmask).
        ///Used to determine which layers this collider can collide with.
        ///</summary>
        public int LayerMask { get; set; } = unchecked((int)0xFFFFFFFF); //Collide with all layers by default

        ///<summary>
        ///Whether this collider is enabled and participating in collision detection.
        ///</summary>
        public bool Enabled { get; set; } = true;

        ///

        /// Constructors

        ///<summary>
        ///Creates a new CollisionComponent with default box shape.
        ///</summary>
        public CollisionComponent() { }

        ///<summary>
        ///Creates a new CollisionComponent with specified shape type.
        ///</summary>
        ///<param name="shapeType">Type of collider shape</param>
        public CollisionComponent(ColliderShapeType shapeType)
        {
            ShapeType = shapeType;
        }

        ///<summary>
        ///Creates a new box CollisionComponent with specified dimensions.
        ///</summary>
        ///<param name="width">Width of the box collider</param>
        ///<param name="height">Height of the box collider</param>
        ///<param name="offset">Offset position relative to ECSEntityCore</param>
        ///<param name="isTrigger">Whether this is a trigger collider</param>
        ///<param name="layerMask">Layer mask for collision filtering</param>
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

        ///<summary>
        ///Creates a new circle CollisionComponent with specified radius.
        ///</summary>
        ///<param name="radius">Radius of the circle collider</param>
        ///<param name="offset">Offset position relative to ECSEntityCore</param>
        ///<param name="isTrigger">Whether this is a trigger collider</param>
        ///<param name="layerMask">Layer mask for collision filtering</param>
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

        ///<summary>
        ///Creates a new CollisionComponent with full configuration.
        ///</summary>
        ///<param name="shapeType">Type of collider shape</param>
        ///<param name="width">Width (for Box shape)</param>
        ///<param name="height">Height (for Box shape)</param>
        ///<param name="radius">Radius (for Circle shape)</param>
        ///<param name="offset">Offset position relative to ECSEntityCore</param>
        ///<param name="isTrigger">Whether this is a trigger collider</param>
        ///<param name="layerMask">Layer mask for collision filtering</param>
        ///<param name="enabled">Whether collider is enabled</param>
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

        /// Methods

        ///<summary>
        ///Gets a string representation for debugging.
        ///</summary>
        public override string ToString()
        {
            return $"CollisionComponent(Shape: {ShapeType}, Size: {Width}x{Height}, Radius: {Radius}, Trigger: {IsTrigger}, Enabled: {Enabled})";
        }

        ///
    }
}




