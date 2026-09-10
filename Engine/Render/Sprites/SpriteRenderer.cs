// =====================================================================================================
//  FILE: SpriteRenderer.cs
//  PATH: Engine/Render/Sprites/SpriteRenderer.cs
//  SUBSYSTEM: Render Sprites
//
//  ROLE:
//      Provides deterministic sprite rendering utilities for the engine’s render layer. Responsible for
//      drawing textured quads, applying tint, scale, rotation, and delegating low-level blitting and
//      region-copy operations to the underlying render context.
//
//  RESPONSIBILITIES:
//      - Draw individual sprites and textures.
//      - Apply tinting, scaling, rotation, and blending.
//      - Support deterministic rendering behavior for UI and game objects.
//      - Act as the bridge between render contexts and texture operations.
//
//  NON-RESPONSIBILITIES:
//      - Managing framebuffer memory or GPU device resources.
//      - Loading or caching texture assets (handled by asset subsystems).
//      - Handling ECS-level entity composition or game logic.
//      - Performing low-level pixel blitting (delegated to TextureBlitter or render context).
//
//  ARCHITECTURAL NOTES:
//      - Invoked by SoftwareRenderContext for DrawSprite and DrawTexture calls.
//      - Designed to remain deterministic and free of engine lifecycle responsibilities.
//      - Can be extended to support batching, atlas rendering, or GPU acceleration.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.Sprites
{
    /// <summary>
    /// Minimal deterministic sprite rendering subsystem.
    /// </summary>
    internal sealed class SpriteRenderer
    {
        private readonly D3D11Adapter_Core _renderContext;

        /// <summary>
        /// Creates a new SpriteRenderer bound to a specific render context.
        /// </summary>
        /// <param name="renderContext">The render context used for drawing operations.</param>
        public SpriteRenderer(D3D11Adapter_Core renderContext)
        {
            _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
        }

        /// <summary>
        /// Draws a sprite texture at the given position, applying scale and tint.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">The position in screen space.</param>
        /// <param name="scale">Uniform scale factor.</param>
        /// <param name="rotation">Rotation in radians.</param>
        /// <param name="tint">Color tint applied to the sprite.</param>
        public void DrawSprite(Texture2D texture, Vector2 position, float scale, float rotation, ColorRGBA tint)
        {
            if (texture == null)
                return;

            // Compute deterministic transform
            Vector2 size = new Vector2(texture.Width, texture.Height) * scale;
            Matrix3x2 transform = Matrix3x2.CreateRotation(rotation) * Matrix3x2.CreateTranslation(position);

            // Delegate to render context
            _renderContext.DrawTexture(texture, transform, tint);
        }

        /// <summary>
        /// Draws a raw texture region at the specified position.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="sourceRect">The source rectangle within the texture.</param>
        /// <param name="destination">Destination position in screen space.</param>
        /// <param name="tint">Color tint applied to the texture.</param>
        public void DrawTextureRegion(Texture2D texture, Rectangle sourceRect, Vector2 destination, ColorRGBA tint)
        {
            if (texture == null)
                return;

            _renderContext.DrawTextureRegion(texture, sourceRect, destination, tint);
        }
    }
}