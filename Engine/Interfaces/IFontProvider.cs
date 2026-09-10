// ====================================================================================================
//  FILE: IFontProvider.cs
//  PATH: Engine?/Interfaces/IFontProvider.cs
//  MODULE: Render / Text
//  SUBSYSTEMS: Interfaces
//  ROLE:
//      Defines the contract for glyph retrieval and font metrics in the GPU text rendering subsystem.
//      Provides glyph lookup for TextRenderer and any custom font implementations.
//
//  RESPONSIBILITIES:
//      - Provide deterministic glyph lookup.
//      - Expose line height for text layout.
//      - Integrate cleanly with BitmapFont and Texture2D atlas textures.
//
//  NON-RESPONSIBILITIES:
//      - CPU framebuffer rendering.
//      - Logging, diagnostics, or fallback behavior.
//      - Resource loading or font file parsing.
//
//  NOTES:
//      - Pure interface for Option‑B deterministic rendering.
//      - GlyphData is UV‑based and GPU‑friendly.
// ====================================================================================================

using SASZombieAssaultTD.Engine.TextRendering;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Provides glyph lookup and font metrics for GPU text rendering.
    /// </summary>
    public interface IFontProvider
    {
        /// <summary>
        /// Attempts to retrieve glyph metrics for a character.
        /// </summary>
        bool TryGetGlyph(char c, out GlyphData glyph);

        /// <summary>
        /// Height of a single line of text in pixels.
        /// </summary>
        int LineHeight { get; }

        /// <summary>
        /// The GPU texture atlas containing all glyphs.
        /// </summary>
        Texture2D Texture { get; }
    }
}
