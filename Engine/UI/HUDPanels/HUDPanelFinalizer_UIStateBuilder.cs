// =====================================================================================================
//  FILE: HUDPanelFinalizer_UIStateBuilder.cs
//  PATH: Engine/UI/HUDPanels/HUDPanelFinalizer_UIStateBuilder.cs
//  SUBSYSTEM: HUDPanels → Finalizer
//
//  ROLE:
//      Constructs the authoritative GPU‑ready UIState object for the CASH HUD panel. Consumes fully
//      resolved geometry, colors, item price color, and crosshair configuration from the Resolver and
//      produces a deterministic UIState consumed by HUDPanel_CashUpdate and HUDOverlay diagnostics.
//
//  RESPONSIBILITIES:
//      - Convert resolved geometry and color values into a normalized UIState.
//      - Package crosshair configuration.
//      - Ensure all values are deterministic and free of implicit defaults.
//      - Emit tracing for all state construction operations.
//
//  NON‑RESPONSIBILITIES:
//      - Manual override processing (Control).
//      - Color parsing (ColorParser).
//      - Geometry/crosshair resolution (Resolver).
//      - Rendering (HUDPanel_CashUpdate).
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class HUDPanelFinalizer_UIStateBuilder
    {
        public HUDPanels.UIState ConstructState(HUDPanelFinalizer_Resolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "UIStateBuilder: ConstructState ENTRY");

            var resolved = resolver.GetResolvedState();

            var state = Build(
                resolved.X,
                resolved.Y,
                resolved.Width,
                resolved.Height,
                resolved.FillColor,
                resolved.TextColor,
                resolved.ItemPriceColor,
                resolved.CrosshairCenterX,
                resolved.CrosshairCenterY,
                resolved.CrosshairArmLength,
                resolved.CrosshairThickness,
                resolved.CrosshairColor,
                resolved.UseFlashColor,
                resolved.FlashColor,
                resolved.ManualOverrideEnabled
            );

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "UIStateBuilder: ConstructState EXIT");

            return state;
        }

        public HUDPanels.UIState Build(
            int x,
            int y,
            int width,
            int height,
            Color fillColor,
            Color textColor,
            Color itemPriceColor,
            int crosshairCenterX,
            int crosshairCenterY,
            int crosshairArmLength,
            int crosshairThickness,
            Color crosshairColor,
            bool useFlashColor,
            Color flashColor,
            bool useManualColors)
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "UIStateBuilder: Build ENTRY");

            var state = new HUDPanels.UIState
            {
                PanelX = x,
                PanelY = y,
                PanelWidth = width,
                PanelHeight = height,

                PanelFillColor = fillColor,
                PanelTextColor = textColor,
                ItemPriceColor = itemPriceColor,

                CrosshairCenterX = crosshairCenterX,
                CrosshairCenterY = crosshairCenterY,
                CrosshairArmLength = crosshairArmLength,
                CrosshairThickness = crosshairThickness,
                CrosshairColor = crosshairColor,

                UseFlashColor = useFlashColor,
                FlashColor = flashColor,
                UseManualColors = useManualColors
            };

            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                "UIStateBuilder: Build EXIT");

            return state;
        }
    }
}

namespace SASZombieAssaultTD.Engine.UI.HUDPanels
{
    public sealed class UIState
    {
        internal object Width;
        internal object X;
        internal object ManualOverrideEnabled;
        internal object FillColor;
        internal object TextColor;
        internal object Y;
        internal object Height;

        public int PanelX { get; set; }
        public int PanelY { get; set; }
        public int PanelWidth { get; set; }
        public int PanelHeight { get; set; }

        public Color PanelFillColor { get; set; }
        public Color PanelTextColor { get; set; }
        public Color ItemPriceColor { get; set; }

        public int CrosshairCenterX { get; set; }
        public int CrosshairCenterY { get; set; }
        public int CrosshairArmLength { get; set; }
        public int CrosshairThickness { get; set; }
        public Color CrosshairColor { get; set; }

        public bool UseFlashColor { get; set; }
        public Color FlashColor { get; set; }
        public bool UseManualColors { get; set; }
    }
}
