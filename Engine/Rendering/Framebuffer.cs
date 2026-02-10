/*
    File:    Framebuffer.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Minimal framebuffer implementing IRenderContext with a raw pixel buffer.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
using System;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Minimal framebuffer representation with a pixel buffer.
    /// </summary>
    public sealed class Framebuffer : IRenderContext
    {
        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Raw pixel buffer (RGBA).
        /// </summary>
        public byte[] Pixels { get; }

        public Framebuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Framebuffer dimensions must be positive.");

            Width = width;
            Height = height;

            // 4 bytes per pixel (RGBA)
            Pixels = new byte[Width * Height * 4];
        }

        /// <summary>
        /// Fills the entire framebuffer with a single RGBA color.
        /// </summary>
        public void Clear(uint rgba)
        {
            byte r = (byte)((rgba >> 24) & 0xFF);
            byte g = (byte)((rgba >> 16) & 0xFF);
            byte b = (byte)((rgba >> 8) & 0xFF);
            byte a = (byte)(rgba & 0xFF);

            for (int i = 0; i < Pixels.Length; i += 4)
            {
                Pixels[i + 0] = r;
                Pixels[i + 1] = g;
                Pixels[i + 2] = b;
                Pixels[i + 3] = a;
            }
        }

        /// <summary>
        /// Writes a single pixel to the framebuffer.
        /// </summary>
        public void SetPixel(int x, int y, uint rgba)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            int index = (y * Width + x) * 4;

            Pixels[index + 0] = (byte)((rgba >> 24) & 0xFF); // R
            Pixels[index + 1] = (byte)((rgba >> 16) & 0xFF); // G
            Pixels[index + 2] = (byte)((rgba >> 8) & 0xFF);  // B
            Pixels[index + 3] = (byte)(rgba & 0xFF);         // A
        }

        // IRenderContext implementation
        public void DrawTexture(Texture2D texture, int x, int y)
        {
            if (texture is null)
                return;

            // Blit texture pixels into the framebuffer using SetPixel.
            // Texture2D carries dimensions but no pixel data yet;
            // fill the bounding rect with a placeholder color (magenta)
            // so the draw call is visually verifiable.
            uint placeholderColor = 0xFF00FFFF; // Magenta, full alpha

            for (int ty = 0; ty < texture.Height; ty++)
            {
                for (int tx = 0; tx < texture.Width; tx++)
                {
                    SetPixel(x + tx, y + ty, placeholderColor);
                }
            }
        }

        public void DrawText(string text, int x, int y)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Render each character as a small filled rectangle (6x8 per glyph)
            // using a placeholder color so the draw call is visually verifiable.
            const int glyphWidth = 6;
            const int glyphHeight = 8;
            uint textColor = 0xFFFFFFFF; // White, full alpha

            for (int i = 0; i < text.Length; i++)
            {
                int gx = x + (i * glyphWidth);

                for (int py = 0; py < glyphHeight; py++)
                {
                    for (int px = 0; px < glyphWidth; px++)
                    {
                        SetPixel(gx + px, y + py, textColor);
                    }
                }
            }
        }

        public void ClearScreen()
        {
            // Clear to a default color (e.g., blue: 0x0000FFFF)
            Clear(0x0000FFFF);
        }
    }
}