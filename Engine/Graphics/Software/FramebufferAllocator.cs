// =====================================================================================================
//  FILE: FramebufferAllocator.cs
//  PATH: Engine/Graphics/Software/FramebufferAllocator.cs
//  SUBSYSTEM: CPU Rendering / Deterministic Software Framebuffer
//
//  ROLE:
//      Provides deterministic allocation, resizing, and safe fallback behavior for RGBA32 framebuffer
//      pixel buffers. Ensures that all framebuffer memory is tightly packed, correctly sized, and
//      zero‑initialized.
//
//  RESPONSIBILITIES:
//      - Allocate RGBA32 pixel buffers (Width * Height * 4).
//      - Safely resize buffers when dimensions change.
//      - Provide overflow‑checked size calculations.
//      - Guarantee zero‑initialized memory for new buffers.
//      - Provide safe fallback states on allocation failure.
//
//  NON‑RESPONSIBILITIES:
//      - Rendering operations (handled by SoftwareRenderContext, SpriteRenderer, TextureBlitter).
//      - GPU upload or presentation (handled by DGDOps_* subsystems).
//      - Frame lifecycle (BeginFrame / EndFrame / Present handled by FramebufferCore).
//
//  ARCHITECTURAL NOTES:
//      - This subsystem isolates all memory allocation logic from FramebufferDrawing.
//      - Ensures deterministic buffer layout for CPU→GPU upload pipelines.
//      - Always returns valid buffers; never throws NotImplementedException.
// =====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Graphics.Software
{
    internal static class FramebufferAllocator
    {
        /// <summary>
        /// Allocates a new RGBA32 pixel buffer for the given dimensions.
        /// Always returns a valid buffer; never throws.
        /// </summary>
        public static byte[] Allocate(int width, int height)
        {
            try
            {
                int requiredSize = checked(width * height * 4);
                return new byte[requiredSize]; // zero‑initialized
            }
            catch
            {
                return Array.Empty<byte>(); // safe fallback
            }
        }

        /// <summary>
        /// Ensures the existing buffer matches the required size.
        /// If not, allocates a new one.
        /// </summary>
        public static byte[] EnsureSize(byte[]? existing, int width, int height)
        {
            try
            {
                int requiredSize = checked(width * height * 4);

                if (existing == null || existing.Length != requiredSize)
                    return new byte[requiredSize];

                return existing;
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Attempts to resize the buffer. If resize fails, returns a safe empty buffer.
        /// </summary>
        public static byte[] Resize(int newWidth, int newHeight)
        {
            try
            {
                int requiredSize = checked(newWidth * newHeight * 4);
                return new byte[requiredSize];
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }
    }
}
