// =====================================================================================================
//  FILE: MainMenuLayoutRules.cs
//  PATH: Engine/UI/MainMenu/MainMenuLayoutRules.cs
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Provides deterministic layout rules for the Main Menu UI subsystem. Ensures consistent spacing,
//      alignment, and visual hierarchy across all screen resolutions.
//
//  RESPONSIBILITIES:
//      - Compute button positions based on viewport size.
//      - Maintain consistent vertical spacing between menu items.
//      - Support Option‑B deterministic layout behavior.
//      - Provide a single authoritative layout pass for MainMenuUI.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (delegated to MainMenuRenderer).
//      - Input dispatch (UIEventSystem).
//      - Scene transitions (SceneManager).
//
//  ARCHITECTURAL NOTES:
//      - Works directly with MainMenuUI and UIRoot.
//      - Called once per layout invalidation or viewport resize.
//      - Ensures all UIElements remain resolution‑independent.
//      - Matches the deterministic layout pipeline used across all UI subsystems.
//
//  AUTHORSHIP:
//      Modernized by Copilot and Brian Campbell — 2026‑09‑10
// =====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public static class MainMenuLayoutRules
    {
        /// <summary>
        /// Applies deterministic layout rules to the Main Menu UI.
        /// </summary>
        public static void Apply(MainMenuUI ui, SizeF viewport)
        {
            float centerX = viewport.Width * 0.5f;

            // Vertical spacing between buttons
            const float spacing = 110f;

            // PLAY button
            ui.PlayButton.Position = new PointF(
                centerX - ui.PlayButton.Size.Width * 0.5f,
                viewport.Height * 0.35f
            );

            // OPTIONS button
            ui.OptionsButton.Position = new PointF(
                centerX - ui.OptionsButton.Size.Width * 0.5f,
                ui.PlayButton.Position.Y + spacing
            );

            // EXIT button
            ui.ExitButton.Position = new PointF(
                centerX - ui.ExitButton.Size.Width * 0.5f,
                ui.OptionsButton.Position.Y + spacing
            );
        }
    }
}
