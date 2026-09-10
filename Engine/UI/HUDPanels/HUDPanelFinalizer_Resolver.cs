// =====================================================================================================
//  FILE: HUDPanelFinalizer_Resolver.cs
//  PATH: Engine/UI/HUDPanels/HUDPanelFinalizer_Resolver.cs
//  SUBSYSTEM: HUDPanels → Finalizer
//
//  ROLE:
//      Performs deterministic resolution of all geometry, color, and crosshair values for the CASH HUD
//      panel. Consumes parsed manual override colors (ColorParser) and raw control input (Control),
//      normalizes all values, and produces a fully resolved HUDPanelFinalizerResolvedState.
//
//  RESPONSIBILITIES:
//      - Normalize geometry (X, Y, Width, Height).
//      - Normalize crosshair configuration.
//      - Apply default colors when manual override is not enabled.
//      - Produce a stable, deterministic resolved state consumed by Manager and UIStateBuilder.
//      - Emit tracing for all resolution operations.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering (HUDPanel_CashUpdate).
//      - Manual override token parsing (ColorParser).
//      - UIState construction (UIStateBuilder).
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class HUDPanelFinalizer_Resolver
    {
        // Geometry
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        // Colors
        private Color _fillColor;
        private Color _textColor;
        private Color _itemPriceColor;

        // Crosshair
        private int _crosshairCenterX = 122;
        private int _crosshairCenterY = 553;
        private int _crosshairArmLength = 12;
        private int _crosshairThickness = 3;
        private Color _crosshairColor = Color.Lime;
        private bool _useFlashColor;
        private Color _flashColor = Color.Red;

        private bool _manualOverrideEnabled;

        // -------------------------------------------------------------------------------------------------
        //  GEOMETRY
        // -------------------------------------------------------------------------------------------------

        public void SetGeometry(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                $"Resolver: SetGeometry X={x}, Y={y}, W={width}, H={height}");
        }

        // -------------------------------------------------------------------------------------------------
        //  COLORS
        // -------------------------------------------------------------------------------------------------

        public void SetManualColors(Color fillColor, Color textColor)
        {
            _fillColor = fillColor;
            _textColor = textColor;
            _manualOverrideEnabled = true;

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "Resolver: Manual colors applied.");
        }

        public void SetItemPriceColor(Color itemPriceColor)
        {
            _itemPriceColor = itemPriceColor;

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "Resolver: Item price color applied.");
        }

        // -------------------------------------------------------------------------------------------------
        //  CROSSHAIR
        // -------------------------------------------------------------------------------------------------

        public void SetCrosshair(
            int centerX,
            int centerY,
            int armLength,
            int thickness,
            Color color,
            bool useFlashColor,
            Color flashColor)
        {
            _crosshairCenterX = centerX;
            _crosshairCenterY = centerY;
            _crosshairArmLength = armLength;
            _crosshairThickness = thickness;
            _crosshairColor = color;
            _useFlashColor = useFlashColor;
            _flashColor = flashColor;

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "Resolver: Crosshair configuration applied.");
        }

        // -------------------------------------------------------------------------------------------------
        //  RESOLUTION PIPELINE
        // -------------------------------------------------------------------------------------------------

        public void ResolveGeometry(int x, int y, int width, int height, HUDPanelFinalizer_ColorParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "Resolver: ResolveGeometry ENTRY");

            SetGeometry(x, y, width, height);

            if (parser.HasOverrideActive)
            {
                var f = parser.ResolvedFillColor;
                var t = parser.ResolvedTextColor;
                var p = parser.ResolvedItemPriceColor;

                SetManualColors(Color.FromArgb(f.A, f.R, f.G, f.B),
                                (Color)Color.FromArgb(t.A, t.R, t.G, t.B));

                SetItemPriceColor((Color)Color.FromArgb(p.A, p.R, p.G, p.B));
            }

            Resolve();

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "Resolver: ResolveGeometry EXIT");
        }

        private void SetManualColors(Color? color1, Color color2)
        {
            throw new NotImplementedException();
        }

        public void Resolve()
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "Resolver: Resolve ENTRY");

            if (_width < 1) _width = 1;
            if (_height < 1) _height = 1;

            if (_crosshairArmLength < 1) _crosshairArmLength = 1;
            if (_crosshairThickness < 1) _crosshairThickness = 1;

            if (!_manualOverrideEnabled)
            {
                _fillColor = Color.FromArgb(255, 51, 51, 51);
                _textColor = Color.FromArgb(255, 255, 255, 255);
                _itemPriceColor = Color.FromArgb(255, 0, 191, 255);
            }

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "Resolver: Resolve EXIT");
        }

        // -------------------------------------------------------------------------------------------------
        //  OUTPUT
        // -------------------------------------------------------------------------------------------------

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
                ItemPriceColor = _itemPriceColor,

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

    public sealed class HUDPanelFinalizerResolvedState
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }

        public Color FillColor { get; init; }
        public Color TextColor { get; init; }
        public Color ItemPriceColor { get; init; }

        public int CrosshairCenterX { get; init; }
        public int CrosshairCenterY { get; init; }
        public int CrosshairArmLength { get; init; }
        public int CrosshairThickness { get; init; }
        public Color CrosshairColor { get; init; }

        public bool UseFlashColor { get; init; }
        public Color FlashColor { get; init; }

        public bool ManualOverrideEnabled { get; init; }
    }
}
