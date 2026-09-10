// =====================================================================================================
//  FILE: CashDisplayLayout.cs
//  PATH: Engine/UI/HUD/CashDisplay/CashDisplayLayout.cs
//  PROGRAM: HUD Cash Display Layout
//
//  ROLE:
//      Implements deterministic layout logic for the modern Cash Display HUD module. This program
//      computes and exposes widget position, size, and alignment values used by ModernCashDisplay
//      when constructing and updating its UI widgets.
//
//  RESPONSIBILITIES:
//      - Maintain configured position and size values for the Cash Display panel and text widget.
//      - Provide deterministic layout values consumed by ModernCashDisplay.
//      - Support margin/padding configuration for future HUD expansion.
//      - Expose instant layout updates for non‑animated changes.
//
//  NON-RESPONSIBILITIES:
//      - Rendering or widget manipulation (handled by ModernCashDisplay).
//      - Color threshold evaluation (handled by CashDisplayColor).
//      - Text formatting (handled by CashDisplayFormatting).
//      - Animation or pulse effects (handled by CashDisplayAnimation).
//      - Sound routing or audio playback (handled by CashDisplaySound).
//
//  ARCHITECTURAL NOTES:
//      - This is a NEW PROGRAM module: fully isolated, no legacy dependencies.
//      - ModernCashDisplay delegates all layout behavior to this program.
//      - Ensures deterministic, consistent layout values across the HUD pipeline.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUD.CashDisplay
{
    internal class CashDisplayLayout
    {
        // Panel layout
        public int PanelX { get; private set; }
        public int PanelY { get; private set; }
        public int PanelWidth { get; private set; }
        public int PanelHeight { get; private set; }

        // Text layout
        public int TextX { get; private set; }
        public int TextY { get; private set; }

        // Optional margins
        public int MarginLeft { get; private set; }
        public int MarginTop { get; private set; }

        public CashDisplayLayout(
            int panelX,
            int panelY,
            int panelWidth,
            int panelHeight,
            int textX,
            int textY,
            int marginLeft = 0,
            int marginTop = 0)
        {
            PanelX = panelX;
            PanelY = panelY;
            PanelWidth = panelWidth;
            PanelHeight = panelHeight;

            TextX = textX;
            TextY = textY;

            MarginLeft = marginLeft;
            MarginTop = marginTop;
        }

        // Update panel layout
        public void SetPanel(int x, int y, int width, int height)
        {
            PanelX = x;
            PanelY = y;
            PanelWidth = width;
            PanelHeight = height;
        }

        // Update text layout
        public void SetText(int x, int y)
        {
            TextX = x;
            TextY = y;
        }

        // Update margins
        public void SetMargins(int left, int top)
        {
            MarginLeft = left;
            MarginTop = top;
        }
    }
}
