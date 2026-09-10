// ====================================================================================================
//  FILE: SizeUtil.cs
//  PATH: ./Engine/Core/Size/
//  MODULE: Core Size
//
//  ROLE:
//      Provide utility helpers for the Core Size subsystem.
//
//  RESPONSIBILITIES:
//      - Provide GetDominantDimension() behavior for the Core subsystem.
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide GetHashCode() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide ToCompactString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.CoreSize
{
    internal static class SizeUtil
    {
        public static string GetDominantDimension(Size size)
        {
            if (size.Width > size.Height)
                return "Width";
            if (size.Height > size.Width)
                return "Height";
            return "Equal";
        }

        public static bool Equals(Size a, Size b)
        {
            return a.Width == b.Width && a.Height == b.Height;
        }

        public static int GetHashCode(Size size)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + size.Width.GetHashCode();
                hash = hash * 31 + size.Height.GetHashCode();
                return hash;
            }
        }

        public static string ToString(Size size)
        {
            return $"Size(Width: {size.Width:F1}, Height: {size.Height:F1})";
        }

        public static string ToCompactString(Size size)
        {
            return $"{size.Width:F0}x{size.Height:F0}";
        }
    }
}
