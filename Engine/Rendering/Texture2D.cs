using System;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Minimal texture representation with optional pixel data.
    /// </summary>
    public sealed class Texture2D
    {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Raw pixel data (BGRA byte order, matching Framebuffer layout). Null when dimensions-only.
        /// </summary>
        public byte[]? Pixels { get; }

        /// <summary>
        /// Constructs a Texture2D with optional pixel data.
        /// Throws if pixel array length does not match expected size.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="pixels">The pixel data for the texture.</param>
        /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when width or height is less than or equal to zero.</exception>
        /// <exception cref="ArgumentException">Thrown when pixels array length does not match the expected size.</exception>
        public Texture2D(string name, int width, int height, byte[]? pixels)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Texture dimensions must be positive.");

            if (pixels is not null && pixels.Length != width * height * 4)
                throw new ArgumentException(
                    $"Pixel array length ({pixels.Length}) does not match expected size ({width * height * 4}).",
                    nameof(pixels));

            Width = width;
            Height = height;
            Pixels = pixels;
        }
    }
}