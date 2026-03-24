using System;

namespace SASZombieAssaultTD.Engine.Core
{
    public readonly struct Rect : IEquatable<Rect>
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
        
        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = System.Math.Max(0f, width);
            Height = System.Math.Max(0f, height);
        }
        
        public Rect(System.Numerics.Vector2 position, Size size)
        {
            X = position.X;
            Y = position.Y;
            Width = System.Math.Max(0f, size.Width);
            Height = System.Math.Max(0f, size.Height);
        }
        
        public bool Contains(float x, float y) => x >= Left && x <= Right && y >= Top && y <= Bottom;
        public bool Contains(System.Numerics.Vector2 point) => Contains(point.X, point.Y);
        public bool Intersects(Rect other) => !(other.Left > Right || other.Right < Left || other.Top > Bottom || other.Bottom < Top);
        
        public static Rect FromCenter(System.Numerics.Vector2 center, Size size)
        {
            return new Rect(center.X - size.Width * 0.5f, center.Y - size.Height * 0.5f, size.Width, size.Height);
        }
        
        public static Rect Intersect(Rect a, Rect b)
        {
            var left = System.Math.Max(a.Left, b.Left);
            var top = System.Math.Max(a.Top, b.Top);
            var right = System.Math.Min(a.Right, b.Right);
            var bottom = System.Math.Min(a.Bottom, b.Bottom);
            
            if (right < left || bottom < top) return default;
            return new Rect(left, top, right - left, bottom - top);
        }
        
        public static Rect Union(Rect a, Rect b)
        {
            var left = System.Math.Min(a.Left, b.Left);
            var top = System.Math.Min(a.Top, b.Top);
            var right = System.Math.Max(a.Right, b.Right);
            var bottom = System.Math.Max(a.Bottom, b.Bottom);
            return new Rect(left, top, right - left, bottom - top);
        }
        
        public Rect Inflate(float amount) => new Rect(X - amount, Y - amount, Width + amount * 2f, Height + amount * 2f);
        public Rect Inflate(float horizontal, float vertical) => new Rect(X - horizontal, Y - vertical, Width + horizontal * 2f, Height + vertical * 2f);
        public Rect Deflate(float amount) => Inflate(-amount);
        public Rect Offset(float dx, float dy) => new Rect(X + dx, Y + dy, Width, Height);
        public Rect Offset(System.Numerics.Vector2 offset) => Offset(offset.X, offset.Y);
        public Rect Scale(float scale) => new Rect(X * scale, Y * scale, Width * scale, Height * scale);
        public Rect Scale(float scaleX, float scaleY) => new Rect(X * scaleX, Y * scaleY, Width * scaleX, Height * scaleY);
        
        public bool Equals(Rect other)
        {
            return System.Math.Abs(X - other.X) < 0.001f &&
                   System.Math.Abs(Y - other.Y) < 0.001f &&
                   System.Math.Abs(Width - other.Width) < 0.001f &&
                   System.Math.Abs(Height - other.Height) < 0.001f;
        }
        
        public override bool Equals(object obj) => obj is Rect other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
        
        public static bool operator ==(Rect left, Rect right) => left.Equals(right);
        public static bool operator !=(Rect left, Rect right) => !left.Equals(right);
        
        /// <summary>
        /// Creates a Rect from position and size.
        /// </summary>
        public static Rect FromPositionAndSize(float x, float y, float width, float height)
        {
            return new Rect(x, y, width, height);
        }
        
        /// <summary>
        /// Empty rectangle.
        /// </summary>
        public static Rect Empty { get; } = new Rect(0, 0, 0, 0);
        
        public override string ToString() => $"Rect(X: {X:F1}, Y: {Y:F1}, Width: {Width:F1}, Height: {Height:F1})";
    }
}
