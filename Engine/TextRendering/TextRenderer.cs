// ====================================================================================================
//  FILE: TextRenderer.cs
//  PATH: ./Engine/Render/Text/
//  MODULE: Render / Text
//
//  ROLE:
//      GPU‑accelerated text renderer that converts strings into Sprite draw commands using a BitmapFont.
//      Handles alignment, scaling, and glyph layout in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Measure text using glyph metrics.
//      - Layout glyphs according to alignment and scale.
//      - Emit Sprite commands for SpriteBatch submission.
//      - Remain deterministic and side‑effect free.
//
//  NON-RESPONSIBILITIES:
//      - CPU framebuffer rendering.
//      - Logging, diagnostics, or fallback paths.
//      - Resource loading or font file parsing.
//      - Word wrapping (handled by TextLayout).
//
//  NOTES:
//      - Pure GPU text rendering subsystem.
//      - Compatible with Option‑B deterministic rendering.
//      - Uses immutable Sprite structs.
// ====================================================================================================

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows;
using SASZombieAssaultTD.Engine.Render.Sprites;

namespace SASZombieAssaultTD.Engine.TextRendering
{
    /// <summary>
    /// GPU‑accelerated text renderer using BitmapFont glyph atlases.
    /// </summary>
    public sealed class TextRenderer
    {
        private readonly BitmapFont _font;

        public TextRenderer(BitmapFont font) => _font = font;

        /// <summary>
        /// Measures the width of a string using the font's glyph metrics.
        /// </summary>
        public float MeasureText(string text, float scale = 1f)
        {
            float width = 0f;

            foreach (char c in text)
            {
                if (_font.TryGetGlyph(c, out var glyph))
                    width += glyph.AdvanceX * scale;
            }

            return width;
        }

        /// <summary>
        /// Draws text by emitting Sprite commands into the provided list.
        /// </summary>
        public void DrawText(
            List<Sprite> output,
            string text,
            Vector2 position,
            Color color,
            float scale = 1f,
            TextAlignment alignment = TextAlignment.Left)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Measure total width for alignment
            float totalWidth = MeasureText(text, scale);

            float startX = alignment switch
            {
                TextAlignment.Center => position.X - (totalWidth / 2f),
                TextAlignment.Right => position.X - totalWidth,
                _ => position.X
            };

            float x = startX;
            float y = position.Y;

            foreach (char c in text)
            {
                if (!_font.TryGetGlyph(c, out var glyph))
                    continue;

                var dest = new RectangleF(
                    x + glyph.OffsetX * scale,
                    y + glyph.OffsetY * scale,
                    glyph.Width * scale,
                    glyph.Height * scale
                );

                var src = new RectangleF(
                    glyph.U,
                    glyph.V,
                    glyph.Width,
                    glyph.Height
                );

                output.Add(new Sprite(
                    _font.Texture,
                    dest,
                    src,
                    color,
                    0f,
                    Vector2.Zero,
                    0f
                ));

                x += glyph.AdvanceX * scale;
            }
        }
    }
}
