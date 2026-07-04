// ====================================================================================================
//  FILE: HUDPanelFinalizer_Resolver.cs
//  PATH: Engine/UI/HUDPanels/
//  MODULE: HUD Panel Finalizer Resolver
//
//  ROLE:
//      Resolves final geometry, colors, and crosshair configuration for the CASH HUD panel. Consumes
//      manual entry data (via Control) and parsed colors (via ColorParser) and produces a deterministic
//      HUDPanelFinalizerResolvedState used by the Manager and UIStateBuilder.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUDPanels
{
    /// <summary>
    /// Resolves final geometry, colors, and crosshair configuration for the CASH HUD panel.
    /// This module normalizes all values and produces a deterministic HUDPanelFinalizerResolvedState.
    /// </summary>
    public class HUDPanelFinalizer_Resolver
    {
        // Geometry
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        // Colors
        private Color _fillColor;
        private Color _textColor;
        private Color _itemPriceColor;   // ⭐ NEW

        // Crosshair
        private int _crosshairCenterX;
        private int _crosshairCenterY;
        private int _crosshairArmLength;
        private int _crosshairThickness;
        private System.Drawing.Color _crosshairColor;
        private bool _useFlashColor;
        private System.Drawing.Color _flashColor;

        // Manual override flag
        private bool _manualOverrideEnabled;

        /// <summary>
        /// Sets geometry values from the Control module.
        /// </summary>
        public void SetGeometry(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Sets manual colors parsed by the ColorParser.
        /// </summary>
        public void SetManualColors(Color fillColor, Color textColor)
        {
            _fillColor = fillColor;
            _textColor = textColor;
            _manualOverrideEnabled = true;
        }

        /// <summary>
        /// Sets item price color parsed by the ColorParser.
        /// </summary>
        public void SetItemPriceColor(Color itemPriceColor)
        {
            _itemPriceColor = itemPriceColor;
        }

        /// <summary>
        /// Sets crosshair configuration from the Control module.
        /// </summary>
        public void SetCrosshair(
            int centerX,
            int centerY,
            int armLength,
            int thickness,
            System.Drawing.Color color,
            bool useFlashColor,
            System.Drawing.Color flashColor)
        {
            _crosshairCenterX = centerX;
            _crosshairCenterY = centerY;
            _crosshairArmLength = armLength;
            _crosshairThickness = thickness;
            _crosshairColor = color;
            _useFlashColor = useFlashColor;
            _flashColor = flashColor;
        }

        /// <summary>
        /// Performs final normalization and resolution of all values.
        /// </summary>
        public void Resolve()
        {
            // Geometry normalization
            if (_width < 1) _width = 1;
            if (_height < 1) _height = 1;

            // Crosshair normalization
            if (_crosshairArmLength < 1) _crosshairArmLength = 1;
            if (_crosshairThickness < 1) _crosshairThickness = 1;

            // If manual override was never set, use defaults
            if (!_manualOverrideEnabled)
            {
                _fillColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                _textColor = new Color(1f, 1f, 1f, 1f);
                _itemPriceColor = new Color(0f, 0.75f, 1f, 1f); // ⭐ Default #00BFFF
            }
        }

        /// <summary>
        /// Returns the fully resolved finalizer state.
        /// </summary>
        public HUDPanelFinalizerResolvedState GetResolvedState()
        {
            return new HUDPanelFinalizerResolvedState
            {
                X = _x,
                Y = _y,
                Width = _width,
                Height = _height,

                FillColor = _fillColor,
                TextColor = _textColor,
                ItemPriceColor = _itemPriceColor,   // ⭐ NEW

                CrosshairCenterX = _crosshairCenterX,
                CrosshairCenterY = _crosshairCenterY,
                CrosshairArmLength = _crosshairArmLength,
                CrosshairThickness = _crosshairThickness,
                CrosshairColor = _crosshairColor,

                UseFlashColor = _useFlashColor,
                FlashColor = _flashColor,

                ManualOverrideEnabled = _manualOverrideEnabled
            };
        }
    }
}
