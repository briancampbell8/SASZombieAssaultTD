// =====================================================================================================
//  FILE: HUDPanelFinalizer_ColorParser.cs
//  PATH: Engine/UI/HUDPanels/HUDPanelFinalizer_ColorParser.cs
//  SUBSYSTEM: HUDPanels → Finalizer
//
//  ROLE:
//      Deterministic parser for developer-facing color tokens used in the HUD Finalizer pipeline.
//      Converts raw string tokens (#RRGGBB, #AARRGGBB, "red", "255,128,0") into engine Color values.
//      Produces structured parse results consumed by Control, Resolver, and Manager.
//
//  RESPONSIBILITIES:
//      - Parse hex, named, and RGB color formats.
//      - Validate all tokens strictly; invalid formats produce explicit errors.
//      - Return normalized engine Color values (byte-based).
//      - Provide deterministic, side-effect-free parsing for the Finalizer pipeline.
//      - Emit tracing for all parsing operations.
//
//  NON-RESPONSIBILITIES:
//      - Manual override enablement (Control).
//      - Geometry/crosshair resolution (Resolver).
//      - UIState construction (UIStateBuilder).
//      - Rendering (HUDPanel_CashUpdate).
//
//  ARCHITECTURE NOTES:
//      - Strict parser: no implicit defaults, no silent fallbacks.
//      - Extended to expose ParseColors batch pipeline hooks for Manager orchestration.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    public struct HUDPanelFinalizerColorParseResult
    {
        public bool Success;
        public Color Color;
        public string Error;
    }

    public sealed class HUDPanelFinalizer_ColorParser
    {
        public Color ResolvedFillColor { get; private set; } = Color.FromArgb(255, 32, 32, 32);
        public Color ResolvedTextColor { get; private set; } = Color.FromArgb(255, 0, 224, 255);
        public Color ResolvedItemPriceColor { get; private set; } = Color.FromArgb(255, 0, 191, 255);
        public bool HasOverrideActive { get; private set; }

        // -------------------------------------------------------------------------------------------------
        //  PUBLIC API — Deterministic color parsing
        // -------------------------------------------------------------------------------------------------

        public HUDPanelFinalizerColorParseResult TryParse(string token)
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                $"ColorParser: TryParse token='{token}'");

            if (string.IsNullOrWhiteSpace(token))
                return Error("Color token is empty.");

            token = token.Trim();

            if (token.StartsWith("#"))
                return ParseHex(token);

            if (token.Contains(","))
                return ParseRgb(token);

            return ParseNamed(token);
        }

        public void ParseColors(Color manualFill, Color manualText, bool overrideEnabled)
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                $"ColorParser: ParseColors override={overrideEnabled}");

            HasOverrideActive = overrideEnabled;

            if (overrideEnabled)
            {
                ResolvedFillColor = manualFill;
                ResolvedTextColor = manualText;

                ResolvedItemPriceColor = Color.FromArgb(
                    manualText.A,
                    System.Math.Max(0, manualText.R - 30),
                    manualText.G,
                    manualText.B);
            }
            else
            {
                ResolvedFillColor = Color.FromArgb(255, 32, 32, 32);
                ResolvedTextColor = Color.FromArgb(255, 0, 224, 255);
                ResolvedItemPriceColor = Color.FromArgb(255, 0, 191, 255);
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  HEX (#RRGGBB or #AARRGGBB)
        // -------------------------------------------------------------------------------------------------

        private HUDPanelFinalizerColorParseResult ParseHex(string token)
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                $"ColorParser: ParseHex token='{token}'");

            try
            {
                string hex = token.Substring(1);

                if (hex.Length == 6)
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);

                    return Ok(Color.FromArgb((byte)255, r, g, b));
                }
                else if (hex.Length == 8)
                {
                    byte a = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte r = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(4, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(6, 2), 16);

                    return Ok(Color.FromArgb(a, r, g, b));
                }

                return Error("Invalid hex color format.");
            }
            catch
            {
                return Error("Failed to parse hex color.");
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  RGB ("255,128,0")
        // -------------------------------------------------------------------------------------------------

        private HUDPanelFinalizerColorParseResult ParseRgb(string token)
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                $"ColorParser: ParseRgb token='{token}'");

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

                return Ok(Color.FromArgb(
                    (byte)255,
                    (byte)r,
                    (byte)g,
                    (byte)b));
            }
            catch
            {
                return Error("Failed to parse RGB color.");
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  NAMED ("red", "lime", "blue")
        // -------------------------------------------------------------------------------------------------

        private HUDPanelFinalizerColorParseResult ParseNamed(string token)
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Trace,
                $"ColorParser: ParseNamed token='{token}'");

            try
            {
                var sysColor = ColorTranslator.FromHtml(token);
                return Ok(Color.FromArgb(sysColor.A, sysColor.R, sysColor.G, sysColor.B));
            }
            catch
            {
                return Error($"Unknown color name '{token}'.");
            }
        }

        private HUDPanelFinalizerColorParseResult Ok(Color? color)
        {
            throw new NotImplementedException();
        }

        // -------------------------------------------------------------------------------------------------
        //  HELPERS
        // -------------------------------------------------------------------------------------------------

        private bool IsByte(int value) => value >= 0 && value <= 255;

        private HUDPanelFinalizerColorParseResult Ok(Color color)
        {
            return new HUDPanelFinalizerColorParseResult
            {
                Success = true,
                Color = color,
                Error = string.Empty
            };
        }

        private HUDPanelFinalizerColorParseResult Error(string message)
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Warn,
                $"ColorParser: ERROR '{message}'");

            return new HUDPanelFinalizerColorParseResult
            {
                Success = false,
                Color = Color.FromArgb(0, 0, 0, 0),
                Error = message
            };
        }
    }
}
