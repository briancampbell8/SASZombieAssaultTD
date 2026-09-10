// =====================================================================================================
//  FILE: HUDOverlayRenderer.cs
//  PATH: Engine/UI/HUDOverlay/HUDOverlayRenderer.cs
//  SUBSYSTEM: HUDOverlay (Debug Visualization Subsystem)
//  LAYER: UI → HUDOverlay
//
//  ROLE:
//      Renders the HUDOverlayWindow and its child diagnostic panels. This includes drawing the window
//      frame, title bar, background, and all diagnostic text blocks. HUDOverlayRenderer is the ONLY
//      renderer permitted to draw HUDOverlay content under the Management‑Only Policy.
//
//  RESPONSIBILITIES:
//      - Draw window frame, title bar, and background.
//      - Draw diagnostic text blocks and page content.
//      - Maintain strict separation from HUDManager, HUDRenderer, and Finalizer subsystems.
//      - Emit tracing for all rendering operations.
//
//  NON‑RESPONSIBILITIES:
//      - Does NOT handle input (HUDOverlayInput).
//      - Does NOT manage window state (HUDOverlayManager).
//      - Does NOT modify Finalizer or HUDManager internals.
//
//  CHANGE LOG:
//      - FIXED CS0176: TitleBarHeight is static; replaced instance access with HUDOverlayWindow.TitleBarHeight.
//      - Cleaned up all three offending lines (98, 123, 143).
// =====================================================================================================

using System.Drawing;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.UI.HUDOverlay
{
    internal sealed class HUDOverlayRenderer
    {
        private HUDOverlayWindow window;
        private HUDOverlayPages pages;

        public HUDOverlayRenderer(HUDOverlayWindow window, HUDOverlayPages pages)
        {
            this.window = window;
            this.pages = pages;
        }

        internal void Draw(D3D11Adapter_Core context)
        {
            var window = this.window;
            if (context == null || window == null || !window.IsVisible)
                return;

            // -----------------------------------------------------------------------------------------
            // WINDOW FRAME
            // -----------------------------------------------------------------------------------------
            var abs = (Point)window.AbsolutePosition;
            int x = abs.X;
            int y = abs.Y;
            int w = window.Width;
            int h = window.Height;

            // Background
            context.FillRectangle(
                new Rectangle(x, y, w, h),
                Color.FromArgb(200, 30, 30, 30));

            // -----------------------------------------------------------------------------------------
            // TITLE BAR (STATIC HEIGHT FIX)
            // -----------------------------------------------------------------------------------------

            int titleBarHeight = HUDOverlayWindow.TitleBarHeight;   // FIXED CS0176

            context.FillRectangle(
                new Rectangle(x, y, w, titleBarHeight),
                Color.FromArgb(255, 45, 45, 45));

            context.DrawText(
                window.Title,
                x + 8,
                y + 6,
                Color.White);

            // -----------------------------------------------------------------------------------------
            // PAGE CONTENT REGION (STATIC HEIGHT FIX)
            // -----------------------------------------------------------------------------------------

            int contentY = y + HUDOverlayWindow.TitleBarHeight;     // FIXED CS0176
            int contentHeight = h - HUDOverlayWindow.TitleBarHeight; // FIXED CS0176

            context.FillRectangle(
                new Rectangle(x, contentY, w, contentHeight),
                Color.FromArgb(255, 25, 25, 25));

            // -----------------------------------------------------------------------------------------
            // PAGE CONTENT DRAW
            // -----------------------------------------------------------------------------------------

            foreach (var line in window.GetPageLines())
            {
                context.DrawText(
                    (string)line,
                    x + 12,
                    contentY + 12,
                    Color.White);

                contentY += 18;
            }

            // -----------------------------------------------------------------------------------------
            // CHILD ELEMENTS
            // -----------------------------------------------------------------------------------------

            // Draw child elements

            foreach (var child in window.Children)
            {
                if (child is IDrawable drawable)
                {
                    drawable.Draw(context);
                }
            }
        }
    }

    internal interface IDrawable
    {
        void Draw(D3D11Adapter_Core context);
    }
}
