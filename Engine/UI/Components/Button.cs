// =====================================================================================================
//  FILE: Button.cs
//  PATH: Engine/UI/Components/Button.cs
//  SUBSYSTEM: UI Framework — Interactive Element Component
//
//  ROLE:
//      Deterministic, lightweight UI button element rendered through the modern RenderSystem pipeline.
//      Provides position, size, text, visibility, and click‑action behavior.
//
//  RESPONSIBILITIES:
//      - Maintain button state (position, size, visibility, text).
//      - Provide deterministic Update() and Render() behavior.
//      - Forward all rendering through RenderSystem → IDrawingContext.
//      - Expose an OnClick action for UI interaction systems.
//
//  NON‑RESPONSIBILITIES:
//      - Handling input detection (delegated to UI/Input subsystem).
//      - Managing GPU resources or issuing GPU commands directly.
//      - Performing layout, anchoring, or container logic.
//      - Managing UI hierarchy or event bubbling.
// =====================================================================================================

using System;
using System.Drawing;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI
{
    public class Button : UIElementBase
    {
        private RenderSystem? _renderSystem;

        public string ButtonId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public Action? OnClick { get; set; }

        public Vector2 Size { get; set; }
        public Vector2 PositionVector { get; set; }

        public Button(string buttonId, bool visible, string text, Action onClick, Vector2 position, Vector2 size)
        {
            ButtonId = buttonId;
            IsVisible = visible;
            Text = text;
            OnClick = onClick;
            PositionVector = position;
            Size = size;
        }

        // Bind RenderSystem from MainMenuScene
        public void SetRenderSystem(RenderSystem system)
        {
            _renderSystem = system;
        }

        public override void Update(float deltaTime)
        {
            if (float.IsNaN(PositionVector.X) || float.IsNaN(PositionVector.Y))
                return;

            Position = new Point(
                (int)System.Math.Round(PositionVector.X),
                (int)System.Math.Round(PositionVector.Y));
        }

        public override void Render(SASZombieAssaultTD.Engine.Render.D3D11.Adapter.D3D11Adapter_Core adapter, float deltaTime)
        {
            if (!IsVisible || _renderSystem == null)
                return;

            var bounds = new System.Drawing.Rectangle(
                (int)PositionVector.X,
                (int)PositionVector.Y,
                (int)Size.X,
                (int)Size.Y);

            // Draw button background
            _renderSystem.DrawRectangle(bounds, ColorRGBA.White);

            // Measure text
            Vector2 textSize;
            try { textSize = _renderSystem.MeasureText(Text ?? string.Empty); }
            catch { textSize = Vector2.Zero; }

            var textPos = new Vector2(
                PositionVector.X + (Size.X - textSize.X) * 0.5f,
                PositionVector.Y + (Size.Y - textSize.Y) * 0.5f);

            // Draw text
            try { _renderSystem.DrawText(Text ?? string.Empty, textPos, ColorRGBA.Black); }
            catch { }
        }
    }
}
