// ====================================================================================================
//  FILE: Vector3Types.cs
//  PATH: ./Engine/VectorMath/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Vector3Types module.
//
//  RESPONSIBILITIES:
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide GetHashCode() behavior for the Core subsystem.
//      - Provide Dot() behavior for the Core subsystem.
//      - Provide Cross() behavior for the Core subsystem.
//      - Provide Distance() behavior for the Core subsystem.
//      - Provide DistanceSquared() behavior for the Core subsystem.
//      - Provide Lerp() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide Parse() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.VectorMath
{
    ///<summary>
    ///Canonical Vector3 type - single authoritative 3D vector for entire engine.
    ///All engine code must use this type to avoid namespace conflicts.
    ///</summary>
    public readonly struct Vector3 : IEquatable<Vector3>
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public static readonly Vector3 Zero = new(0, 0, 0);
        public static readonly Vector3 One = new(1, 1, 1);
        public static readonly Vector3 UnitX = new(1, 0, 0);
        public static readonly Vector3 UnitY = new(0, 1, 0);
        public static readonly Vector3 UnitZ = new(0, 0, 1);
        public static readonly Vector3 Forward = new(0, 0, 1);
        public static readonly Vector3 Backward = new(0, 0, -1);
        public static readonly Vector3 Left = new(-1, 0, 0);
        public static readonly Vector3 Right = new(1, 0, 0);
        public static readonly Vector3 Up = new(0, 1, 0);
        public static readonly Vector3 Down = new(0, -1, 0);
        public static readonly Vector3 Min = new(float.MinValue, float.MinValue, float.MinValue);
        public static readonly Vector3 Max = new(float.MaxValue, float.MaxValue, float.MaxValue);
        internal readonly float Length;
        private readonly object x;
        private readonly int v;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(object x, int v) : this()
        {
            this.x = x;
            this.v = v;
        }

        public float Magnitude => (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
        public float MagnitudeSquared => X * X + Y * Y + Z * Z;
        public Vector3 Normalized => Magnitude > 0 ? this / Magnitude : Zero;

        public bool Equals(Vector3 other) =>
            System.Math.Abs(X - other.X) < 1e-6f &&
            System.Math.Abs(Y - other.Y) < 1e-6f &&
            System.Math.Abs(Z - other.Z) < 1e-6f;

        public override bool Equals(object obj) => obj is Vector3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static Vector3 operator +(Vector3 left, Vector3 right) =>
            new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        public static Vector3 operator -(Vector3 left, Vector3 right) =>
            new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        public static Vector3 operator *(Vector3 vector, float scalar) =>
            new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

        public static Vector3 operator *(float scalar, Vector3 vector) =>
            new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

        public static Vector3 operator /(Vector3 vector, float scalar) =>
            scalar != 0 ? new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar) : Zero;

        //1) Component-wise multiply (fixes Vector3 *= Vector3 / Vector3 * Vector3)
        public static Vector3 operator *(Vector3 a, Vector3 b) =>
            new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        //2) float - Vector3
        public static Vector3 operator -(float a, Vector3 b) =>
            new(a - b.X, a - b.Y, a - b.Z);

        //3) float + Vector3
        public static Vector3 operator +(float a, Vector3 b) =>
            new(a + b.X, a + b.Y, a + b.Z);

        public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);
        public static bool operator !=(Vector3 left, Vector3 right) => !left.Equals(right);

        public static implicit operator Vector3(System.Numerics.Vector3 v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator System.Numerics.Vector3(Vector3 v)
        {
            throw new NotImplementedException();
        }

        public static explicit operator System.Numerics.Vector2(Vector3 v)
        {
            throw new NotImplementedException();
        }

        public static float Dot(Vector3 left, Vector3 right) =>
            left.X * right.X + left.Y * right.Y + left.Z * right.Z;

        public static Vector3 Cross(Vector3 left, Vector3 right) =>
            new(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X
            );

        public static float Distance(Vector3 left, Vector3 right) =>
            (left - right).Magnitude;

        public static float DistanceSquared(Vector3 left, Vector3 right) =>
            (left - right).MagnitudeSquared;

        public static Vector3 Lerp(Vector3 from, Vector3 to, float t) =>
            from + (to - from) * System.Math.Clamp(t, 0f, 1f);

        public override string ToString() => $"({X}, {Y}, {Z})";

        ///<summary>
        ///Parses a string into a Vector3.
        ///</summary>
        ///<param name="s">String to parse in format "X,Y,Z"</param>
        ///<returns>Parsed Vector3</returns>
        public static Vector3 Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException("String cannot be null or empty", nameof(s));

            var parts = s.Split(',');
            if (parts.Length != 3)
                throw new FormatException("Input string must be in the format 'X,Y,Z'");

            return new Vector3(
                float.Parse(parts[0]),
                float.Parse(parts[1]),
                float.Parse(parts[2])
            );
        }
    }
}

