// ====================================================================================================
//  FILE: Size.cs
//  PATH: Engine/CoreSize/Size/
//  MODULE: Engine CoreSize Size
//
//  ROLE:
//      Provide canonical width/height data representation for the Core subsystem.
//
//  RESPONSIBILITIES:
//      - Store Width and Height.
//      - Provide basic geometric properties (AspectRatio, Area, Perimeter, Diagonal).
//      - Provide basic constructors for the Core subsystem.
//      - Provide equality behavior for the Core subsystem.
//      - Provide hashing behavior for the Core subsystem.
//      - Provide operator behavior for the Core subsystem.
//      - Provide string representation behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Fit/fill logic (handled by SizeFit.cs).
//      - Aspect-ratio logic (handled by SizeAspect.cs).
//      - Size math operations (handled by SizeOps.cs).
//      - Utility helpers (handled by SizeUtil.cs).
//      - Low-level data persistence or file serialization.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.CoreSize
{
    /// <summary>
    /// Canonical two-dimensional size type used throughout the engine.
    /// </summary>
    public sealed class Size : IEquatable<Size>
    {
        // ----------------------------------------------------------------------------------------------------
        // Properties
        // ----------------------------------------------------------------------------------------------------

        public float Width { get; }
        public float Height { get; }

        public float AspectRatio => Height > 0f ? Width / Height : 0f;
        public float Area => Width * Height;
        public float Perimeter => 2f * (Width + Height);
        public float Diagonal => (float)System.Math.Sqrt(Width * Width + Height * Height);

        public bool IsEmpty => Width <= 0f || Height <= 0f;
        public bool HasArea => Width > 0f && Height > 0f;

        // ----------------------------------------------------------------------------------------------------
        // Constructors
        // ----------------------------------------------------------------------------------------------------

        public Size(float width, float height)
        {
            Width = System.Math.Max(width, 0f);
            Height = System.Math.Max(height, 0f);
        }

        public Size(float size) : this(size, size) { }

        public Size(Size other)
        {
            if (other is null)
            {
                Width = 0f;
                Height = 0f;
            }
            else
            {
                Width = other.Width;
                Height = other.Height;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // Equality and Hashing
        // ----------------------------------------------------------------------------------------------------

        public bool Equals(Size other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return System.Math.Abs(Width - other.Width) < 0.001f &&
                   System.Math.Abs(Height - other.Height) < 0.001f;
        }

        public override bool Equals(object obj) => obj is Size other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Width, Height);

        // ----------------------------------------------------------------------------------------------------
        // Operators
        // ----------------------------------------------------------------------------------------------------

        public static bool operator ==(Size left, Size right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Size left, Size right) => !(left == right);

        public static Size operator +(Size left, Size right)
        {
            if (left is null || right is null) return left ?? right ?? new Size(0f, 0f);
            return new Size(left.Width + right.Width, left.Height + right.Height);
        }

        public static Size operator -(Size left, Size right)
        {
            if (left is null || right is null) return left ?? new Size(0f, 0f);

            return new Size(
                System.Math.Max(0f, left.Width - right.Width),
                System.Math.Max(0f, left.Height - right.Height));
        }

        public static Size operator *(Size size, float scale)
        {
            if (size is null) return new Size(0f, 0f);
            return new Size(size.Width * scale, size.Height * scale);
        }

        public static Size operator *(float scale, Size size)
        {
            if (size is null) return new Size(0f, 0f);
            return new Size(size.Width * scale, size.Height * scale);
        }

        public static Size operator /(Size size, float scale)
        {
            if (size is null || scale == 0f) return new Size(0f, 0f);
            return new Size(size.Width / scale, size.Height / scale);
        }

        public static implicit operator Vector2(Size v)
        {
            if (v is null) return Vector2.Zero;
            return new Vector2(v.Width, v.Height);
        }

        // ----------------------------------------------------------------------------------------------------
        // String Representation
        // ----------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return $"Size(Width: {Width:F1}, Height: {Height:F1})";
        }

        public string ToCompactString()
        {
            return $"{Width:F0}x{Height:F0}";
        }
    }
}
