// ====================================================================================================
//  FILE: UIModButton.cs
//  PATH: ./Engine/UI/Widgets/
//  MODULE: UI Widgets (Hybrid)
//
//  ROLE:
//      Hybrid button type that extends the existing Widgets UI system while exposing deterministic
//      metadata compatible with the new Elements UI system. This allows mod menus, extended menus,
//      and special UI controls to coexist with both architectures.
//
//  AUTHORSHIP:
//      Created by the duo developers of Copilot and Brian Campbell respectively
//      Updated or changed by Copilot on 2026‑07‑27
//
// ====================================================================================================

using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.Elements
{
    internal class UIModButton : UIWidgetBase
    {
        private string Label;
        internal string ButtonId;
        internal bool ButtonVisible;

        // Unique identifier for deterministic systems
        public string ModButtonId { get; set; } = string.Empty;

        // Deterministic text field (Widgets uses Label)
        public string Text
        {
            get => Label;
            set => Label = value ?? string.Empty;
        }

        // Deterministic visibility flag
        public bool IsModVisible { get; set; } = true;

        // Deterministic geometry
        public PointF Position { get; set; }
        public SizeF Size { get; set; } = new SizeF(240f, 60f);

        // Deterministic style object
        public UIStyleElement Style { get; set; } = new UIStyleElement();

        // Deterministic click delegate
        public Action? OnClick { get; set; }

        public UIModButton() =>
            // Sync Widgets label with deterministic text
            Label = Text;

        // Fixed: Changed from public to protected to match UIElementBase
        protected override void OnMouseRelease()
        {
            base.OnMouseRelease();

            if (!IsDisabled && IsPressed)
            {
                OnClick?.Invoke();
            }
        }

        public void Render()
        {
            if (!IsModVisible)
                return;

            base.Render();

            // Optional deterministic styling hook
            // Example: log style usage or apply custom background
        }

        // Fixed: Removed the illegal self-conversion operator (CS0555) 
        // that was defined here.
    }


    // Deterministic style object for hybrid buttons
    internal class UIStyleElement
    {
        public float BorderThickness { get; set; } = 2f;
        public Color BorderColor { get; set; } = Color.Black;

        public Color BackgroundColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color HoverColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color PressedColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ActiveColor { get; set; } = Color.FromArgb(100, 100, 100);

        public PointF ShadowOffset { get; set; } = new PointF(3, 3);
        public Color ShadowColor { get; set; } = Color.FromArgb(20, 20, 20);
    }
}
