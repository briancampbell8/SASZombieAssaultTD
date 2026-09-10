// =====================================================================================================
//  FILE: MainM_PanelBuilder.cs
//  PATH: Engine/UI/MainMenu/MainM_PanelBuilder.cs
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Provides deterministic layout assembly for the Main Menu UI. Replaces legacy panel construction
//      with direct UIRoot element positioning.
//
//  RESPONSIBILITIES:
//      - Compute positions for Play, Options, Exit buttons, title, and background.
//      - Apply spacing and alignment rules.
//      - Guarantee stable, reproducible layout across all hardware configurations.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (MainMenuRenderer).
//      - Input dispatch (UIEventSystem).
//      - Scene transitions (SceneManager).
//
//  AUTHORSHIP:
//      Modernized by Copilot and Brian Campbell — 2026‑09‑10
// =====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class MainM_PanelBuilder
    {
        /// <summary>
        /// Applies deterministic layout rules to all Main Menu elements.
        /// </summary>
        public void ApplyLayout(MainMenuUI ui, SizeF viewport)
        {
            float centerX = viewport.Width * 0.5f;
            const float spacing = 110f;

            // Background fills entire viewport
            ui.BackgroundPanel.Position = new PointF(0, 0);
            ui.BackgroundPanel.Size = viewport;

            // Title centered near top
            ui.TitleLabel.Position = new PointF(
                centerX - ui.TitleLabel.Size.Width * 0.5f,
                viewport.Height * 0.15f
            );

            // Buttons vertically stacked
            ui.PlayButton.Position = new PointF(
                centerX - ui.PlayButton.Size.Width * 0.5f,
                viewport.Height * 0.35f
            );

            ui.OptionsButton.Position = new PointF(
                centerX - ui.OptionsButton.Size.Width * 0.5f,
                ui.PlayButton.Position.Y + spacing
            );

            ui.ExitButton.Position = new PointF(
                centerX - ui.ExitButton.Size.Width * 0.5f,
                ui.OptionsButton.Position.Y + spacing
            );
        }
    }
}
