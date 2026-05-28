// ============================================================================
// File:    D3D11DeviceCoreTexture.cs
// File Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\Rendering\D3D11\
// Author:  BDC
// Purpose: Texture2D + ShaderResourceView creation and framebuffer upload
//          for the hybrid CPU-framebuffer → D3D11 presentation pipeline.
// ============================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static Vortice.Direct3D11.D3D11;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    /// <summary>
    /// Partial D3D11DeviceCore: texture creation and upload helpers
    /// for the CPU framebuffer (uint[] BGRA → B8G8R8A8_UNorm).
    /// </summary>
    public sealed partial class D3D11DeviceCore
    {
        /// <summary>
        /// Creates a 2D texture suitable for holding a BGRA framebuffer.
        /// Format: B8G8R8A8_UNorm, BindFlags.ShaderResource.
        /// </summary>
        /// <param name="width">Texture width in pixels.</param>
        /// <param name="height">Texture height in pixels.</param>
        /// <returns>The created ID3D11Texture2D instance.</returns>
        public ID3D11Texture2D CreateTexture(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Texture dimensions must be positive");

            DebugLogger.LogInfo($"D3D11DeviceCore: creating framebuffer texture {width}x{height}");

            var desc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            var texture = Device.CreateTexture2D(desc);

            if (texture == null)
            {
                DebugLogger.LogError("D3D11DeviceCore: CreateTexture returned null texture");
                throw new InvalidOperationException("Failed to create framebuffer texture");
            }

            return texture;
        }

        /// <summary>
        /// Creates a ShaderResourceView for the given framebuffer texture.
        /// </summary>
        /// <param name="texture">The texture to create a view for.</param>
        /// <returns>The created ID3D11ShaderResourceView instance.</returns>
        public ID3D11ShaderResourceView CreateTextureView(ID3D11Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            DebugLogger.LogInfo("D3D11DeviceCore: creating ShaderResourceView for framebuffer texture");

            var srvDesc = new ShaderResourceViewDescription
            {
                Format = Format.B8G8R8A8_UNorm,
                Texture2D = new Texture2DShaderResourceView
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            };

            var view = Device.CreateShaderResourceView(texture, srvDesc);

            if (view == null)
            {
                DebugLogger.LogError("D3D11DeviceCore: CreateTextureView returned null SRV");
                throw new InvalidOperationException("Failed to create framebuffer ShaderResourceView");
            }

            return view;
        }

        /// <summary>
        /// Uploads the CPU framebuffer pixels into the given texture.
        /// Assumes pixels are 32-bit BGRA (uint per pixel) matching B8G8R8A8_UNorm.
        /// </summary>
        /// <param name="texture">Destination GPU texture.</param>
        /// <param name="pixels">Source CPU pixel buffer (uint[] BGRA).</param>
        /// <param name="width">Framebuffer width in pixels.</param>
        /// <param name="height">Framebuffer height in pixels.</param>
        public void UpdateTexture(ID3D11Texture2D texture, uint[] pixels, int width, int height)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Framebuffer dimensions must be positive");
            if (pixels.Length < width * height)
                throw new ArgumentException("Pixel buffer is smaller than width * height", nameof(pixels));

            int rowPitch = width * sizeof(uint);

            DebugLogger.LogDebug(
                $"D3D11DeviceCore: updating framebuffer texture {width}x{height}, rowPitch={rowPitch}");

            unsafe
            {
                fixed (uint* pPixels = pixels)
                {
                    var dataBox = new SubresourceData
                    {
                        DataPointer = new IntPtr(pPixels),
                        RowPitch = (uint)rowPitch,
                        SlicePitch = 0
                    };

                    // UpdateSubresource with a full replacement of subresource 0
                    ImmediateContext.UpdateSubresource(dataBox, texture, 0);
                }
            }
        }
    }
}
