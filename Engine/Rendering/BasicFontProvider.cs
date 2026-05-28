// ============================================================================
// File Path: Engine/Rendering/BasicFontProvider.cs
// File: BasicFontProvider.cs
// Program: Rendering (Font Provider)
// Subsystem: Rendering / Text Rendering
//
// Purpose:
//     Provides a minimal, fixed‑size 6×8 monochrome bitmap font for HUD/UI text.
//     Used by TextRenderer and debug overlays where a lightweight, dependency‑free
//     font is required.
//
// Responsibilities:
//     - Store glyph definitions for supported characters
//     - Provide glyph lookup for TextRenderer
//     - Convert ASCII art glyph definitions into alpha masks
//
// Architecture:
//     - Sealed class implementing IFontProvider
//     - Glyphs stored as Dictionary<char, GlyphData>
//     - 6×8 fixed‑size grid, monospaced
//
// Integration Points:
//     - TextRenderer (Engine/Rendering/TextRenderer.cs)
//     - HUD elements and debug overlays
//
// Notes:
//     - This is intentionally minimal and fast
//     - Only includes characters required by HUD and debug systems
//     - Additional glyphs can be added safely
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class BasicFontProvider : IFontProvider
    {
        private readonly Dictionary<char, GlyphData> _glyphs;

        /// <summary>
        /// Height of each glyph in pixels.
        /// </summary>
        public int LineHeight { get; }

        public BasicFontProvider()
        {
            LineHeight = 8;

            _glyphs = new Dictionary<char, GlyphData>
            {
                // --------------------------------------------------------------------
                // DIGITS
                // --------------------------------------------------------------------
                ['0'] = MakeGlyph6x8(new[]
                {
                    " .#### ",
                    " ##  ##",
                    " ##  ##",
                    " ##  ##",
                    " ##  ##",
                    " ##  ##",
                    " ##  ##",
                    " .#### ",
                }),

                ['1'] = MakeGlyph6x8(new[]
                {
                    " ..##  ",
                    " .###  ",
                    " ..##  ",
                    " ..##  ",
                    " ..##  ",
                    " ..##  ",
                    " ..##  ",
                    " .#### ",
                }),

                ['2'] = MakeGlyph6x8(new[]
                {
                    " .#### ",
                    " ##  ##",
                    "    ## ",
                    "   ##  ",
                    "  ##   ",
                    " ##    ",
                    " ######",
                    "       ",
                }),

                ['3'] = MakeGlyph6x8(new[]
                {
                    " .#### ",
                    " ##  ##",
                    "    ## ",
                    "  .##  ",
                    "    ## ",
                    " ##  ##",
                    " .#### ",
                    "       ",
                }),

                ['4'] = MakeGlyph6x8(new[]
                {
                    "   ##  ",
                    "  ###  ",
                    " ###   ",
                    " ##  ##",
                    " ######",
                    "    ## ",
                    "    ## ",
                    "       ",
                }),

                ['5'] = MakeGlyph6x8(new[]
                {
                    " ######",
                    " ##    ",
                    " ##### ",
                    "    ## ",
                    "    ## ",
                    " ##  ##",
                    " .#### ",
                    "       ",
                }),

                ['6'] = MakeGlyph6x8(new[]
                {
                    " .#### ",
                    " ##  ##",
                    " ##    ",
                    " ##### ",
                    " ##  ##",
                    " ##  ##",
                    " .#### ",
                    "       ",
                }),

                ['7'] = MakeGlyph6x8(new[]
                {
                    " ######",
                    "    ## ",
                    "   ##  ",
                    "  ##   ",
                    " ##    ",
                    " ##    ",
                    " ##    ",
                    "       ",
                }),

                ['8'] = MakeGlyph6x8(new[]
                {
                    " .#### ",
                    " ##  ##",
                    " ##  ##",
                    " .#### ",
                    " ##  ##",
                    " ##  ##",
                    " .#### ",
                    "       ",
                }),

                ['9'] = MakeGlyph6x8(new[]
                {
                    " .#### ",
                    " ##  ##",
                    " ##  ##",
                    " .#####",
                    "    ## ",
                    " ##  ##",
                    " .#### ",
                    "       ",
                }),

                // --------------------------------------------------------------------
                // SYMBOLS
                // --------------------------------------------------------------------
                ['$'] = MakeGlyph6x8(new[]
                {
                    "  .##  ",
                    " .#### ",
                    " ##    ",
                    " .###  ",
                    "   ### ",
                    "    ## ",
                    " .#### ",
                    "  .##  ",
                }),

                [':'] = MakeGlyph6x8(new[]
                {
                    "       ",
                    "  ##   ",
                    "  ##   ",
                    "       ",
                    "       ",
                    "  ##   ",
                    "  ##   ",
                    "       ",
                }),

                [' '] = MakeGlyph6x8(new[]
                {
                    "       ",
                    "       ",
                    "       ",
                    "       ",
                    "       ",
                    "       ",
                    "       ",
                    "       ",
                }),

                // --------------------------------------------------------------------
                // LETTERS (minimal HUD set)
                // --------------------------------------------------------------------
                ['A'] = MakeGlyph6x8(new[]
                {
                    " ####  ",
                    "##  ## ",
                    "##  ## ",
                    "###### ",
                    "##  ## ",
                    "##  ## ",
                    "##  ## ",
                    "       ",
                }),

                ['E'] = MakeGlyph6x8(new[]
                {
                    "###### ",
                    "##     ",
                    "####   ",
                    "##     ",
                    "##     ",
                    "##     ",
                    "###### ",
                    "       ",
                }),

                ['F'] = MakeGlyph6x8(new[]
                {
                    "###### ",
                    "##     ",
                    "####   ",
                    "##     ",
                    "##     ",
                    "##     ",
                    "##     ",
                    "       ",
                }),

                ['H'] = MakeGlyph6x8(new[]
                {
                    "##  ## ",
                    "##  ## ",
                    "##  ## ",
                    "###### ",
                    "##  ## ",
                    "##  ## ",
                    "##  ## ",
                    "       ",
                }),

                ['I'] = MakeGlyph6x8(new[]
                {
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "       ",
                }),

                ['L'] = MakeGlyph6x8(new[]
                {
                    "##     ",
                    "##     ",
                    "##     ",
                    "##     ",
                    "##     ",
                    "##     ",
                    "###### ",
                    "       ",
                }),

                ['T'] = MakeGlyph6x8(new[]
                {
                    "###### ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "  ##   ",
                    "       ",
                }),
            };
        }

        // --------------------------------------------------------------------
        // GLYPH LOOKUP
        // --------------------------------------------------------------------

        public bool TryGetGlyph(char c, out GlyphData glyph)
        {
            if (_glyphs.TryGetValue(c, out glyph))
                return true;

            // Fallback to space
            if (_glyphs.TryGetValue(' ', out glyph))
                return true;

            glyph = default;
            return false;
        }

        // --------------------------------------------------------------------
        // GLYPH CREATION
        // --------------------------------------------------------------------

        private static GlyphData MakeGlyph6x8(string[] rows)
        {
            const int width = 6;
            const int height = 8;

            var alpha = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                string row = rows[y];

                for (int x = 0; x < width; x++)
                {
                    char ch = x < row.Length ? row[x] : ' ';
                    alpha[y * width + x] = (ch != ' ' && ch != '.') ? (byte)255 : (byte)0;
                }
            }

            return new GlyphData(width, height, alpha);
        }
    }
}
