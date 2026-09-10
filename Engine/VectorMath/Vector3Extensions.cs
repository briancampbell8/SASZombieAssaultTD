// ====================================================================================================
//  FILE: Vector3Extensions.cs
//  PATH: Engine/VectorMath/Vector3Extensions.cs
//  MODULE: VectorMath
//
//  ROLE:
//      Provides deterministic, subsystem‑agnostic math helpers for Vector3 operations. These helpers
//      support lightweight arithmetic, clamping, projection, normalization, reflection, rotation,
//      and directional math without introducing cross‑subsystem dependencies.
//
//  RESPONSIBILITIES:
//      - Provide Length() behavior for the Core subsystem.
//      - Provide Normalize() behavior for the Core subsystem.
//      - Provide Min() behavior for the Core subsystem.
//      - Provide Max() behavior for the Core subsystem.
//      - Provide Clamp() behavior for the Core subsystem.
//      - Provide Lerp() behavior for the Core subsystem.
//      - Provide DistanceSquared() behavior for the Core subsystem.
//      - Provide MoveTowards() behavior for the Core subsystem.
//      - Provide Reflect() behavior for the Core subsystem.
//      - Provide Project() behavior for the Core subsystem.
//      - Provide ProjectOnPlane() behavior for the Core subsystem.
//      - Provide Angle() behavior for the Core subsystem.
//      - Provide SignedAngle() behavior for the Core subsystem.
//      - Provide RotateY() behavior for the Core subsystem.
//      - Provide RotateX() behavior for the Core subsystem.
//      - Provide RotateZ() behavior for the Core subsystem.
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
    /// <summary>
    /// Deterministic extension methods for Vector3 providing compatibility with legacy math behavior.
    /// </summary>
    public static class Vector3Extensions
    {
        // ----------------------------------------------------------------------------------------------
        //  CONSTANTS
        // ----------------------------------------------------------------------------------------------

        public static Vector3 Zero => new Vector3(0f, 0f, 0f);

        // ----------------------------------------------------------------------------------------------
        //  BASIC MAGNITUDE / NORMALIZATION
        // ----------------------------------------------------------------------------------------------

        public static float Length(this Vector3 v)
        {
            return MathF.Sqrt((v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z));
        }

        public static Vector3 Normalize(this Vector3 v)
        {
            float mag = v.Length();
            if (mag <= 0f)
                return Zero;

            return new Vector3(v.X / mag, v.Y / mag, v.Z / mag);
        }

        // ----------------------------------------------------------------------------------------------
        //  MIN / MAX / CLAMP
        // ----------------------------------------------------------------------------------------------

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(
                MathF.Min(a.X, b.X),
                MathF.Min(a.Y, b.Y),
                MathF.Min(a.Z, b.Z)
            );
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(
                MathF.Max(a.X, b.X),
                MathF.Max(a.Y, b.Y),
                MathF.Max(a.Z, b.Z)
            );
        }

        public static float Clamp(this float value, float min, float max)
        {
            return MathF.Max(min, MathF.Min(max, value));
        }

        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(
                value.X.Clamp(min.X, max.X),
                value.Y.Clamp(min.Y, max.Y),
                value.Z.Clamp(min.Z, max.Z)
            );
        }

        // ----------------------------------------------------------------------------------------------
        //  LERP
        // ----------------------------------------------------------------------------------------------

        public static Vector3 Lerp(this Vector3 from, Vector3 to, float t)
        {
            t = t.Clamp(0f, 1f);
            return new Vector3(
                from.X + (to.X - from.X) * t,
                from.Y + (to.Y - from.Y) * t,
                from.Z + (to.Z - from.Z) * t
            );
        }

        // ----------------------------------------------------------------------------------------------
        //  DISTANCE
        // ----------------------------------------------------------------------------------------------

        public static float DistanceSquared(this Vector3 a, Vector3 b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return (dx * dx) + (dy * dy) + (dz * dz);
        }

        // ----------------------------------------------------------------------------------------------
        //  MOVE TOWARDS
        // ----------------------------------------------------------------------------------------------

        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 toVector = new Vector3(
                target.X - current.X,
                target.Y - current.Y,
                target.Z - current.Z
            );

            float dist = toVector.Length();
            if (dist <= maxDistanceDelta || dist == 0f)
                return target;

            float scale = maxDistanceDelta / dist;
            return new Vector3(
                current.X + toVector.X * scale,
                current.Y + toVector.Y * scale,
                current.Z + toVector.Z * scale
            );
        }

        // ----------------------------------------------------------------------------------------------
        //  DOT / CROSS
        // ----------------------------------------------------------------------------------------------

        private static float Dot(Vector3 a, Vector3 b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }

        private static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                (a.Y * b.Z) - (a.Z * b.Y),
                (a.Z * b.X) - (a.X * b.Z),
                (a.X * b.Y) - (a.Y * b.X)
            );
        }

        // ----------------------------------------------------------------------------------------------
        //  REFLECT
        // ----------------------------------------------------------------------------------------------

        public static Vector3 Reflect(this Vector3 direction, Vector3 normal)
        {
            float d = Dot(direction, normal);
            return new Vector3(
                direction.X - 2f * d * normal.X,
                direction.Y - 2f * d * normal.Y,
                direction.Z - 2f * d * normal.Z
            );
        }

        // ----------------------------------------------------------------------------------------------
        //  PROJECT / PROJECT ON PLANE
        // ----------------------------------------------------------------------------------------------

        public static Vector3 Project(this Vector3 vector, Vector3 onto)
        {
            float denom = Dot(onto, onto);
            if (denom <= 1e-6f)
                return Zero;

            float scale = Dot(vector, onto) / denom;
            return new Vector3(
                onto.X * scale,
                onto.Y * scale,
                onto.Z * scale
            );
        }

        public static Vector3 ProjectOnPlane(this Vector3 vector, Vector3 planeNormal)
        {
            return vector - vector.Project(planeNormal);
        }

        // ----------------------------------------------------------------------------------------------
        //  ANGLE / SIGNED ANGLE
        // ----------------------------------------------------------------------------------------------

        public static float Angle(this Vector3 from, Vector3 to)
        {
            float denom = MathF.Sqrt(from.DistanceSquared(Zero) * to.DistanceSquared(Zero));
            if (denom <= 1e-6f)
                return 0f;

            float dot = Dot(from, to) / denom;
            dot = dot.Clamp(-1f, 1f);
            return MathF.Acos(dot);
        }

        public static float SignedAngle(this Vector3 from, Vector3 to, Vector3 axis)
        {
            float unsigned = Angle(from, to);
            Vector3 cross = Cross(from, to);

            return (cross.X * axis.X + cross.Y * axis.Y + cross.Z * axis.Z) >= 0f
                ? unsigned
                : -unsigned;
        }

        // ----------------------------------------------------------------------------------------------
        //  ROTATION HELPERS
        // ----------------------------------------------------------------------------------------------

        public static Vector3 RotateY(this Vector3 v, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            return new Vector3(
                v.X * cos - v.Z * sin,
                v.Y,
                v.X * sin + v.Z * cos
            );
        }

        public static Vector3 RotateX(this Vector3 v, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            return new Vector3(
                v.X,
                v.Y * cos - v.Z * sin,
                v.Y * sin + v.Z * cos
            );
        }

        public static Vector3 RotateZ(this Vector3 v, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            return new Vector3(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos,
                v.Z
            );
        }
    }
}
