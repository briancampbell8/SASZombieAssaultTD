// ====================================================================================================
//  FILE: HUDPanelFinalizer_ColorParser.cs
//  PATH: Engine/UI/HUDPanels/
//  MODULE: Finalizer Color Parser
//
//  ROLE:
//      Validates and normalizes developer-facing color tokens from HUDPanelFinalizer_Control. Converts
//      raw string tokens (e.g., "#FF00AA", "red", "255,128,0") into deterministic engine Color values.
//      Produces a structured parse result consumed by HUDPanelFinalizer_Manager and Resolver.
//
//  RESPONSIBILITIES:
//      - Accept raw color tokens from the Control module.
//      - Parse hex, named colors, and RGB formats deterministically.
//      - Return structured success/error results for diagnostics.
//      - Ensure no implicit defaults: invalid tokens must produce explicit errors.
//      - Provide normalized engine Color values for the Finalizer pipeline.
//
//  NON-RESPONSIBILITIES:
//      - Manual override logic (handled by Control).
//      - Geometry/crosshair resolution (handled by Resolver).
//      - UIState construction (handled by UIStateBuilder).
//      - Rendering (handled by HUDPanel_CashUpdate and HUDManager).
//
//  ARCHITECTURAL NOTES:
//      - ColorParser is intentionally strict: invalid tokens never silently fallback.
//      - All parsing paths produce deterministic results.
//      - Error messages are surfaced through Control and logged by the Manager.
// ====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.HUDPanels
{
    /// <summary>
    /// Structured result returned by the ColorParser.
    /// </summary>
    public struct HUDPanelFinalizerColorParseResult
    {
        public bool Success;
        public Color Color;
        public string Error;
    }

    public class HUDPanelFinalizer_ColorParser
    {
        // ==============================================================================================
        // PUBLIC API — Parse a raw color token deterministically
        // ==============================================================================================

        public HUDPanelFinalizerColorParseResult TryParse(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Error("Color token is empty.");
            }

            token = token.Trim();

            // ------------------------------------------------------------------------------------------
            // HEX FORMAT (#RRGGBB or #AARRGGBB)
            // ------------------------------------------------------------------------------------------
            if (token.StartsWith("#"))
            {
                return ParseHex(token);
            }

            // ------------------------------------------------------------------------------------------
            // RGB FORMAT ("255,128,0")
            // ------------------------------------------------------------------------------------------
            if (token.Contains(","))
            {
                return ParseRgb(token);
            }

            // ------------------------------------------------------------------------------------------
            // NAMED COLOR ("red", "lime", "blue")
            // ------------------------------------------------------------------------------------------
            return ParseNamed(token);
        }

        // ==============================================================================================
        // HEX PARSING
        // ==============================================================================================

        private HUDPanelFinalizerColorParseResult ParseHex(string token)
        {
            try
            {
                // Remove '#'
                string hex = token.Substring(1);

                if (hex.Length == 6)
                {
                    // RRGGBB
                    // Changing 'int' to 'float' here lets you divide by 255f cleanly
                    float r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    float g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    float b = Convert.ToInt32(hex.Substring(4, 2), 16);

                    return Ok(new Color(r / 255f, g / 255f, b / 255f, 1f));
                }
                else if (hex.Length == 8)
                {
                    // AARRGGBB
                    // Changing 'int' to 'float' here lets you divide by 255f cleanly
                    float a = Convert.ToInt32(hex.Substring(0, 2), 16);
                    float r = Convert.ToInt32(hex.Substring(2, 2), 16);
                    float g = Convert.ToInt32(hex.Substring(4, 2), 16);
                    float b = Convert.ToInt32(hex.Substring(6, 2), 16);

                    return Ok(new Color(r / 255f, g / 255f, b / 255f, a / 255f));
                }


                return Error("Invalid hex color format.");
            }
            catch
            {
                return Error("Failed to parse hex color.");
            }
        }

        // ==============================================================================================
        // RGB PARSING
        // ==============================================================================================

        private HUDPanelFinalizerColorParseResult ParseRgb(string token)
        {
            try
            {
                var parts = token.Split(',');

                if (parts.Length != 3)
                    return Error("RGB format must be 'R,G,B'.");

                int r = int.Parse(parts[0].Trim());
                int g = int.Parse(parts[1].Trim());
                int b = int.Parse(parts[2].Trim());

                if (!IsByte(r) || !IsByte(g) || !IsByte(b))
                    return Error("RGB values must be between 0 and 255.");

                return Ok(new Color(r / 255f, g / 255f, b / 255f, 1f));
            }
            catch
            {
                return Error("Failed to parse RGB color.");
            }
        }

        // ==============================================================================================
        // NAMED COLOR PARSING
        // ==============================================================================================

        private HUDPanelFinalizerColorParseResult ParseNamed(string token)
        {
            try
            {
                var sysColor = ColorTranslator.FromHtml(token);

                return Ok(new Color(
                    sysColor.R / 255f,
                    sysColor.G / 255f,
                    sysColor.B / 255f,
                    sysColor.A / 255f
                ));
            }
            catch
            {
                return Error($"Unknown color name '{token}'.");
            }
        }

        // ==============================================================================================
        // HELPERS
        // ==============================================================================================

        private bool IsByte(int value) => value >= 0 && value <= 255;

        private HUDPanelFinalizerColorParseResult Ok(Color color)
        {
            return new HUDPanelFinalizerColorParseResult
            {
                Success = true,
                Color = color,
                Error = null
            };
        }

        private HUDPanelFinalizerColorParseResult Error(string message)
        {
            return new HUDPanelFinalizerColorParseResult
            {
                Success = false,
                Color = new Color(0, 0, 0, 0),
                Error = message
            };
        }
    }
}
