// =====================================================================================================
//  FILE: Color.cs
//  PATH: Engine/Core/Color.cs
//  SUBSYSTEM: Core
//
// PURPOSE:     Specific Engine-only case bridge wrapper interacting natively
//              with standard System.Drawing.Color types.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine
{
    public struct Color : IEquatable<Color>
    {
        // Backing case value using standard System.Drawing structure
        public System.Drawing.Color _backingColor;

        // Optional packed tint value (reserved for future use)
        private uint colorTint;

        // Engine palette (internal for now; can be exposed via properties later)
        internal static Color LightGray;
        internal static Color Lime;
        internal static Color Cyan;
        internal static Color Gray;
        internal static Color White;
        internal static Color Green;
        internal static Color LightGreen;
        internal static Color LightCoral;
        internal static Color Red;
        internal static Color Yellow;
        internal static Color Purple;
        internal static Color Blue;
        internal static Color Brown;
        internal static Color DarkGray;
        internal static Color LightBlue;
        internal static Color Gold;
        internal static Color Magenta;
        internal static Color Orange;
        internal static Color Transparent;
        internal static Color Black;

        static Color()
        {
            White = new Color(255, 255, 255);
            Black = new Color(0, 0, 0);
            Red = new Color(255, 0, 0);
            Green = new Color(0, 255, 0);
            Blue = new Color(0, 0, 255);
            Yellow = new Color(255, 255, 0);
            Purple = new Color(128, 0, 128);
            Gold = new Color(255, 215, 0);
            Orange = new Color(255, 165, 0);
            LightGray = new Color(192, 192, 192);
            Gray = new Color(128, 128, 128);
            DarkGray = new Color(64, 64, 64);
            Cyan = new Color(0, 255, 255);
            LightBlue = new Color(173, 216, 230);
            LightGreen = new Color(144, 238, 144);
            LightCoral = new Color(240, 128, 128);
            Lime = new Color(0, 255, 0);
            Brown = new Color(165, 42, 42);
            Transparent = new Color(0, 0, 0, 0);
            Magenta = new Color(255, 0, 255);
        }

        public byte R => _backingColor.R;
        public byte G => _backingColor.G;
        public byte B => _backingColor.B;
        public byte A => _backingColor.A;

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            _backingColor = System.Drawing.Color.FromArgb(a, r, g, b);
            colorTint = 0;
        }

        public Color(System.Drawing.Color drawingColor)
        {
            _backingColor = drawingColor;
            colorTint = 0;
        }

        // Reserved tint constructor: currently only stores tint value for future use.
        public Color(uint colorTint, int g, byte b)
        {
            _backingColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);
            this.colorTint = colorTint;
        }

        // Reserved tint constructor: currently initializes from tint tuple as opaque color.
        public Color(byte r, (float R, float G, float B, float A) tint)
        {
            byte rr = (byte)System.Math.Clamp((int)(tint.R * 255.0f), 0, 255);
            byte gg = (byte)System.Math.Clamp((int)(tint.G * 255.0f), 0, 255);
            byte bb = (byte)System.Math.Clamp((int)(tint.B * 255.0f), 0, 255);
            byte aa = (byte)System.Math.Clamp((int)(tint.A * 255.0f), 0, 255);

            _backingColor = System.Drawing.Color.FromArgb(aa, rr, gg, bb);
            colorTint = 0;
        }

        // Implicit operators to let engine color drop cleanly into System.Drawing methods
        public static implicit operator System.Drawing.Color(Color c) => c._backingColor;
        public static implicit operator Color(System.Drawing.Color c) => new Color(c);

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override bool Equals(object obj)
        {
            return obj is Color other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public static Color Lerp(Color start, Color end, float amount)
        {
            float clampedAmount = (float)System.Math.Max(0.0, System.Math.Min((double)amount, 1.0));

            byte r = (byte)(start.R + (end.R - start.R) * clampedAmount);
            byte g = (byte)(start.G + (end.G - start.G) * clampedAmount);
            byte b = (byte)(start.B + (end.B - start.B) * clampedAmount);
            byte a = (byte)(start.A + (end.A - start.A) * clampedAmount);

            return new Color(r, g, b, a);
        }

        public override string ToString()
        {
            return _backingColor.ToString();
        }

        internal static Color FromUint(uint boundsColor)
        {
            // Interpret the uint as 0xAARRGGBB (alpha in highest byte)
            byte a = (byte)((boundsColor >> 24) & 0xFFu);
            byte r = (byte)((boundsColor >> 16) & 0xFFu);
            byte g = (byte)((boundsColor >> 8) & 0xFFu);
            byte b = (byte)(boundsColor & 0xFFu);

            return new Color(r, g, b, a);
        }

        internal static Color? FromArgb(byte a, byte r, byte g, byte b)
        {
            // Treat fully transparent colors as null to allow callers to distinguish "no color".
            if (a == 0)
                return null;

            return new Color(r, g, b, a);
        }

        internal static Color FromArgb(int v, int r, byte g, byte b)
        {
            byte a = (byte)System.Math.Clamp(v, 0, 255);
            byte rr = (byte)System.Math.Clamp(r, 0, 255);

            return new Color(rr, g, b, a);
        }

        internal static Color FromArgb(int v1, int v2, int v3, int v4)
        {
            static byte ClampToByte(int v) => (byte)System.Math.Max(0, System.Math.Min(255, v));

            byte a = ClampToByte(v1);
            byte r = ClampToByte(v2);
            byte g = ClampToByte(v3);
            byte b = ClampToByte(v4);

            return new Color(r, g, b, a);
        }

        internal static Color FromArgb(byte v)
        {
            return new Color(v, v, v, 255);
        }

        internal static Color FromArgb(int v)
        {
            return new Color(System.Drawing.Color.FromArgb(v));
        }

        internal static Color FromArgb(int maxValue, int v, int r, int g1, int g2, int b)
        {
            if (maxValue <= 0)
            {
                maxValue = 255;
            }

            static int ClampToRange(int val, int min, int max)
            {
                if (val < min) return min;
                if (val > max) return max;
                return val;
            }

            int aInt = ClampToRange(v, 0, maxValue);
            int rInt = ClampToRange(r, 0, maxValue);
            int gInt = ClampToRange((g1 + g2) / 2, 0, maxValue);
            int bInt = ClampToRange(b, 0, maxValue);

            static byte ScaleToByte(int value, int max)
            {
                double scaled = value * 255.0 / max;
                int rounded = (int)System.Math.Round(scaled);
                if (rounded < 0) rounded = 0;
                if (rounded > 255) rounded = 255;
                return (byte)rounded;
            }

            byte a = ScaleToByte(aInt, maxValue);
            byte rr = ScaleToByte(rInt, maxValue);
            byte gg = ScaleToByte(gInt, maxValue);
            byte bb = ScaleToByte(bInt, maxValue);

            return new Color(rr, gg, bb, a);
        }

        internal static Color FromArgb(int v1, int v2, int v3)
        {
            if (v1 < 0) v1 = 0;
            else if (v1 > 255) v1 = 255;

            if (v2 < 0) v2 = 0;
            else if (v2 > 255) v2 = 255;

            if (v3 < 0) v3 = 0;
            else if (v3 > 255) v3 = 255;

            return new Color((byte)v1, (byte)v2, (byte)v3, 255);
        }

        internal static Color FromArgb(int v, Color textColor)
        {
            throw new NotImplementedException();
        }
    }
}
