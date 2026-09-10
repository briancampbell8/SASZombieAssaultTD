// =====================================================================================================
//  FILE: MainMenuUI.cs
//  PATH: Engine/UI/MainMenu/
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Deterministic controller for the Main Menu UI subsystem. Owns the UIRoot, constructs all
//      UIElement instances (buttons, labels, panels), applies transitions, and exposes a fully
//      assembled UI tree for rendering.
//
//  RESPONSIBILITIES:
//      - Initialize UIRoot using Option‑B deterministic rules.
//      - Construct Main Menu UI elements (Play, Options, Exit).
//      - Attach all elements to UIRoot.
//      - Apply fade‑in transitions on scene entry.
//      - Provide deterministic hooks for button actions.
//      - Expose UIRoot for rendering by UIRenderer.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (handled by UIRenderer).
//      - Scene switching (handled by SceneManager).
//      - Input dispatch (handled by UIEventSystem).
//
//  AUTHORSHIP:
//      Modernized by Copilot and Brian Campbell — 2026‑09‑10
// =====================================================================================================

using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class MainMenuUI
    {
        public UIRoot Root { get; private set; }

        public UIButton PlayButton { get; set; }
        public UIButton OptionsButton { get; set; }
        public UIButton ExitButton { get; set; }
        // Title and background elements exposed for layout rules
        public UILabel TitleLabel { get; set; }
        public UIPanel BackgroundPanel { get; set; }


        public MainM_ButtonBuilder ButtonBuilder { get; } = new MainM_ButtonBuilder();



        private readonly MainM_Transitions _transitions = new MainM_Transitions();

        public MainMenuUI()
        {
            // Initialize root
            Root = new UIRoot();
            Root.Initialize();
            Root.BackgroundColor = Color.Transparent;

            // Create buttons
            PlayButton = new UIButton
            {
                ButtonId = "Play",
                Text = "PLAY",
                Position = new System.Drawing.PointF(200, 150),
                Size = new SizeF(300, 80)
            };

            OptionsButton = new UIButton
            {
                ButtonId = "Options",
                Text = "OPTIONS",
                Position = new System.Drawing.PointF(200, 260),
                Size = new SizeF(300, 80)
            };

            ExitButton = new UIButton
            {
                ButtonId = "Exit",
                Text = "EXIT",
                Position = new System.Drawing.PointF(200, 370),
                Size = new SizeF(300, 80)
            };

            // Attach elements to root
            Root.AddElement(PlayButton);
            Root.AddElement(OptionsButton);
            Root.AddElement(ExitButton);

            // Apply fade‑in transition
            ApplySceneEntryTransition();
        }

        private void ApplySceneEntryTransition()
        {
            // Build a temporary UIState for transitions
            var state = new UIState
            {
                Elements = new List<UIElement> { PlayButton, OptionsButton, ExitButton },
                StateId = "MainMenu",
                IsVisible = true
            };

            _transitions.FadeIn(state, duration: 0.45f);
        }

        // Deterministic button action hooks
        public void OnPlay() { }
        public void OnOptions() { }
        public void OnExit() { }
    }
}
