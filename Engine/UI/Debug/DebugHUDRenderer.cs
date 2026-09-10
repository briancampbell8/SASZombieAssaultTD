// ====================================================================================================
//  FILE: DebugHUDRenderer.cs
//  PATH: Engine/UI/Debug
//  MODULE: UI Debug Visualization
//
//  ROLE:
//      GPU‑accelerated diagnostic renderer for HUD development.
//      Provides pixel‑perfect outlines, rectangles, and circle diagnostics using D3D11Adapter_Core
//      and the ModernUIRenderer command construction subsystem.
//
//  RESPONSIBILITIES:
//      - Visualize HUD element boundaries for alignment and layout debugging.
//      - Draw rectangle outlines using stretched 1x1 pixel materials.
//      - Render pixel‑perfect circles using Bresenham’s algorithm.
//      - Apply global offsets for HUD alignment tuning.
//      - Submit RenderCommand primitives through ModernUIRenderer’s P5 subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Resource loading, caching, or deterministic asset lookup.
//      - Gameplay logic or UI layout management.
//      - GPU pipeline management or batching.
//      - Performance metrics or logging (beyond optional debug output).
//
//  ARCHITECTURAL NOTES:
//      - Uses minimal GPU resources (1x1 materials reused for all outlines).
//      - Designed for development‑time visualization; toggleable via Enabled property.
//      - Deterministic, side‑effect‑free debug rendering.
// ====================================================================================================

using System;
using System.Drawing;
using System.Numerics;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;

namespace SASZombieAssaultTD.Engine.UI.Debug
{
    public sealed class DebugHUDRenderer
    {
        // --------------------------------------------------------------------
        // PUBLIC FLAGS
        // --------------------------------------------------------------------
        public bool Enabled = true;
        public bool DrawCircleEnabled = false;

        public float GlobalOffsetX = 0f;
        public float GlobalOffsetY = 0f;

        // --------------------------------------------------------------------
        // ENGINE REFERENCES (BOUND EXTERNALLY)
        // --------------------------------------------------------------------
        private readonly ModernUIRenderer P8_renderer;
        private readonly UIMaterial P8_pixelRed;
        private readonly UIMaterial P8_pixelWhite;

        // --------------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------------
        public DebugHUDRenderer(ModernUIRenderer renderer, UIMaterial pixelRed, UIMaterial pixelWhite)
        {
            P8_renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            P8_pixelRed = pixelRed ?? throw new ArgumentNullException(nameof(pixelRed));
            P8_pixelWhite = pixelWhite ?? throw new ArgumentNullException(nameof(pixelWhite));
        }

        // --------------------------------------------------------------------
        // DRAW RECTANGLE OUTLINE
        // --------------------------------------------------------------------
        public void DrawRectangleOutline(RectangleF rect, Color color, float thickness)
        {
            if (!Enabled)
                return;

            DrawLine(rect.Left, rect.Top, rect.Right, rect.Top, color, thickness);
            DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom, color, thickness);
            DrawLine(rect.Left, rect.Top, rect.Left, rect.Bottom, color, thickness);
            DrawLine(rect.Right, rect.Top, rect.Right, rect.Bottom, color, thickness);
        }

        // --------------------------------------------------------------------
        // DRAW LINE USING STRETCHED 1x1 MATERIAL
        // --------------------------------------------------------------------
        private void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness)
        {
            if (!Enabled)
                return;

            x1 += GlobalOffsetX;
            y1 += GlobalOffsetY;
            x2 += GlobalOffsetX;
            y2 += GlobalOffsetY;

            float dx = x2 - x1;
            float dy = y2 - y1;
            float length = MathF.Sqrt(dx * dx + dy * dy);
            if (length <= 0f)
                return;

            float angle = MathF.Atan2(dy, dx);

            Vector3 pos = new Vector3(x1, y1, 0f);
            Vector2 size = new Vector2(length, thickness);

            var mat = color.Equals(Color.Red) ? P8_pixelRed : P8_pixelWhite;

            RenderCommand cmd = RenderCommand.CreateRectangle(
                id: 0u,
                pos: pos,
                size: size,
                mat: mat,
                color: new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f),
                depth: 0f,
                rotation: angle
            );

            P8_renderer.SubmitCommand(in cmd);
        }

        // --------------------------------------------------------------------
        // DRAW AREA OUTLINE FOR ELEMENT DEBUGGING
        // --------------------------------------------------------------------
        public void DrawAreaOutline(string elementId, Rectangle destRect, float thickness = 2f)
        {
            if (!Enabled)
                return;

            if (string.IsNullOrEmpty(elementId) ||
                !elementId.EndsWith("_area", StringComparison.OrdinalIgnoreCase))
                return;

            var r = new RectangleF(
                destRect.X + GlobalOffsetX,
                destRect.Y + GlobalOffsetY,
                destRect.Width,
                destRect.Height
            );

            DrawRectangleOutline(r, Color.Red, thickness);
        }

        // --------------------------------------------------------------------
        // DRAW CIRCLE USING BRESENHAM
        // --------------------------------------------------------------------
        public void DrawCircleDiagnostic(float centerX, float centerY, float radius)
        {
            if (!Enabled || !DrawCircleEnabled || radius <= 0)
                return;

            int cx = (int)MathF.Round(centerX + GlobalOffsetX);
            int cy = (int)MathF.Round(centerY + GlobalOffsetY);
            int r = (int)MathF.Round(radius);

            int x = 0;
            int y = r;
            int d = 3 - 2 * r;

            while (x <= y)
            {
                DrawPixel(cx + x, cy + y);
                DrawPixel(cx - x, cy + y);
                DrawPixel(cx + x, cy - y);
                DrawPixel(cx - x, cy - y);
                DrawPixel(cx + y, cy + x);
                DrawPixel(cx - y, cy + x);
                DrawPixel(cx + y, cy - x);
                DrawPixel(cx - y, cy - x);

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

        // --------------------------------------------------------------------
        // DRAW SINGLE PIXEL (1x1 RECTANGLE)
        // --------------------------------------------------------------------
        private void DrawPixel(int x, int y)
        {
            Vector3 pos = new Vector3(x, y, 0f);
            Vector2 size = new Vector2(1f, 1f);

            RenderCommand cmd = RenderCommand.CreateRectangle(
                id: 0u,
                pos: pos,
                size: size,
                mat: P8_pixelRed,
                color: Vector4.One,
                depth: 0f
            );

            P8_renderer.SubmitCommand(in cmd);
        }

        // --------------------------------------------------------------------
        // DRAW SOLID RECTANGLE
        // --------------------------------------------------------------------
        public void DrawSolidRect(float x, float y, float width, float height, Color color)
        {
            Vector3 pos = new Vector3(x + GlobalOffsetX, y + GlobalOffsetY, 0f);
            Vector2 size = new Vector2(width, height);

            var mat = P8_pixelWhite;

            RenderCommand cmd = RenderCommand.CreateRectangle(
                id: 0u,
                pos: pos,
                size: size,
                mat: mat,
                color: new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f),
                depth: 0f
            );

            P8_renderer.SubmitCommand(in cmd);
        }
    }
}
