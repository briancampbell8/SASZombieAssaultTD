//============================================================================
//File: IFontProvider.cs
//Program: IFontProvider
//Subsystem: Rendering / Text Rendering Pipeline
//
//Purpose:
//    Defines the contract for glyph retrieval and font metrics.
//    Used by TextRenderer, BasicFontProvider, and any custom font loaders.
//
//Diagnostics:
//    - Uses DLogger.Log() for glyph lookup failures.
//    - Deterministic, grep‑friendly trace naming.
//============================================================================

//

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering
{
    public interface IFontProvider
    {
        ///<summary>
        ///Attempts to retrieve glyph data for the given character.
        ///Returns true if the glyph exists.
        ///</summary>
        bool TryGetGlyph(char c, out GlyphData glyph);

        ///<summary>
        ///Height of a single line of text in pixels.
        ///</summary>
        int LineHeight { get; }
    }

    ///<summary>
    ///Immutable glyph data used by the text rendering pipeline.
    ///Alpha is row-major: width * height.
    ///</summary>
    public readonly struct GlyphData
    {
        public int Width { get; }
        public int Height { get; }
        public byte[] Alpha { get; }

        public GlyphData(int width, int height, byte[] alpha)
        {
            Width = width;
            Height = height;
            Alpha = alpha;
        }
    }
}
