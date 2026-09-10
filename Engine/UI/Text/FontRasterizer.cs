// =====================================================================================================
//  FILE: FontRasterizer.cs
//  PATH: Engine/UI/Text/FontRasterizer.cs
//  SUBSYSTEM: UI Text
//
//  ROLE:
//      Provides minimal deterministic glyph rasterization and font sampling utilities for the UI text
//      subsystem. This class acts as a backend helper for TextRenderer, supplying glyph bitmaps or
//      approximations when a full font system is not yet implemented.
//
//  RESPONSIBILITIES:
//      - Supply deterministic glyph rasterization or placeholder glyphs.
//      - Provide glyph metrics (width, height) in a backend‑agnostic manner.
//      - Serve as a stable abstraction layer for future font systems (bitmap, SDF, vector).
//
//  NON-RESPONSIBILITIES:
//      - Managing framebuffer memory or GPU resources.
//      - Implementing complex font loading, kerning, or shaping logic.
//      - Handling UI layout, wrapping, or alignment (handled by TextRenderer).
//
//  ARCHITECTURAL NOTES:
//      - Designed to be used internally by TextRenderer.
//      - Can be replaced by a full font subsystem without changing callers.
//      - Must remain deterministic and safe for all inputs.
// =====================================================================================================

using System.Numerics;

namespace SASZombieAssaultTD.Engine.UI.Text
{
    /// <summary>
    /// Minimal deterministic glyph rasterization helper.
    /// Provides placeholder glyph metrics and bitmap generation until a full font system is available.
    /// </summary>
    internal sealed class FontRasterizer
    {
        /// <summary>
        /// Returns a deterministic placeholder glyph size for the given character and font size.
        /// This is intentionally simple and can be replaced by real font metrics later.
        /// </summary>
        /// <param name="character">The character to measure.</param>
        /// <param name="fontSize">The nominal font size.</param>
        /// <returns>Approximate glyph width and height.</returns>
        public Vector2 MeasureGlyph(char character, float fontSize)
        {
            if (fontSize <= 0f)
                return Vector2.Zero;

            // Deterministic placeholder:
            // - Width: 0.6 × fontSize
            // - Height: fontSize
            float width = fontSize * 0.6f;
            float height = fontSize;

            return new Vector2(width, height);
        }

        /// <summary>
        /// Generates a placeholder glyph bitmap for the given character.
        /// This implementation returns a simple boolean grid representing a filled rectangle.
        /// A real font system can replace this with actual glyph rasterization.
        /// </summary>
        /// <param name="character">The character to rasterize.</param>
        /// <param name="fontSize">The nominal font size.</param>
        /// <returns>A 2D boolean array representing the glyph bitmap.</returns>
        public bool[,] RasterizeGlyph(char character, float fontSize)
        {
            if (fontSize <= 0f)
                return new bool[0, 0];

            Vector2 size = MeasureGlyph(character, fontSize);
            int width = System.Math.Max(1, (int)size.X);
            int height = System.Math.Max(1, (int)size.Y);

            bool[,] bitmap = new bool[width, height];

            // Placeholder rasterization: fill the entire glyph rectangle.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap[x, y] = true;
                }
            }

            return bitmap;
        }
    }
}
