// =====================================================================================================
//  FILE: ColorRGBA.cs
//  PATH: Engine/Core/ColorRGBA.cs
//  SUBSYSTEM: Core Engine Utilities
//
//  ROLE:
//      Provides a deterministic, immutable RGBA color structure used across all rendering subsystems.
//      Serves as the engine‑standard color representation for sprites, primitives, UI, text rendering,
//      and D3D11 adapter operations.
//
//  RESPONSIBILITIES:
//      - Store red, green, blue, and alpha channels in a compact immutable format.
//      - Provide safe construction from byte, int, float, and hex inputs.
//      - Supply engine‑wide predefined color constants for common rendering operations.
//      - Offer deterministic conversion helpers (float4, normalized floats, premultiply alpha).
//      - Provide blending, lerp, and arithmetic color operations without introducing rendering‑layer dependencies.
//      - Ensure compatibility with D3D11Adapter_Core and all Option‑B rendering subsystems.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU‑level color operations or shader manipulation.
//      - Managing textures, materials, or render targets.
//      - Handling color spaces beyond standard 0–255 RGBA.
//      - Providing gradient generation, palette management, or theme systems.
//
//  ARCHITECTURAL NOTES:
//      - Immutable by design to guarantee deterministic behavior across frames.
//      - Lightweight and dependency‑free for use in all engine subsystems.
//      - Intended as the foundational color type for Option‑B rendering architecture.
//      - Complements higher‑level color utilities but does not replace them.
// =====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Immutable RGBA color structure used throughout the engine.
    /// </summary>
    public readonly struct ColorRGBA
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;
        public readonly byte A;

        // --------------------------------------------------------------------------------------------
        // CONSTRUCTORS
        // --------------------------------------------------------------------------------------------

        public ColorRGBA(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static ColorRGBA FromInts(int r, int g, int b, int a)
        {
            return new ColorRGBA(
                (byte)r,
                (byte)g,
                (byte)b,
                (byte)a);
        }

        public static ColorRGBA FromFloats(float r, float g, float b, float a)
        {
            return new ColorRGBA(
                (byte)(r * 255f),
                (byte)(g * 255f),
                (byte)(b * 255f),
                (byte)(a * 255f));
        }

        public static ColorRGBA FromHex(string hex)
        {
            hex = hex.Replace("#", "").Trim();

            if (hex.Length == 6)
            {
                return new ColorRGBA(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16),
                    255);
            }
            else if (hex.Length == 8)
            {
                return new ColorRGBA(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16),
                    Convert.ToByte(hex.Substring(6, 2), 16));
            }

            throw new ArgumentException("Invalid hex color format.");
        }

        // --------------------------------------------------------------------------------------------
        // PREDEFINED ENGINE COLORS
        // --------------------------------------------------------------------------------------------

        public static readonly ColorRGBA White = new ColorRGBA(255, 255, 255, 255);
        public static readonly ColorRGBA Black = new ColorRGBA(0, 0, 0, 255);
        public static readonly ColorRGBA Transparent = new ColorRGBA(0, 0, 0, 0);

        public static readonly ColorRGBA Red = new ColorRGBA(255, 0, 0, 255);
        public static readonly ColorRGBA Green = new ColorRGBA(0, 255, 0, 255);
        public static readonly ColorRGBA Blue = new ColorRGBA(0, 0, 255, 255);

        public static readonly ColorRGBA Yellow = new ColorRGBA(255, 255, 0, 255);
        public static readonly ColorRGBA Cyan = new ColorRGBA(0, 255, 255, 255);
        public static readonly ColorRGBA Magenta = new ColorRGBA(255, 0, 255, 255);

        // --------------------------------------------------------------------------------------------
        // FLOAT4 / NORMALIZED CONVERSION
        // --------------------------------------------------------------------------------------------

        public (float r, float g, float b, float a) ToFloat4()
        {
            return (
                R / 255f,
                G / 255f,
                B / 255f,
                A / 255f
            );
        }

        // --------------------------------------------------------------------------------------------
        // PREMULTIPLY / UNPREMULTIPLY ALPHA
        // --------------------------------------------------------------------------------------------

        public ColorRGBA PremultiplyAlpha()
        {
            float a = A / 255f;
            return new ColorRGBA(
                (byte)(R * a),
                (byte)(G * a),
                (byte)(B * a),
                A);
        }

        public ColorRGBA UnpremultiplyAlpha()
        {
            if (A == 0)
                return new ColorRGBA(0, 0, 0, 0);

            float invA = 255f / A;
            return new ColorRGBA(
                (byte)(R * invA),
                (byte)(G * invA),
                (byte)(B * invA),
                A);
        }

        // --------------------------------------------------------------------------------------------
        // COLOR ARITHMETIC
        // --------------------------------------------------------------------------------------------

        public ColorRGBA Add(ColorRGBA other)
        {
            return new ColorRGBA(
                (byte)System.Math.Min(R + other.R, 255),
                (byte)System.Math.Min(G + other.G, 255),
                (byte)System.Math.Min(B + other.B, 255),
                (byte)System.Math.Min(A + other.A, 255));
        }

        public ColorRGBA Multiply(float scalar)
        {
            return new ColorRGBA(
                (byte)System.Math.Clamp(R * scalar, 0, 255),
                (byte)System.Math.Clamp(G * scalar, 0, 255),
                (byte)System.Math.Clamp(B * scalar, 0, 255),
                (byte)System.Math.Clamp(A * scalar, 0, 255));
        }

        public ColorRGBA WithAlpha(byte alpha)
        {
            return new ColorRGBA(R, G, B, alpha);
        }

        // --------------------------------------------------------------------------------------------
        // LERP / BLEND
        // --------------------------------------------------------------------------------------------

        public static ColorRGBA Lerp(ColorRGBA a, ColorRGBA b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);

            return new ColorRGBA(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t),
                (byte)(a.A + (b.A - a.A) * t));
        }

        public static ColorRGBA Blend(ColorRGBA src, ColorRGBA dst)
        {
            float sa = src.A / 255f;
            float da = dst.A / 255f;

            float outA = sa + da * (1f - sa);

            if (outA <= 0f)
                return Transparent;

            float r = (src.R * sa + dst.R * da * (1f - sa)) / outA;
            float g = (src.G * sa + dst.G * da * (1f - sa)) / outA;
            float b = (src.B * sa + dst.B * da * (1f - sa)) / outA;

            return new ColorRGBA(
                (byte)r,
                (byte)g,
                (byte)b,
                (byte)(outA * 255f));
        }

        // --------------------------------------------------------------------------------------------
        // EQUALITY + HASHCODE
        // --------------------------------------------------------------------------------------------

        public override bool Equals(object? obj)
        {
            if (obj is ColorRGBA c)
                return R == c.R && G == c.G && B == c.B && A == c.A;

            return false;
        }

        public override int GetHashCode()
        {
            return (R << 24) | (G << 16) | (B << 8) | A;
        }

        public override string ToString()
        {
            return $"RGBA({R}, {G}, {B}, {A})";
        }
    }
}
