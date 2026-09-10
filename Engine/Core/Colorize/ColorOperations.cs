// ====================================================================================================
//  FILE: ColorOperations.cs
//  PATH: ./Engine/Core/Colorize/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ColorOperations module.
//
//  RESPONSIBILITIES:
//      - Provide Lerp() behavior for the Core subsystem.
//      - Provide Multiply() behavior for the Core subsystem.
//      - Provide Add() behavior for the Core subsystem.
//      - Provide Subtract() behavior for the Core subsystem.
//      - Provide WithBrightness() behavior for the Core subsystem.
//      - Provide WithAlpha() behavior for the Core subsystem.
//      - Provide ToGrayscale() behavior for the Core subsystem.
//      - Provide Invert() behavior for the Core subsystem.
//      - Provide Saturate() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//File:    ColorOperations.cs
//Purpose: Hot-path color math operations with optimization and SIMD readiness.
//         Provides aggressively inlined methods for tight rendering loops.
//
//Architecture:
//- Partial struct extension of core Color type
//- Hot-path operations for performance-critical rendering
//- Aggressive inlining for tight loop optimization
//- SIMD-ready design for future vectorization
//- Isolated for optimization and compiler analysis
//
//Usage:
//    Color blended = Color.Lerp(color1, color2, 0.5f);
//    Color brightened = color1.Brighten(0.2f);
//    //High-performance color operations
//

//

