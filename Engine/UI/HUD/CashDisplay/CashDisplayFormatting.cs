// =====================================================================================================
//  FILE: CashDisplayFormatting.cs
//  PATH: Engine/UI/HUD/CashDisplay/CashDisplayFormatting.cs
//  PROGRAM: HUD Cash Display Formatting
//
//  ROLE:
//      Implements deterministic text‑formatting logic for the modern Cash Display HUD module.
//      This program assembles the final display string (including prefix and numeric formatting)
//      used by the HUD text widget.
//
//  RESPONSIBILITIES:
//      - Maintain prefix configuration (e.g., "$").
//      - Maintain numeric formatting configuration (e.g., "N0").
//      - Format raw integer cash values into final display strings.
//      - Expose a deterministic formatting API consumed by ModernCashDisplay.
//      - Provide instant formatting for non‑animated updates.
//
//  NON-RESPONSIBILITIES:
//      - Rendering or widget manipulation (handled by ModernCashDisplay).
//      - Color threshold evaluation (handled by CashDisplayColor).
//      - Animation or pulse effects (handled by CashDisplayAnimation).
//      - Sound routing or audio playback (handled by CashDisplaySound).
//      - Layout or positioning (handled by CashDisplayLayout).
//
//  ARCHITECTURAL NOTES:
//      - This is a NEW PROGRAM module: fully isolated, no legacy dependencies.
//      - ModernCashDisplay delegates all formatting behavior to this program.
//      - Ensures deterministic, consistent formatting across the HUD pipeline.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUD.CashDisplay
{
    internal class CashDisplayFormatting
    {
        private readonly string _prefix;
        private readonly string _numberFormat;

        public CashDisplayFormatting(string prefix = "$", string numberFormat = "N0")
        {
            _prefix = prefix;
            _numberFormat = numberFormat;
        }

        // Format the cash value into a final display string
        public string Format(int cash)
        {
            return _prefix + cash.ToString(_numberFormat);
        }

        // Force-format without animation context
        public string FormatInstant(int cash)
        {
            return _prefix + cash.ToString(_numberFormat);
        }
    }
}
