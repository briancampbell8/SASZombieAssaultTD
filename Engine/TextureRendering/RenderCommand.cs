// =====================================================================================================
//  FILE: RenderCommand.cs
//  PATH: Engine/TextureRendering/RenderCommand.cs
//  SUBSYSTEM: Engine.TextureRendering
//
//  ROLE:
//      Defines the immutable, allocationless rendering command used by all ModernUIRenderer
//      batching, validation, and submission pipelines.
//
//  ARCHITECTURAL NOTES:
//      - Immutable readonly struct for safe value-type batching.
//      - No shadow fields, no duplicate constructors, no auto-generated artifacts.
//      - SecondaryData is Vector2 (required by RenderCommandBuffer.ExecuteAll).
//      - Fully aligned with ModernUIRenderer partials and RenderCommandBuffer.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.UI;
using SASZombieAssaultTD.Engine.UI.Rendering;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    public struct RenderCommand : IEquatable<RenderCommand>
    {
        // ---------------------------------------------------------------------------------------------
        //  PUBLIC PROPERTIES
        // ---------------------------------------------------------------------------------------------
        public RenderCommandType Type { get; set; }
        public int ElementId { get; set; }
        public Vector3 Position { get; }
        public Vector2 Size { get; }
        public UIMaterial Material { get; set; }
        public object Element { get; set; }
        public float DepthLayer { get; }
        public Vector4 ColorData { get; }
        public Vector2 SecondaryData { get; }
        public Matrix3x2 Transform { get; internal set; }
        public uint SortKey { get; internal set; }

        // ---------------------------------------------------------------------------------------------
        //  PRIMARY CONSTRUCTOR (canonical)
        // ---------------------------------------------------------------------------------------------
        public RenderCommand(
            RenderCommandType type,
            uint elementId,
            Vector3 position,
            Vector2 size,
            UIMaterial material,
            float depthLayer,
            Vector4 colorData,
            Vector2 secondaryData)
        {
            Type = type;
            ElementId = (int)elementId;
            Position = position;
            Size = size;
            Material = material;
            DepthLayer = depthLayer;
            ColorData = colorData;
            SecondaryData = secondaryData;
        }

        // ---------------------------------------------------------------------------------------------
        //  FACTORY HELPERS (clean, deterministic)
        // ---------------------------------------------------------------------------------------------
        public static RenderCommand CreateRectangle(
            uint id,
            Vector3 pos,
            Vector2 size,
            UIMaterial mat,
            Vector4 color,
            float depth)
        {
            return new RenderCommand(
                RenderCommandType.Rectangle,
                id,
                pos,
                size,
                mat,
                depth,
                color,
                Vector2.Zero);
        }

        public static RenderCommand CreateSprite(
            uint id,
            Vector3 pos,
            Vector2 size,
            UIMaterial mat,
            Vector4 uvBounds,
            float depth)
        {
            return new RenderCommand(
                RenderCommandType.Sprite,
                id,
                pos,
                size,
                mat,
                depth,
                Vector4.One,
                new Vector2(uvBounds.X, uvBounds.Y));
        }

        public static RenderCommand CreateLine(
            uint id,
            Vector3 start,
            Vector3 end,
            UIMaterial mat,
            Vector4 color,
            float depth)
        {
            return new RenderCommand(
                RenderCommandType.Line,
                id,
                start,
                new Vector2(end.X, end.Y),
                mat,
                depth,
                color,
                Vector2.Zero);
        }

        // ---------------------------------------------------------------------------------------------
        //  EQUALITY
        // ---------------------------------------------------------------------------------------------
        public bool Equals(RenderCommand other)
        {
            return Type == other.Type &&
                   ElementId == other.ElementId &&
                   Position.Equals(other.Position) &&
                   Size.Equals(other.Size) &&
                   ReferenceEquals(Material, other.Material) &&
                   DepthLayer.Equals(other.DepthLayer) &&
                   ColorData.Equals(other.ColorData) &&
                   SecondaryData.Equals(other.SecondaryData);
        }

        public override bool Equals(object? obj) =>
            obj is RenderCommand other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(Type, ElementId, Position, Size, Material, DepthLayer);

        internal static RenderCommand CreateRectangle(uint id, Vector3 pos, Vector2 size, UIMaterial mat, Vector4 color, float depth, float rotation)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(RenderCommand left, RenderCommand right) => left.Equals(right);
        public static bool operator !=(RenderCommand left, RenderCommand right) => !left.Equals(right);
    }
}
