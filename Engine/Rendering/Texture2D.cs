using System;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Minimal placeholder texture representation.
    /// </summary>
    public sealed class Texture2D
    {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }

        public Texture2D(string name, int width, int height)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Texture dimensions must be positive.");

            Width = width;
            Height = height;
        }
    }
}