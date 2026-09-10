// ====================================================================================================
//  FILE: GlyphMetrics.cs
//  PATH: Engine/Render/Text/
//  MODULE: Render / Text
//
//  ROLE:
//      CPU‑side glyph metrics used before packing into a GPU atlas.
//      Represents raw glyph information loaded from font files.
//
//  RESPONSIBILITIES:
//      - Store raw glyph dimensions and offsets.
//      - Provide deterministic data for atlas building.
//      - Serve as the CPU‑side precursor to GPU GlyphData.
//
//  NON-RESPONSIBILITIES:
//      - GPU rendering or UV coordinates.
//      - Logging, diagnostics, or fallback behavior.
//      - Resource loading or file parsing.
//
//  NOTES:
//      - Pure data container.
//      - Converted into GlyphData during atlas packing.
//      - Option‑B deterministic rendering compliant.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.TextRendering
{
    /// <summary>
    /// CPU‑side glyph metrics used before packing into a GPU atlas.
    /// </summary>
    public readonly struct GlyphMetrics
    {
        public readonly char Character;
        public readonly int Width;
        public readonly int Height;
        public readonly int OffsetX;
        public readonly int OffsetY;
        public readonly int AdvanceX;

        public GlyphMetrics(
            char character,
            int width,
            int height,
            int offsetX,
            int offsetY,
            int advanceX)
        {
            Character = character;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            AdvanceX = advanceX;
        }
    }
}
