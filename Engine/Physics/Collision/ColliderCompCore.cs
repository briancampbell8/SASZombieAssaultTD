// ====================================================================================================
//  FILE: ColliderCompCore.cs
//  PATH: Engine/Physics/Components/
//  MODULE: Core
//
//  ROLE:
//      Core data container for collider configuration, shape, layers, masks, and world‑space transforms.
//
//  RESPONSIBILITIES:
//      - Store collision shape, layer, mask, trigger flag, enabled flag.
//      - Provide world‑space bounds and world‑space shape.
//      - Provide ContainsPoint() behavior.
//      - Provide SetShape() and SetLayers() behavior.
//
//  NON-RESPONSIBILITIES:
//      - Collision detection logic.
//      - Collision event dispatch (moved to ColliderCompEvents.cs).
// ====================================================================================================

using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Physics.Collision;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics
{
    public sealed class ColliderCompCore : ECSComponents
    {
        private CollisionShape? _shape;
        private CollisionLayer _layer;
        private CollisionLayer _mask;
        private bool _isTrigger;
        private bool _enabled;
        private ECSEntityCore? _owner;

        public ECSEntityCore? Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public CollisionShape? Shape
        {
            get => _shape;
            set => _shape = value;
        }

        public CollisionLayer Layer
        {
            get => _layer;
            set => _layer = value;
        }

        public CollisionLayer Mask
        {
            get => _mask;
            set => _mask = value;
        }

        public bool IsTrigger
        {
            get => _isTrigger;
            set => _isTrigger = value;
        }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public ColliderCompCore()
        {
            _shape = null;
            _layer = CollisionLayer.Default;
            _mask = CollisionLayer.All;
            _isTrigger = false;
            _enabled = true;
        }

        public ColliderCompCore(CollisionShape shape) : this()
        {
            _shape = shape;
        }

        public ColliderCompCore(
            CollisionShape shape,
            CollisionLayer layer,
            CollisionLayer mask = CollisionLayer.All,
            bool isTrigger = false) : this()
        {
            _shape = shape;
            _layer = layer;
            _mask = mask;
            _isTrigger = isTrigger;
        }

        public void SetShape(CollisionShape shape)
        {
            _shape = shape;
            DLogger.Log(LogSubsystems.Physics, LogLevel.Info,
                $"ColliderComponent: Set shape to {shape?.ShapeType} for ECSEntityCore {Owner?.Id}");
        }

        public void SetLayers(CollisionLayer layer, CollisionLayer mask)
        {
            _layer = layer;
            _mask = mask;
            DLogger.Log(LogSubsystems.Physics, LogLevel.Info,
                $"ColliderComponent: Set layer={layer}, mask={mask} for ECSEntityCore {Owner?.Id}");
        }

        public bool CanCollideWith(ColliderCompCore other)
        {
            if (other == null || !Enabled || !other.Enabled)
                return false;

            var layerMatches = (_mask & other._layer) != 0;
            var otherLayerMatches = (other._mask & _layer) != 0;

            return layerMatches && otherLayerMatches;
        }

        public BoundingBox Bounds
        {
            get
            {
                if (_shape == null || !Enabled)
                    return new BoundingBox(Vector3.Zero, Vector3.Zero);

                var transform =
                    Owner != null && Owner.HasComponent<TransformComponent>()
                        ? Owner.GetComponent<TransformComponent>()
                        : null;

                if (transform == null)
                    return _shape.Bounds;

                var shapeBounds = _shape.Bounds;
                var worldMin = shapeBounds.Min + transform.Position;
                var worldMax = shapeBounds.Max + transform.Position;
                return new BoundingBox(worldMin, worldMax);
            }
        }

        public BoundingBox WorldBounds
        {
            get
            {
                if (_shape == null || !Enabled)
                    return new BoundingBox(Vector3.Zero, Vector3.Zero);

                var transform =
                    Owner != null && Owner.HasComponent<TransformComponent>()
                        ? Owner.GetComponent<TransformComponent>()
                        : null;

                if (transform == null)
                    return _shape.Bounds;

                var worldShape = _shape.Clone();
                worldShape.Translate(transform.Position);
                return worldShape.Bounds;
            }
        }

        public CollisionShape? GetWorldShape()
        {
            if (_shape == null || !Enabled)
                return null;

            var transform =
                Owner != null && Owner.HasComponent<TransformComponent>()
                    ? Owner.GetComponent<TransformComponent>()
                    : null;

            if (transform == null)
                return _shape.Clone();

            var worldShape = _shape.Clone();
            worldShape.Translate(transform.Position);
            return worldShape;
        }

        public bool ContainsPoint(Vector3 point)
        {
            var worldShape = GetWorldShape();
            return worldShape?.ContainsPoint(point) ?? false;
        }
    }
}
