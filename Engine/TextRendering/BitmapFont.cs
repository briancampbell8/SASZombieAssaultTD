// ====================================================================================================
//  FILE: BitmapFont.cs
//  PATH: Engine/Render/Text/
//  MODULE: Render / Text
//
//  ROLE:
//      Immutable GPU bitmap font containing glyph metrics and a GPU atlas texture.
//      Provides glyph lookup for TextRenderer and SpriteBatch text rendering.
//
//  RESPONSIBILITIES:
//      - Store glyph metrics in a deterministic dictionary.
//      - Provide TryGetGlyph() for fast lookup.
//      - Expose the GPU atlas texture used for glyph rendering.
//
//  NON-RESPONSIBILITIES:
//      - Loading font files or parsing bitmap font data.
//      - Logging, diagnostics, or fallback behavior.
//      - Word wrapping or text layout (handled by TextLayout).
//
//  NOTES:
//      - Pure data container.
//      - Compatible with Option‑B deterministic rendering.
//      - Used by TextRenderer to emit Sprite commands.
// ====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.TextRendering
{
    /// <summary>
    /// Immutable GPU glyph metrics for a single character.
    /// </summary>


    /// <summary>
    /// Immutable bitmap font containing glyph metrics and a GPU atlas texture.
    /// </summary>
    public sealed class BitmapFont : IFontProvider
    {
        private readonly Dictionary<char, GlyphData> _glyphs;

        /// <summary>
        /// GPU texture atlas containing all glyphs.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// Height of a single line of text in pixels.
        /// </summary>
        public int LineHeight { get; }

        public BitmapFont(
            Texture2D texture,
            Dictionary<char, GlyphData> glyphs,
            int lineHeight)
        {
            Texture = texture;
            _glyphs = glyphs;
            LineHeight = lineHeight;
        }

        public bool TryGetGlyph(char c, out GlyphData glyph)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Attempts to retrieve glyph metrics for a character.
        /// </summary>

    }
}
