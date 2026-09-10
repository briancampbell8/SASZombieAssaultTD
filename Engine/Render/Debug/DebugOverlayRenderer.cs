// ====================================================================================================
//  FILE: DebugOverlayRenderer.cs
//  PATH: ./Engine/Render/Debug/
//  MODULE: Render / Debug
//
//  ROLE:
//      GPU-accelerated debug overlay renderer for drawing outlines, pixels, and diagnostic shapes.
//      Provides real-time visualization of HUD/UI boundaries, alignment guides, and pixel-perfect
//      debug primitives using the engine's SpriteBatch subsystem.
//
//  RESPONSIBILITIES:
//      - Draw rectangle outlines using GPU quads.
//      - Draw pixel-perfect circles using Bresenham's algorithm.
//      - Provide global offset tuning for HUD alignment.
//      - Provide toggleable debug rendering for development builds.
//      - Use System.Drawing primitives for convenience while outputting via GPU.
//
//  NON-RESPONSIBILITIES:
//      - Performing gameplay rendering.
//      - Managing engine resources or textures beyond the debug pixel.
//      - Handling text rendering or font layout.
//      - Performing diagnostics logging (handled by Diagnostics subsystem).
//
//  NOTES:
//      Hybrid System/Engine design:
//      - System.Drawing.RectangleF and System.Drawing.Color are allowed for debug tooling.
//      - GPU rendering is performed via SpriteBatch and Texture2D.
//      - Deterministic Option‑B architecture compliant.
// ====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.Render.Sprites;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.Debug
{
    /// <summary>
    /// GPU-accelerated debug overlay renderer for drawing outlines, pixels, and diagnostic shapes.
    /// </summary>
    internal sealed class DebugOverlayRenderer
    {
        public bool Enabled = true;
        public bool DrawCircleEnabled = false;

        public float GlobalOffsetX = 0f;
        public float GlobalOffsetY = 0f;

        private readonly Texture2D _pixel;
        private readonly SpriteBatchRenderer _batch;

        /// <summary>
        /// Creates a new DebugOverlayRenderer using the provided SpriteBatch.
        /// </summary>
        public DebugOverlayRenderer(SpriteBatchRenderer batch)
        {
            _batch = batch ?? throw new ArgumentNullException(nameof(batch));

            // Create a 1x1 white pixel texture for tinting.
            var pixelData = new byte[] { 255, 255, 255, 255 }; // BGRA32
            _pixel = new Texture2D("debug_pixel", 1, 1, pixelData);
        }

        // ====================================================================================================
        //  RECTANGLE OUTLINES
        // ====================================================================================================

        /// <summary>
        /// Draws a rectangle outline using GPU quads.
        /// </summary>
        public void DrawRectangleOutline(RectangleF rect, Color color, float thickness = 2f)
        {
            if (!Enabled || thickness <= 0f)
                return;

            // Apply global offsets
            rect = new RectangleF(
                rect.X + GlobalOffsetX,
                rect.Y + GlobalOffsetY,
                rect.Width,
                rect.Height
            );

            // Convert to engine Rectangle
            var r = new Rectangle(
                (int)rect.X,
                (int)rect.Y,
                (int)rect.Width,
                (int)rect.Height);

            // Top
            DrawSolidRect(r.X, r.Y, r.Width, thickness, color);

            // Bottom
            DrawSolidRect(
                r.X,
                r.Y + r.Height - thickness,
                r.Width,
                thickness,
                color);

            // Left
            DrawSolidRect(r.X, r.Y, thickness, r.Height, color);

            // Right
            DrawSolidRect(
                r.X + r.Width - thickness,
                r.Y,
                thickness,
                r.Height,
                color);
        }

        // ====================================================================================================
        //  PIXEL DRAWING
        // ====================================================================================================

        private void DrawPixel(int x, int y, Color color)
        {
            var dest = new Rectangle(x, y, 1, 1);
            _batch.Draw(_pixel, dest, color);
        }

        // ====================================================================================================
        //  SOLID RECTANGLES
        // ====================================================================================================

        public void DrawSolidRect(float x, float y, float width, float height, Color color)
        {
            var dest = new Rectangle(
                (int)(float)MathF.Round(x),
                (int)(float)MathF.Round(y),
                (int)(float)MathF.Round(width),
                (int)(float)MathF.Round(height)
            );

            _batch.Draw(_pixel, dest, color);
        }

        // ====================================================================================================
        //  CIRCLE DIAGNOSTICS
        // ====================================================================================================

        /// <summary>
        /// Draws a pixel-perfect circle using Bresenham's algorithm.
        /// </summary>
        public void DrawCircleDiagnostic(float centerX, float centerY, float radius)
        {
            if (!Enabled || !DrawCircleEnabled || radius <= 0f)
                return;

            int cx = (int)MathF.Round(centerX + GlobalOffsetX);
            int cy = (int)MathF.Round(centerY + GlobalOffsetY);
            int r = (int)MathF.Round(radius);

            int x = 0;
            int y = r;
            int d = 3 - 2 * r;

            while (x <= y)
            {
                DrawPixel(cx + x, cy + y, Color.Red);
                DrawPixel(cx - x, cy + y, Color.Red);
                DrawPixel(cx + x, cy - y, Color.Red);
                DrawPixel(cx - x, cy - y, Color.Red);

                DrawPixel(cx + y, cy + x, Color.Red);
                DrawPixel(cx - y, cy + x, Color.Red);
                DrawPixel(cx + y, cy - x, Color.Red);
                DrawPixel(cx - y, cy - x, Color.Red);

                if (d < 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    d += 4 * (x - y) + 10;
                    y--;
                }

                x++;
            }
        }
    }
}
