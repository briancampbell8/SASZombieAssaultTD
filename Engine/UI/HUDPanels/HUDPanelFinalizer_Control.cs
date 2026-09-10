// =====================================================================================================
//  FILE: HUDPanelFinalizer_Control.cs
//  PATH: Engine/UI/HUDPanels/HUDPanelFinalizer_Control.cs
//  MODULE: Finalizer Control Surface (Manual Override Input)
//  LAYER: UI → HUDPanels → Finalizer
//
//  ROLE:
//      Developer-facing manual override surface for the CASH HUD panel. This module exposes editable
//      geometry, color tokens, and crosshair configuration. The Finalizer Manager consumes these values,
//      passes them into the ColorParser and Resolver, and produces the authoritative UIState.
//
//  RESPONSIBILITIES:
//      - Store manual override fields for geometry, colors, and crosshair configuration.
//      - Provide a clean DTO (HUDPanelFinalizerControl) for the Finalizer pipeline.
//      - Surface error messages when color parsing fails.
//      - Enable/disable manual override mode deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Color parsing (ColorParser).
//      - Geometry/crosshair resolution (Resolver).
//      - UIState construction (UIStateBuilder).
//      - Rendering (HUDPanel_CashUpdate).
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.UI
{
    public class HUDPanelFinalizer_Control
    {
        private bool _manualOverrideEnabled = false;
        private string? _errorMessage = null;

        // Geometry fields
        public int X { get; set; } = 120;
        public int Y { get; set; } = 540;
        public int Width { get; set; } = 80;
        public int Height { get; set; } = 40;

        // Color tokens
        public string FillColorToken { get; set; } = "#202020";
        public string TextColorToken { get; set; } = "#00E0FF";
        public string ItemPriceColorToken { get; set; } = "#00BFFF";

        // Crosshair configuration
        public int CrosshairCenterX { get; set; } = 122;
        public int CrosshairCenterY { get; set; } = 553;
        public int CrosshairArmLength { get; set; } = 12;
        public int CrosshairThickness { get; set; } = 3;

        public Color CrosshairColor { get; set; } = Color.Lime;
        public bool UseFlashColor { get; set; } = false;
        public Color FlashColor { get; set; } = Color.Red;

        public void SetErrorMessage(string message) => _errorMessage = message;
        public string? GetErrorMessage() => _errorMessage;

        public void SetManualOverrideEnabled(bool enabled) => _manualOverrideEnabled = enabled;
        public bool IsManualOverrideEnabled() => _manualOverrideEnabled;

        public HUDPanelFinalizerControl GetManualEntryData()
        {
            return new HUDPanelFinalizerControl(
                X,
                Y,
                Width,
                Height,
                FillColorToken,
                TextColorToken,
                ItemPriceColorToken,
                CrosshairCenterX,
                CrosshairCenterY,
                CrosshairArmLength,
                CrosshairThickness,
                CrosshairColor,
                UseFlashColor,
                FlashColor
            );
        }
    }

    public sealed record HUDPanelFinalizerControl(
        int X,
        int Y,
        int Width,
        int Height,
        string FillColorToken,
        string TextColorToken,
        string ItemPriceColorToken,
        int CrosshairCenterX,
        int CrosshairCenterY,
        int CrosshairArmLength,
        int CrosshairThickness,
        Color CrosshairColor,
        bool UseFlashColor,
        Color FlashColor);
}
