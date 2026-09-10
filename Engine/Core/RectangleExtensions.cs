// =====================================================================================================
//  FILE: RectangleExtensions.cs
//  PATH: Engine/Core/RectangleExtensions.cs
//  MODULE: Core
//
//  ROLE:
//      Provide deterministic, type‑safe helper methods for the Core Rectangle struct, supporting
//      containment checks, intersection tests, expansion, contraction, clamping, and alignment
//      operations without modifying the Rectangle struct itself.
//
//  RESPONSIBILITIES:
//      - Provide pure, stateless helper functions for deterministic Rectangle operations.
//      - Support containment, overlap, intersection, union, expansion, contraction, and clamping logic.
//      - Improve readability and maintainability of geometry‑related logic without altering core systems.
//      - Remain fully deterministic and side‑effect free.
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU operations.
//      - Managing ECS entities or world‑grid occupancy.
//      - Allocating engine resources or mutating engine state.
//      - Replacing or overriding Rectangle’s built‑in core methods.
//
//  ARCHITECTURAL NOTES:
//      - Rectangle is a Core geometry primitive; therefore, its helpers belong in the Core subsystem.
//      - This module must not introduce cross‑subsystem coupling or hidden dependencies.
//      - Relocated from Engine/Extensions during subsystem cleanup.
// =====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Core
{
    internal static class RectangleExtensions
    {
        public static bool Contains(this Rectangle r, Vector3 point)
        {
            return point.X >= r.X &&
                   point.X <= r.X + r.Width &&
                   point.Y >= r.Y &&
                   point.Y <= r.Y + r.Height;
        }

        public static bool Contains(this Rectangle r, int x, int y)
        {
            return x >= r.X &&
                   x <= r.X + r.Width &&
                   y >= r.Y &&
                   y <= r.Y + r.Height;
        }

        public static bool ContainsRect(this Rectangle a, Rectangle b)
        {
            return b.X >= a.X &&
                   b.Y >= a.Y &&
                   b.Right <= a.Right &&
                   b.Bottom <= a.Bottom;
        }

        public static bool Touches(this Rectangle a, Rectangle b)
        {
            bool horizontalTouch =
                (a.Right == b.X || b.Right == a.X) &&
                !(a.Bottom < b.Y || b.Bottom < a.Y);

            bool verticalTouch =
                (a.Bottom == b.Y || b.Bottom == a.Y) &&
                !(a.Right < b.X || b.Right < a.X);

            return horizontalTouch || verticalTouch;
        }

        public static Rectangle Expand(this Rectangle r, float amount)
        {
            return new Rectangle(
                r.X - amount,
                r.Y - amount,
                r.Width + amount * 2f,
                r.Height + amount * 2f
            );
        }

        public static Rectangle Contract(this Rectangle r, float amount)
        {
            float newWidth = r.Width - amount * 2f;
            float newHeight = r.Height - amount * 2f;

            if (newWidth <= 0f || newHeight <= 0f)
                return Rectangle.Empty;

            return new Rectangle(
                r.X + amount,
                r.Y + amount,
                newWidth,
                newHeight
            );
        }

        public static Rectangle AlignCenter(this Rectangle r, Rectangle container)
        {
            float x = container.X + (container.Width - r.Width) * 0.5f;
            float y = container.Y + (container.Height - r.Height) * 0.5f;

            return new Rectangle(x, y, r.Width, r.Height);
        }

        public static Rectangle AlignTopLeft(this Rectangle r, Rectangle container)
        {
            return new Rectangle(container.X, container.Y, r.Width, r.Height);
        }

        public static Rectangle AlignBottomRight(this Rectangle r, Rectangle container)
        {
            float x = container.Right - r.Width;
            float y = container.Bottom - r.Height;

            return new Rectangle(x, y, r.Width, r.Height);
        }

        public static Rectangle ClampInside(this Rectangle r, Rectangle bounds)
        {
            float x = r.X;
            float y = r.Y;

            if (x < bounds.X) x = bounds.X;
            if (y < bounds.Y) y = bounds.Y;

            if (x + r.Width > bounds.Right)
                x = bounds.Right - r.Width;

            if (y + r.Height > bounds.Bottom)
                y = bounds.Bottom - r.Height;

            return new Rectangle(x, y, r.Width, r.Height);
        }
    }
}
