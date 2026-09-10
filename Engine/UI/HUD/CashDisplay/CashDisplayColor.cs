// =====================================================================================================
//  FILE: CashDisplayColor.cs
//  PATH: Engine/UI/HUD/CashDisplay/CashDisplayColor.cs
//  PROGRAM: HUD Cash Display Color Program
//
//  ROLE:
//      Implements deterministic color‑state evaluation for the modern Cash Display HUD module.
//      This program selects Normal, Warning, or Danger colors based solely on cash thresholds,
//      providing a clean separation between UI logic and color‑evaluation logic.
//
//  RESPONSIBILITIES:
//      - Maintain configured color values for Normal, Warning, and Danger states.
//      - Evaluate the correct color based on the current cash amount.
//      - Expose a deterministic color selection API consumed by ModernCashDisplay.
//      - Provide threshold configuration for warning and danger levels.
//
//  NON-RESPONSIBILITIES:
//      - Rendering or widget manipulation (handled by ModernCashDisplay).
//      - Animation or pulse effects (handled by CashDisplayAnimation).
//      - Text formatting (handled by CashDisplayFormatter).
//      - Sound routing (handled by CashDisplaySound).
//      - Layout or positioning (handled by CashDisplayLayout).
//
//  ARCHITECTURAL NOTES:
//      - This is a NEW PROGRAM module: fully isolated, no legacy dependencies.
//      - ModernCashDisplay delegates all color‑selection behavior to this program.
//      - Ensures deterministic, threshold‑based color evaluation across the HUD pipeline.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUD.CashDisplay
{
    internal class CashDisplayColor
    {
        // Thresholds
        private readonly int _warningThreshold;
        private readonly int _dangerThreshold;

        // Colors
        public Color NormalColor { get; }
        public Color WarningColor { get; }
        public Color DangerColor { get; }

        // Current evaluated color
        public Color CurrentColor { get; private set; }

        public CashDisplayColor(
            int warningThreshold,
            int dangerThreshold,
            Color normalColor,
            Color warningColor,
            Color dangerColor)
        {
            _warningThreshold = warningThreshold;
            _dangerThreshold = dangerThreshold;

            NormalColor = normalColor;
            WarningColor = warningColor;
            DangerColor = dangerColor;

            CurrentColor = normalColor;
        }

        // Evaluate color based on current cash
        public void Update(int cash)
        {
            if (cash <= _dangerThreshold)
            {
                CurrentColor = DangerColor;
            }
            else if (cash <= _warningThreshold)
            {
                CurrentColor = WarningColor;
            }
            else
            {
                CurrentColor = NormalColor;
            }
        }

        // Force-set color instantly
        public void SetInstant(Color color)
        {
            CurrentColor = color;
        }
    }
}
