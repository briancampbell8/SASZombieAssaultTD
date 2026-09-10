// ====================================================================================================
//  FILE: GlyphData.cs
//  PATH: Engine/Render/Text/
//  MODULE: Render / Text
//
//  ROLE:
//      Immutable GPU glyph metrics used by BitmapFont and TextRenderer.
//      Represents UV coordinates and layout metrics for a single glyph.
//
//  RESPONSIBILITIES:
//      - Store UV coordinates for sampling the glyph from the atlas.
//      - Store layout metrics (offsets, advance width).
//      - Remain deterministic and immutable.
//
//  NON-RESPONSIBILITIES:
//      - CPU bitmap storage or alpha masks.
//      - Logging, diagnostics, or fallback behavior.
//      - Resource loading or file parsing.
//
//  NOTES:
//      - Pure GPU-side data container.
//      - Converted from GlyphMetrics during atlas packing.
//      - Option‑B deterministic rendering compliant.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.TextRendering
{
    /// <summary>
    /// Immutable GPU glyph metrics for text rendering.
    /// </summary>
    public readonly struct GlyphData
    {
        public readonly char Character;
        public readonly float U;
        public readonly float V;
        public readonly float Width;
        public readonly float Height;
        public readonly float OffsetX;
        public readonly float OffsetY;
        public readonly float AdvanceX;
        public readonly byte[] alpha;


        public GlyphData(int width, int height, byte[] alpha) : this()
        {
            Width = width;
            Height = height;
            this.alpha = alpha;
        }

        public GlyphData(
            char character,
            float u,
            float v,
            float width,
            float height,
            float offsetX,
            float offsetY,
            float advanceX,
            byte[] alpha = null)
        {
            Character = character;
            U = u;
            V = v;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            AdvanceX = advanceX;
            this.alpha = alpha;
        }
    }
}
