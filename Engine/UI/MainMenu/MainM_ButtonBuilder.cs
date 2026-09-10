// =====================================================================================================
//  FILE: MainM_ButtonBuilder.cs
//  PATH: Engine/UI/MainMenu/MainM_ButtonBuilder.cs
//  SUBSYSTEM: UI.MainMenu
//
//  ROLE:
//      Deterministic factory for constructing Main Menu UIButton instances. Applies style, geometry,
//      interaction states, and layout‑ready properties consistent with the Option‑B UI pipeline.
//
//  RESPONSIBILITIES:
//      - Build fully‑styled UIButton instances for the Main Menu.
//      - Apply raised geometry, hover/press/active states, and MainMenu style sheet rules.
//      - Provide a clean construction surface for MainMenuUI and MainM_Finalizer.
//      - Ensure all buttons are ready for deterministic layout via MainMenuLayoutRules.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (MainMenuRenderer).
//      - Input dispatch (UIEventSystem).
//      - Scene transitions (SceneManager).
//
//  ARCHITECTURAL NOTES:
//      - Replaces the legacy UIButtonElement + UIState + PanelBuilder pipeline.
//      - Produces real UIButton objects compatible with UIRoot and UIElement.
//      - Matches the modernized Option‑B deterministic UI architecture.
// =====================================================================================================

using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MainMenu
{
    public sealed class MainM_ButtonBuilder
    {
        /// <summary>
        /// Core deterministic button construction entry point.
        /// </summary>
        public UIButton Build(string id, string text, Action? onClick)
        {
            var button = new UIButton
            {
                ButtonId = id,
                Text = text,
                OnClick = onClick,
                ButtonVisible = true,

                // Default size — layout rules will reposition
                Size = new SizeF(300f, 80f),
                Position = new PointF(0f, 0f),

                Style = ResolveStyle(id)
            };

            ApplyRaisedGeometry(button);
            ApplyInteractionStates(button);

            return button;
        }

        // ---------------------------------------------------------------------------------------------
        // Style + Geometry
        // ---------------------------------------------------------------------------------------------

        private void ApplyRaisedGeometry(UIButton button)
        {
            button.Style.BorderThickness = 3;
            button.Style.BorderColor = UIStyleSheet.MainMenu.BorderColor;
            button.Style.BackgroundColor = UIStyleSheet.MainMenu.ButtonBackground;
            button.Style.ShadowOffset = UIStyleSheet.MainMenu.ShadowOffset;
            button.Style.ShadowColor = UIStyleSheet.MainMenu.ShadowColor;
        }

        private void ApplyInteractionStates(UIButton button)
        {
            button.Style.HoverColor = UIStyleSheet.MainMenu.ButtonHover;
            button.Style.PressedColor = UIStyleSheet.MainMenu.ButtonPressed;
            button.Style.ActiveColor = UIStyleSheet.MainMenu.ButtonActive;
        }

        private UIStyle ResolveStyle(string id)
        {
            return UIStyleResolver.Resolve("MainMenu.Button." + id);
        }
    }
}
