// ====================================================================================================
//  FILE: Texture2D.cs
//  PATH: Engine/Render/Textures/
//  MODULE: Render / Textures
//
//  ROLE:
//      Immutable GPU texture wrapper used by the rendering subsystem.
//      Holds a GPU handle, dimensions, and format for SpriteBatch, TextRenderer, and UI/HUD rendering.
//
//  RESPONSIBILITIES:
//      - Represent a GPU texture in a deterministic, immutable form.
//      - Provide width, height, and format metadata.
//      - Provide a GPU handle for RenderDevice submission.
//
//  NON-RESPONSIBILITIES:
//      - Loading image files or performing resource management.
//      - CPU pixel storage or framebuffer operations.
//      - Logging, diagnostics, or fallback behavior.
//      - Texture caching or validation.
//
//  NOTES:
//      - Pure GPU texture wrapper.
//      - Compatible with Option‑B deterministic rendering.
//      - Used by SpriteBatch, TextRenderer, and UI subsystems.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Core;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    /// <summary>
    /// Immutable GPU texture wrapper.
    /// </summary>
    public sealed class Texture2D : IDisposable
    {
        /// <summary>
        /// GPU handle (e.g., ID3D11Texture2D pointer).
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// Texture width in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Texture height in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Texture format (BGRA32, etc.).
        /// </summary>
        public Render.Composite.GpuTextureFormat Format { get; }

        public string V1 { get; }
        public int V2 { get; }
        public int V3 { get; }
        public byte[] PixelData { get; }
        public nint NativeHandle { get; internal set; }
        public object Pixels { get; internal set; }

        private bool _disposed;
        private ID3D11Texture2D nativeTexture;
        private uint width;
        private uint height;

        public Texture2D(IntPtr handle, int width, int height, Render.Composite.GpuTextureFormat format)
        {
            Handle = handle;
            Width = width;
            Height = height;
            Format = format;
        }

        public Texture2D(string v, IntPtr handle, int width, int height)
        {
            Handle = handle;
            Width = width;
            Height = height;
            Format = Render.Composite.GpuTextureFormat.BGRA32;
        }

        public Texture2D(string v1, int v2, int v3, byte[] pixelData)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            PixelData = pixelData;
        }

        public Texture2D(ID3D11Texture2D nativeTexture) => this.nativeTexture = nativeTexture;

        public Texture2D(ID3D11Texture2D nativeTexture, uint width, uint height) : this(nativeTexture)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Releases GPU resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // GPU resource release handled by RenderDevice or native layer.
            _disposed = true;
        }

        internal static object Create(int v1, int v2, int[] ints, string path)
        {
            if (ints == null)
            {
                throw new ArgumentNullException(nameof(ints));
            }

            if (v1 <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(v1));
            }

            if (v2 <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(v2));
            }

            if (ints.Length != v1 * v2)
            {
                throw new ArgumentException("Pixel array length does not match provided width and height.", nameof(ints));
            }

            // If no path is provided, prefer the existing helper that accepts int[] pixels.
            if (string.IsNullOrEmpty(path))
            {
                return CreateRGBATexture(v1, v2, ints);
            }

            // Convert packed int pixels to a byte[] in RGBA order.
            // Assumes each int is in 0xAARRGGBB format. Conversion produces bytes: R, G, B, A.
            var pixelBytes = new byte[ints.Length * 4];
            for (int i = 0; i < ints.Length; i++)
            {
                int p = ints[i];
                byte a = (byte)(p >> 24 & 0xFF);
                byte r = (byte)(p >> 16 & 0xFF);
                byte g = (byte)(p >> 8 & 0xFF);
                byte b = (byte)(p & 0xFF);

                int baseIndex = i * 4;
                pixelBytes[baseIndex + 0] = r;
                pixelBytes[baseIndex + 1] = g;
                pixelBytes[baseIndex + 2] = b;
                pixelBytes[baseIndex + 3] = a;
            }

            // Use the constructor that accepts (string v1, int v2, int v3, byte[] pixelData)
            return new Texture2D(path, v1, v2, pixelBytes);
        }

        internal static Texture2D CreateAlphaTexture(int atlasWidth, int atlasHeight, byte[] atlasAlpha)
        {
            // Validate inputs
            if (atlasWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(atlasWidth), "atlasWidth must be greater than zero.");
            if (atlasHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(atlasHeight), "atlasHeight must be greater than zero.");
            if (atlasAlpha is null)
                throw new ArgumentNullException(nameof(atlasAlpha));

            int pixelCount = checked(atlasWidth * atlasHeight);
            if (atlasAlpha.Length != pixelCount)
                throw new ArgumentException("atlasAlpha length does not match atlas dimensions.", nameof(atlasAlpha));

            // Create RGBA pixel buffer where RGB = 255 (white) and A = atlasAlpha
            byte[] pixels = new byte[pixelCount * 4];
            ReadOnlySpan<byte> alphaSpan = atlasAlpha;
            Span<byte> outSpan = pixels;

            for (int i = 0, dst = 0; i < pixelCount; i++, dst += 4)
            {
                outSpan[dst + 0] = 255; // R
                outSpan[dst + 1] = 255; // G
                outSpan[dst + 2] = 255; // B
                outSpan[dst + 3] = alphaSpan[i]; // A
            }

            // Use the existing constructor that accepts pixel data (name, width, height, pixelData)
            return new Texture2D(string.Empty, atlasWidth, atlasHeight, pixels);
        }

        internal static Texture2D CreateRGBATexture(int width, int height, int[] pixels)
        {
            // Validate inputs
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));
            if (pixels.Length != width * height)
                throw new ArgumentException("Pixel array length does not match width*height.", nameof(pixels));

            // Prepare a byte array in RGBA order expected by the texture constructor
            var bytes = new byte[checked(width * height * 4)];
            Span<byte> span = bytes;

            // Many C# color ints are provided as ARGB (0xAARRGGBB). Convert from ARGB -> RGBA
            for (int i = 0, o = 0; i < pixels.Length; i++, o += 4)
            {
                uint p = (uint)pixels[i];
                // Extract components assuming input is 0xAARRGGBB
                byte a = (byte)(p >> 24 & 0xFF);
                byte r = (byte)(p >> 16 & 0xFF);
                byte g = (byte)(p >> 8 & 0xFF);
                byte b = (byte)(p & 0xFF);

                // Write out in R,G,B,A order
                span[o] = r;
                span[o + 1] = g;
                span[o + 2] = b;
                span[o + 3] = a;
            }

            // Use the byte[] constructor overload to create the texture.
            // The first string parameter is used by other Texture2D constructors in the project; use "rgba" to indicate layout.
            return new Texture2D("rgba", width, height, bytes);
        }

        internal static Texture2D LoadFromFile(string v)
        {
            // Validate input path
            if (v is null)
            {
                throw new ArgumentNullException(nameof(v));
            }

            // Ensure the file exists before attempting to read it.
            if (!System.IO.File.Exists(v))
            {
                throw new System.IO.FileNotFoundException($"Texture file not found: {v}", v);
            }

            // Read the file bytes and construct a Texture2D using the (string, int, int, byte[]) constructor.
            // Width and height are unknown at this level; defer actual decoding/upscaling to consumers or other constructors.
            byte[] fileBytes = System.IO.File.ReadAllBytes(v);

            return new Texture2D(v, 0, 0, fileBytes);
        }

        internal static object LoadFromFile(object value)
        {
            // Support null, string paths and already-loaded Texture2D instances.
            if (value is null)
            {
                return null;
            }

            // If a file path is provided, call the string overload that returns a Texture2D.
            if (value is string path)
            {
                return LoadFromFile(path);
            }

            // If a Texture2D instance is passed through, return it directly.
            if (value is Texture2D tex)
            {
                return tex;
            }

            // Unsupported type: provide a clear exception indicating the problem.
            throw new ArgumentException("Unsupported value type for LoadFromFile. Expected null, string (path) or Texture2D.", nameof(value));
        }

        internal ColorRGBA GetPixel(int sx, int sy)
        {
            throw new NotImplementedException();
        }
    }
}