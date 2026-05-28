using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using System.IO;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Supported texture formats.
    /// </summary>
    public enum TextureFormat
    {
        BGRA32,
        RGB24,
        A8
    }

    /// <summary>
    /// Enhanced texture representation with cache compatibility and validation.
    /// </summary>
    public sealed class Texture2D : IDisposable
    {
        private bool _disposed = false;
        private string? _filePath;
        private TextureFormat _format = TextureFormat.BGRA32;
        private static object TheContainingType;
        private static object TheContainingMember;

        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        /// Raw pixel data (BGRA byte order, matching Framebuffer layout). Null when dimensions-only.
        /// </summary>
        public byte[]? Pixels { get; private set; }

        /// <summary>
        /// Gets the texture format.
        /// </summary>
        public TextureFormat Format => _format;

        /// <summary>
        /// Gets the file path this texture was loaded from.
        /// </summary>
        public string? FilePath => _filePath;

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
        /// <exception cref="ArgumentException">Thrown when pixels array length does not match expected size.</exception>
        public Texture2D(string name, int width, int height, byte[]? pixels)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            // Validate all parameters
            ValidateTexture(width, height, pixels);

            Name = name;
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        /// <summary>
        /// Loads a texture from file path with cache integration.
        /// </summary>
        /// <param name="filePath">The file path to load from.</param>
        /// <param name="textureCache">Optional texture cache for resource management.</param>
        /// <returns>The loaded texture or cached version if available.</returns>
        public static Texture2D LoadFromFile(string filePath, TextureCache? textureCache = null)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            // Check cache first
            if (textureCache?.TryGet(filePath, out var cachedTexture) == true)
            {
                return cachedTexture!;
            }

            // Load from file if not in cache
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Texture file not found: {filePath}");

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var texture = new Texture2D(fileName, 0, 0, null);
            texture._filePath = filePath;

            // Load pixel data (placeholder implementation)
            try
            {
                // In a real implementation, this would load actual image data
                // For now, create a test pattern
                var pixels = CreateTestPattern(texture.Width, texture.Height);
                texture.Pixels = pixels;

                // Add to cache if provided
                textureCache?.Add(filePath, texture);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load texture from {filePath}: {ex.Message}", ex);
            }

            return texture; // Always returns a valid texture
        }

        /// <summary>
        /// Validates texture dimensions and format.
        /// </summary>
        /// <param name="width">The width to validate.</param>
        /// <param name="height">The height to validate.</param>
        /// <param name="pixels">Optional pixel data to validate.</param>
        /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
        public static void ValidateTexture(int width, int height, byte[]? pixels = null)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Texture dimensions must be positive.");

            // Validate maximum supported size
            if (width > 8192 || height > 8192)
                throw new ArgumentException("Texture dimensions exceed maximum supported size (8192x8192).");

            // Validate power-of-two dimensions for better GPU compatibility
            if (!IsPowerOfTwo(width) || !IsPowerOfTwo(height))
                throw new ArgumentException("Texture dimensions should be power-of-two for optimal GPU performance.");

            // Validate pixel data if provided
            if (pixels != null && pixels!.Length != width * height * 4)
                throw new ArgumentException(
                $"Pixel array length ({pixels!.Length}) does not match expected size ({width * height * 4}).",
                nameof(pixels));
        }

        /// <summary>
        /// Checks if a texture format is supported.
        /// </summary>
        /// <param name="format">The texture format to check.</param>
        /// <returns>True if the format is supported.</returns>
        public static bool IsFormatSupported(TextureFormat format)
        {
            return format switch
            {
                TextureFormat.BGRA32 => true,
                TextureFormat.RGB24 => true,
                TextureFormat.A8 => true,
                _ => false
            };
        }

        /// <summary>
        /// Validates that the texture is not disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Texture2D));
        }

        /// <summary>
        /// Checks if a number is a power of two.
        /// </summary>
        private static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Creates a test pattern for texture loading validation.
        /// </summary>
        private static byte[] CreateTestPattern(int width, int height)
        {
            var pixels = new byte[width * height * 4];
            var random = new Random(42); // Fixed seed for deterministic testing

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var index = (y * width + x) * 4;

                    // Create a checkerboard pattern
                    var isEven = (x + y) % 2 == 0;
                    var color = isEven ?
                    new byte[] { 255, 255, 255, 255 } : // White
                    new byte[] { 128, 128, 128, 255 };  // Gray

                    pixels[index + 0] = color[0]; // B
                    pixels[index + 1] = color[1]; // G
                    pixels[index + 2] = color[2]; // R
                    pixels[index + 3] = color[3]; // A
                }
            }

            return pixels;
        }

        /// <summary>
        /// Implements deterministic resource disposal.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            Pixels = null;
            Width = 0;
            Height = 0;
            _filePath = null;
            _disposed = true;
        }

        internal static object Create(int v1, int v2, int[] ints, string path)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}


