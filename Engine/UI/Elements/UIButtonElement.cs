// =====================================================================================================
//  FILE: UIButtonElement.cs
//  PATH: Engine/UI/Elements/UIButtonElement.cs
//  SUBSYSTEM: UI Framework — Interactive Button Element
//
//  ROLE:
//      Deterministic interactive button element used by MainMenu, MapMenu, and other UI subsystems.
//      Provides geometry, style metadata, and interaction hooks for the modern UI.Elements pipeline.
//      Serves as the foundational interactive component for menu construction and UI builders.
//
//  RESPONSIBILITIES:
//      - Store deterministic geometry (Position, Size) for layout builders.
//      - Expose ButtonId and Text for UI routing and display.
//      - Maintain ButtonVisible and Opacity for visual state control.
//      - Provide OnClick delegate for interaction binding.
//      - Carry UIStyle for consistent theming across UI.Elements.
//
//  NON‑RESPONSIBILITIES:
//      - Performing rendering (handled by the modern RenderSystem pipeline).
//      - Layout resolution (handled by MainM_ButtonBuilder and layout rules).
//      - Asset loading (handled by TextureManager / UIAssetLoader).
//      - Scene transitions or global UI state management.
//
//  ARCHITECTURAL NOTES:
//      - UIButtonElement is part of the modern UI.Elements subsystem (not legacy UIElementBase).
//      - Used heavily by MainM_ButtonBuilder for deterministic menu construction.
//      - Rendering is delegated to higher‑level UI renderers and the RenderSystem.
//      - Safe and stable component required for MainMenuScene modernization.
// =====================================================================================================

using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.UI.Styles;

namespace SASZombieAssaultTD.Engine.UI.Elements
{
    internal class UIButtonElement : UIElement
    {
        // -------------------------------------------------------------------------------------------------
        // IDENTIFIERS & INTERACTION
        // -------------------------------------------------------------------------------------------------

        public string ButtonId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool ButtonVisible { get; set; } = true;
        public Action? OnClick { get; set; }

        // -------------------------------------------------------------------------------------------------
        // GEOMETRY & STYLE
        // -------------------------------------------------------------------------------------------------

        public Components.PointF Position { get; set; }
        public SizeF Size { get; set; }
        public UIStyle Style { get; set; }
        public float Opacity { get; set; } = 1f;

        // -------------------------------------------------------------------------------------------------
        // CONSTRUCTION — Deterministic Defaults
        // -------------------------------------------------------------------------------------------------

        public UIButtonElement()
        {
            Style = new UIStyle();
            Position = new Components.PointF(0f, 0f);
            Size = new SizeF(240f, 60f);

            // Reset style to default palette
            Style.Reset();

            // Default background color (transparent black)
            Style.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
            Style.ShadowColor = Color.FromArgb(0, 0, 0, 0);
        }

        // -------------------------------------------------------------------------------------------------
        // INTERACTION
        // -------------------------------------------------------------------------------------------------

        public void Click() => OnClick?.Invoke();
    }
}
