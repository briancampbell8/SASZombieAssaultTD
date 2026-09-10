// =====================================================================================================
//  FILE: FramebufferContext.cs
//  PATH: Engine/Graphics/Software/FramebufferContext.cs
//  SUBSYSTEM: CPU Rendering / Deterministic Software Framebuffer
//
//  ROLE:
//      Lightweight lifecycle wrapper around the authoritative CPU‑side framebuffer. Coordinates
//      frame begin/end, presentation, and reset for the software renderer and upload pipeline.
//
//  RESPONSIBILITIES:
//      - Own a reference to the FramebufferDrawing instance.
//      - Provide deterministic frame lifecycle (BeginFrame / EndFrame / Present / Reset).
//      - Expose basic pixel operations via the underlying framebuffer.
//
//  NON‑RESPONSIBILITIES:
//      - No D3D11Adapter_Core implementation.
//      - No text, sprite, texture, or shape rendering.
//      - No GPU device or swap‑chain management.
//      - No legacy UI overloads.
// =====================================================================================================

using System;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.Graphics.Software
{
    /// <summary>
    /// Deterministic context for a CPU‑side framebuffer. This type coordinates frame lifecycle
    /// around a <see cref="FramebufferDrawing"/> instance and exposes only basic pixel operations.
    /// All higher‑level rendering is handled by dedicated subsystems.
    /// </summary>
    public sealed class FramebufferContext
    {
        /// <summary>The underlying RGBA32 framebuffer.</summary>
        public FramebufferDrawing Framebuffer { get; }

        /// <summary>Width of the framebuffer in pixels.</summary>
        public int Width => Framebuffer.Width;

        /// <summary>Height of the framebuffer in pixels.</summary>
        public int Height => Framebuffer.Height;

        /// <summary>Convenience viewport size vector.</summary>
        public Vector2 ViewportSize => new Vector2(Width, Height);

        /// <summary>Direct access to the raw pixel buffer.</summary>
        public byte[] Pixels => Framebuffer.Pixels;

        public FramebufferContext(FramebufferDrawing framebuffer)
        {
            Framebuffer = framebuffer ?? throw new ArgumentNullException(nameof(framebuffer));
        }

        // ------------------------------------------------------------------------------------------
        // Frame lifecycle
        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// Prepares the framebuffer for a new frame by clearing or reallocating as needed.
        /// </summary>
        public void BeginFrame()
        {
            Framebuffer.BeginFrame();
        }

        /// <summary>
        /// Marks the end of the current frame. No‑op for pure CPU framebuffer.
        /// </summary>
        public void EndFrame()
        {
            Framebuffer.EndFrame();
        }

        /// <summary>
        /// Presents the framebuffer. For a pure CPU buffer this is a no‑op; the upload pipeline
        /// or software renderer will consume <see cref="Pixels"/> as needed.
        /// </summary>
        public void Present()
        {
            Framebuffer.Present();
        }

        /// <summary>
        /// Resets the framebuffer by allocating a fresh pixel buffer of the correct size.
        /// </summary>
        public void Reset()
        {
            Framebuffer.Reset();
        }

        // ------------------------------------------------------------------------------------------
        // Basic pixel operations (delegated)
        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// Sets a single pixel in the framebuffer. Coordinates outside the bounds are ignored.
        /// </summary>
        public void SetPixel(int x, int y, byte r, byte g, byte b, byte a = 255)
        {
            Framebuffer.SetPixel(x, y, r, g, b, a);
        }

        /// <summary>
        /// Clears the entire framebuffer to a single RGBA color.
        /// </summary>
        public void Clear(byte r, byte g, byte b, byte a = 255)
        {
            Framebuffer.Clear(r, g, b, a);
        }
    }
}
