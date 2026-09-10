// ====================================================================================================
//  FILE: Vector4.cs
//  PATH: ./Engine/VectorMath/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Vector4 module.
//
//  RESPONSIBILITIES:
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide GetHashCode() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    Vector4.cs
Purpose: 4D vector type for SAS Zombie Assault TD.
Features: 4D vector operations, constructors, and utility methods.

P11-04-07-B: 4D vector type for color and other 4D data.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.VectorMath
{
    ///<summary>
    ///4D vector type for color values and other 4D data.
    ///</summary>
    public readonly struct Vector4 : IEquatable<Vector4>
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public float W { get; }

        public static readonly Vector4 Zero = new(0, 0, 0, 0);
        public static readonly Vector4 One = new(1, 1, 1, 1);
        public readonly float Length;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float Magnitude => (float)System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        public float MagnitudeSquared => X * X + Y * Y + Z * Z + W * W;
        public Vector4 Normalized => Magnitude > 0 ? this / Magnitude : Zero;

        public bool Equals(Vector4 other) =>
            System.Math.Abs(X - other.X) < 1e-6f &&
            System.Math.Abs(Y - other.Y) < 1e-6f &&
            System.Math.Abs(Z - other.Z) < 1e-6f &&
            System.Math.Abs(W - other.W) < 1e-6f;

        public override bool Equals(object obj) => obj is Vector4 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        public static Vector4 operator +(Vector4 left, Vector4 right) =>
            new(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);

        public static Vector4 operator -(Vector4 left, Vector4 right) =>
            new(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

        public static Vector4 operator *(Vector4 vector, float scalar) =>
            new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar, vector.W * scalar);

        public static Vector4 operator /(Vector4 vector, float scalar) =>
            scalar != 0 ? new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar, vector.W / scalar) : Zero;

        public static implicit operator Vector4(System.Drawing.Color color) =>
            new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

        public static implicit operator System.Drawing.Color(Vector4 vector) =>
            System.Drawing.Color.FromArgb(
                (int)(vector.W * 255f),
                (int)(vector.X * 255f),
                (int)(vector.Y * 255f),
                (int)(vector.Z * 255f)
            );
    }
}

