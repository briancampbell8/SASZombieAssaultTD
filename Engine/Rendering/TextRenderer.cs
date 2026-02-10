namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class TextRenderer
    {
        public TextRenderer()
        {
        }

        // Stub: later you can plug in a bitmap font atlas
        // For now, this is just a placeholder API.

        public void DrawString(Framebuffer fb, int x, int y, string text, int color)
        {
            // Render each character as a small filled rectangle (6×8 per glyph)
            // using the supplied color packed as 0xRRGGBBAA.
            if (fb is null || string.IsNullOrEmpty(text))
                return;

            uint rgba = (uint)color;
            const int glyphWidth = 6;
            const int glyphHeight = 8;

            for (int i = 0; i < text.Length; i++)
            {
                int gx = x + (i * glyphWidth);

                for (int py = 0; py < glyphHeight; py++)
                {
                    for (int px = 0; px < glyphWidth; px++)
                    {
                        fb.SetPixel(gx + px, y + py, rgba);
                    }
                }
            }
        }
    }
}
