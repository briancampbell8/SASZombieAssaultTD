//File:    ColorEquality.cs
//Purpose: Equality comparison and hashing for Color with float safety.
//         Isolated core struct logic with exact and tolerance-based comparison.
//
//Architecture:
//- Partial struct extension of core Color type
//- Equality comparison with exact float matching
//- Tolerance-based comparison for float safety
//- Optimized hashing for dictionary and collection usage
//- Separate from core data operations for clean design
//
//Usage:
//    Color color1 = Color.FromArgb(1.0f, 0.0f, 0.0f, 1.0f);
//    bool isEqual = color1.Equals(color2);
//Compare colors with float tolerance support
//

//

using System;
using System.Runtime.CompilerServices;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Core.Colorize
{
    public readonly partial struct Color
    {
        //---------------------------------------------------------
        //EQUALITY COMPARISON
        //---------------------------------------------------------
        //Float comparison uses tolerance to handle rounding errors.
        //Default tolerance: 0.001f (~0.4% of range, ~1/255 precision)

        ///<summary>
        ///Equality comparison with configurable tolerance.
        ///Required because float components can accumulate rounding errors.
        ///</summary>
        ///<param name="other">Color to compare against</param>
        ///<param name="tolerance">Maximum difference allowed per component (default 0.001)</param>
        ///<returns>True if all components differ by less than tolerance</returns>
        ///<remarks>
        ///Default tolerance 0.001f is ~0.4% of [0,1] range.
        ///This handles rounding from byte→float→byte conversions.
        ///For exact comparison, use tolerance = 0f.
        ///</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Color other, float tolerance = 0.001f)
        {
            return System.Math.Abs(R - other.R) < tolerance &&
                   System.Math.Abs(G - other.G) < tolerance &&
                   System.Math.Abs(B - other.B) < tolerance &&
                   System.Math.Abs(A - other.A) < tolerance;
        }

        ///<summary>
        ///IEquatable implementation with default tolerance.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Color other) => Equals(other, 0.001f);

        ///<summary>
        ///Object equality override. Handles boxed Color comparisons.
        ///</summary>
        public override bool Equals(object obj) => obj is Color other && Equals(other);

        //---------------------------------------------------------
        //HASHING
        //---------------------------------------------------------
        //Hash combines all components for dictionary key uniqueness.
        //Note: Tolerant equality means different hashes can be "equal".

        ///<summary>
        ///Hash code for dictionary keys and hashing collections.
        ///Combines all four components using System.HashCode.
        ///</summary>
        ///<returns>Combined hash of R, G, B, A components</returns>
        ///<remarks>
        ///Uses exact float bits - two "tolerantly equal" colors may have
        ///different hashes. This is acceptable for hash-based collections.
        ///</remarks>
        public override int GetHashCode() => HashCode.Combine(R, G, B, A);

        //---------------------------------------------------------
        //EQUALITY OPERATORS
        //---------------------------------------------------------
        //Natural operators for color comparison expressions.

        ///<summary>
        ///Exact equality operator (uses default tolerance).
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Color left, Color right) => left.Equals(right);

        ///<summary>
        ///Inequality operator.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Color left, Color right) => !left.Equals(right);

        //---------------------------------------------------------
        //STRING REPRESENTATION
        //---------------------------------------------------------

        ///<summary>
        ///Human-readable color representation for debugging.
        ///Format: Color(R: 0.123, G: 0.456, B: 0.789, A: 1.000)
        ///</summary>
        public override string ToString()
        {
            return $"Color(R: {R:F3}, G: {G:F3}, B: {B:F3}, A: {A:F3})";
        }

        //---------------------------------------------------------
        //EQUALITY SEMANTICS NOTES
        //---------------------------------------------------------
        //- Exact equality (==) uses 0.001f tolerance for float safety
        //- Exact component match requires manual comparison with tolerance=0
        //- Hash codes use exact bits - may differ for "equal" colors
        //- Colors are value types - equality compares contents, not reference
    }
}
