// =====================================================================================================
//  FILE: MainM_Finalizer.cs
//  PATH: Engine/UI/MainMenu/MainM_Finalizer.cs
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Finalizes the Main Menu UI by attaching all constructed UIElements (buttons, title, background)
//      to the UIRoot. Replaces the legacy UIState → PanelBuilder pipeline with a deterministic,
//      UIRoot‑based UI tree.
//
//  RESPONSIBILITIES:
//      - Attach Play, Options, and Exit buttons to the UIRoot.
//      - Construct and attach the Main Menu title label.
//      - Construct and attach the background panel.
//      - Ensure layout invalidation is triggered for deterministic Option‑B layout rules.
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
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class MainM_Finalizer
    {
        private readonly MainM_ButtonBuilder _buttonBuilder = new MainM_ButtonBuilder();

        /// <summary>
        /// Finalizes the Main Menu UI by attaching all elements to the UIRoot.
        /// </summary>
        public void Finalize(MainMenuUI ui)
        {
            // Build buttons
            var playButton = _buttonBuilder.Build("Play", "PLAY", ui.OnPlay);
            var optionsButton = _buttonBuilder.Build("Options", "OPTIONS", ui.OnOptions);
            var exitButton = _buttonBuilder.Build("Exit", "EXIT", ui.OnExit);

            // Attach buttons to root
            ui.Root.AddElement(playButton);
            ui.Root.AddElement(optionsButton);
            ui.Root.AddElement(exitButton);

            // Attach title
            var title = BuildTitle();
            ui.Root.AddElement(title);

            // Attach background
            var background = BuildBackground();
            ui.Root.AddElement(background);

            // Expose elements to MainMenuUI for layout rules
            ui.PlayButton = playButton;
            ui.OptionsButton = optionsButton;
            ui.ExitButton = exitButton;
            ui.TitleLabel = title;
            ui.BackgroundPanel = background;
        }

        // ---------------------------------------------------------------------------------------------
        // Title
        // ---------------------------------------------------------------------------------------------

        private UILabel BuildTitle()
        {
            return new UILabel
            {
                Text = "SAS Zombie Assault TD",
                Position = new System.Drawing.PointF(0, 0),   // LayoutRules will reposition
                Size = new SizeF(600, 80),
                LabelVisible = true,
                Style = UIStyleResolver.Resolve("MainMenu.Title")
            };
        }

        // ---------------------------------------------------------------------------------------------
        // Background
        // ---------------------------------------------------------------------------------------------

        private UIPanel BuildBackground()
        {
            return new UIPanel
            {
                PanelId = "MainMenuBackground",
                Position = new System.Drawing.PointF(0, 0),
                Size = new SizeF(1920, 1080),   // LayoutRules will resize
                PanelVisible = true,
                Style = UIStyleResolver.Resolve("MainMenu.Background")
            };
        }
    }
}
