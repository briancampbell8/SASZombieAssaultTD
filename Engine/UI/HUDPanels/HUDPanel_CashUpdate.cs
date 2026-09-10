// =====================================================================================================
//  FILE: HUDPanel_CashUpdate.cs
//  PATH: Engine/UI/HUDPanels/HUDPanel_CashUpdate.cs
//  MODULE: CASH HUD Panel (Runtime Update + Rendering)
//  LAYER: UI → HUDPanels
//
//  ROLE:
//      Runtime renderer and per‑frame update module for the CASH HUD panel. This class consumes fully
//      resolved configuration data produced by the Finalizer pipeline (Manager → Resolver → UIStateBuilder)
//      and renders the CASH panel deterministically using the engine's drawing context.
//
//  RESPONSIBILITIES:
//      - Render background fill, text, and item price color using either default or manual override colors.
//      - Provide deterministic Update() behavior for future animations or flash effects.
//      - Expose geometry and resolved colors as internal‑set properties populated by the Finalizer Manager.
//
//  NON‑RESPONSIBILITIES:
//      - Manual override parsing (handled by Control + ColorParser).
//      - Geometry/crosshair resolution (handled by Resolver).
//      - UIState construction (handled by UIStateBuilder).
//      - Finalizer pipeline sequencing (handled by HUDPanelFinalizer_Manager).
//
//  ARCHITECTURAL NOTES:
//      - Refactored to derive cleanly from UIElement to support the engine's unified rendering list order.
//      - This module is intentionally lightweight. All configuration logic occurs upstream in the Finalizer
//        pipeline. HUDPanel_CashUpdate performs no validation or normalization; it renders exactly what the
//        Manager provides.
// =====================================================================================================

using System.Drawing;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI.HUDPanels
{
    public class HUDPanel_CashUpdate : UIElement
    {
        private int Layer;

        // ---------------------------------------------------------------------------------------------
        // Runtime State (populated by Finalizer Manager)
        // ---------------------------------------------------------------------------------------------

        public Color ManualFillColor { get; internal set; }
        public Color ManualTextColor { get; internal set; }
        public Color ManualItemPriceColor { get; internal set; }

        public bool UseManualColors { get; internal set; }
        public string DisplayText { get; internal set; } = "0";

        // Geometry Bounds Mapping (Synchronized seamlessly with the parent UIElement structures)
        public int X
        {
            get => (int)Position.X;
            set => Position = new Components.PointF(value, Position.Y);
        }

        public int Y
        {
            get => (int)Position.Y;
            set => Position = new Components.PointF(Position.X, value);
        }

        public int Width
        {
            get => (int)Size.Width;
            set => Size = new SizeF(value, Size.Height);
        }

        public int Height
        {
            get => (int)Size.Height;
            set => Size = new SizeF(Size.Width, value);
        }

        // ---------------------------------------------------------------------------------------------
        // CONSTRUCTOR
        // ---------------------------------------------------------------------------------------------
        public HUDPanel_CashUpdate()
        {
            // Set base UI properties inherited from the modern unified layout class
            Id = "cash_panel";
            Layer = 999; // Always drawn above base HUD surface
        }

        // ---------------------------------------------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------------------------------------------

        public override void Update(float deltaTime)
        {
            // Reserved for future flash effects or transitions.
            base.Update(deltaTime);
        }

        // ---------------------------------------------------------------------------------------------
        // DRAW
        // ---------------------------------------------------------------------------------------------

        public void Draw(D3D11Adapter_Core adapter_Core)
        {
            if (adapter_Core == null || !IsVisible) return;

            // Compute absolute coordinates inside the hierarchy node tree map graph
            var absPos = AbsolutePosition;
            int finalX = (int)absPos.X;
            int finalY = (int)absPos.Y;

            // Background fill
            var fillColor = UseManualColors
                ? ManualFillColor
                : new Color(
                    (byte)(0.15f * 255),
                    (byte)(0.15f * 255),
                    (byte)(0.15f * 255),
                    (byte)(0.85f * 255)); // Default background

            adapter_Core.FillRectangle(
                new Rectangle(finalX, finalY, Width, Height),
                fillColor
            );

            // Text color (CASH amount)
            var textColor = UseManualColors
                ? ManualTextColor
                : new Color(
                    (byte)(1f * 255),
                    (byte)(1f * 255),
                    (byte)(1f * 255),
                    (byte)(1f * 255)); // Default white

            adapter_Core.DrawText(
                DisplayText,
                finalX + 8,
                finalY + 6,
                textColor
            );

            // Item price color
            var priceColor = UseManualColors
                ? ManualItemPriceColor
                : new Color(
                    (byte)(0f * 255),
                    (byte)(0.75f * 255),
                    (byte)(1f * 255),
                    (byte)(1f * 255)); // Default #00BFFF

            // Placeholder example (replace with actual store logic)
            adapter_Core.DrawText(
                "$100",
                finalX + 8,
                finalY + Height - 18,
                priceColor
            );

            // Standard cascading loop propagation pass to safely process child elements if attached
            base.Draw(adapter_Core);
        }
    }
}
