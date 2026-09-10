// =====================================================================================================
//  FILE: MainM_Transitions.cs
//  PATH: Engine/UI/MainMenu/MainM_Transitions.cs
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Provides deterministic fade‑in / fade‑out transitions and button activation states for the
//      Main Menu UI subsystem. Replaces the legacy UIState → RegisterTransition pipeline with direct
//      UIElement property animation compatible with UIRoot.
//
//  RESPONSIBILITIES:
//      - Apply fade‑in transitions to all Main Menu UIElements.
//      - Apply fade‑out transitions when leaving the Main Menu.
//      - Provide deterministic button activation / deactivation behavior.
//      - Guarantee stable, reproducible animation curves across all hardware.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (MainMenuRenderer).
//      - Scene transitions (SceneManager).
//      - Input dispatch (UIEventSystem).
//
//  AUTHORSHIP:
//      Modernized by Copilot and Brian Campbell — 2026‑09‑10
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class MainM_Transitions
    {
        /// <summary>
        /// Fade all UIElements from opacity 0 → 1 over the given duration.
        /// </summary>
        public void FadeIn(IEnumerable<UIElement> elements, float duration)
        {
            foreach (var element in elements)
            {
                element.Opacity = 0f;
                element.AnimateOpacityTo(1f, duration);
            }
        }

        /// <summary>
        /// Fade all UIElements from opacity 1 → 0 over the given duration.
        /// </summary>
        public void FadeOut(IEnumerable<UIElement> elements, float duration)
        {
            foreach (var element in elements)
            {
                element.AnimateOpacityTo(0f, duration);
            }
        }

        /// <summary>
        /// Deterministic button activation (hover/press/active).
        /// </summary>
        public void ActivateButton(UIButton button)
        {
            var style = button.Style;

            style.ActiveColor = style.PressedColor;
            style.ShadowOffset = 1f;
        }

        /// <summary>
        /// Deterministic button deactivation (return to idle).
        /// </summary>
        public void DeactivateButton(UIButton button)
        {
            var style = button.Style;

            style.ActiveColor = style.BackgroundColor;
            style.ShadowOffset = 3f;
        }
    }
}
