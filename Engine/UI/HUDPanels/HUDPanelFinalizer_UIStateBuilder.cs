// ====================================================================================================
//  FILE: HUDPanelFinalizer_UIStateBuilder.cs
//  PATH: Engine/UI/HUDPanels/
//  MODULE: UIState Builder (Finalizer Pipeline)
//
//  ROLE:
//      Constructs the authoritative UIState object for the CASH HUD panel. This module receives fully
//      resolved geometry, colors, and crosshair configuration from HUDPanelFinalizer_Resolver and
//      produces a deterministic UIState consumed by the GPU UI pipeline.
//
//  RESPONSIBILITIES:
//      - Convert resolved geometry and color values into a normalized UIState.
//      - Package crosshair configuration for HUDManager’s rendering stage.
//      - Ensure all values are deterministic and free of implicit defaults.
//      - Provide a single authoritative UIState for the CASH HUD panel.
//
//  NON-RESPONSIBILITIES:
//      - Manual override processing (handled by HUDPanelFinalizer_Control).
//      - Color parsing (handled by HUDPanelFinalizer_ColorParser).
//      - Geometry/crosshair resolution (handled by HUDPanelFinalizer_Resolver).
//      - Runtime rendering (handled by HUDPanel_CashUpdate and HUDManager).
//
//  ARCHITECTURAL NOTES:
//      - UIStateBuilder is intentionally simple and deterministic.
//      - No implicit defaults: all values must be provided by the Resolver.
//      - Produces a GPU‑ready UIState consumed by HUDManager and the rendering pipeline.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.UI.HUDPanels
{
    public class HUDPanelFinalizer_UIStateBuilder
    {
        /// <summary>
        /// Builds the authoritative UIState object for the CASH HUD panel.
        /// All values must be provided explicitly by the Resolver.
        /// </summary>
        public UIState Build(
            int x,
            int y,
            int width,
            int height,
            Color fillColor,
            Color textColor,
            int crosshairCenterX,
            int crosshairCenterY,
            int crosshairArmLength,
            int crosshairThickness,
            System.Drawing.Color crosshairColor,
            bool useFlashColor,
            System.Drawing.Color flashColor)
        {
            var state = new UIState
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,

                FillColor = fillColor,
                TextColor = textColor,

                CrosshairCenterX = crosshairCenterX,
                CrosshairCenterY = crosshairCenterY,
                CrosshairArmLength = crosshairArmLength,
                CrosshairThickness = crosshairThickness,
                CrosshairColor = crosshairColor,

                UseFlashColor = useFlashColor,
                FlashColor = flashColor
            };

            return state;
        }
    }

    // ====================================================================================================
    //  UIState DTO (GPU‑ready)
    // ====================================================================================================

    public class UIState
    {
        // Geometry
        public int X;
        public int Y;
        public int Width;
        public int Height;

        // Colors
        public Color FillColor;
        public Color TextColor;

        // Crosshair
        public int CrosshairCenterX;
        public int CrosshairCenterY;
        public int CrosshairArmLength;
        public int CrosshairThickness;
        public System.Drawing.Color CrosshairColor;

        // Flash behavior
        public bool UseFlashColor;
        public System.Drawing.Color FlashColor;
    }
}
