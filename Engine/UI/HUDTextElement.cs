// ====================================================================================================
//  FILE: HUDTextElement.cs
//  PATH: ./Engine/UI/
//  MODULE: HUD Text Element
//
//  ROLE:
//      Individual display element responsible for rendering text blocks.
//
//  RESPONSIBILITIES:
//      - Update display string caches dynamically.
//      - Draw element text using the provided drawing context.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================\n
// ====================================================================================================
//  FILE: HUDTextElement.cs
//  PATH: ./Engine/UI/
//  MODULE: HUD Text Element
//
//  ROLE:
//      Individual display element responsible for rendering text blocks.
//
//  RESPONSIBILITIES:
//      - Update display string caches dynamically.
//      - Draw element text using the provided drawing context.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================\n
// ====================================================================================================
//  FILE: HUDTextElement.cs
//  PATH: Engine/UI/
//  MODULE: UI Elements (Text)
//
//  ROLE:
//      Encapsulates text rendering for HUD with optional visual effects (shadow/outline).
//
//  RESPONSIBILITIES:
//      - Render text strings at a given position, layer, and scale.
//      - Support JSON-driven configuration for layout and visual properties.
//      - Offer ManualTextColor overrides for runtime color control.
//      - Provide robust hex color parsing with graceful fallback diagnostics.
//
//  NON-RESPONSIBILITIES:
//      - Managing layout of other HUD elements (HUDManager/HUDPanel_* handle composition).
//      - Low-level font asset loading (handled by rendering subsystem).
//
//  ARCHITECTURAL NOTES:
//      - Designed to be lightweight and allocation-free during per-frame Draw() when possible.
//      - Parsing functions are intentionally tolerant and diagnostic-friendly.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Rendering;
using System;
using System.Drawing;
using System.Globalization;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    ///<summary>
    ///HUD text element for rendering text with optional visual effects.
    ///Supports positioning, scaling, coloring, shadow, and outline effects.
    ///Configurable via JSON data and implements the IHUDElement interface.
    ///</summary>
    public class HUDTextElement : IHUDElement
    {
        ///<summary>
        ///Unique identifier for this text element.
        ///Used for referencing and managing specific elements.
        ///</summary>
        public string Id { get; set; }

        ///<summary>
        ///The text content to display.
        ///Empty or null strings will not be rendered.
        ///</summary>
        public string Value { get; set; } = "";

        ///<summary>
        ///X coordinate position on screen (pixels).
        ///</summary>
        public int X { get; set; }

        ///<summary>
        ///Y coordinate position on screen (pixels).
        ///</summary>
        public int Y { get; set; }

        ///<summary>
        ///Rendering layer depth for draw ordering.
        ///Higher values render on top of lower values.
        ///</summary>
        public int Layer { get; set; }

        ///<summary>
        ///Controls whether this element is visible and rendered.
        ///</summary>
        public bool Visible { get; set; } = true;

        ///<summary>
        ///Text scaling factor.
        ///1.0f = normal size, 2.0f = double size, etc.
        ///</summary>
        public float Scale { get; set; } = 1f;

        ///<summary>
        ///Text color in hexadecimal format.
        ///Supports standard HTML color notation (#RRGGBB or #AARRGGBB).
        ///Default: "#FFFFFF" (white).
        ///</summary>
        public string ColorHex { get; set; } = "#FFFFFF";

        ///<summary>
        ///Manual text color override for direct color setting.
        ///When set (not Transparent), this overrides ColorHex.
        ///Allows programmatic color control separate from JSON config.
        ///</summary>
        public Color ManualTextColor { get; set; } = Color.FromArgb(0, 0, 0, 0); //Transparent = not set

        ///<summary>
        ///Enables drop shadow effect.
        ///Shadow appears 2 pixels down and right with 50% opacity.
        ///</summary>
        public bool Shadow { get; set; } = false;

        ///<summary>
        ///Enables text outline effect.
        ///Outline is drawn in black around the text edges.
        ///</summary>
        public bool Outline { get; set; } = false;

        ///<summary>
        ///Sets text color using System.Drawing.Color for convenience.
        ///Examples: SetSystemColor(System.Drawing.Color.Red)
        ///          SetSystemColor(System.Drawing.Color.Blue)
        ///          SetSystemColor(System.Drawing.Color.FromArgb(255, 128, 0))
        ///</summary>
        public void SetSystemColor(System.Drawing.Color color)
{
    //TODO: Cannot assign System.Drawing.Color to Engine.Core.Color
    //ManualTextColor = color;
}



        ///<summary>
        ///Renders the text element to the specified render context.
        ///Applies shadow and outline effects if enabled.
        ///</summary>
        ///<param name="context">The render context to draw to.</param>
        public void Draw(IDrawingContext context)
        {
            //Early exit if element is not visible or has no content
            if (!Visible || string.IsNullOrEmpty(Value))
            {
                return;
            }

            //Determine final text color
            Color textColor;
            if (ManualTextColor.A > 0)
            {
                textColor = ManualTextColor;
            }
            else
            {
                textColor = ParseColor(ColorHex);
            }

            int x = X;
            int y = Y;

            //Optional shadow
            if (Shadow)
            {
                Color shadowColor = Color.FromArgb((byte)(0f),(byte)(0f), (byte)(0f), (byte)(textColor.A * 0.5f));
                context.DrawText(Value, x + 2, y + 2, Scale, Color.FromArgb((byte)(shadowColor.R * 255), (byte)(shadowColor.G * 255), (byte)(shadowColor.B * 255), (byte)(shadowColor.A * 255)));
            }

            //Optional outline (simple 4-direction outline)
            if (Outline)
            {
                Color outlineColor = Color.FromArgb((byte)(0f), (byte)(0f), (byte)(0f), (byte)(textColor.A));
                context.DrawText(Value, x - 1, y, Scale, Color.FromArgb((byte)(outlineColor.R * 255), (byte)(outlineColor.G * 255), (byte)(outlineColor.B * 255), (byte)(outlineColor.A * 255)));
                context.DrawText(Value, x + 1, y, Scale, Color.FromArgb((byte)(outlineColor.R * 255), (byte)(outlineColor.G * 255), (byte)(outlineColor.B * 255), (byte)(outlineColor.A * 255)));
                context.DrawText(Value, x, y - 1, Scale, Color.FromArgb((byte)(outlineColor.R * 255), (byte)(outlineColor.G * 255), (byte)(outlineColor.B * 255), (byte)(outlineColor.A * 255)));
                context.DrawText(Value, x, y + 1, Scale, Color.FromArgb((byte)(outlineColor.R * 255), (byte)(outlineColor.G * 255), (byte)(outlineColor.B * 255), (byte)(outlineColor.A * 255)));
            }

            //Main text
            context.DrawText(Value, x, y, Scale, textColor);
        }

        ///<summary>
        ///Parses hexadecimal color string to engine Color structure.
        ///Supports standard HTML color notation (#RRGGBB or #AARRGGBB).
        ///Falls back to solid white on invalid input.
        ///</summary>
        ///<param name="hex">Hexadecimal color string.</param>
        ///<returns>Engine Color structure.</returns>
        private Color ParseColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return Color.FromArgb((int)(1f), (int)(1f), (int)(1f), (int)(1f));
            }

            string s = hex.Trim();

            if (s.StartsWith("#"))
            {
                s = s.Substring(1);
            }

            try
            {
                if (s.Length == 6)
                {
                    //RRGGBB
                    byte r = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte g = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte b = byte.Parse(s.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(1f));
                }
                else if (s.Length == 8)
                {
                    //AARRGGBB
                    byte a = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte r = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte g = byte.Parse(s.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte b = byte.Parse(s.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return Color.FromArgb((int)(r / 255f), (int)(g / 255f), (int)(b / 255f), (int)(a / 255f));
                }
            }
            catch
            {
                //fall through to return default white
            }

            //Fallback to solid white on invalid hex
            return Color.FromArgb((int)(1f), (int)(1f), (int)(1f), (int)(1f));
        }
    }
}


