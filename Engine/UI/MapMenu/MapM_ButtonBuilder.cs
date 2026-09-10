// ====================================================================================================
//  FILE: MapM_ButtonBuilder.cs
//  PATH: ./Engine/UI/MapMenu/
//  MODULE: UI.MapMenu
//
//  ROLE:
//      Deterministic construction of Map Menu button elements, including geometry, interaction
//      behavior, and render‑ready state packaging for the UI pipeline.
//
//  RESPONSIBILITIES:
//      - Build raised button geometry for Map Menu UI.
//      - Apply deterministic layout rules defined by MapM_LayoutRules.
//      - Package button state for UIManager and UIRenderer.
//      - Provide hover, press, and active state transitions.
//      - Ensure all button builds follow Option‑B deterministic architecture.
//
//  NON‑RESPONSIBILITIES:
//      - Scene transitions (handled by MapMenuUI).
//      - Texture loading (handled by UIAssetLoader).
//      - Runtime input dispatch (handled by UIEventSystem).
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.UI.Components;
using SASZombieAssaultTD.Engine.UI.MainMenu;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.MapMenu
{
    internal class MapM_ButtonBuilder
    {
        // Core deterministic build entry point
        public UIButton Build(string id, string text, Action? onClick)
        {
            // This gets the layout using standard System.Drawing structures
            var layout = MapM_LayoutRules.GetButtonLayout(id);

            var button = new UIButton
            {
                ButtonId = id,
                Text = text,
                OnClick = onClick,
                ButtonVisible = true,
                // Explicitly map System.Drawing.SizeF to your custom Engine.UI.Components.SizeF
                Size = new System.Drawing.SizeF(layout.Size.Width, layout.Size.Height),
                // Explicitly map System.Drawing.PointF to your custom Engine.UI.Components.PointF
                Position = new PointF(layout.Position.X, layout.Position.Y),
                Style = ResolveStyle(id)
            };

            ApplyRaisedGeometry(button);
            ApplyInteractionStates(button);

            return button;
        }

        // Raised geometry for Map Menu buttons
        private void ApplyRaisedGeometry(UIButton button)
        {
            button.Style.BorderThickness = 3;
            button.Style.BorderColor = UIStyleSheet.MapMenu.BorderColor;
            button.Style.BackgroundColor = UIStyleSheet.MapMenu.ButtonBackground;
            button.Style.ShadowOffset = 3f;
            button.Style.ShadowColor = UIStyleSheet.MapMenu.ShadowColor;
        }

        // Hover / Press / Active states
        private void ApplyInteractionStates(UIButton button)
        {
            button.Style.HoverColor = UIStyleSheet.MapMenu.ButtonHover;
            button.Style.PressedColor = UIStyleSheet.MapMenu.ButtonPressed;
            button.Style.ActiveColor = UIStyleSheet.MapMenu.ButtonActive;
        }

        // Style resolver for Map Menu buttons
        private UIStyle ResolveStyle(string id)
        {
            return UIStyleResolver.Resolve("MapMenu.Button." + id);
        }
    }
}
