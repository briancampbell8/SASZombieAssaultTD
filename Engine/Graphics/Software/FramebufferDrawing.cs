// =====================================================================================================
//  FILE: FramebufferDrawing.cs
//  PATH: Engine/Graphics/Software/FramebufferDrawing.cs
//  SUBSYSTEM: CPU Rendering / Deterministic Software Framebuffer
//
//  ROLE:
//      Authoritative CPU‑side framebuffer used by the engine’s software renderer and by the
//      D3D11 upload pipeline. Represents a 2D RGBA32 pixel buffer consumed deterministically
//      each frame.
//
//  RESPONSIBILITIES:
//      - Maintain a CPU pixel buffer with deterministic layout (RGBA32).
//      - Provide basic pixel operations.
//      - Support frame lifecycle (BeginFrame / EndFrame / Present / Reset).
//
//  NON‑RESPONSIBILITIES:
//      - Text rendering (TextRenderer)
//      - Sprite rendering (SpriteRenderer)
//      - Texture blitting (TextureBlitter)
//      - Shape primitives (SoftwareRenderContext)
//      - Legacy UI overloads (LegacyRenderContextAdapter)
// =====================================================================================================

using System;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.Graphics.Software
{
    /// <summary>
    /// Deterministic CPU-side RGBA32 framebuffer. This type owns the pixel buffer and exposes
    /// only basic pixel operations and frame lifecycle methods. All higher-level rendering
    /// (text, sprites, shapes, UI) is handled by separate subsystems.
    /// </summary>
    public sealed class FramebufferDrawing
    {
        /// <summary>Width of the framebuffer in pixels.</summary>
        public int Width { get; }

        /// <summary>Height of the framebuffer in pixels.</summary>
        public int Height { get; }

        /// <summary>Underlying RGBA32 pixel buffer (row-major, 4 bytes per pixel).</summary>
        public byte[] Pixels { get; private set; }

        /// <summary>Convenience viewport size vector.</summary>
        public Vector2 ViewportSize => new Vector2(Width, Height);

        public FramebufferDrawing(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            Pixels = new byte[width * height * 4];
        }

        // ----------------------------------------------------------------------------------------------
        //  Pixel operations
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Sets a single pixel in the framebuffer. Coordinates outside the bounds are ignored.
        /// </summary>
        public void SetPixel(int x, int y, byte r, byte g, byte b, byte a = 255)
        {
            if (x < 0 || x >= Width) return;
            if (y < 0 || y >= Height) return;

            int index = (y * Width + x) * 4;
            Pixels[index + 0] = r;
            Pixels[index + 1] = g;
            Pixels[index + 2] = b;
            Pixels[index + 3] = a;
        }

        /// <summary>
        /// Clears the entire framebuffer to a single RGBA color.
        /// </summary>
        public void Clear(byte r, byte g, byte b, byte a = 255)
        {
            for (int i = 0; i < Pixels.Length; i += 4)
            {
                Pixels[i + 0] = r;
                Pixels[i + 1] = g;
                Pixels[i + 2] = b;
                Pixels[i + 3] = a;
            }
        }

        // ----------------------------------------------------------------------------------------------
        //  Frame lifecycle
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Prepares the framebuffer for a new frame. Ensures buffer size is correct and clears it.
        /// </summary>
        public void BeginFrame()
        {
            int requiredSize = Width * Height * 4;

            if (Pixels == null || Pixels.Length != requiredSize)
            {
                Reset();
            }

            Array.Clear(Pixels, 0, Pixels.Length);
        }

        /// <summary>
        /// Marks the end of the current frame. No-op for pure CPU framebuffer.
        /// </summary>
        public void EndFrame()
        {
            // Intentionally empty: higher-level systems decide when to upload/use the buffer.
        }

        /// <summary>
        /// Presents the framebuffer. For a pure CPU buffer this is a no-op; the D3D11 upload
        /// pipeline or software renderer will consume Pixels as needed.
        /// </summary>
        public void Present()
        {
            // Intentionally empty: presentation is handled by the GPU/upload subsystem.
        }

        /// <summary>
        /// Resets the framebuffer by allocating a fresh pixel buffer of the correct size.
        /// </summary>
        public void Reset()
        {
            Pixels = new byte[Width * Height * 4];
        }

        internal void Clear(Color color)
        {
            // Delegate to the existing byte-based Clear overload using the color's components.
            Clear(color.R, color.G, color.B, color.A);
        }


    }
}
