using System;
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Math;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.VectorMath
{
    ///<summary>
    ///Extension methods for Vector3 to provide compatibility with legacy code.
    ///Phase 2: Vector3 Extension Pack - Fixes ~25% of CS1061 errors
    ///</summary>
    public static class Vector3Extensions
    {
        ///<summary>
        ///Gets a zero vector (0, 0, 0).
        ///</summary>
        public static Vector3 Zero => new Vector3(0, 0, 0);

        ///<summary>
        ///Gets the magnitude (length) of the vector.
        ///Compatibility alias for Magnitude property.
        ///</summary>
        public static float Length(this Vector3 v) => v.Magnitude;

        ///<summary>
        ///Returns a normalized version of the vector.
        ///Compatibility alias for Normalized property.
        ///</summary>
        public static Vector3 Normalize(this Vector3 v) => v.Normalized;

        ///<summary>
        ///Component-wise minimum of two vectors
        ///</summary>
        public static Vector3 Min(Vector3 a, Vector3 b) => 
            new Vector3(System.Math.Min(a.X, b.X), System.Math.Min(a.Y, b.Y), System.Math.Min(a.Z, b.Z));
        
        ///<summary>
        ///Component-wise maximum of two vectors
        ///</summary>
        public static Vector3 Max(Vector3 a, Vector3 b) => 
            new Vector3(System.Math.Max(a.X, b.X), System.Math.Max(a.Y, b.Y), System.Math.Max(a.Z, b.Z));

        ///<summary>
        ///Clamps a float value between min and max.
        ///</summary>
        public static float Clamp(this float value, float min, float max) =>
            System.Math.Clamp(value, min, max);

        ///<summary>
        ///Clamps a vector's components between corresponding min and max vectors.
        ///</summary>
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max) =>
            new(System.Math.Clamp(value.X, min.X, max.X), 
                 System.Math.Clamp(value.Y, min.Y, max.Y), 
                 System.Math.Clamp(value.Z, min.Z, max.Z));

        ///<summary>
        ///Linear interpolation between two vectors.
        ///</summary>
        public static Vector3 Lerp(this Vector3 from, Vector3 to, float t) =>
            from + (to - from) * System.Math.Clamp(t, 0f, 1f);

        ///<summary>
        ///Returns the squared distance between two vectors (avoids sqrt for performance).
        ///</summary>
        public static float DistanceSquared(this Vector3 a, Vector3 b) =>
            (a - b).MagnitudeSquared;

        ///<summary>
        ///Moves a point towards a target by a maximum distance.
        ///</summary>
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            var toVector = target - current;
            float distance = toVector.Magnitude;
            
            if (distance <= maxDistanceDelta || distance == 0f)
                return target;
                
            return current + toVector / distance * maxDistanceDelta;
        }

        ///<summary>
        ///Reflects a vector off a surface defined by a normal.
        ///</summary>
        public static Vector3 Reflect(this Vector3 direction, Vector3 normal)
        {
            return direction - 2f * Dot(direction, normal) * normal;
        }

        ///<summary>
        ///Projects a vector onto another vector.
        ///</summary>
        public static Vector3 Project(this Vector3 vector, Vector3 onto)
        {
            float magnitudeSquared = onto.MagnitudeSquared;
            if (magnitudeSquared < 1e-6f)
                return Zero;
                
            return onto * (Dot(vector, onto) / magnitudeSquared);
        }

        ///<summary>
        ///Projects a vector onto a plane defined by a normal (removes the component parallel to the normal).
        ///</summary>
        public static Vector3 ProjectOnPlane(this Vector3 vector, Vector3 planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }

        ///<summary>
        ///Returns the angle between two vectors in radians.
        ///</summary>
        public static float Angle(this Vector3 from, Vector3 to)
        {
            float denominator = MathF.Sqrt(from.MagnitudeSquared * to.MagnitudeSquared);
            if (denominator < 1e-6f)
                return 0f;
                
            float dot = System.Math.Clamp(Dot(from, to) / denominator, -1f, 1f);
            return MathF.Acos(dot);
        }

        ///<summary>
        ///Returns the signed angle between two vectors in radians.
        ///</summary>
        public static float SignedAngle(this Vector3 from, Vector3 to, Vector3 axis)
        {
            float unsignedAngle = Angle(from, to);
            float cross = Cross(from, to).Z;
            return cross * axis.Z >= 0f ? unsignedAngle : -unsignedAngle;
        }

        ///<summary>
        ///Returns a vector rotated around the Y axis by the specified angle in radians.
        ///</summary>
        public static Vector3 RotateY(this Vector3 vector, float angleRadians)
        {
            float cos = MathF.Cos(angleRadians);
            float sin = MathF.Sin(angleRadians);
            return new Vector3(
                vector.X * cos - vector.Z * sin,
                vector.Y,
                vector.X * sin + vector.Z * cos
            );
        }

        ///<summary>
        ///Returns a vector rotated around the X axis by the specified angle in radians.
        ///</summary>
        public static Vector3 RotateX(this Vector3 vector, float angleRadians)
        {
            float cos = MathF.Cos(angleRadians);
            float sin = MathF.Sin(angleRadians);
            return new Vector3(
                vector.X,
                vector.Y * cos - vector.Z * sin,
                vector.Y * sin + vector.Z * cos
            );
        }

        ///<summary>
        ///Returns a vector rotated around the Z axis by the specified angle in radians.
        ///</summary>
        public static Vector3 RotateZ(this Vector3 vector, float angleRadians)
        {
            float cos = MathF.Cos(angleRadians);
            float sin = MathF.Sin(angleRadians);
            return new Vector3(
                vector.X * cos - vector.Y * sin,
                vector.X * sin + vector.Y * cos,
                vector.Z
            );
        }

        //Helper methods that reference the static Vector3 methods
        private static float Dot(Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        private static Vector3 Cross(Vector3 a, Vector3 b) => Vector3.Cross(a, b);
    }
}
