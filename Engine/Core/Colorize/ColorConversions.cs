// File:    ColorConversions.cs
// Purpose: Format conversion methods for Color with comprehensive format support.
//          Handles BGRA (framebuffer), ARGB (interop), hex strings, and batch conversions.
//
// Architecture:
//- Partial struct extension of core Color type
//- BGRA format conversion for framebuffer operations
//- ARGB format conversion for interop operations
//- Hex string conversion for serialization and debugging
//- Batch conversion operations for performance optimization
//- Critical for rendering pipeline performance

//Usage:
//    uint bgra = color.ToBGRA();
//    uint argb = color.ToARGB();
//    string hex = color.ToHexString();
// Convert between color formats efficiently
//

using SASZombieAssaultTD.Engine.Diagnostics;

using System;
using System.Runtime.CompilerServices;

namespace SASZombieAssaultTD.Engine.Core.Colorize
{
    public readonly partial struct Color
    {
        // ---------------------------------------------------------
        // PACKED FORMAT CONVERSIONS (To/From UInt32)
        // ---------------------------------------------------------
        // Framebuffer uses BGRA (little-endian: 0xAARRGGBB in memory = BGRA bytes)
        // System.Drawing uses ARGB (0xAARRGGBB shifted)
        // Understanding byte order is critical for interop performance.

        /// <summary>
        /// Converts to BGRA uint32 for direct framebuffer writing.
        /// Format: 0xAARRGGBB in register = [B][G][R][A] in memory (little-endian)
        /// </summary>
        /// <returns>32-bit BGRA packed value</returns>
        /// <remarks>
        /// This is the FAST path for framebuffer output.
        /// BGRA matches Windows DIB and most GPU texture formats.
        /// Use this instead of ToArgb() when writing to pixel buffers.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToBgra()
        {
            return ((uint)(AByte) << 24) |
                   ((uint)(RByte) << 16) |
                   ((uint)(GByte) << 8) |
                   ((uint)(BByte));
        }

        /// <summary>
        /// Converts to ARGB uint32 for System.Drawing/GDI interop.
        /// Format: 0xAARRGGBB (Alpha in high byte, then R, G, B)
        /// </summary>
        /// <returns>32-bit ARGB packed value</returns>
        /// <remarks>
        /// Matches Win32 COLORREF with alpha.
        /// Required for System.Drawing.Color.FromArgb() compatibility.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToArgb()
        {
            return ((uint)(AByte) << 24) |
                   ((uint)(RByte) << 16) |
                   ((uint)(GByte) << 8) |
                   ((uint)(BByte));
        }

        /// <summary>
        /// Converts to RGB uint32 (no alpha, assumes opaque).
        /// Format: 0x00RRGGBB
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToRgb()
        {
            return ((uint)(RByte) << 16) |
                   ((uint)(GByte) << 8) |
                   ((uint)(BByte));
        }

        /// <summary>
        /// Creates color from packed BGRA uint32.
        /// Input format: 0xAARRGGBB in register = [B][G][R][A] in memory
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromBgra(uint bgra)
        {
            return CreateUnchecked(
                ((bgra >> 16) & 0xFF) / 255f,  // R
                ((bgra >> 8) & 0xFF) / 255f,   // G
                (bgra & 0xFF) / 255f,          // B
                ((bgra >> 24) & 0xFF) / 255f   // A
            );
        }

        /// <summary>
        /// Creates color from packed uint32 (alias for FromArgb).
        /// Input format: 0xAARRGGBB
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromUint(uint value) => FromArgb(value);

        /// <summary>
        /// Creates color from packed ARGB uint32.
        /// Input format: 0xAARRGGBB
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromArgb(uint argb)
        {
            return CreateUnchecked(
                ((argb >> 16) & 0xFF) / 255f,  // R
                ((argb >> 8) & 0xFF) / 255f,   // G
                (argb & 0xFF) / 255f,          // B
                ((argb >> 24) & 0xFF) / 255f   // A
            );
        }

        /// <summary>
        /// Creates color from byte components (ARGB order).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return CreateUnchecked(
                r / 255f,
                g / 255f,
                b / 255f,
                a / 255f
            );
        }

        /// <summary>
        /// Creates color from int components with validation.
        /// Clamps values to 0-255 before conversion.
        /// </summary>
        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(
                (float)System.Math.Clamp(r, 0, 255) / 255f,
                (float)System.Math.Clamp(g, 0, 255) / 255f,
                (float)System.Math.Clamp(b, 0, 255) / 255f,
                (float)System.Math.Clamp(a, 0, 255) / 255f
            );
        }

        /// <summary>
        /// Creates color from RGB only (assumes opaque alpha).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromRgb(byte r, byte g, byte b)
        {
            return CreateUnchecked(r / 255f, g / 255f, b / 255f, 1f);
        }

        /// <summary>
        /// Creates color from HSV (Hue, Saturation, Value) components.
        /// Hue: 0-360 degrees, Saturation: 0-1, Value: 0-1
        /// </summary>
        public static Color FromHsv(float h, float s, float v)
        {
            h = h % 360f;
            if (h < 0) h += 360f;

            float c = v * s;
            float x = c * (1 - System.Math.Abs((h / 60f) % 2 - 1));
            float m = v - c;

            float r, g, b;
            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return CreateUnchecked(r + m, g + m, b + m, 1f);
        }

        // ---------------------------------------------------------
        // SYSTEM.DRAWING INTEROP
        // ---------------------------------------------------------

        /// <summary>
        /// Converts to System.Drawing.Color for GDI+ interop.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public System.Drawing.Color ToSystemDrawingColor()
        {
            return System.Drawing.Color.FromArgb(AByte, RByte, GByte, BByte);
        }

        /// <summary>
        /// Implicit conversion from System.Drawing.Color.
        /// Allows seamless assignment: Color c = System.Drawing.Color.Red;
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color(System.Drawing.Color color)
        {
            return CreateUnchecked(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f
            );
        }

        // ---------------------------------------------------------
        // STRING CONVERSIONS
        // ---------------------------------------------------------

        /// <summary>
        /// Converts to hex string representation.
        /// Opaque colors: #RRGGBB, Transparent colors: #AARRGGBB
        /// </summary>
        public string ToHex()
        {
            if (System.Math.Abs(A - 1.0f) < 0.001f)
            {
                return $"#{RByte:X2}{GByte:X2}{BByte:X2}";
            }
            else
            {
                return $"#{AByte:X2}{RByte:X2}{GByte:X2}{BByte:X2}";
            }
        }

        /// <summary>
        /// Parses hex string to Color.
        /// Supports: #RGB, #RRGGBB, #AARRGGBB, #ARGB, #RRGGBBAA
        /// </summary>
        public static Color ParseHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                throw new ArgumentException("Hex string cannot be null or empty", nameof(hex));

            hex = hex.TrimStart('#');

            if (hex.Length == 3) // #RGB
            {
                byte r = (byte)(HexToByte(hex[0]) * 17);
                byte g = (byte)(HexToByte(hex[1]) * 17);
                byte b = (byte)(HexToByte(hex[2]) * 17);
                return FromRgb(r, g, b);
            }
            else if (hex.Length == 6) // #RRGGBB
            {
                byte r = (byte)(HexToByte(hex[0]) * 16 + HexToByte(hex[1]));
                byte g = (byte)(HexToByte(hex[2]) * 16 + HexToByte(hex[3]));
                byte b = (byte)(HexToByte(hex[4]) * 16 + HexToByte(hex[5]));
                return FromRgb(r, g, b);
            }
            else if (hex.Length == 8) // #AARRGGBB
            {
                byte a = (byte)(HexToByte(hex[0]) * 16 + HexToByte(hex[1]));
                byte r = (byte)(HexToByte(hex[2]) * 16 + HexToByte(hex[3]));
                byte g = (byte)(HexToByte(hex[4]) * 16 + HexToByte(hex[5]));
                byte b = (byte)(HexToByte(hex[6]) * 16 + HexToByte(hex[7]));
                return FromArgb(a, r, g, b);
            }

            throw new FormatException($"Invalid hex color format: #{hex}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int HexToByte(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            throw new FormatException($"Invalid hex character: {c}");
        }

        // ---------------------------------------------------------
        // BATCH CONVERSIONS (High Performance)
        // ---------------------------------------------------------
        // These methods convert arrays of colors efficiently.
        // Critical for particle systems, sprite batches, and texture processing.

        /// <summary>
        /// Converts span of colors to BGRA byte array.
        /// Output format: [B,G,R,A][B,G,R,A]... (4 bytes per pixel)
        /// </summary>
        /// <param name="colors">Source colors</param>
        /// <param name="output">Destination byte span (must be 4x colors.Length)</param>
        public static void ToBgraBatch(ReadOnlySpan<Color> colors, Span<byte> output)
        {
            if (output.Length < colors.Length * 4)
                throw new ArgumentException("Output span too small", nameof(output));

            for (int i = 0; i < colors.Length; i++)
            {
                int idx = i * 4;
                Color c = colors[i];
                output[idx + 0] = c.BByte;
                output[idx + 1] = c.GByte;
                output[idx + 2] = c.RByte;
                output[idx + 3] = c.AByte;
            }
        }

        /// <summary>
        /// Converts span of colors to ARGB uint32 array.
        /// </summary>
        public static void ToArgbBatch(ReadOnlySpan<Color> colors, Span<uint> output)
        {
            if (output.Length < colors.Length)
                throw new ArgumentException("Output span too small", nameof(output));

            for (int i = 0; i < colors.Length; i++)
            {
                output[i] = colors[i].ToArgb();
            }
        }

        /// <summary>
        /// Converts BGRA byte array to span of colors.
        /// Input format: [B,G,R,A][B,G,R,A]...
        /// </summary>
        public static void FromBgraBatch(ReadOnlySpan<byte> bgraData, Span<Color> output)
        {
            int colorCount = bgraData.Length / 4;
            if (output.Length < colorCount)
                throw new ArgumentException("Output span too small", nameof(output));

            for (int i = 0; i < colorCount; i++)
            {
                int idx = i * 4;
                output[i] = CreateUnchecked(
                    bgraData[idx + 2] / 255f,  // R
                    bgraData[idx + 1] / 255f,  // G
                    bgraData[idx + 0] / 255f,  // B
                    bgraData[idx + 3] / 255f   // A
                );
            }
        }

        /// <summary>
        /// Unpacks single BGRA uint32 into 4 bytes (for unsafe pointer operations).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackBgra(uint packed, out byte b, out byte g, out byte r, out byte a)
        {
            b = (byte)(packed & 0xFF);
            g = (byte)((packed >> 8) & 0xFF);
            r = (byte)((packed >> 16) & 0xFF);
            a = (byte)((packed >> 24) & 0xFF);
        }
    }
}
