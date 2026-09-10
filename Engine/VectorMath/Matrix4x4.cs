// ====================================================================================================
//  FILE: Matrix4x4.cs
//  PATH: ./Engine/VectorMath/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Matrix4x4 module.
//
//  RESPONSIBILITIES:
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide GetHashCode() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    Matrix4x4.cs
Purpose: 4x4 matrix type for 3D transformations and camera projections.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.VectorMath
{
    ///<summary>
    ///4x4 matrix type for 3D transformations and camera projections.
    ///</summary>
    public readonly struct Matrix4x4 : IEquatable<Matrix4x4>
    {
        public float M11 { get; }
        public float M12 { get; }
        public float M13 { get; }
        public float M14 { get; }
        public float M21 { get; }
        public float M22 { get; }
        public float M23 { get; }
        public float M24 { get; }
        public float M31 { get; }
        public float M32 { get; }
        public float M33 { get; }
        public float M34 { get; }
        public float M41 { get; }
        public float M42 { get; }
        public float M43 { get; }
        public float M44 { get; }

        public static readonly Matrix4x4 IdECSEntityCore = new Matrix4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        public Matrix4x4(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }

        public static Matrix4x4 operator +(Matrix4x4 left, Matrix4x4 right)
        {
            return new Matrix4x4(
                left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13, left.M14 + right.M14,
                left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23, left.M24 + right.M24,
                left.M31 + right.M31, left.M32 + right.M32, left.M33 + right.M33, left.M34 + right.M34,
                left.M41 + right.M41, left.M42 + right.M42, left.M43 + right.M43, left.M44 + right.M44
            );
        }

        public static Matrix4x4 operator -(Matrix4x4 left, Matrix4x4 right)
        {
            return new Matrix4x4(
                left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13, left.M14 - right.M14,
                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23, left.M24 - right.M24,
                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33, left.M34 - right.M34,
                left.M41 - right.M41, left.M42 - right.M42, left.M43 - right.M43, left.M44 - right.M44
            );
        }

        public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
        {
            return new Matrix4x4(
                left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41,
                left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42,
                left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43,
                left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44,
                left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41,
                left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42,
                left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43,
                left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44,
                left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41,
                left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42,
                left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43,
                left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44,
                left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41,
                left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42,
                left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43,
                left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44
            );
        }

        public static Matrix4x4 operator *(Matrix4x4 matrix, float scalar)
        {
            return new Matrix4x4(
                matrix.M11 * scalar, matrix.M12 * scalar, matrix.M13 * scalar, matrix.M14 * scalar,
                matrix.M21 * scalar, matrix.M22 * scalar, matrix.M23 * scalar, matrix.M24 * scalar,
                matrix.M31 * scalar, matrix.M32 * scalar, matrix.M33 * scalar, matrix.M34 * scalar,
                matrix.M41 * scalar, matrix.M42 * scalar, matrix.M43 * scalar, matrix.M44 * scalar
            );
        }

        public static Matrix4x4 operator *(float scalar, Matrix4x4 matrix)
        {
            return matrix * scalar;
        }

        public static bool operator ==(Matrix4x4 left, Matrix4x4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix4x4 left, Matrix4x4 right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Matrix4x4(System.Numerics.Matrix3x2 v)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Matrix4x4 other)
        {
            return global::System.Math.Abs(M11 - other.M11) < 1e-6f &&
                   global::System.Math.Abs(M12 - other.M12) < 1e-6f &&
                   global::System.Math.Abs(M13 - other.M13) < 1e-6f &&
                   global::System.Math.Abs(M14 - other.M14) < 1e-6f &&
                   global::System.Math.Abs(M21 - other.M21) < 1e-6f &&
                   global::System.Math.Abs(M22 - other.M22) < 1e-6f &&
                   global::System.Math.Abs(M23 - other.M23) < 1e-6f &&
                   global::System.Math.Abs(M24 - other.M24) < 1e-6f &&
                   global::System.Math.Abs(M31 - other.M31) < 1e-6f &&
                   global::System.Math.Abs(M32 - other.M32) < 1e-6f &&
                   global::System.Math.Abs(M33 - other.M33) < 1e-6f &&
                   global::System.Math.Abs(M34 - other.M34) < 1e-6f &&
                   global::System.Math.Abs(M41 - other.M41) < 1e-6f &&
                   global::System.Math.Abs(M42 - other.M42) < 1e-6f &&
                   global::System.Math.Abs(M43 - other.M43) < 1e-6f &&
                   global::System.Math.Abs(M44 - other.M44) < 1e-6f;
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix4x4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            //Add the first 8 elements
            hash.Add(M11); hash.Add(M12); hash.Add(M13); hash.Add(M14);
            hash.Add(M21); hash.Add(M22); hash.Add(M23); hash.Add(M24);

            //Add the remaining 8 elements
            hash.Add(M31); hash.Add(M32); hash.Add(M33); hash.Add(M34);
            hash.Add(M41); hash.Add(M42); hash.Add(M43); hash.Add(M44);

            return hash.ToHashCode();
        }


        public override string ToString()
        {
            return $"[{M11}, {M12}, {M13}, {M14}]\n[{M21}, {M22}, {M23}, {M24}]\n[{M31}, {M32}, {M33}, {M34}]\n[{M41}, {M42}, {M43}, {M44}]";
        }
    }
}

