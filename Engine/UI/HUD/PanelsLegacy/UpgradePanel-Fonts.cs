// =====================================================================================================
//  FILE: UpgradePanel-Fonts.cs
//  PATH: Engine/UI/HUD/PanelsLegacy/UpgradePanel-Fonts.cs
//  SUBSYSTEM: UI / HUD (Legacy Panel Partials)
//
//  ROLE:
//      Provides deterministic font loading and font assignment logic for UpgradePanel.
//      This partial isolates all font‑related behavior from rendering, layout,
//      transitions, and upgrade selection/purchase logic.
// =====================================================================================================

using SASZombieAssaultTD.Engine.TextRendering;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    public partial class UpgradePanel
    {
        // ---------------------------------------------------------------------------------------------
        // Font Loading Implementation
        // ---------------------------------------------------------------------------------------------
        private void LoadFonts()
        {
            var titleMeta = FontCache.GetFont("title");
            _titleFont = new System.Drawing.Font(titleMeta.Name ?? "Arial", titleMeta.Size > 0 ? titleMeta.Size : 14);

            var textMeta = FontCache.GetFont("default");
            _textFont = new System.Drawing.Font(textMeta.Name ?? "Arial", textMeta.Size > 0 ? textMeta.Size : 12);

            var smallMeta = FontCache.GetFont("small");
            _smallFont = new System.Drawing.Font(smallMeta.Name ?? "Arial", smallMeta.Size > 0 ? smallMeta.Size : 10);
        }

        // ---------------------------------------------------------------------------------------------
        // Constructor Font Assignment (Engine Font → System.Drawing.Font Implementation)
        // ---------------------------------------------------------------------------------------------
        private void AssignEngineFonts(
            Font titleFont,
            Font textFont,
            Font smallFont)
        {
            _titleFont = new System.Drawing.Font(titleFont.Name ?? "Arial", titleFont.Size > 0 ? titleFont.Size : 14);
            _textFont = new System.Drawing.Font(textFont.Name ?? "Arial", textFont.Size > 0 ? textFont.Size : 12);
            _smallFont = new System.Drawing.Font(smallFont.Name ?? "Arial", smallFont.Size > 0 ? smallFont.Size : 10);
        }

        // ---------------------------------------------------------------------------------------------
        // Constructor Font Assignment (System.Drawing.Font passthrough Implementation)
        // ---------------------------------------------------------------------------------------------
        private void AssignDrawingFonts(System.Drawing.Font titleFont, System.Drawing.Font textFont, System.Drawing.Font smallFont)
        {
            _titleFont = titleFont;
            _textFont = textFont;
            _smallFont = smallFont;
        }
    }
}
