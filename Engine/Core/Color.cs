/*
File:    Color.cs
Purpose: Canonical engine color type used by rendering, UI, particles, and effects.
         Unifies all color usage across the engine with comprehensive color operations.
         
Features: Complete color operations with type safety, performance optimization, and math integration.
          Supports ARGB and RGB color spaces, blending, and conversion operations.
          All engine code must use this unified Color type.

Created: Engine Core Implementation
Notes:   This replaces all fragmented color implementations across the engine.
         Provides conversion helpers for System.Drawing.Color when needed.
*/

using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Canonical engine color type representing RGBA color with single-precision floating point components.
    /// Used by rendering, UI, particles, and effects systems throughout the engine.
    /// </summary>
    public readonly struct Color : IEquatable<Color>
    {
        #region Components
        
        /// <summary>Red component (0.0 - 1.0).</summary>
        public readonly float R;
        
        /// <summary>Green component (0.0 - 1.0).</summary>
        public readonly float G;
        
        /// <summary>Blue component (0.0 - 1.0).</summary>
        public readonly float B;
        
        /// <summary>Alpha component (0.0 - 1.0, where 0.0 is transparent and 1.0 is opaque).</summary>
        public readonly float A;
        
        #endregion

        #region Predefined Colors
        
        /// <summary>Completely transparent color (0, 0, 0, 0).</summary>
        public static readonly Color Transparent = new(0f, 0f, 0f, 0f);
        
        /// <summary>Completely black color (0, 0, 0, 1).</summary>
        public static readonly Color Black = new(0f, 0f, 0f, 1f);
        
        /// <summary>Completely white color (1, 1, 1, 1).</summary>
        public static readonly Color White = new(1f, 1f, 1f, 1f);
        
        /// <summary>Pure red color (1, 0, 0, 1).</summary>
        public static readonly Color Red = new(1f, 0f, 0f, 1f);
        
        /// <summary>Pure green color (0, 1, 0, 1).</summary>
        public static readonly Color Green = new(0f, 1f, 0f, 1f);
        
        /// <summary>Pure blue color (0, 0, 1, 1).</summary>
        public static readonly Color Blue = new(0f, 0f, 1f, 1f);
        
        /// <summary>Pure yellow color (1, 1, 0, 1).</summary>
        public static readonly Color Yellow = new(1f, 1f, 0f, 1f);
        
        /// <summary>Pure magenta color (1, 0, 1, 1).</summary>
        public static readonly Color Magenta = new(1f, 0f, 1f, 1f);
        
        /// <summary>Gray color (0.5, 0.5, 0.5, 1).</summary>
        public static readonly Color Gray = new(0.5f, 0.5f, 0.5f, 1f);
        
        /// <summary>Light green color (0.5, 1.0, 0.5, 1).</summary>
        public static readonly Color LightGreen = new(0.5f, 1.0f, 0.5f, 1f);
        
        /// <summary>Light coral color (1.0, 0.5, 0.5, 1).</summary>
        public static readonly Color LightCoral = new(1.0f, 0.5f, 0.5f, 1f);
        
        /// <summary>Orange color (1.0, 0.5, 0.0, 1).</summary>
        public static readonly Color Orange = new(1.0f, 0.5f, 0.0f, 1f);
        
        /// <summary>Dark gray color (0.25, 0.25, 0.25, 1).</summary>
        public static readonly Color DarkGray = new(0.25f, 0.25f, 0.25f, 1f);
        
        /// <summary>Brown color (0.6, 0.3, 0.1, 1).</summary>
        public static readonly Color Brown = new(0.6f, 0.3f, 0.1f, 1f);
        
        /// <summary>Purple color (0.5, 0.0, 1.0, 1).</summary>
        public static readonly Color Purple = new(0.5f, 0.0f, 1.0f, 1f);
        
        /// <summary>Cyan color (0.0, 1.0, 1.0, 1).</summary>
        public static readonly Color Cyan = new(0f, 1f, 1f, 1f);
        
        /// <summary>Light gray color (0.827, 0.827, 0.827, 1).</summary>
        public static readonly Color LightGray = new(0.827f, 0.827f, 0.827f, 1f);
        
        /// <summary>Gold color (1.0, 0.843, 0.0, 1).</summary>
        public static readonly Color Gold = new(1f, 0.843f, 0f, 1f);
        
        /// <summary>Light blue color (0.678, 0.847, 0.902, 1).</summary>
        public static readonly Color LightBlue = new(0.678f, 0.847f, 0.902f, 1f);
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new color with specified RGB components and full alpha (1.0).
        /// </summary>
        /// <param name="r">Red component (0.0 - 1.0).</param>
        /// <param name="g">Green component (0.0 - 1.0).</param>
        /// <param name="b">Blue component (0.0 - 1.0).</param>
        public Color(float r, float g, float b) : this(r, g, b, 1f) { }
        
        /// <summary>
        /// Creates a new color with specified RGBA components.
        /// </summary>
        /// <param name="r">Red component (0.0 - 1.0).</param>
        /// <param name="g">Green component (0.0 - 1.0).</param>
        /// <param name="b">Blue component (0.0 - 1.0).</param>
        /// <param name="a">Alpha component (0.0 - 1.0).</param>
        public Color(float r, float g, float b, float a)
        {
            R = System.Math.Clamp(r, 0f, 1f);
            G = System.Math.Clamp(g, 0f, 1f);
            B = System.Math.Clamp(b, 0f, 1f);
            A = System.Math.Clamp(a, 0f, 1f);
        }
        
        /// <summary>
        /// Creates a new color from 32-bit ARGB integer value.
        /// </summary>
        /// <param name="argb">32-bit ARGB value (0xAARRGGBB format).</param>
        public Color(uint argb)
        {
            A = ((argb >> 24) & 0xFF) / 255f;
            R = ((argb >> 16) & 0xFF) / 255f;
            G = ((argb >> 8) & 0xFF) / 255f;
            B = (argb & 0xFF) / 255f;
        }
        
        /// <summary>
        /// Creates a new color from System.Drawing.Color for external API compatibility.
        /// </summary>
        /// <param name="color">System.Drawing.Color to convert.</param>
        public Color(System.Drawing.Color color)
        {
            R = color.R / 255f;
            G = color.G / 255f;
            B = color.B / 255f;
            A = color.A / 255f;
        }
        
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">Color to copy.</param>
        public Color(Color other)
        {
            R = other.R;
            G = other.G;
            B = other.B;
            A = other.A;
        }
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the grayscale value of this color (luminance).
        /// </summary>
        public float Grayscale => 0.299f * R + 0.587f * G + 0.114f * B;
        
        /// <summary>
        /// Gets the maximum component value.
        /// </summary>
        public float MaxComponent => System.Math.Max(System.Math.Max(R, G), B);
        
        /// <summary>
        /// Gets the minimum component value.
        /// </summary>
        public float MinComponent => System.Math.Min(System.Math.Min(R, G), B);
        
        /// <summary>
        /// Gets the color brightness (average of RGB components).
        /// </summary>
        public float Brightness => (R + G + B) / 3f;
        
        #endregion

        #region Color Operations
        
        /// <summary>
        /// Linearly interpolates between two colors.
        /// </summary>
        /// <param name="a">Start color.</param>
        /// <param name="b">End color.</param>
        /// <param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        /// <returns>Interpolated color.</returns>
        public static Color Lerp(Color a, Color b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            return new Color(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
            );
        }
        
        /// <summary>
        /// Multiplies two colors component-wise (for lighting calculations).
        /// </summary>
        /// <param name="a">First color.</param>
        /// <param name="b">Second color.</param>
        /// <returns>Multiplied color.</returns>
        public static Color Multiply(Color a, Color b)
        {
            return new Color(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
        }
        
        /// <summary>
        /// Adds two colors component-wise with clamping.
        /// </summary>
        /// <param name="a">First color.</param>
        /// <param name="b">Second color.</param>
        /// <returns>Added color.</returns>
        public static Color Add(Color a, Color b)
        {
            return new Color(
                System.Math.Clamp(a.R + b.R, 0f, 1f),
                System.Math.Clamp(a.G + b.G, 0f, 1f),
                System.Math.Clamp(a.B + b.B, 0f, 1f),
                System.Math.Clamp(a.A + b.A, 0f, 1f)
            );
        }
        
        /// <summary>
        /// Creates a color with adjusted brightness.
        /// </summary>
        /// <param name="brightness">Brightness factor (1.0 = normal, >1.0 = brighter, <1.0 = darker).</param>
        /// <returns>Brightness-adjusted color.</returns>
        public Color WithBrightness(float brightness)
        {
            return new Color(R * brightness, G * brightness, B * brightness, A);
        }
        
        /// <summary>
        /// Creates a color with adjusted alpha.
        /// </summary>
        /// <param name="alpha">New alpha value (0.0 - 1.0).</param>
        /// <returns>Alpha-adjusted color.</returns>
        public Color WithAlpha(float alpha)
        {
            return new Color(R, G, B, alpha);
        }
        
        /// <summary>
        /// Converts this color to grayscale while preserving alpha.
        /// </summary>
        /// <returns>Grayscale version of this color.</returns>
        public Color ToGrayscale()
        {
            return new Color(Grayscale, Grayscale, Grayscale, A);
        }
        
        #endregion

        #region Conversions
        
        /// <summary>
        /// Converts this engine Color to System.Drawing.Color for external API compatibility.
        /// </summary>
        /// <returns>System.Drawing.Color equivalent.</returns>
        public System.Drawing.Color ToSystemDrawingColor()
        {
            return System.Drawing.Color.FromArgb(
                (int)(A * 255),
                (int)(R * 255),
                (int)(G * 255),
                (int)(B * 255)
            );
        }
        
        /// <summary>
        /// Converts this color to 32-bit ARGB integer value.
        /// </summary>
        /// <returns>32-bit ARGB value (0xAARRGGBB format).</returns>
        public uint ToArgb()
        {
            return ((uint)(A * 255) << 24) |
                   ((uint)(R * 255) << 16) |
                   ((uint)(G * 255) << 8) |
                   (uint)(B * 255);
        }
        
        /// <summary>
        /// Converts this color to HTML hex string format.
        /// </summary>
        /// <returns>Hex string in format "#RRGGBB" or "#AARRGGBB" if alpha is not 1.0.</returns>
        public string ToHex()
        {
            if (System.Math.Abs(A - 1.0f) < 0.001f)
            {
                return $"#{(int)(R * 255):X2}{(int)(G * 255):X2}{(int)(B * 255):X2}";
            }
            else
            {
                return $"#{(int)(A * 255):X2}{(int)(R * 255):X2}{(int)(G * 255):X2}{(int)(B * 255):X2}";
            }
        }
        
        #endregion

        #region Equality and Hashing
        
        /// <summary>
        /// Determines if two colors are approximately equal within a small tolerance.
        /// </summary>
        /// <param name="other">Other color to compare.</param>
        /// <param name="tolerance">Comparison tolerance (default: 0.001).</param>
        /// <returns>True if colors are approximately equal.</returns>
        public bool Equals(Color other, float tolerance = 0.001f)
        {
            return System.Math.Abs(R - other.R) < tolerance &&
                   System.Math.Abs(G - other.G) < tolerance &&
                   System.Math.Abs(B - other.B) < tolerance &&
                   System.Math.Abs(A - other.A) < tolerance;
        }
        
        public bool Equals(Color other) => Equals(other, 0.001f);
        
        public override bool Equals(object obj) => obj is Color other && Equals(other);
        
        public override int GetHashCode() => System.HashCode.Combine(R, G, B, A);
        
        #endregion

        #region Operators
        
        public static bool operator ==(Color left, Color right) => left.Equals(right);
        public static bool operator !=(Color left, Color right) => !left.Equals(right);
        
        public static Color operator *(Color color, float scalar) => color.WithBrightness(scalar);
        public static Color operator *(float scalar, Color color) => color.WithBrightness(scalar);
        
        #endregion

        #region Static Constructors
        
        /// <summary>
        /// Creates a Color from ARGB byte values.
        /// </summary>
        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
        
        /// <summary>
        /// Creates a Color from ARGB int values.
        /// </summary>
        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }
        
        /// <summary>
        /// Creates a Color from 32-bit uint value.
        /// </summary>
        public static Color FromUint(uint value)
        {
            byte a = (byte)((value >> 24) & 0xFF);
            byte r = (byte)((value >> 16) & 0xFF);
            byte g = (byte)((value >> 8) & 0xFF);
            byte b = (byte)(value & 0xFF);
            return new Color(r, g, b, a);
        }
        
        #endregion

        #region String Representation
        
        public override string ToString()
        {
            return $"Color(R: {R:F3}, G: {G:F3}, B: {B:F3}, A: {A:F3})";
        }
        
        #endregion
    }
}