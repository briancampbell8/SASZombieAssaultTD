// ====================================================================================================
//  FILE: SizeOps.cs
//  PATH: ./Engine/Core/Size/
//  MODULE: Core Size
//
//  ROLE:
//      Provide core size‑math operations for the Core Size subsystem.
//
//  RESPONSIBILITIES:
//      - Provide Square() behavior for the Core subsystem.
//      - Provide Max() behavior for the Core subsystem.
//      - Provide Min() behavior for the Core subsystem.
//      - Provide Lerp() behavior for the Core subsystem.
//      - Provide Scale() behavior for the Core subsystem.
//      - Provide Expand() behavior for the Core subsystem.
//      - Provide Contract() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.CoreSize
{
    internal static class SizeOps
    {
        public static Size Square(float size)
        {
            return new Size(size, size);
        }

        public static Size Max(Size a, Size b)
        {
            return new Size(
                System.Math.Max(a.Width, b.Width),
                System.Math.Max(a.Height, b.Height)
            );
        }

        public static Size Min(Size a, Size b)
        {
            return new Size(
                System.Math.Min(a.Width, b.Width),
                System.Math.Min(a.Height, b.Height)
            );
        }

        public static Size Lerp(Size a, Size b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);

            float w = a.Width + (b.Width - a.Width) * t;
            float h = a.Height + (b.Height - a.Height) * t;

            return new Size(w, h);
        }

        public static Size Scale(Size size, float scale)
        {
            return new Size(size.Width * scale, size.Height * scale);
        }

        public static Size Expand(Size size, float amount)
        {
            return new Size(size.Width + amount, size.Height + amount);
        }

        public static Size Contract(Size size, float amount)
        {
            return new Size(
                System.Math.Max(0f, size.Width - amount),
                System.Math.Max(0f, size.Height - amount)
            );
        }
    }
}
