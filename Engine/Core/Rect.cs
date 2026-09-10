// ====================================================================================================
//  FILE: Rect.cs
//  PATH: ./Engine/Core/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the Rect module.
//
//  RESPONSIBILITIES:
//      - Provide Contains() behavior for the Core subsystem.
//      - Provide Intersects() behavior for the Core subsystem.
//      - Provide FromCenter() behavior for the Core subsystem.
//      - Provide Intersect() behavior for the Core subsystem.
//      - Provide Union() behavior for the Core subsystem.
//      - Provide Inflate() behavior for the Core subsystem.
//      - Provide Deflate() behavior for the Core subsystem.
//      - Provide Offset() behavior for the Core subsystem.
//      - Provide Scale() behavior for the Core subsystem.
//      - Provide Equals() behavior for the Core subsystem.
//      - Provide GetHashCode() behavior for the Core subsystem.
//      - Provide FromPositionAndSize() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;
using Vortice.Mathematics;

namespace SASZombieAssaultTD.Engine.Core
{
    public readonly struct Rectangle : IEquatable<Rectangle>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;

        public float Left => X;
        public float Top => Y;
        public float Right => X + Width;
        public float Bottom => Y + Height;
        public float CenterX => X + Width * 0.5f;
        public float CenterY => Y + Height * 0.5f;
        public System.Numerics.Vector2 Center => new System.Numerics.Vector2(CenterX, CenterY);
        public Size Size => new Size(Width, Height);
        public float Area => Width * Height;
        public bool IsEmpty => Width <= 0f || Height <= 0f;

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = System.Math.Max(0f, width);
            Height = System.Math.Max(0f, height);
        }

        public Rectangle(System.Numerics.Vector2 position, Size size)
        {
            X = position.X;
            Y = position.Y;
            Width = System.Math.Max(0f, size.Width);
            Height = System.Math.Max(0f, size.Height);
        }

        public bool Contains(float x, float y) =>
            x >= Left && x <= Right && y >= Top && y <= Bottom;

        public bool Contains(System.Numerics.Vector2 point) =>
            Contains(point.X, point.Y);

        public bool Intersects(Rectangle other) =>
            !(other.Left > Right || other.Right < Left || other.Top > Bottom || other.Bottom < Top);

        public static Rectangle FromCenter(System.Numerics.Vector2 center, Size size)
        {
            return new Rectangle(center.X - size.Width * 0.5f,
                                 center.Y - size.Height * 0.5f,
                                 size.Width,
                                 size.Height);
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            var left = System.Math.Max(a.Left, b.Left);
            var top = System.Math.Max(a.Top, b.Top);
            var right = System.Math.Min(a.Right, b.Right);
            var bottom = System.Math.Min(a.Bottom, b.Bottom);

            if (right < left || bottom < top)
                return default;

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            var left = System.Math.Min(a.Left, b.Left);
            var top = System.Math.Min(a.Top, b.Top);
            var right = System.Math.Max(a.Right, b.Right);
            var bottom = System.Math.Max(a.Bottom, b.Bottom);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public Rectangle Inflate(float amount) =>
            new Rectangle(X - amount, Y - amount, Width + amount * 2f, Height + amount * 2f);

        public Rectangle Inflate(float horizontal, float vertical) =>
            new Rectangle(X - horizontal, Y - vertical, Width + horizontal * 2f, Height + vertical * 2f);

        public Rectangle Deflate(float amount) =>
            Inflate(-amount);

        public Rectangle Offset(float dx, float dy) =>
            new Rectangle(X + dx, Y + dy, Width, Height);

        public Rectangle Offset(System.Numerics.Vector2 offset) =>
            Offset(offset.X, offset.Y);

        public Rectangle Scale(float scale) =>
            new Rectangle(X * scale, Y * scale, Width * scale, Height * scale);

        public Rectangle Scale(float scaleX, float scaleY) =>
            new Rectangle(X * scaleX, Y * scaleY, Width * scaleX, Height * scaleY);

        public bool Equals(Rectangle other)
        {
            return System.Math.Abs(X - other.X) < 0.001f &&
                   System.Math.Abs(Y - other.Y) < 0.001f &&
                   System.Math.Abs(Width - other.Width) < 0.001f &&
                   System.Math.Abs(Height - other.Height) < 0.001f;
        }

        public override bool Equals(object obj) =>
            obj is Rectangle other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(X, Y, Width, Height);

        public static bool operator ==(Rectangle left, Rectangle right) =>
            left.Equals(right);

        public static bool operator !=(Rectangle left, Rectangle right) =>
            !left.Equals(right);

        // ---------------------------------------------------------------------------------------------
        // FIXED IMPLICIT OPERATORS (NO RECURSION)
        // ---------------------------------------------------------------------------------------------

        public static implicit operator Rectangle(System.Drawing.Rectangle v)
        {
            return new Rectangle((float)v.X, (float)v.Y, (float)v.Width, (float)v.Height);
        }

        public static implicit operator System.Drawing.Rectangle(Rectangle r)
        {
            return new System.Drawing.Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        /// <summary>
        /// Creates a Rect from position and size.
        /// </summary>
        public static Rectangle FromPositionAndSize(float x, float y, float width, float height)
        {
            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Empty rectangle.
        /// </summary>
        public static Rectangle Empty { get; } = new Rectangle(0f, 0f, 0f, 0f);

        public override string ToString() =>
            $"Rect(X: {X:F1}, Y: {Y:F1}, Width: {Width:F1}, Height: {Height:F1})";

        internal bool Contains(Vector3 vector3)
        {
            return Contains(vector3.X, vector3.Y);
        }

        internal static System.Numerics.Vector2 FromPositionAndSize(Rectangle backgroundRect, Color color)
        {
            return FromPositionAndSize(backgroundRect);
        }

        internal static System.Numerics.Vector2 FromPositionAndSize(Rectangle backgroundRect)
        {
            return new System.Numerics.Vector2(backgroundRect.X, backgroundRect.Y);
        }

        internal static System.Drawing.Rectangle FromPositionAndSize(object x1, object y1, object x2, object y2)
        {
            throw new NotImplementedException();
        }
    }
}
