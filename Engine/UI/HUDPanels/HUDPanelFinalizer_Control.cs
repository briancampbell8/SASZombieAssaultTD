// ====================================================================================================
//  FILE: HUDPanelFinalizer_Control.cs
//  PATH: Engine/UI/HUDPanels/
//  MODULE: Finalizer Control Surface (Manual Override Input)
//
//  ROLE:
//      Provides the developer-facing manual override surface for the CASH HUD panel. This module
//      exposes editable fields (geometry, colors, crosshair configuration) that can be populated
//      either manually or via HUDConfigManager. The Finalizer Manager reads these values and passes
//      them into the ColorParser and Resolver.
//
//  RESPONSIBILITIES:
//      - Store manual override fields for geometry, colors, and crosshair configuration.
//      - Provide a clean DTO (HUDPanelFinalizerControlData) for the Finalizer pipeline.
//      - Accept persisted configuration from HUDConfigManager.
//      - Surface error messages when color parsing fails.
//      - Enable/disable manual override mode deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Color parsing (handled by HUDPanelFinalizer_ColorParser).
//      - Geometry/crosshair resolution (handled by HUDPanelFinalizer_Resolver).
//      - UIState construction (handled by HUDPanelFinalizer_UIStateBuilder).
//      - Rendering (handled by HUDPanel_CashUpdate and HUDManager).
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally simple and declarative.
//      - No implicit defaults: all values must be set explicitly by config or developer input.
//      - Error messages are stored here so the Manager can surface them in diagnostics.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUDPanels
{
    public class HUDPanelFinalizer_Control
    {
        // ---------------------------------------------------------------------------------------------
        // Manual override flags
        // ---------------------------------------------------------------------------------------------

        private bool _manualOverrideEnabled = false;
        private string _errorMessage = null;

        // ---------------------------------------------------------------------------------------------
        // Geometry (developer-facing)
        // ---------------------------------------------------------------------------------------------

        public int X { get; set; } = 120;
        public int Y { get; set; } = 540;
        public int Width { get; set; } = 80;
        public int Height { get; set; } = 40;

        // ---------------------------------------------------------------------------------------------
        // Color tokens (developer-facing)
        // These are raw strings that ColorParser will validate and convert.
        // ---------------------------------------------------------------------------------------------

        public string FillColorToken { get; set; } = "#202020";
        public string TextColorToken { get; set; } = "#00E0FF"; //changing this to a different color will change the color of the cash text to better match the color of the actual panel text. The default is a cyan color, but you can change it to any valid color token (e.g., "red", "#FF0000", "255,0,0") to customize the text color.
        public string ItemPriceColorToken { get; set; } = "#00BFFF";   // Light blue, matches original item prices

        // ---------------------------------------------------------------------------------------------
        // Crosshair configuration (developer-facing)
        // ---------------------------------------------------------------------------------------------

        public int CrosshairCenterX { get; set; } = 122;
        public int CrosshairCenterY { get; set; } = 553;
        public int CrosshairArmLength { get; set; } = 12;
        public int CrosshairThickness { get; set; } = 3;

        public Color CrosshairColor { get; set; } = Color.Lime;
        public bool UseFlashColor { get; set; } = false;
        public Color FlashColor { get; set; } = Color.Red;


        // ---------------------------------------------------------------------------------------------
        // Error message handling
        // ---------------------------------------------------------------------------------------------

        public void SetErrorMessage(string message)
        {
            _errorMessage = message;
        }

        public string GetErrorMessage()
        {
            return _errorMessage;
        }

        // ---------------------------------------------------------------------------------------------
        // Manual override toggle
        // ---------------------------------------------------------------------------------------------

        public void SetManualOverrideEnabled(bool enabled)
        {
            _manualOverrideEnabled = enabled;
        }

        public bool IsManualOverrideEnabled()
        {
            return _manualOverrideEnabled;
        }

        // ---------------------------------------------------------------------------------------------
        // DTO Export
        // Finalizer Manager calls this to obtain a clean snapshot of all manual fields.
        // ---------------------------------------------------------------------------------------------

        public HUDPanelFinalizerControlData GetManualEntryData()
        {
            return new HUDPanelFinalizerControlData
            {
                X = this.X,
                Y = this.Y,
                Width = this.Width,
                Height = this.Height,

                FillColorToken = this.FillColorToken,
                TextColorToken = this.TextColorToken,
                ItemPriceColorToken = this.ItemPriceColorToken,   // ⭐ REQUIRED

                CrosshairCenterX = this.CrosshairCenterX,
                CrosshairCenterY = this.CrosshairCenterY,
                CrosshairArmLength = this.CrosshairArmLength,
                CrosshairThickness = this.CrosshairThickness,

                CrosshairColor = this.CrosshairColor,
                UseFlashColor = this.UseFlashColor,
                FlashColor = this.FlashColor
            };
        }


    }
}
