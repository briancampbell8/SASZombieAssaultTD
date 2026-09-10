// ====================================================================================================
//  FILE: Sprite.cs
//  PATH: ./Engine/Render/Sprites/
//  MODULE: Render / Sprites
//
//  ROLE:
//      Defines the immutable rendering primitive used by the SpriteBatch subsystem.
//      Represents a single GPU draw command for a textured quad, including transform,
//      color modulation, source region, and layer depth.
//
//  RESPONSIBILITIES:
//      - Hold rendering data for a single sprite draw.
//      - Provide deterministic, immutable values for SpriteBatch batching.
//      - Support source rectangles, tinting, rotation, scaling, and origin offsets.
//      - Integrate cleanly with Texture2D and the Render pipeline.
//
//  NON-RESPONSIBILITIES:
//      - Gameplay movement, rotation, scaling, or collision.
//      - Resource loading, caching, or texture management.
//      - Diagnostics logging or debug output.
//      - UI layout or aspect ratio calculations.
//
//  NOTES:
//      - Pure value-type struct for deterministic batching.
//      - Compatible with hybrid System.Drawing usage for debug tooling.
//      - Fully compliant with Option‑B deterministic rendering architecture.
// ====================================================================================================

using System.Drawing;
using System.Numerics;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.Sprites
{
    /// <summary>
    /// Immutable rendering primitive representing a single sprite draw command.
    /// </summary>
    public class Sprite
    {
        public readonly Texture2D Texture;
        public readonly RectangleF Destination;
        public readonly RectangleF? Source;
        public readonly Color Color;
        public readonly float Rotation;
        public readonly Vector2 Origin;
        public readonly float LayerDepth;
        internal object Width;
        internal object Height;

        /// <summary>
        /// Creates a new immutable sprite draw command.
        /// </summary>
        public Sprite(
            Texture2D texture,
            RectangleF destination,
            RectangleF? source,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
        {
            Texture = texture;
            Destination = destination;
            Source = source;
            Color = color;
            Rotation = rotation;
            Origin = origin;
            LayerDepth = System.Math.Clamp(layerDepth, 0f, 1f);
        }

        // ------------------------------------------------------------------------------------------------
        //  Convenience Constructors
        // ------------------------------------------------------------------------------------------------

        public Sprite(Texture2D texture, RectangleF destination, Color color)
            : this(
                  texture,
                  destination,
                  null,
                  color,
                  0f,
                  Vector2.Zero,
                  0f)
        {
        }

        public Sprite(Texture2D texture, RectangleF destination, RectangleF source)
            : this(texture, destination, source, Color.White, 0f, Vector2.Zero, 0f)
        {
        }

        public Sprite()
        {
        }

        public Sprite(Texture2D texture, RectangleF destination, Color color, Vector2 origin, float rotation, float layerDepth) : this(texture, destination, color)
        {
        }

        // ------------------------------------------------------------------------------------------------
        //  With* Modifiers (Immutable)
        // ------------------------------------------------------------------------------------------------
        public Sprite WithTexture(Texture2D texture)
            => new Sprite(
                texture,
                Destination,
                Source,
                Color,
                Rotation,
                Origin,
                LayerDepth);
        public Sprite WithRotation(float rotation)
            => new Sprite(
                Texture,
                Destination,
                Source,
                Color,
                rotation,
                Origin,
                LayerDepth);

        public Sprite WithOrigin(Vector2 origin)
            => new Sprite(
                Texture,
                Destination,
                Source,
                Color,
                Rotation,
                origin,
                LayerDepth);

        public Sprite WithLayerDepth(float depth)
            => new Sprite(
                Texture,
                Destination,
                Source,
                Color,
                Rotation,
                Origin,
                depth);

        public Sprite WithSource(RectangleF? source)
            => new Sprite(
                Texture,
                Destination,
                source,
                Color,
                Rotation,
                Origin,
                LayerDepth);

        public Sprite WithDestination(RectangleF dest)
            => new Sprite(
                Texture,
                dest,
                Source,
                Color,
                Rotation,
                Origin,
                LayerDepth);
    }
}
