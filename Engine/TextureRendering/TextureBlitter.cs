// =====================================================================================================
//  FILE: TextureBlitter.cs
//  PATH: Engine/Render/Textures/TextureBlitter.cs
//  SUBSYSTEM: Render Textures
//
//  ROLE:
//      Provides deterministic low-level texture blitting operations for the engine’s render layer.
//      Responsible for copying pixel regions, applying tint, and performing raw texture-to-framebuffer
//      transfers in a backend-agnostic manner.
//
//  RESPONSIBILITIES:
//      - Copy texture regions to destination coordinates.
//      - Apply tinting and simple blending during blit operations.
//      - Provide deterministic pixel-level operations for render contexts.
//      - Serve as the backend utility used by SpriteRenderer and SoftwareRenderContext.
//
//  NON-RESPONSIBILITIES:
//      - Managing framebuffer memory or GPU device resources.
//      - Loading or caching texture assets (handled by asset subsystems).
//      - Performing high-level sprite composition or batching (handled by SpriteRenderer).
//      - Handling ECS-level entity composition or game logic.
//
//  ARCHITECTURAL NOTES:
//      - Designed to be invoked internally by render contexts and sprite subsystems.
//      - Must remain deterministic and free of engine lifecycle responsibilities.
//      - Can be extended to support GPU acceleration or advanced blending modes.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    /// <summary>
    /// Minimal deterministic texture blitting subsystem.
    /// </summary>
    internal sealed class TextureBlitter
    {
        /// <summary>
        /// Copies a region of a texture into a destination pixel buffer, applying tint.
        /// </summary>
        /// <param name="source">The source texture.</param>
        /// <param name="sourceRect">The region of the source texture to copy.</param>
        /// <param name="destination">The destination pixel buffer.</param>
        /// <param name="destX">Destination X coordinate.</param>
        /// <param name="destY">Destination Y coordinate.</param>
        /// <param name="tint">Tint color applied to each pixel.</param>
        public void Blit(Texture2D source, Rectangle sourceRect, ColorRGBA[] destination, int destWidth, int destHeight, int destX, int destY, ColorRGBA tint)
        {
            if (source == null || destination == null)
                return;

            // Ensure the texture exposes a typed pixel buffer we can index.
            if (!(source.Pixels is ColorRGBA[] srcPixels))
                return;

            int srcWidth = source.Width;
            int srcHeight = source.Height;

            // Clamp source rectangle
            int x0 = (int)System.Math.Max(0, sourceRect.X);
            int y0 = (int)System.Math.Max(0, sourceRect.Y);
            int x1 = (int)System.Math.Min(srcWidth, sourceRect.X + sourceRect.Width);
            int y1 = (int)System.Math.Min(srcHeight, sourceRect.Y + sourceRect.Height);

            for (int y = y0; y < y1; y++)
            {
                for (int x = x0; x < x1; x++)
                {
                    int srcIndex = y * srcWidth + x;
                    ColorRGBA srcPixel = srcPixels[srcIndex];

                    // Apply tint (simple multiply)
                    ColorRGBA finalPixel = new ColorRGBA(
                        (byte)(srcPixel.R * tint.R),
                        (byte)(srcPixel.G * tint.G),
                        (byte)(srcPixel.B * tint.B),
                        (byte)(srcPixel.A * tint.A)
                    );

                    int dx = destX + (x - x0);
                    int dy = destY + (y - y0);

                    if (dx < 0 || dy < 0 || dx >= destWidth || dy >= destHeight)
                        continue;

                    int destIndex = dy * destWidth + dx;
                    destination[destIndex] = finalPixel;
                }
            }
        }
    }
}