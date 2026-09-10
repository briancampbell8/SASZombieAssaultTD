// ====================================================================================================
//  FILE: GraphicsTypes.cs
//  PATH: Engine/TextureRendering/GraphicsTypes.cs
//  MODULE: GraphicsTypes
//
//  ROLE:
//      Provide minimal stub types for legacy XNA/MonoGame compatibility.
//      Allows older UI and gameplay code paths to compile without full SpriteBatch support.
//
//  RESPONSIBILITIES:
//      - Define compatibility enums (SpriteSortMode, SpriteEffects).
//      - Provide stub SpriteBatch methods for legacy code paths.
//      - Provide placeholder SamplerState and BlendState types.
//      - Maintain API shape without implementing actual rendering behavior.
//
//  NON-RESPONSIBILITIES:
//      - GPU rendering, batching, or draw call submission.
//      - Resource loading or caching.
//      - Text layout, font rendering, or sprite management.
//      - Any real rendering logic (all methods are stubs).
//
//  ARCHITECTURAL NOTES:
//      - Exists solely for compatibility with older XNA-style code.
//      - All methods are intentionally empty or throw NotImplementedException.
//      - Modern rendering is handled by Renderer, RenderDevice, and ModernUIRenderer.
// ====================================================================================================

using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.TextureRendering
{


    /// <summary>
    /// Stub SpriteBatch for legacy compatibility.
    /// All methods are placeholders and do not perform rendering.
    /// </summary>
    public class SpriteBatch
    {
        public static readonly SamplerState DefaultSamplerState = SamplerState.LinearClamp;
        public static readonly BlendState DefaultBlendState = BlendState.AlphaBlend;

        private object TheType;
        private object TheMember;

        public void Begin(SpriteSortMode sortMode) { }
        public void Begin(SpriteSortMode sortMode, BlendState blendState) { }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState) { }

        public void End() { }

        public void Draw(object texture, Vector2 position, Color color) { }
        public void Draw(object texture, Vector2 position, Rectangle? sourceRectangle, Color color) { }

        public void DrawString(object spriteFont, string text, Vector2 position, Color color) { }
        public void DrawString(object spriteFont, string text, Vector2 position, Color color,
            float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        { }

        internal void Draw(nint v, Vector3 vector31,
            Rectangle sourceRect, Color color, float rotation, Vector3 vector32, Vector3 vector33,
            SpriteEffects none, float layerDepth)
        {
            // If no texture handle is provided, nothing to draw.
            if (v == nint.Zero)
            {
                return;
            }

            // Project 3D vectors to 2D by dropping the Z component.
            var position2 = new Vector2(vector31.X, vector31.Y);

            // Box the native pointer (nint) into an object to reuse the existing 2D Draw overload.
            // Note: This overload delegates to the simpler Draw overload which does not support rotation,
            // origin, scale, sprite effects or layer depth. Those parameters are ignored here. If full
            // 3D/rotated drawing is required, implement the appropriate batching and vertex setup.
            Draw(v, position2, sourceRect, color);
        }

        internal void Draw(nint v, Vector3 vector31, System.Drawing.Rectangle sourceRect,
            Color color, float rotation, Vector3 vector32, Vector3 vector33,
            SpriteEffects none, float layerDepth)
        {
            // Map System.Drawing.Rectangle to the project's Core.Rectangle and delegate to the overload that uses it.
            var coreRect = new Rectangle(
                sourceRect.X,
                sourceRect.Y,
                sourceRect.Width,
                sourceRect.Height);
            Draw(
                v,
                vector31,
                coreRect,
                color,
                rotation,
                vector32,
                vector33,
                none,
                layerDepth);
        }
    }

    /// <summary>
    /// Stub sampler states for compatibility.
    /// </summary>
    public class SamplerState
    {
        public static SamplerState LinearClamp { get; } = new SamplerState();
        public static SamplerState PointClamp { get; } = new SamplerState();
        public static SamplerState AnisotropicClamp { get; } = new SamplerState();
        public static SamplerState LinearWrap { get; } = new SamplerState();
        public static SamplerState PointWrap { get; } = new SamplerState();
        public static SamplerState AnisotropicWrap { get; } = new SamplerState();
    }

    /// <summary>
    /// Stub blend states for compatibility.
    /// </summary>
    public class BlendState
    {
        public static BlendState AlphaBlend { get; } = new BlendState();
        public static BlendState Additive { get; } = new BlendState();
        public static BlendState Opaque { get; } = new BlendState();
        public static BlendState NonPremultiplied { get; } = new BlendState();
    }
}
