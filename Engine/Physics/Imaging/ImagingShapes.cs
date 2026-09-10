// =====================================================================================================
//  FILE: ImagingShapes.cs
//  PATH: Engine/Physics/Imaging/
//  SUBSYSTEM: Physics → Imaging
//
//  ROLE:
//      Render physics collision shapes in a deterministic, read‑only manner.
//      ImagingShapes does not perform simulation, debugging, or diagnostics.
//      It strictly visualizes shapes already computed by the physics subsystem.
//
//  RESPONSIBILITIES:
//      - Render circles, AABBs, capsules.
//      - Render shape bounds when requested.
//      - Apply trigger vs non‑trigger imaging colors.
//      - Remain strictly read‑only toward ECS and physics state.
//      - Provide shape‑level imaging only (no grid, no text).
//
//  NON‑RESPONSIBILITIES:
//      - Collision detection.
//      - Physics simulation.
//      - Diagnostics.
//      - Grid rendering (ImagingGrid).
//      - Text rendering (ImagingText).
//      - Stats parsing (ImagingStatsParser).
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Physics.Collision;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics.Imaging
{
    /// <summary>
    /// Deterministic physics‑shape imaging renderer.
    /// </summary>
    public sealed class ImagingShapes
    {
        private readonly ECSRuntimeCore _ecsWorld;

        public ImagingShapes(ECSRuntimeCore ecsWorld)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
        }

        /// <summary>
        /// Render all collision shapes for entities containing ColliderCompCore + TransformComponent.
        /// </summary>
        public void Render(
            D3D11Adapter_Core context,
            uint shapeColor,
            uint boundsColor,
            uint triggerColor,
            bool showShapes,
            bool showBounds)
        {
            if (context == null)
                return;

            foreach (var entity in _ecsWorld.GetEntitiesWith<ColliderCompCore, TransformComponent>())
            {
                var collider = entity.GetComponent<ColliderCompCore>();
                var transform = entity.GetComponent<TransformComponent>();

                if (collider == null || transform == null || !collider.Enabled)
                    continue;

                var worldShape = collider.GetWorldShape();
                if (worldShape == null)
                    continue;

                var position = transform.Position;
                uint color = collider.IsTrigger ? triggerColor : shapeColor;

                if (showShapes)
                    RenderShape(context, worldShape, position, color);

                if (showBounds)
                    RenderBounds(context, worldShape, boundsColor);
            }
        }

        // =====================================================================================================
        //  SHAPE DISPATCH
        // =====================================================================================================

        private void RenderShape(
            D3D11Adapter_Core context,
            CollisionShape shape,
            Vector3 position,
            uint color)
        {
            switch (shape.ShapeType)
            {
                case CollisionShapeType.Circle:
                    RenderCircle(context, (CircleShape)shape, position, color);
                    break;

                case CollisionShapeType.AABB:
                    RenderAABB(context, (AABBShape)shape, position, color);
                    break;

                case CollisionShapeType.Capsule:
                    RenderCapsule(context, (CapsuleShape)shape, position, color);
                    break;
            }
        }

        // =====================================================================================================
        //  BOUNDS
        // =====================================================================================================

        private void RenderBounds(
            D3D11Adapter_Core context,
            CollisionShape shape,
            uint color)
        {
            var b = shape.Bounds;

            var rect = Rectangle.FromPositionAndSize(
                b.Min.X,
                b.Min.Y,
                b.Width,
                b.Height);

            context.DrawRectangle(rect, Color.FromUint(color));
        }

        // =====================================================================================================
        //  CIRCLE
        // =====================================================================================================

        private void RenderCircle(
            D3D11Adapter_Core context,
            CircleShape circle,
            Vector3 position,
            uint color)
        {
            var center = new Vector3(
                position.X + circle.Center.X,
                position.Y + circle.Center.Y,
                0);

            context.DrawCircle(center, circle.Radius, Color.FromUint(color));
        }

        // =====================================================================================================
        //  AABB
        // =====================================================================================================

        private void RenderAABB(
            D3D11Adapter_Core context,
            AABBShape aabb,
            Vector3 position,
            uint color)
        {
            var min = position + aabb.Min;
            var size = aabb.Size;

            var rect = Rectangle.FromPositionAndSize(
                min.X,
                min.Y,
                size.X,
                size.Y);

            context.DrawRectangle(rect, Color.FromUint(color));
        }

        // =====================================================================================================
        //  CAPSULE
        // =====================================================================================================

        private void RenderCapsule(
            D3D11Adapter_Core context,
            CapsuleShape capsule,
            Vector3 position,
            uint color)
        {
            float radius = capsule.Radius;
            float halfHeight = capsule.HalfHeight;

            // Body rectangle
            var rectMin = position + new Vector3(-radius, -halfHeight, 0);
            var rectSize = new Vector3(radius * 2, capsule.Height, 0);

            var capsuleRect = Rectangle.FromPositionAndSize(
                rectMin.X,
                rectMin.Y,
                rectSize.X,
                rectSize.Y);

            context.DrawRectangle(capsuleRect, Color.FromUint(color));

            // Hemispheres
            var topCenter = position + capsule.GetTopHemisphereCenter();
            var bottomCenter = position + capsule.GetBottomHemisphereCenter();

            context.DrawCircle(topCenter, radius, Color.FromUint(color));
            context.DrawCircle(bottomCenter, radius, Color.FromUint(color));
        }

        internal void GetRender(D3D11Adapter_Core context, uint shapeColor, uint boundsColor, uint triggerColor, bool showShapes, bool showBounds)
        {
            throw new NotImplementedException();
        }
    }
}
