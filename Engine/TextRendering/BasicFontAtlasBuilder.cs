// ====================================================================================================
//  FILE: BasicFontAtlasBuilder.cs
//  PATH: ./Engine/Render/Text/
//  MODULE: Render / Text
//
//  ROLE:
//      Builds a GPU atlas for the 6×8 ASCII debug font and produces a BasicFontProvider.
//      Converts ASCII-art glyph definitions into alpha masks, packs them into a deterministic
//      atlas layout, uploads the atlas to the GPU, and generates UV-based GlyphData entries.
//
//  RESPONSIBILITIES:
//      - Convert ASCII glyph definitions into alpha masks.
//      - Pack glyphs into a GPU atlas using deterministic grid placement.
//      - Upload atlas alpha data into a Texture2D GPU resource.
//      - Produce UV-based GlyphData for use by TextRenderer.
//      - Return a fully constructed BasicFontProvider.
//
//  NON-RESPONSIBILITIES:
//      - Font file parsing or external resource loading.
//      - Logging, diagnostics, or fallback behavior.
//      - Word wrapping or text layout (handled by TextLayout).
//
//  NOTES:
//      - Pure deterministic builder for minimal HUD/debug font.
//      - Compatible with Option‑B deterministic rendering architecture.
//      - Produces immutable GlyphData entries and a single atlas Texture2D.
// ====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.TextRendering
{
    /// <summary>
    /// Builds a GPU atlas for the 6×8 ASCII debug font and produces a BasicFontProvider.
    /// </summary>
    internal static class BasicFontAtlasBuilder
    {
        private const int GlyphWidth = 6;
        private const int GlyphHeight = 8;

        /// <summary>
        /// Builds the GPU atlas and returns a fully constructed BasicFontProvider.
        /// </summary>
        public static BasicFontProvider Build(Dictionary<char, string[]> asciiGlyphs)
        {
            // 1. Convert ASCII art → alpha masks
            var alphaGlyphs = new Dictionary<char, byte[]>();
            foreach (var kv in asciiGlyphs)
                alphaGlyphs[kv.Key] = ConvertAsciiToAlpha(kv.Value);

            // 2. Pack glyphs into a simple atlas (deterministic grid)
            int glyphCount = alphaGlyphs.Count;
            int atlasCols = NextPowerOfTwo(
                (int)System.Math.Ceiling(System.Math.Sqrt(glyphCount)));
            int atlasRows = atlasCols;

            int atlasWidth = atlasCols * GlyphWidth;
            int atlasHeight = atlasRows * GlyphHeight;

            byte[] atlasAlpha = new byte[atlasWidth * atlasHeight];
            var glyphData = new Dictionary<char, GlyphData>();

            int index = 0;
            foreach (var kv in alphaGlyphs)
            {
                char c = kv.Key;
                byte[] alpha = kv.Value;

                int col = index % atlasCols;
                int row = index / atlasCols;

                int destX = col * GlyphWidth;
                int destY = row * GlyphHeight;

                // Copy alpha mask into atlas
                for (int y = 0; y < GlyphHeight; y++)
                {
                    for (int x = 0; x < GlyphWidth; x++)
                    {
                        int srcIndex = y * GlyphWidth + x;
                        int dstIndex = (destY + y) * atlasWidth + (destX + x);
                        atlasAlpha[dstIndex] = alpha[srcIndex];
                    }
                }

                // Compute UVs
                float u = (float)destX / atlasWidth;
                float v = (float)destY / atlasHeight;

                glyphData[c] = new GlyphData(
                    c,
                    u,
                    v,
                    GlyphWidth,
                    GlyphHeight,
                    0f,
                    0f,
                    GlyphWidth);

                //glyphData[c] = new GlyphData(
                //    character: c,
                //    u: u,
                //    v: v,
                //    width: GlyphWidth,
                //    height: GlyphHeight,
                //    offsetX: 0f,
                //    offsetY: 0f,
                //    advanceX: GlyphWidth);

                index++;
            }

            // 3. Upload atlas to GPU
            Texture2D atlasTexture = Texture2D.CreateAlphaTexture(atlasWidth, atlasHeight, atlasAlpha);

            // 4. Build provider
            return new BasicFontProvider(atlasTexture, glyphData);
        }

        // --------------------------------------------------------------------
        // ASCII → Alpha conversion
        // --------------------------------------------------------------------

        private static byte[] ConvertAsciiToAlpha(string[] rows)
        {
            var alpha = new byte[GlyphWidth * GlyphHeight];

            for (int y = 0; y < GlyphHeight; y++)
            {
                string row = rows[y];
                for (int x = 0; x < GlyphWidth; x++)
                {
                    char ch = x < row.Length ? row[x] : ' ';
                    alpha[y * GlyphWidth + x] = (ch != ' ' && ch != '.') ? (byte)255 : (byte)0;
                }
            }

            return alpha;
        }

        // --------------------------------------------------------------------
        // Utility
        // --------------------------------------------------------------------

        private static int NextPowerOfTwo(int x)
        {
            int p = 1;
            while (p < x) p <<= 1;
            return p;
        }
    }
}
