//==========================================================================================
// FILE: DGDOps_FramebufferUpload.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_FramebufferUpload.cs
// SUBSYSTEM: DGD / CPU Framebuffer → GPU Upload
//
// ROLE:
//     Uploads CPU-side framebuffer pixels (ARGB uint[]) into a GPU render target.
//     Converts ARGB → BGRA byte layout for DXGI formats.
//     Performs deterministic UpdateSubresource calls.
//
// RESPONSIBILITIES:
//     - Convert FramebufferDrawing pixel data to BGRA.
//     - Upload pixel data into the active render target resource.
//     - Wrap D3D11DeviceCore.Context for safe GPU updates.
//
// NON-RESPONSIBILITIES:
//     - Creating render targets.
//     - Presenting swap chains.
//     - Managing shaders or samplers.
//==========================================================================================

using System;
using System.Runtime.InteropServices;
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic framebuffer upload subsystem.
    /// </summary>
    public sealed class DGDOps_FramebufferUpload
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_FramebufferUpload(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Upload a CPU framebuffer into the main render target.
        /// </summary>
        public void UploadToMainRenderTarget(FramebufferDrawing fb)
        {
            if (fb == null)
                throw new ArgumentNullException(nameof(fb));

            var ctx = _deviceCore.Context;
            if (ctx == null)
                throw new InvalidOperationException("D3D11 device context is not initialized.");

            var rtv = _deviceCore.BackbufferRtv;
            if (rtv == null)
                throw new InvalidOperationException("Main render target view is not initialized.");

            UploadFramebuffer(fb, ctx, rtv);
        }

        /// <summary>
        /// Core upload routine: converts ARGB → BGRA and updates the GPU resource.
        /// </summary>
        // --- Replace the UploadFramebuffer method body with this fixed implementation ---
        public void UploadFramebuffer(FramebufferDrawing fb, ID3D11DeviceContext context, ID3D11RenderTargetView rtv)
        {
            if (fb == null)
                throw new ArgumentNullException(nameof(fb));
            if (context == null)
                throw new InvalidOperationException("Device context is not initialized.");
            if (rtv == null)
                throw new InvalidOperationException("Render target view is not initialized.");

            int width = fb.Width;
            int height = fb.Height;

            // Use the pixel array provided by FramebufferDrawing
            byte[] pixels = fb.Pixels;
            if (pixels == null || pixels.Length < width * height)
                throw new ArgumentException("Framebuffer pixel data is invalid or too small.", nameof(fb));

            int pixelCount = width * height;
            int bytesPerPixel = pixels.Length / pixelCount;

            if (bytesPerPixel < 1 || bytesPerPixel > 4)
                throw new ArgumentException("Unsupported pixel format in framebuffer.Pixels.", nameof(fb));

            // Convert source (assumed per-pixel layout) -> BGRA bytes for DXGI
            byte[] bytes = new byte[pixelCount * 4];

            for (int i = 0; i < pixelCount; i++)
            {
                int srcOff = i * bytesPerPixel;
                byte r, g, b, a;

                if (bytesPerPixel == 4)
                {
                    // Common layout: R, G, B, A (matches FramebufferDrawing.SetPixel(r,g,b,a))
                    r = pixels[srcOff + 0];
                    g = pixels[srcOff + 1];
                    b = pixels[srcOff + 2];
                    a = pixels[srcOff + 3];
                }
                else if (bytesPerPixel == 3)
                {
                    // R, G, B (no alpha) -> assume opaque
                    r = pixels[srcOff + 0];
                    g = pixels[srcOff + 1];
                    b = pixels[srcOff + 2];
                    a = 255;
                }
                else // bytesPerPixel == 1
                {
                    // Grayscale -> replicate to RGB, opaque
                    r = g = b = pixels[srcOff];
                    a = 255;
                }

                int dstOff = i * 4;
                bytes[dstOff + 0] = b; // B
                bytes[dstOff + 1] = g; // G
                bytes[dstOff + 2] = r; // R
                bytes[dstOff + 3] = a; // A
            }

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                nint dataPtr = handle.AddrOfPinnedObject();

                // Vortice: retrieve underlying resource
                ID3D11Resource resource = rtv.Resource;
                if (resource == null)
                    throw new InvalidOperationException("Failed to retrieve render target resource.");

                try
                {
                    var region = new Box
                    {
                        Left = 0,
                        Top = 0,
                        Front = 0,
                        Right = width,
                        Bottom = height,
                        Back = 1
                    };

                    int rowPitch = width * 4;

                    context.UpdateSubresource(
                        resource,
                        0,
                        region,
                        dataPtr,
                        (uint)rowPitch,
                        0);
                }
                finally
                {
                    resource.Dispose();
                }
            }
            finally
            {
                handle.Free();
            }
        }
    }
}