// =====================================================================================================
//  FILE: Vector2.cs
//  PATH: Engine/VectorMath/Vector2.cs
//  SUBSYSTEM: Vector Math — 2D Vector Operations
//
//  ROLE:
//      Represents a deterministic, immutable 2D vector with X and Y components.
//
//  RESPONSIBILITIES:
//      - Store and manipulate 2D vector data.
//      - Provide basic vector arithmetic operations.
//      - Provide vector normalization and length computation.
//      - Provide vector dot product and cross product.
//      - Provide vector equality and hashing.
//      - Provide vector serialization and deserialization.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  ARCHITECTURAL NOTES:
//      - Immutable readonly struct for deterministic value-type math.
//      - No heap allocation, no mutation, no hidden state.
//      - Mirrors System.Numerics.Vector2 semantics for engine-wide consistency.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.VectorMath
{
    public readonly struct Vector2 : IEquatable<Vector2>
    {
        // ---------------------------------------------------------------------------------------------
        //  PUBLIC IMMUTABLE FIELDS
        // ---------------------------------------------------------------------------------------------
        public float X { get; }
        public float Y { get; }

        // ---------------------------------------------------------------------------------------------
        //  CONSTRUCTORS
        // ---------------------------------------------------------------------------------------------
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        // Zero vector
        public static readonly Vector2 Zero = new Vector2(0f, 0f);

        // ---------------------------------------------------------------------------------------------
        //  BASIC ARITHMETIC
        // ---------------------------------------------------------------------------------------------
        public static Vector2 operator +(Vector2 a, Vector2 b)
            => new Vector2(a.X + b.X, a.Y + b.Y);

        public static Vector2 operator -(Vector2 a, Vector2 b)
            => new Vector2(a.X - b.X, a.Y - b.Y);

        public static Vector2 operator *(Vector2 a, float scalar)
            => new Vector2(a.X * scalar, a.Y * scalar);

        public static Vector2 operator /(Vector2 a, float scalar)
            => new Vector2(a.X / scalar, a.Y / scalar);

        // ---------------------------------------------------------------------------------------------
        //  LENGTH & NORMALIZATION
        // ---------------------------------------------------------------------------------------------
        public float Length
            => MathF.Sqrt(X * X + Y * Y);

        public float LengthSquared
            => X * X + Y * Y;

        public Vector2 Normalized
        {
            get
            {
                float len = Length;
                return len > 0f ? new Vector2(X / len, Y / len) : Zero;
            }
        }

        // ---------------------------------------------------------------------------------------------
        //  DOT & CROSS PRODUCTS
        // ---------------------------------------------------------------------------------------------
        public static float Dot(Vector2 a, Vector2 b)
            => a.X * b.X + a.Y * b.Y;

        // 2D cross product returns scalar magnitude
        public static float Cross(Vector2 a, Vector2 b)
            => a.X * b.Y - a.Y * b.X;

        // ---------------------------------------------------------------------------------------------
        //  EQUALITY & HASHING
        // ---------------------------------------------------------------------------------------------
        public bool Equals(Vector2 other)
            => X == other.X && Y == other.Y;

        public override bool Equals(object? obj)
            => obj is Vector2 other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public static bool operator ==(Vector2 left, Vector2 right)
            => left.Equals(right);

        public static bool operator !=(Vector2 left, Vector2 right)
            => !left.Equals(right);

        // ---------------------------------------------------------------------------------------------
        //  CONVERSIONS (BRIDGE TO SYSTEM.NUMERICS)
        // ---------------------------------------------------------------------------------------------
        public static implicit operator System.Numerics.Vector2(Vector2 v)
            => new System.Numerics.Vector2(v.X, v.Y);

        public static implicit operator Vector2(System.Numerics.Vector2 v)
            => new Vector2(v.X, v.Y);

        // ---------------------------------------------------------------------------------------------
        //  STRING REPRESENTATION
        // ---------------------------------------------------------------------------------------------
        public override string ToString()
            => $"({X}, {Y})";
    }
}