using System.Runtime.CompilerServices;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.CoreSize.Colorize
{
    public readonly partial struct Color
    {
        //---------------------------------------------------------
        //BLENDING OPERATIONS (Hot Path)
        //---------------------------------------------------------
        //These operations are called per-pixel in particle systems,
        //UI animations, and effect shaders. Performance critical.

        ///<summary>
        ///Linear interpolation between two colors.
        ///t=0 returns 'a', t=1 returns 'b', t=0.5 returns midpoint.
        ///</summary>
        ///<param name="a">Start color</param>
        ///<param name="b">End color</param>
        ///<param name="t">Interpolation factor (0.0 to 1.0, auto-clamped)</param>
        ///<returns>Interpolated color</returns>
        ///<remarks>
        ///SIMD Optimization: Can be vectorized as: result = a + (b - a) * t
        ///All 4 components processed in parallel with Vector4.
        ///</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Lerp(Color a, Color b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            //Unchecked creation - Lerp of valid colors is always valid
            return CreateUnchecked(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
            );
        }

        ///<summary>
        ///Component-wise multiplication (modulate blend).
        ///Used for tinting and multiplicative color effects.
        ///</summary>
        ///<remarks>
        ///Formula: result = a × b (per component)
        ///Common use: Texture tinting, shadow darkening.
        ///</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Multiply(Color a, Color b)
        {
            return CreateUnchecked(
                a.R * b.R,
                a.G * b.G,
                a.B * b.B,
                a.A * b.A
            );
        }

        ///<summary>
        ///Component-wise addition with clamping.
        ///Used for additive lighting effects.
        ///</summary>
        ///<remarks>
        ///Formula: result = clamp(a + b, 0, 1)
        ///Note: Clamping required because addition can exceed 1.0.
        ///Common use: Glow effects, light accumulation.
        ///</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Add(Color a, Color b)
        {
            return CreateUnchecked(
                System.Math.Clamp(a.R + b.R, 0f, 1f),
                System.Math.Clamp(a.G + b.G, 0f, 1f),
                System.Math.Clamp(a.B + b.B, 0f, 1f),
                System.Math.Clamp(a.A + b.A, 0f, 1f)
            );
        }

        ///<summary>
        ///Component-wise subtraction with clamping.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Subtract(Color a, Color b)
        {
            return CreateUnchecked(
                System.Math.Clamp(a.R - b.R, 0f, 1f),
                System.Math.Clamp(a.G - b.G, 0f, 1f),
                System.Math.Clamp(a.B - b.B, 0f, 1f),
                System.Math.Clamp(a.A - b.A, 0f, 1f)
            );
        }

        //---------------------------------------------------------
        //COLOR MODIFICATION (Instance Methods)
        //---------------------------------------------------------

        ///<summary>
        ///Returns new color with scaled brightness.
        ///Multiplies R, G, B by factor. Alpha unchanged.
        ///</summary>
        ///<param name="brightness">Scale factor (0.0 = black, 1.0 = unchanged, >1.0 = brighten)</param>
        ///<returns>Brightness-adjusted color</returns>
        ///<remarks>
        ///Used by: Hover effects, disabled UI states, lighting.
        ///SIMD: Vector3 multiply for RGB components.
        ///</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color WithBrightness(float brightness)
        {
            return CreateUnchecked(
                R * brightness,
                G * brightness,
                B * brightness,
                A
            );
        }

        ///<summary>
        ///Returns new color with replaced alpha.
        ///</summary>
        ///<param name="alpha">New alpha value (0.0 = transparent, 1.0 = opaque)</param>
        ///<returns>Alpha-adjusted color with same RGB</returns>
        ///<remarks>
        ///Used by: Fade in/out animations, opacity transitions.
        ///More efficient than creating Color.FromArgb(R, G, B, alpha).
        ///</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color WithAlpha(float alpha)
        {
            return CreateUnchecked(R, G, B, alpha);
        }

        ///<summary>
        ///Converts to grayscale using luminance weights.
        ///</summary>
        ///<returns>Grayscale color (R=G=B=luminance, A preserved)</returns>
        ///<remarks>
        ///Uses ITU-R BT.709 weights: 0.299R + 0.587G + 0.114B
        ///Common use: Desaturation effects, monochrome modes.
        ///</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color ToGrayscale()
        {
            float lum = Grayscale; //Uses 0.299R + 0.587G + 0.114B
            return CreateUnchecked(lum, lum, lum, A);
        }

        ///<summary>
        ///Inverts the color (1.0 - component).
        ///</summary>
        ///<returns>Inverted color (negative effect)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color Invert()
        {
            return CreateUnchecked(1f - R, 1f - G, 1f - B, A);
        }

        ///<summary>
        ///Saturates the color toward grayscale (0.0 = full grayscale, 1.0 = unchanged).
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color Saturate(float saturation)
        {
            float lum = Grayscale;
            return CreateUnchecked(
                lum + (R - lum) * saturation,
                lum + (B - lum) * saturation,
                lum + (B - lum) * saturation,
                A
            );
        }

        //---------------------------------------------------------
        //OPERATOR OVERLOADS
        //---------------------------------------------------------
        //Provide natural math syntax for color operations.
        //All operators delegate to optimized static methods.

        ///<summary>
        ///Scalar multiplication: color * factor
        ///Scales RGB components uniformly (brightness adjustment).
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator *(Color color, float scalar)
        {
            return color.WithBrightness(scalar);
        }

        ///<summary>
        ///Scalar multiplication: factor * color
        ///Commutative version of color * scalar.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator *(float scalar, Color color)
        {
            return color.WithBrightness(scalar);
        }

        ///<summary>
        ///Component-wise addition: color1 + color2
        ///Clamped to [0, 1] range.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator +(Color a, Color b)
        {
            return Add(a, b);
        }

        ///<summary>
        ///Component-wise subtraction: color1 - color2
        ///Clamped to [0, 1] range.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator -(Color a, Color b)
        {
            return Subtract(a, b);
        }

        ///<summary>
        ///Component-wise multiplication (modulate): color1 * color2
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator *(Color a, Color b)
        {
            return Multiply(a, b);
        }

        ///<summary>
        ///Scalar division: color / divisor
        ///Equivalent to color * (1/divisor).
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator /(Color color, float divisor)
        {
            return color.WithBrightness(1f / divisor);
        }

        ///<summary>
        ///Unary negation (inversion): -color
        ///Returns color with inverted RGB components.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color operator -(Color color)
        {
            return color.Invert();
        }
    }
}

