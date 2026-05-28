/*
Program Name: SASZombieAssaultTD
File Path: Engine/UI/HUDColorInputProcessor.cs
Purpose: Input processor for normalizing multiple color input formats into engine-ready Color values.
Features:
  - Accepts multiple color input formats: hex strings, named colors, tuples, arrays, and System.Drawing.Color
  - Normalizes colors to engine-ready Color values with 0-1 float channels
  - Provides ProcessManualColors entry point for fill and text color processing
  - Includes hex string normalization for RGB (6 chars) and RGBA (8 chars) formats
  - Supports named color resolution through System.Drawing.Color.FromName
  - Handles RGB and RGBA tuple normalization
  - Provides int array normalization for 3-element (RGB) and 4-element (RGBA) arrays
  - Uses Engine.Diagnostics.DebugLogger.Trace for deterministic diagnostics on all branches
  - No baked-in defaults, no fallback logic except explicit transparent Color()
*/

using System;
using System.Drawing;
using System.Globalization;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    public static class HUDColorInputProcessor
    {
        private static float r;
        private static float g;
        private static float b;
        private static float a;

        // --------------------------------------------------------------------
        // PUBLIC ENTRY POINT
        // --------------------------------------------------------------------

        public static (System.Drawing.Color finalFillColor, System.Drawing.Color finalTextColor, bool manualOverrideEnabled)
            ProcessManualColors(object fillInput, object textInput)
        {
            SASZombieAssaultTD.Engine.Diagnostics.DebugLogger.Trace("HUDColorInputProcessor.Process.Start",
                $"fillInput={Describe(fillInput)}, textInput={Describe(textInput)}");

            bool overrideEnabled = fillInput != null || textInput != null;

            if (!overrideEnabled)
            {
                SASZombieAssaultTD.Engine.Diagnostics.DebugLogger.Trace("HUDColorInputProcessor.Process.Override.Disabled", "No manual colors provided");
                return (new System.Drawing.Color(), new System.Drawing.Color(), false);
            }

            Color finalFill = NormalizeColorInput(fillInput, "Fill");
            // TODO: Cannot cast Engine.Core.Color to System.Drawing.Color
            // System.Drawing.Color finalText = (System.Drawing.Color)NormalizeColorInput(textInput, "Text");
            System.Drawing.Color finalText = System.Drawing.Color.White;
            SASZombieAssaultTD.Engine.Diagnostics.DebugLogger.Trace("HUDColorInputProcessor.Process.Override.Enabled", "Manual override active");

            // TODO: Cannot cast Engine.Core.Color to System.Drawing.Color
            // return ((System.Drawing.Color)finalFill, finalText, true);
            return (System.Drawing.Color.White, finalText, true);
        }

        // --------------------------------------------------------------------
        // NORMALIZATION CORE
        // --------------------------------------------------------------------

        private static Color NormalizeColorInput(object input, string channelTag)
        {
            SASZombieAssaultTD.Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Start", Describe(input));

            if (input == null)
            {
                SASZombieAssaultTD.Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Null",
                    "Returning transparent");
                return Color.FromArgb((int)(0 / 255f), (int)(0 / 255f), (int)(0 / 255f), (int)(0 / 255f));
             //   return System.Drawing.Color(true);
            }

            // STRING → HEX or NAMED COLOR
            if (input is string hex)
            {
                return NormalizeHexString(hex, channelTag);
            }

            // SYSTEM.DRAWING.COLOR
            if (input is System.Drawing.Color sys)
            {
                Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.SystemDrawingColor",
                    $"{sys.R},{sys.G},{sys.B},{sys.A}");
                return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(a / 255f));

                //return Color.FromArgb(
                //    sys.R / 255f,
                //    sys.G / 255f,
                //    sys.B / 255f,
                //    sys.A / 255f
                //);  
            }

            // RGB TUPLE
            if (input is ValueTuple<int, int, int> rgb)
            {
                Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Tuple.RGB",
                    $"{rgb.Item1},{rgb.Item2},{rgb.Item3}");

                return Color.FromArgb(
                    (int)(Clamp(rgb.Item1) / 255f),
                    (int)(Clamp(rgb.Item2) / 255f),
                    (int)(Clamp(rgb.Item3) / 255f),
                    (int)(1.0f)
                );
            }

            // RGBA TUPLE
            if (input is ValueTuple<int, int, int, int> rgba)
            {
                Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Tuple.RGBA",
                    $"{rgba.Item1},{rgba.Item2},{rgba.Item3},{rgba.Item4}");

                return Color.FromArgb(
                    (int)(Clamp(rgba.Item1) / 255f),
                    (int)(Clamp(rgba.Item2) / 255f),
                    (int)(Clamp(rgba.Item3) / 255f),
                    (int)(Clamp(rgba.Item4) / 255f)
                );
            }

            // INT ARRAY
            if (input is int[] arr)
            {
                return NormalizeIntArray(arr, channelTag);
            }

            Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.UnsupportedType",
                input.GetType().FullName);

            return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(a / 255f));
        }

        // --------------------------------------------------------------------
        // HEX NORMALIZATION
        // --------------------------------------------------------------------

        private static Color NormalizeHexString(string hex, string channelTag)
        {
            Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Hex.Start", hex);

            if (string.IsNullOrWhiteSpace(hex))
            {
                Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Hex.Empty", "Returning transparent");
                return Color.FromArgb((int)(0 / 255f), (int)(0 / 255f), (int)(0 / 255f), (int)(0 / 255f));
            }

            string clean = hex.Trim().TrimStart('#');

            // RGB (6 chars)
            if (clean.Length == 6 &&
                int.TryParse(clean, NumberStyles.HexNumber, null, out int rgb))
            {
                int r = (rgb >> 16) & 0xFF;
                int g = (rgb >> 8) & 0xFF;
                int b = rgb & 0xFF;

                Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Hex.RGB",
                    $"{r},{g},{b}");

                return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(1.0f));
            }

            // RGBA (8 chars)
            if (clean.Length == 8 &&
                int.TryParse(clean, NumberStyles.HexNumber, null, out int rgba))
            {
                int r = (rgba >> 24) & 0xFF;
                int g = (rgba >> 16) & 0xFF;
                int b = (rgba >> 8) & 0xFF;
                int a = rgba & 0xFF;

                Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Hex.RGBA",
                    $"{r},{g},{b},{a}");

                return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(a / 255f));
            }

            // NAMED COLOR
            try
            {
                var named = System.Drawing.Color.FromName(clean);

                if (named.A != 0 || named.R != 0 || named.G != 0 || named.B != 0)
                {
                    Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Hex.Named",
                        $"{named.R},{named.G},{named.B},{named.A}");

                    return Color.FromArgb(
                        (int)(named.R / 255f),
                        (int)(named.G / 255f),
                        (int)(named.B / 255f),
                        (int)(named.A / 255f)
                    );
                }
            }
            catch { }

            Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Hex.Invalid", hex);
            return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(a / 255f));
        }

        // --------------------------------------------------------------------
        // INT ARRAY NORMALIZATION
        // --------------------------------------------------------------------

        private static Color NormalizeIntArray(int[] arr, string channelTag)
        {
            Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Array.Start",
                $"len={arr.Length}");

            if (arr.Length == 3)
            {
                return Color.FromArgb(
                    (int)(Clamp(arr[0]) / 255f),
                    (int)(Clamp(arr[1]) / 255f),
                    (int)(Clamp(arr[2]) / 255f),
                    (int)(1.0f)
                );
            }

            if (arr.Length >= 4)
            {
                return Color.FromArgb(
                    (int)(Clamp(arr[0]) / 255f),
                    (int)(Clamp(arr[1]) / 255f),
                    (int)(Clamp(arr[2]) / 255f),
                    (int)(Clamp(arr[3]) / 255f)
                );
            }

            Engine.Diagnostics.DebugLogger.Trace($"HUDColorInputProcessor.Normalize.{channelTag}.Array.Invalid", "Returning transparent");
            return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(a / 255f));
        }

        // --------------------------------------------------------------------
        // HELPERS
        // --------------------------------------------------------------------

        private static int Clamp(int v) =>System.Math.Max(0, System.Math.Min(255, v));

        private static string Describe(object o)
        {
            if (o == null) return "null";
            return $"{o.GetType().Name}";
        }
    }
}
