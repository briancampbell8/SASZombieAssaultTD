// =====================================================================================================
//  FILE: CashDisplaySound.cs
//  PATH: Engine/UI/HUD/CashDisplay/CashDisplaySound.cs
//  PROGRAM: HUD Cash Display Sound
//
//  ROLE:
//      Implements deterministic sound‑trigger logic for the modern Cash Display HUD module.
//      This program decides which sound to play when the player's cash value changes and exposes
//      a clean, isolated API for ModernCashDisplay to call.
//
//  RESPONSIBILITIES:
//      - Maintain configured sound identifiers for cash increase and cash decrease events.
//      - Evaluate cash delta and select the correct sound to trigger.
//      - Expose deterministic sound‑selection API consumed by ModernCashDisplay.
//      - Provide instant sound routing for non‑animated updates.
//
//  NON-RESPONSIBILITIES:
//      - Playing audio directly (handled by the engine's audio system).
//      - Rendering or widget manipulation (handled by ModernCashDisplay).
//      - Color threshold evaluation (handled by CashDisplayColor).
//      - Text formatting (handled by CashDisplayFormatting).
//      - Animation or pulse effects (handled by CashDisplayAnimation).
//      - Layout or positioning (handled by CashDisplayLayout).
//
//  ARCHITECTURAL NOTES:
//      - This is a NEW PROGRAM module: fully isolated, no legacy dependencies.
//      - ModernCashDisplay delegates all sound‑routing behavior to this program.
//      - Ensures deterministic, consistent sound selection across the HUD pipeline.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUD.CashDisplay
{
    internal class CashDisplaySound
    {
        private readonly string _soundIncrease;
        private readonly string _soundDecrease;

        public CashDisplaySound(string soundIncrease, string soundDecrease)
        {
            _soundIncrease = soundIncrease;
            _soundDecrease = soundDecrease;
        }

        // Determine which sound should play based on cash delta
        public string? SelectSound(int previousCash, int currentCash)
        {
            int delta = currentCash - previousCash;

            if (delta > 0)
                return _soundIncrease;

            if (delta < 0)
                return _soundDecrease;

            return null; // No change → no sound
        }

        // Force-select sound instantly (no animation context)
        public string? SelectInstant(int previousCash, int currentCash)
        {
            return SelectSound(previousCash, currentCash);
        }
    }
}
