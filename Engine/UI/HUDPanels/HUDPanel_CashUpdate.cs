// ====================================================================================================
//  FILE: HUDPanel_CashUpdate.cs
//  PATH: Engine/UI/HUDPanels/
//  MODULE: CASH HUD Panel (Runtime Update + Rendering)
// ====================================================================================================

using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.UI.HUDPanels
{
    public class HUDPanel_CashUpdate : IHUDElement
    {
        // ---------------------------------------------------------------------------------------------
        // Runtime State (populated by Finalizer Manager)
        // ---------------------------------------------------------------------------------------------

        public Color ManualFillColor { get; internal set; }
        public Color ManualTextColor { get; internal set; }
        public Color ManualItemPriceColor { get; internal set; }   // ⭐ FIXED TYPE

        public bool UseManualColors { get; internal set; }
        public string DisplayText { get; internal set; } = "0";

        // Geometry (Finalizer Manager sets these via UIStateBuilder)
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        // ---------------------------------------------------------------------------------------------
        // IHUDElement Identity
        // ---------------------------------------------------------------------------------------------

        public string Id => "cash_panel";
        public int Layer => 999; // Always drawn above base HUD surface

        // ---------------------------------------------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            // Currently no animations or time‑based behavior.
            // Reserved for future flash effects or transitions.
        }

        // ---------------------------------------------------------------------------------------------
        // DRAW
        // ---------------------------------------------------------------------------------------------

        public void Draw(IDrawingContext context)
        {
            // Background fill
            var fillColor = UseManualColors
                ? ManualFillColor
                : new Color(0.15f, 0.15f, 0.15f, 0.85f); // Default background

            context.FillRectangle(
                new Rectangle(X, Y, Width, Height),
                fillColor
            );

            // Text color (CASH amount)
            var textColor = UseManualColors
                ? ManualTextColor
                : new Color(1f, 1f, 1f, 1f); // Default white

            // Draw CASH text
            context.DrawText(
                DisplayText,
                X + 8,
                Y + 6,
                textColor
            );

            // -----------------------------------------------------------------------------------------
            // Draw Item Prices (NEW)
            // -----------------------------------------------------------------------------------------

            // If manual override is active, use the resolved item price color
            var priceColor = UseManualColors
                ? ManualItemPriceColor
                : new Color(0f, 0.75f, 1f, 1f); // Default #00BFFF

            // Example item price rendering (you will replace this with your actual store logic)
            // This is only a placeholder to demonstrate correct color usage.
            context.DrawText(
                "$100",           // Replace with actual item price
                X + 8,
                Y + Height - 18,  // Example position
                priceColor
            );
        }
    }
}
