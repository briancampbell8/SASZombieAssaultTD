// ====================================================================================================
//  FILE: VectorMathExtensions.cs
//  PATH: Engine/VectorMath/VectorMathExtensions.cs
//  MODULE: VectorMath
//
//  ROLE:
//      Provide deterministic operator helpers for Vector3, Vector3Int, Point, PointF, and Rectangle.
//
//  RESPONSIBILITIES:
//      - Provide Vector3 ↔ Point / PointF conversions.
//      - Provide Rectangle? → Vector3 conversion.
//      - Provide PointF arithmetic.
//      - Provide Vector3Int arithmetic and conversions.
//      - Provide Vector3Int comparisons.
//      - Provide Vector3Int? → int conversion.
//
//  NOTES:
//      Extracted from OperatorExtensions.cs during subsystem breakup.
// ====================================================================================================

using System.Drawing;

namespace SASZombieAssaultTD.Engine.VectorMath
{
    public static class VectorMathExtensions
    {
        public static Point ToPoint(this Vector3 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        public static PointF ToPointF(this Vector3 vector)
        {
            return new PointF(vector.X, vector.Y);
        }

        public static Vector3 ToVector3(this Point point)
        {
            return new Vector3(point.X, point.Y, 0f);
        }

        public static Vector3 ToVector3(this PointF point)
        {
            return new Vector3(point.X, point.Y, 0f);
        }

        public static Vector3 ToVector3(this Rectangle? rect, Vector3 defaultValue)
        {
            if (rect.HasValue)
                return new Vector3(rect.Value.X, rect.Value.Y, 0f);

            return defaultValue;
        }

        public static PointF Subtract(this PointF point, Vector3 vector)
        {
            return new PointF(point.X - vector.X, point.Y - vector.Y);
        }

        public static PointF Add(this PointF point, Vector3 vector)
        {
            return new PointF(point.X + vector.X, point.Y + vector.Y);
        }

        public static int ToInt(this Vector3Int vector)
        {
            return vector.X + vector.Y + vector.Z;
        }

        public static Vector3Int ToVector3Int(this int value)
        {
            return new Vector3Int(value, value, value);
        }

        public static Vector3Int Multiply(this Vector3Int vector, float scalar)
        {
            return new Vector3Int(
                (int)(vector.X * scalar),
                (int)(vector.Y * scalar),
                (int)(vector.Z * scalar)
            );
        }

        public static Vector3Int Add(this Vector3Int vector, int value)
        {
            return new Vector3Int(vector.X + value, vector.Y + value, vector.Z + value);
        }

        public static int ToInt(this Vector3Int? vector, int defaultValue)
        {
            if (vector.HasValue)
                return vector.Value.ToInt();

            return defaultValue;
        }

        public static bool LessThanOrEqual(this int value, Vector3Int vector)
        {
            return value <= vector.ToInt();
        }

        public static bool LessThan(this int value, Vector3Int vector)
        {
            return value < vector.ToInt();
        }

        public static bool LessThan(this Vector3Int vector, int value)
        {
            return vector.ToInt() < value;
        }

        public static bool GreaterThan(this Vector3Int vector, int value)
        {
            return vector.ToInt() > value;
        }
    }
}
