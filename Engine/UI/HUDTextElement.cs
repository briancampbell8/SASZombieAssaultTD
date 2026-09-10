// =====================================================================================================
//  FILE: HUDTextElement.cs
//  PATH: Engine/UI/HUDTextElement.cs
//  SUBSYSTEM: UI Elements (Text)
//
//  ROLE:
//      Encapsulates text rendering for the HUD with optional visual effects (shadow/outline).
//      Inherits from UIElement to share common structural properties like sizing and node tree
//      positioning parameters within the global layout engine.
//
//  RESPONSIBILITIES:
//      - Render text strings at a given position, layer, and scale factor.
//      - Support configuration values for layout transformations and effects properties.
//      - Provide programmatic text color overrides separate from layout script configs.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Components;
using SASZombieAssaultTD.Engine.UI.Elements;
namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// HUD text element for rendering text with optional shadow or outline visual effects.
    /// </summary>
    public class HUDTextElement : UIElement
    {
        public string Value { get; set; } = string.Empty;
        public float Scale { get; set; } = 1f;
        public string ColorHex { get; set; } = "#FFFFFF";
        public Color ManualTextColor { get; set; } = Color.FromArgb(0, 0, 0, 0); // Transparent = offline
        public bool Shadow { get; set; }
        public bool Outline { get; set; }

        public int X
        {
            get => (int)Position.X;
            set => Position = new PointF(value, Position.Y);
        }

        public int Y
        {
            get => (int)Position.Y;
            set => Position = new PointF(Position.X, value);
        }

        public void SetSystemColor(System.Drawing.Color color)
        {
            ManualTextColor = (Color)Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public void Draw(D3D11Adapter_Core adapter_Core)
        {
            if (adapter_Core == null || !IsVisible || string.IsNullOrEmpty(Value))
                return;

            Color textColor = (ManualTextColor.A > 0) ? ManualTextColor : ParseColor(ColorHex);
            var absPos = AbsolutePosition;
            int finalX = (int)absPos.X;
            int finalY = (int)absPos.Y;

            if (Shadow)
            {
                Color shadowColor = Color.FromArgb(0, 0, 0, (int)(textColor.A * 0.5f));
                adapter_Core.DrawText(Value, finalX + 2, finalY + 2, Scale, shadowColor);
            }

            if (Outline)
            {
                Color outlineColor = Color.FromArgb(
                    (int)0,
                    (int)0,
                    (int)0,
                    (int)textColor.A);
                adapter_Core.DrawText(Value, finalX - 1, finalY, Scale, outlineColor);
                adapter_Core.DrawText(Value, finalX + 1, finalY, Scale, outlineColor);
                adapter_Core.DrawText(Value, finalX, finalY - 1, Scale, outlineColor);
                adapter_Core.DrawText(Value, finalX, finalY + 1, Scale, outlineColor);
            }

            adapter_Core.DrawText(Value, finalX, finalY, Scale, textColor);
            base.Draw(adapter_Core);
        }

        private Color ParseColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Color.FromArgb(255, 255, 255, 255);

            string s = hex.Trim().Replace("#", "");
            try
            {
                if (s.Length == 6)
                {
                    byte r = Convert.ToByte(s.Substring(0, 2), 16);
                    byte g = Convert.ToByte(s.Substring(2, 2), 16);
                    byte b = Convert.ToByte(s.Substring(4, 2), 16);
                    return Color.FromArgb(
                        int.MaxValue, (int)255, (int)r, (int)g, (int)g, (int)b);
                }
                if (s.Length == 8)
                {
                    byte a = Convert.ToByte(s.Substring(0, 2), 16);
                    byte r = Convert.ToByte(s.Substring(2, 2), 16);
                    byte g = Convert.ToByte(s.Substring(4, 2), 16);
                    byte b = Convert.ToByte(s.Substring(6, 2), 16);
                    return (Color)Color.FromArgb(a, r, g, b);
                }
            }
            catch { }

            return Color.FromArgb(255, 255, 255, 255);
        }
    }
}
