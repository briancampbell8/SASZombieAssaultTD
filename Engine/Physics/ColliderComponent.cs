/*
File:    ColliderComponent.cs
Purpose: P11-14-02 - ECS component for collision detection and physics.
*/
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Physics
{
    ///<summary>
    ///P11-14-02: Collision layers for filtering collisions.
    ///Used to control which entities can collide with each other.
    ///</summary>
    [Flags]
    public enum CollisionLayer
    {
        ///<summary>
        ///No collision layer - entity doesn't participate in collisions.
        ///</summary>
        None = 0,

        ///<summary>
        ///Default layer for general entities.
        ///</summary>
        Default = 1 << 0,

        ///<summary>
        ///Player entities and player-owned objects.
        ///</summary>
        Player = 1 << 1,

        ///<summary>
        ///Enemy entities and enemy-owned objects.
        ///</summary>
        Enemy = 1 << 2,

        ///<summary>
        ///Projectile entities (bullets, rockets, etc.).
        ///</summary>
        Projectile = 1 << 3,

        ///<summary>
        ///Environmental objects (walls, obstacles, terrain).
        ///</summary>
        Environment = 1 << 4,

        ///<summary>
        ///Trigger volumes and zones.
        ///</summary>
        Trigger = 1 << 5,

        ///<summary>
        ///Pickup items and collectibles.
        ///</summary>
        Pickup = 1 << 6,

        ///<summary>
        ///UI and decorative elements.
        ///</summary>
        UI = 1 << 7,

        ///<summary>
        ///All collision layers (for queries that should hit everything).
        ///</summary>
        All = ~0
    }

    ///<summary>
    ///P11-14-02: ECS component for collision detection.
    ///Provides collision shape, layer filtering, and trigger functionality.
    ///Pure data component with no logic - logic handled by CollisionSystem.
    ///</summary>
    public sealed class ColliderComponent : BaseComponent
    {
        private CollisionShape? _shape;
        private CollisionLayer _layer;
        private CollisionLayer _mask;
        private bool _isTrigger;
        private bool _enabled;
        private Entity? _owner;

        ///<summary>
        ///Gets or sets the entity that owns this component.
        ///</summary>
        public Entity? Owner
        {
            get => _owner;
            set => _owner = value;
        }

        ///<summary>
        ///Gets or sets the collision shape for this collider.
        ///</summary>
        public CollisionShape? Shape
        {
            get => _shape;
            set => _shape = value;
        }

        ///<summary>
        ///Gets or sets the collision layer this entity belongs to.
        ///</summary>
        public CollisionLayer Layer
        {
            get => _layer;
            set => _layer = value;
        }

        ///<summary>
        ///Gets or sets the collision mask (layers this collider can collide with).
        ///</summary>
        public CollisionLayer Mask
        {
            get => _mask;
            set => _mask = value;
        }

        ///<summary>
        ///Gets or sets whether this collider is a trigger (no solid collision response).
        ///Triggers only generate collision events without physical response.
        ///</summary>
        public bool IsTrigger
        {
            get => _isTrigger;
            set => _isTrigger = value;
        }

        ///<summary>
        ///Gets or sets whether this collider is enabled.
        ///Disabled colliders don't participate in collision detection.
        ///</summary>
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        ///<summary>
        ///Gets the bounding box of this collider for broad-phase checks.
        ///Returns empty bounds if no shape is set.
        ///</summary>
        public BoundingBox Bounds
        {
            get
            {
                if (_shape == null || !Enabled)
                {
                    return new BoundingBox(Vector3.Zero, Vector3.Zero);
                }

                //Transform shape bounds by entity's world position
                var transform = Owner?.GetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>();
                if (transform == null)
                {
                    return _shape.Bounds;
                }

                var shapeBounds = _shape.Bounds;
                var worldMin = shapeBounds.Min + transform.Position;
                var worldMax = shapeBounds.Max + transform.Position;
                return new BoundingBox(worldMin, worldMax);
            }
        }

        ///<summary>
        ///Gets the world-space bounds of the collision shape.
        ///</summary>
        public BoundingBox WorldBounds
        {
            get
            {
                if (_shape == null || !Enabled)
                {
                    return new BoundingBox(Vector3.Zero, Vector3.Zero);
                }

                var transform = Owner?.GetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>();
                if (transform == null)
                {
                    return _shape.Bounds;
                }

                //Create a copy of the shape and translate it to world position
                var worldShape = _shape.Clone();
                worldShape.Translate(transform.Position);
                return worldShape.Bounds;
            }
        }

        ///<summary>
        ///Event fired when this collider enters a collision with another collider.
        ///</summary>
        public event Action<CollisionEvent>? OnCollisionEnter;

        ///<summary>
        ///Event fired when this collider stays in collision with another collider.
        ///</summary>
        public event Action<CollisionEvent>? OnCollisionStay;

        ///<summary>
        ///Event fired when this collider exits a collision with another collider.
        ///</summary>
        public event Action<CollisionEvent>? OnCollisionExit;

        ///<summary>
        ///Initializes a new ColliderComponent.
        ///</summary>
        public ColliderComponent()
        {
            _shape = null;
            _layer = CollisionLayer.Default;
            _mask = CollisionLayer.All;
            _isTrigger = false;
            _enabled = true;
        }

        ///<summary>
        ///Initializes a new ColliderComponent with a collision shape.
        ///</summary>
        ///<param name="shape">The collision shape.</param>
        public ColliderComponent(CollisionShape shape) : this()
        {
            _shape = shape;
        }

        ///<summary>
        ///Initializes a new ColliderComponent with full configuration.
        ///</summary>
        ///<param name="shape">The collision shape.</param>
        ///<param name="layer">The collision layer.</param>
        ///<param name="mask">The collision mask.</param>
        ///<param name="isTrigger">Whether this is a trigger collider.</param>
        public ColliderComponent(CollisionShape shape, CollisionLayer layer, CollisionLayer mask = CollisionLayer.All, bool isTrigger = false) : this()
        {
            _shape = shape;
            _layer = layer;
            _mask = mask;
            _isTrigger = isTrigger;
        }

        ///<summary>
        ///Sets the collision shape and optionally configures it.
        ///</summary>
        ///<param name="shape">The new collision shape.</param>
        public void SetShape(CollisionShape shape)
        {
            _shape = shape;
            DLogger.Log(
                LogSubsystems.Physics, LogLevel.Info, $"ColliderComponent: Set shape to {shape?.ShapeType} for entity {Owner?.Id}");
        }

        ///<summary>
        ///Sets the collision layer and mask.
        ///</summary>
        ///<param name="layer">The collision layer.</param>
        ///<param name="mask">The collision mask.</param>
        public void SetLayers(CollisionLayer layer, CollisionLayer mask)
        {
            _layer = layer;
            _mask = mask;
            DLogger.Log(LogSubsystems.Physics, LogLevel.Info, $"ColliderComponent: Set layer={layer}, mask={mask} for entity {Owner?.Id}");
        }

        ///<summary>
        ///Checks if this collider can collide with another collider based on layers and masks.
        ///</summary>
        ///<param name="other">The other collider to check against.</param>
        ///<returns>True if collision is allowed between these colliders.</returns>
        public bool CanCollideWith(ColliderComponent other)
        {
            if (other == null || !Enabled || !other.Enabled)
                return false;

            //Check if layers match masks
            var layerMatches = (_mask & other._layer) != 0;
            var otherLayerMatches = (other._mask & _layer) != 0;

            return layerMatches && otherLayerMatches;
        }

        ///<summary>
        ///Gets the world-space collision shape (transformed by entity position).
        ///</summary>
        ///<returns>The world-space collision shape, or null if no shape is set.</returns>
        public CollisionShape? GetWorldShape()
        {
            if (_shape == null || !Enabled)
                return null;

            var transform = Owner?.GetComponent<SASZombieAssaultTD.Engine.Components.TransformComponent>();
            if (transform == null)
                return _shape.Clone();

            var worldShape = _shape.Clone();
            worldShape.Translate(transform.Position);
            return worldShape;
        }

        ///<summary>
        ///Checks if a point is inside this collider's world-space shape.
        ///</summary>
        ///<param name="point">The point to test in world coordinates.</param>
        ///<returns>True if the point is inside the collider.</returns>
        public bool ContainsPoint(Vector3 point)
        {
            var worldShape = GetWorldShape();
            return worldShape?.ContainsPoint(point) ?? false;
        }

        ///<summary>
        ///Triggers a collision enter event.
        ///Called by CollisionSystem when a new collision is detected.
        ///</summary>
        ///<param name="collisionEvent">The collision event data.</param>
        public void TriggerCollisionEnter(CollisionEvent collisionEvent)
        {
            OnCollisionEnter?.Invoke(collisionEvent);
        }

        ///<summary>
        ///Triggers a collision stay event.
        ///Called by CollisionSystem when an ongoing collision is detected.
        ///</summary>
        ///<param name="collisionEvent">The collision event data.</param>
        public void TriggerCollisionStay(CollisionEvent collisionEvent)
        {
            OnCollisionStay?.Invoke(collisionEvent);
        }

        ///<summary>
        ///Triggers a collision exit event.
        ///Called by CollisionSystem when a collision ends.
        ///</summary>
        ///<param name="collisionEvent">The collision event data.</param>
        public void TriggerCollisionExit(CollisionEvent collisionEvent)
        {
            OnCollisionExit?.Invoke(collisionEvent);
        }

        ///<summary>
        ///Gets a string representation of this collider component for debugging.
        ///</summary>
        public override string ToString()
        {
            return $"{base.ToString()} [Shape: {_shape?.ShapeType ?? CollisionShapeType.Unknown}, Layer: {_layer}, Mask: {_mask}, Trigger: {_isTrigger}, Enabled: {_enabled}]";
        }
    }
}




